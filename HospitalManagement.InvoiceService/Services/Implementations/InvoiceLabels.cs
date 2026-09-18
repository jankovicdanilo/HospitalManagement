namespace HospitalManagement.InvoiceService.Services.Implementations
{
    public class InvoiceLabels
    {
        public string HospitalName { get; init; } = "";
        public string Title { get; init; } = "";
        public string Invoice { get; init; } = "";
        public string Date { get; init; } = "";
        public string Patient { get; init; } = "";
        public string Doctor { get; init; } = "";
        public string AppointmentDate { get; init; } = "";
        public string Duration { get; init; } = "";
        public string Status { get; init; } = "";
        public string Procedure { get; init; } = "";
        public string Price { get; init; } = "";
        public string Subtotal { get; init; } = "";
        public string Discount { get; init; } = "";
        public string Total { get; init; } = "";
        public string Notes { get; init; } = "";
        public string Minutes { get; init; } = "";
        public string Page { get; init; } = "";
        public string Of { get; init; } = "";
        public string Language { get; init; } = "en";

        public string StatusText(string status) => Language.ToLowerInvariant() == "me" ? status switch
        {
            "Completed" => "Završeno",
            "Pending" => "Na čekanju",
            "Cancelled" => "Otkazano",
            "Confirmed" => "Potvrđeno",
            "Missed" => "Propušteno",
            _ => status
        }
        : status;

        public static InvoiceLabels Get(string language) => language.ToLowerInvariant() switch
        {
            "me" => new InvoiceLabels
            {
                Language = "me",
                HospitalName = "Gradska Bolnica",
                Title = "Medicinski Račun",
                Invoice = "Račun",
                Date = "Datum",
                Patient = "Pacijent",
                Doctor = "Doktor",
                AppointmentDate = "Datum Termina",
                Duration = "Trajanje",
                Status = "Status",
                Procedure = "Procedura",
                Price = "Cijena",
                Subtotal = "Podzbir",
                Discount = "Popust",
                Total = "Ukupno",
                Notes = "Napomene",
                Minutes = "min",
                Page = "Strana",
                Of = "od"
            },
            _ => new InvoiceLabels
            {
                Language = "en",
                HospitalName = "City Hospital",
                Title = "Medical Invoice",
                Invoice = "Invoice",
                Date = "Date",
                Patient = "Patient",
                Doctor = "Doctor",
                AppointmentDate = "Appointment Date",
                Duration = "Duration",
                Status = "Status",
                Procedure = "Procedure",
                Price = "Price",
                Subtotal = "Subtotal",
                Discount = "Discount",
                Total = "Total",
                Notes = "Notes",
                Minutes = "min",
                Page = "Page",
                Of = "of"
            }
        };
    }
}
