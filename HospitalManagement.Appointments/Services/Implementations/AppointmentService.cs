using AutoMapper;
using HospitalManagement.Appointments.Clients.Interfaces;
using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Models.DTOs.Appointment;
using HospitalManagement.Appointments.Models.Enums;
using HospitalManagement.Appointments.Repositories.Interfaces;
using HospitalManagement.Appointments.Services.Calculators.Interfaces;
using HospitalManagement.Appointments.Services.Calculators.Results;
using HospitalManagement.Appointments.Services.Interfaces;
using HospitalManagement.Appointments.Services.Validations;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Models.DTOs.Patient;
using Microsoft.Extensions.Options;
using System.Text;

namespace HospitalManagement.Appointments.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IMapper mapper;
        private readonly IAppointmentValidation appointmentValidation;
        private readonly ILogger<AppointmentService> logger;
        private readonly AppointmentSettings appointmentSettings;
        private readonly IAppointmentDiscountCalculator appointmentDiscountCalculator;
        private readonly IQueryServiceClient queryServiceClient;
        private readonly IClinicTimeZoneProvider clinicTimeZoneProvider;
        private readonly ITreatmentRepository treatmentRepository;
        private readonly IClaudeSummaryService claudeSummaryService;

        public AppointmentService(IAppointmentRepository appointmentRepository, IMapper mapper,
            IAppointmentValidation appointmentValidation, ILogger<AppointmentService> logger,
            IOptions<AppointmentSettings> appointmentSettings, IAppointmentDiscountCalculator appointmentDiscountCalculator,
            IQueryServiceClient queryServiceClient, IClinicTimeZoneProvider clinicTimeZoneProvider,
            ITreatmentRepository treatmentRepository, IClaudeSummaryService claudeSummaryService)
        {
            this.appointmentRepository = appointmentRepository;
            this.mapper = mapper;
            this.appointmentValidation = appointmentValidation;
            this.logger = logger;
            this.appointmentSettings = appointmentSettings.Value;
            this.appointmentDiscountCalculator = appointmentDiscountCalculator;
            this.queryServiceClient = queryServiceClient;
            this.clinicTimeZoneProvider = clinicTimeZoneProvider;
            this.treatmentRepository = treatmentRepository;
            this.claudeSummaryService = claudeSummaryService;
        }

        public async Task<Result<PagedResult<AppointmentListResponseDto>>> GetAllAsync(AppointmentFilterDto filter)
        {
            var (items, totalCount) = await appointmentRepository.GetAllAsync(filter);

            var mapped = mapper.Map<List<AppointmentListResponseDto>>(items);

            var doctorsById = await BuildLookupAsync(items.Select(x => x.DoctorId), queryServiceClient.GetDoctorAsync);
            var patientsById = await BuildLookupAsync(items.Select(x => x.PatientId), queryServiceClient.GetPatientAsync);

            foreach(var (item, dto) in items.Zip(mapped))
            {
                var doctor = doctorsById.GetValueOrDefault(item.DoctorId);
                var patient = patientsById.GetValueOrDefault(item.PatientId);

                dto.DoctorName = doctor != null ? $"{doctor.FirstName} {doctor.LastName}" : null;
                dto.PatientName = patient != null ? $"{patient.Name} {patient.LastName}" : null;

                var calculateDiscount = GetDiscountResult(item);
                dto.TotalCost = calculateDiscount.TotalCost;
                dto.Discount = calculateDiscount.Discount;
            }

            var pagedResult = new PagedResult<AppointmentListResponseDto>
            {
                Items = mapped,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            return Result<PagedResult<AppointmentListResponseDto>>.Ok(pagedResult);
        }

        public async Task<Result<AppointmentResponseDto>> GetByIdAsync(int id)
        {
            var appointmentDomain = await appointmentRepository.GetByIdAsync(id);
            if (appointmentDomain == null)
            {
                logger.LogWarning("Appointment with id {id} not found", id);
                return Result<AppointmentResponseDto>.Fail($"Appointment with the id {id} not found", "INVALID_ID",
                    ErrorType.NotFound);
            }

            var doctor = await queryServiceClient.GetDoctorAsync(appointmentDomain.DoctorId);
            var patient = await queryServiceClient.GetPatientAsync(appointmentDomain.PatientId);

            var result = mapper.Map<AppointmentResponseDto>(appointmentDomain);
            result.Doctor = doctor;
            result.Patient = patient;
            var calculateDiscount = GetDiscountResult(appointmentDomain);
            result.TotalCost = calculateDiscount.TotalCost;
            result.Discount = calculateDiscount.Discount;

            return Result<AppointmentResponseDto>.Ok(result);
        }

        public async Task<Result<AppointmentCreateResponseDto>> CreateAsync(AppointmentCreateRequestDto request)
        {
            var validatedAppointment = await appointmentValidation.ValidateAll(request);

            if (!validatedAppointment.Success)
            {
                logger.LogWarning("Appointment creation failed: {Message}", validatedAppointment.Message);
                return Result<AppointmentCreateResponseDto>.Fail(validatedAppointment.Message, validatedAppointment.ErrorCode,
                    validatedAppointment.ErrorType);
            }

            var patient = await queryServiceClient.GetPatientAsync(request.PatientId);
            var doctor = await queryServiceClient.GetDoctorAsync(request.DoctorId);

            if (patient == null)
            {
                logger.LogWarning("Patient with id {PatientId} not found", request.PatientId);
                return Result<AppointmentCreateResponseDto>.Fail($"Patient with id {request.PatientId} not found", 
                    "INVALID_PATIENT_ID", ErrorType.NotFound);
            }

            if (doctor == null)
            {
                logger.LogWarning("Doctor with id {DoctorId} not found", request.DoctorId);
                return Result<AppointmentCreateResponseDto>.Fail($"Doctor with id {request.DoctorId} not found",
                    "INVALID_DOCTOR_ID", ErrorType.NotFound);
            }

            var appointmentDomain = mapper.Map<Appointment>(request);

            appointmentDomain = await appointmentRepository.CreateAsync(appointmentDomain);

            logger.LogInformation("Appointment created with id {id}", appointmentDomain.Id);
            logger.LogInformation("Email sent to {Email}: Appointment confirmed for {DateTime} with Dr. {Doctor}",
                patient.Email, appointmentDomain.DateTime, $"{doctor.FirstName} {doctor.LastName}");

            var result = mapper.Map<AppointmentCreateResponseDto>(appointmentDomain);
            result.Doctor = doctor;
            result.Patient = patient;

            return Result<AppointmentCreateResponseDto>.Ok(result);
        }

        public async Task<Result<AppointmentUpdateResponseDto>> UpdateAsync(AppointmentUpdateRequestDto request)
        {
            var validatedAppointment = await appointmentValidation.ValidateAll(request);

            if (!validatedAppointment.Success)
            {
                logger.LogWarning("Appointment update failed : {Message}", validatedAppointment.Message);
                return Result<AppointmentUpdateResponseDto>.Fail(validatedAppointment.Message,
                    validatedAppointment.ErrorCode, validatedAppointment.ErrorType);
            }

            var appointmentDomain = await appointmentRepository.GetByIdAsync(request.Id);

            if (appointmentDomain == null)
            {
                logger.LogWarning("Appointment with id {Id} not found", request.Id);
                return Result<AppointmentUpdateResponseDto>.Fail($"Appointment with the id {request.Id} not found",
                    "INVALID_ID", ErrorType.NotFound);
            }

            if (appointmentDomain.Status != AppointmentStatus.Pending)
            {
                logger.LogWarning("Appointment with id {Id} cannot be updated, status is {Status}", request.Id, appointmentDomain.Status);
                return Result<AppointmentUpdateResponseDto>.Fail(
                    $"Only pending appointments can be updated",
                    "INVALID_STATUS", ErrorType.Conflict);
            }

            var patient = await queryServiceClient.GetPatientAsync(request.PatientId);
            var doctor = await queryServiceClient.GetDoctorAsync(request.DoctorId);

            if (patient == null)
            {
                logger.LogWarning("Patient with id {PatientId} not found", request.PatientId);
                return Result<AppointmentUpdateResponseDto>.Fail($"Patient with id {request.PatientId} not found", 
                    "INVALID_PATIENT_ID", ErrorType.NotFound);
            }

            if (doctor == null)
            {
                logger.LogWarning("Doctor with id {DoctorId} not found", request.DoctorId);
                return Result<AppointmentUpdateResponseDto>.Fail($"Doctor with id {request.DoctorId} not found", 
                    "INVALID_DOCTOR_ID", ErrorType.NotFound);
            }

            mapper.Map(request, appointmentDomain);

            appointmentDomain = await appointmentRepository.UpdateAsync(appointmentDomain);

            logger.LogInformation("Appointment with id {Id} updated", appointmentDomain.Id);

            var result = mapper.Map<AppointmentUpdateResponseDto>(appointmentDomain);
            result.Doctor = doctor;
            result.Patient = patient;

            return Result<AppointmentUpdateResponseDto>.Ok(result);
        }

        public async Task<Result> Delete(int id)    
        {
            var appointmentDomain = await appointmentRepository.Delete(id);

            if (appointmentDomain == null)
            {
                logger.LogWarning("Appointment with id {Id} not found for deletion", id);
                return Result.Fail($"Appointment with the id {id} not found", "INVALID_ID", ErrorType.NotFound);
            }

            logger.LogInformation("Appointment with id {Id} deleted", id);
            return Result.Ok("Appointment deleted");
        }

        public async Task<Result<List<TimeSlotDto>>> GetFreeSlotsAsync(int doctorId, DateOnly date)
        {
            var nowLocal = clinicTimeZoneProvider.ToLocal(DateTime.UtcNow);
            if (date < DateOnly.FromDateTime(nowLocal))
            {
                logger.LogWarning("Free slots requested for past date {Date}", date);
                return Result<List<TimeSlotDto>>.Fail("Cannot get free slots for a past date", "INVALID_DATE",
                    ErrorType.Validation);
            }

            var doctorSchedule = await queryServiceClient.GetDoctorScheduleAsync(doctorId, date.DayOfWeek);
            if (doctorSchedule == null)
            {
                logger.LogWarning("Doctor {DoctorId} does not work on {DayOfWeek}", doctorId, date.DayOfWeek.ToString());
                return Result<List<TimeSlotDto>>.Fail($"Doctor does not work on {date.DayOfWeek}", "DOCTOR_NOT_AVAILABLE",
                    ErrorType.Conflict);
            }

            var workStart = new TimeSpan(doctorSchedule.StartHour, 0, 0);
            var workEnd = new TimeSpan(doctorSchedule.EndHour, 0, 0);
            var slotSize = new TimeSpan(0, appointmentSettings.SlotSizeMinutes, 0);

            var appointments = await appointmentRepository.GetByDoctorIdAndDateAsync(doctorId, date);

            var freeSlots = new List<TimeSlotDto>();
            var current = workStart;

            while (current + slotSize <= workEnd)
            {
                var slotStartLocal = date.ToDateTime(TimeOnly.FromTimeSpan(current));
                var slotEndLocal = slotStartLocal.Add(slotSize);
                var slotStartUtc = clinicTimeZoneProvider.ToUtc(slotStartLocal);
                var slotEndUtc = clinicTimeZoneProvider.ToUtc(slotEndLocal);

                var isBooked = appointments.Any(a =>
                    a.Status != AppointmentStatus.Cancelled &&
                    a.Status != AppointmentStatus.Missed &&
                    slotStartUtc < a.DateTime.Add(a.Duration) &&
                    slotEndUtc > a.DateTime);

                if (!isBooked)
                {
                    freeSlots.Add(new TimeSlotDto
                    {
                        Start = TimeOnly.FromDateTime(slotStartLocal),
                        End = TimeOnly.FromDateTime(slotEndLocal)
                    });
                }

                current = current.Add(slotSize);
            }

            return Result<List<TimeSlotDto>>.Ok(freeSlots);
        }

        public async Task<Result> UpdateStatusAsync(AppointmentStatusUpdateDto request)
        {
            var appointmentDomain = await appointmentRepository.GetByIdAsync(request.Id);

            if (appointmentDomain == null)
            {
                logger.LogWarning("Appointment with id {Id} not found", request.Id);
                return Result.Fail($"Appointment with the id {request.Id} not found", "INVALID_ID", ErrorType.NotFound);
            }

            if (appointmentDomain.Status != AppointmentStatus.Pending)
            {
                logger.LogWarning("Appointment with id {Id} cannot be updated, status is {Status}", request.Id, appointmentDomain.Status);
                return Result.Fail("Only pending appointments can have their status changed", "INVALID_STATUS",
                    ErrorType.Conflict);
            }

            appointmentDomain.Status = request.Status;

            await appointmentRepository.UpdateAsync(appointmentDomain);

            logger.LogInformation("Appointment with id {Id} status updated to {Status}", request.Id, request.Status);

            return Result.Ok("Appointment status updated");
        }

        public async Task<Result<List<AppointmentResponseDto>>> GetPatientHistoryAsync(int patientId)
        {
            var patient = await queryServiceClient.GetPatientAsync(patientId);

            if(patient == null)
            {
                logger.LogWarning("Patient with id {PatientId} not found", patientId);
                return Result<List<AppointmentResponseDto>>.Fail(
                    $"Patient with id {patientId} not found", "INVALID_PATIENT_ID", ErrorType.NotFound);
            }

            var appontiments = await appointmentRepository.GetByPatientIdAsync(patientId);

            var result = mapper.Map<List<AppointmentResponseDto>>(appontiments);

            foreach(var (appointment, dto) in appontiments.Zip(result))
            {
                var calculateDiscount = GetDiscountResult(appointment);
                dto.TotalCost = calculateDiscount.TotalCost;
                dto.Discount = calculateDiscount.Discount;
            }

            logger.LogInformation("Patient history retrieved for patient {PatientId}, {Count} appointments found",
                    patientId, result.Count);

            return Result<List<AppointmentResponseDto>>.Ok(result);
        }

        private DiscountResult GetDiscountResult(Appointment appointment)
        {
            if (appointment.Status != AppointmentStatus.Completed)
            {
                return new DiscountResult(appointment.AppointmentProcedures.Sum(ap => ap.ProcedurePrice), 0);
            }

            var discountResult = appointmentDiscountCalculator.Calculate(appointment.AppointmentProcedures);

            return discountResult;
        }

        private static async Task<Dictionary<TKey, TEntity?>> BuildLookupAsync<TKey, TEntity>(
            IEnumerable<TKey> keys, Func<TKey, Task<TEntity?>> fetch) where TKey : notnull
        {
            var uniqueKeys = keys.Distinct().ToList();
            var entities = await Task.WhenAll(uniqueKeys.Select(fetch));

            return uniqueKeys.Zip(entities).ToDictionary(x => x.First, x => x.Second);
        }

        public async Task<Result<List<int>>> GetPopularDoctorIdsAsync(int count)
        {
            if(count <= 0)
            {
                return Result<List<int>>.Fail("Count must be greater than 0", "INVALID_COUNT", ErrorType.Validation);
            }

            var ids = await appointmentRepository.GetTopDoctorIdsByAppointmentCountAsync(count);

            return Result<List<int>>.Ok(ids);
        }

        public async Task<Result<List<int>>> GetPopularPatientIdsAsync(int count)
        {
            if(count <= 0)
            {
                return Result<List<int>>.Fail("Count must be greater than 0", "INVALID_COUNT", ErrorType.Validation);
            }

            var ids = await appointmentRepository.GetTopPatientIdsByAppointmentCountAsync(count);

            return Result<List<int>>.Ok(ids);
        }

        public async Task<Result<PatientSummaryResponseDto>> GetPatientSummaryAsync(int patientId)
        {
            var patient = await queryServiceClient.GetPatientAsync(patientId);
            if(patient == null)
            {
                logger.LogWarning("Patient with id {PatientId} not found", patientId);
                return Result<PatientSummaryResponseDto>.Fail($"Patient with id {patientId} not found", "INVALID_PATIENT_ID", ErrorType.NotFound);
            }

            var appointments = await appointmentRepository.GetByPatientIdAsync(patientId);
            if(appointments.Count == 0)
            {
                return Result<PatientSummaryResponseDto>.Ok(new PatientSummaryResponseDto
                {
                    PatientId = patientId,
                    PatientName = $"{patient.Name} {patient.LastName}",
                    Summary = "No appointments history available for this patient"
                });
            }

            var treatments = await treatmentRepository.GetByAppointmentIdsAsync(appointments.Select(a => a.Id));
            var prompt = BuildSummaryPrompt(patient, appointments, treatments);

            string summaryText;
            try
            {
                summaryText = await claudeSummaryService.GenerateSummaryAsync(prompt);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to generate summary for patient {PatientId}", patientId);
                return Result<PatientSummaryResponseDto>.Fail(
                    "Failed to generate patient summary", "SUMMARY_GENERATION_FAILED", ErrorType.UpstreamFailure);
            }

            logger.LogInformation("Summary generated for patient {PatientId}", patientId);

            var result = new PatientSummaryResponseDto
            {
                PatientId = patientId,
                PatientName = $"{patient.Name} {patient.LastName}",
                Summary = summaryText
            };

            return Result<PatientSummaryResponseDto>.Ok(result);
        }

        private static string BuildSummaryPrompt(
            PatientResponseDto patient, 
            List<Appointment> appointments, 
            List<Treatment> treatments)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Summarize the medical history of patient {patient.Name} {patient.LastName}" +
                $"(DOB: {patient.DateOfBirth:yyyy-MM-dd}) in a concise clinical-style paragraph. " +
                "Base it only on the visit and treatment data below:");
            sb.AppendLine();

            foreach(var appointment in appointments)
            {
                sb.AppendLine($"- Appointment on {appointment.DateTime:yyyy-MM-dd}, status: {appointment.Status}");

                var appointmentTreatments = treatments.Where(t => t.AppointmentId == appointment.Id);
                foreach(var treatment in appointmentTreatments)
                {
                    sb.AppendLine($" Treatment notes: {treatment.Description}");
                    if (!string.IsNullOrWhiteSpace(treatment.Medication))
                    {
                        sb.AppendLine($" Medication: {treatment.Medication}");
                    }
                }
            }

            return sb.ToString();
        }
    }
}