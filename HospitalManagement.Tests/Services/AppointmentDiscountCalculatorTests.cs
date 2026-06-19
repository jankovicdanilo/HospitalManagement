using HospitalManagement.Models.Domain;
using HospitalManagement.Services.Calculators.Implementations;
using HospitalManagement.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Tests.Services
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
            for(int i = 0; i < count; i++)
            {
                procedures.Add(new AppointmentProcedure
                {
                    Procedure = new Procedure { Id = 1, Name = $"Procedure{i}", Price = pricePerProcedure }
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
