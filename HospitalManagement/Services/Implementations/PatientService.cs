using AutoMapper;
using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Patient;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Patient;
using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Patient;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HospitalManagement.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository patientRepository;
        private readonly IMapper mapper;

        public PatientService(IPatientRepository patientRepository, IMapper mapper)
        {
            this.patientRepository = patientRepository;
            this.mapper = mapper;
        }

        public async Task<Result> Delete(int id)
        {
            var patientDomain = await patientRepository.Delete(id);

            if (patientDomain == null)
            {
                return Result.Fail($"Patient with the id {id} not found", "INVALID_ID");
            }

            return Result.Ok($"Patient with id {id} deleted"); 
        }

        public async Task<Result<List<PatientListDto>>> GetAllAsync()
        {
            var patientsListDomain = await patientRepository.GetAllAsync();

            var result = mapper.Map<List<PatientListDto>>(patientsListDomain);

            return Result<List<PatientListDto>>.Ok(result);
        }

        public async Task<Result<PatientGetByIdDto?>> GetByIdAsync(int id)
        {
            var patientDomain = await patientRepository.GetByIdAsync(id);

            if(patientDomain == null)
            {
                return Result<PatientGetByIdDto?>.Fail($"Patient with the id {id} doesn't exist", "INVALID_ID");
            }

            var result = mapper.Map<PatientGetByIdDto>(patientDomain);

            return Result<PatientGetByIdDto?>.Ok(result);
        }

        public async Task<Result<PatientCreateResponseDto?>> CreateAsync(PatientCreateRequestDto request)
        {
            if (request == null)
            {
                return Result<PatientCreateResponseDto?>.Fail("Patient not found", "PATIENT_NOT_FOUND");
            }

            var patientExists = await patientRepository.GetByEmailAsync(request.Email);

            if (patientExists != null)
            {
                return Result<PatientCreateResponseDto?>.Fail($"Email {request.Email} aldready exists", "INVALID_EMAIL");
            }

            var patientDomain = mapper.Map<Patient>(request);

            patientDomain = await patientRepository.CreateAsync(patientDomain);

            var result = mapper.Map<PatientCreateResponseDto>(patientDomain);

            return Result<PatientCreateResponseDto?>.Ok(result);
        }

        public async Task<Result<PatientUpdateResponseDto>> UpdateAsync(PatientUpdateRequestDto request)
        {
            var patientDomain = await patientRepository.GetByIdAsync(request.Id);

            if (patientDomain == null)
            {
                return Result<PatientUpdateResponseDto>.Fail($"Patient with the id {request.Id} not found", "INVALID_ID");
            }

            if (patientRepository.EmailExists(request.Email) && request.Email != patientDomain.Email)
            {
                return Result<PatientUpdateResponseDto>.Fail($"Email {request.Email} already exists", "INVALID_EMAIL");
            }

            mapper.Map(request, patientDomain);

            await patientRepository.UpdateAsync(patientDomain);

            var result = mapper.Map<PatientUpdateResponseDto>(patientDomain);

            return Result<PatientUpdateResponseDto>.Ok(result);
        }
    }
}
