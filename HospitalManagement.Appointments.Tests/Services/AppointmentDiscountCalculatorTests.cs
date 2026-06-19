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

        [Test]
        public void Calculate_AtTier1Boundary_AppliesTier1Discount()
        {
            var procedures = BuildProcedures(3, 100m);

            var result = calculator.Calculate(procedures);

            // 3% of 300 = 9
            Assert.That(result.Discount, Is.EqualTo(9m));
            Assert.That(result.TotalCost, Is.EqualTo(291m));
        }

        [Test]
        public void Calculate_BetweenTier1AndTier2_StaysAtTier1Discount()
        {
            var procedures = BuildProcedures(4, 100m);

            var result = calculator.Calculate(procedures);

            // still 3%, tier2 (5) not reached: 3% of 400 = 12
            Assert.That(result.Discount, Is.EqualTo(12m));
            Assert.That(result.TotalCost, Is.EqualTo(388m));
        }

        [Test]
        public void Calculate_AtTier2Boundary_AppliesTier2Discount()
        {
            var procedures = BuildProcedures(5, 100m);

            var result = calculator.Calculate(procedures);

            // 4% of 500 = 20
            Assert.That(result.Discount, Is.EqualTo(20m));
            Assert.That(result.TotalCost, Is.EqualTo(480m));
        }

        [Test]
        public void Calculate_AtTier3Boundary_AppliesTier3Discount()
        {
            var procedures = BuildProcedures(7, 100m);

            var result = calculator.Calculate(procedures);

            // 5% of 700 = 35
            Assert.That(result.Discount, Is.EqualTo(35m));
            Assert.That(result.TotalCost, Is.EqualTo(665m));
        }

        [Test]
        public void Calculate_DiscountExceedsMax_CapsAtMaxDiscount()
        {
            // 7 procedures at 1000 each = 7000 total, 5% = 350, exceeds MaxDiscount of 100
            var procedures = BuildProcedures(7, 1000m);

            var result = calculator.Calculate(procedures);

            Assert.That(result.Discount, Is.EqualTo(100m));
            Assert.That(result.TotalCost, Is.EqualTo(6900m));
        }

        [Test]
        public void Calculate_EmptyProcedureList_ReturnsZeroCostAndDiscount()
        {
            var procedures = new List<AppointmentProcedure>();

            var result = calculator.Calculate(procedures);

            Assert.That(result.Discount, Is.EqualTo(0));
            Assert.That(result.TotalCost, Is.EqualTo(0));
        }
    }
}