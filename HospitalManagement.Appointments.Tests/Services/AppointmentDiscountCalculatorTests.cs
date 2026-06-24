using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Services.Calculators.Implementations;
using HospitalManagement.Appointments.Settings;
using Microsoft.Extensions.Options;

namespace HospitalManagement.Appointments.Tests.Services
{
    [TestFixture]
    internal class AppointmentDiscountCalculatorTests
    {
        private IOptions<DiscountSettings> discountSettings;
        private AppointmentDiscountCalculator calculator;

        [SetUp]
        public void Setup()
        {
            discountSettings = Options.Create(new DiscountSettings
            {
                Tier1MinCount = 3,
                Tier1Percentage = 3,
                Tier2MinCount = 5,
                Tier2Percentage = 4,
                Tier3MinCount = 7,
                Tier3Percentage = 5,
                MaxDiscount = 100
            });

            calculator = new AppointmentDiscountCalculator(discountSettings);
        }

        private static List<AppointmentProcedure> BuildProcedures(int count, decimal pricePerProcedure)
        {
            var procedures = new List<AppointmentProcedure>();
            for (int i = 0; i < count; i++)
            {
                procedures.Add(new AppointmentProcedure
                {
                    ProcedureId = i + 1,
                    ProcedureName = $"Procedure{i}",
                    ProcedurePrice = pricePerProcedure  // snapshot field, no Procedure navigation needed
                });
            }
            return procedures;
        }

        [Test]
        public void Calculate_BelowTier1_NoDiscount()
        {
            var procedures = BuildProcedures(2, 100m);

            var result = calculator.Calculate(procedures);

            Assert.That(result.Discount, Is.EqualTo(0));
            Assert.That(result.TotalCost, Is.EqualTo(200m));
        }
    }
}