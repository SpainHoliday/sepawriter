﻿using NUnit.Framework;

namespace SpainHoliday.SepaWriter.Test
{
    [TestFixture]
    public class SepaIbanDataTest
    {
        private const string Bic = "SOGEFRPPXXX";
        private const string Iban = "FR7030002005500000157845Z02";
        private const string IbanWithSpace = "FR70 30002  005500000157845Z    02";
        private const string Name = "A_NAME";

        [Test]
        public void ShouldBeValidIfAllDataIsNotNull()
        {
            var data = new SepaIbanData
                {
                    Bic = Bic,
                    Iban = Iban,
                    Name = Name
                };

            Assert.True(data.IsValid);
        }

        [Test]
        public void ShouldBeValidIfAllDataIsNotNullAndBicIsUnknown()
        {
            var data = new SepaIbanData
            {
                UnknownBic = true,
                Iban = Iban,
                Name = Name
            };

            Assert.True(data.IsValid);
        }
        
        [Test]
        public void ShouldRemoveSpaceInIban()
        {
            var data = new SepaIbanData
            {
                Bic = Bic,
                Iban = IbanWithSpace,
                Name = Name
            };

            Assert.True(data.IsValid);
            Assert.AreEqual(Iban, data.Iban);
        }

        [Test]
        public void ShouldKeepNameIfLessThan70Chars()
        {
            var data = new SepaIbanData
                {
                    Bic = Bic,
                    Iban = Iban,
                    Name = Name
                };

            Assert.AreEqual(Bic, data.Bic);
            Assert.AreEqual(Name, data.Name);
            Assert.AreEqual(Iban, data.Iban);
        }

        [Test]
        public void ShouldNotBeValidIfBicIsNull()
        {
            var data = new SepaIbanData
                {
                    Iban = Iban,
                    Name = Name
                };

            Assert.False(data.IsValid);
        }

        [Test]
        public void ShouldNotBeValidIfIbanIsNull()
        {
            var data = new SepaIbanData
                {
                    Bic = Bic,
                    Name = Name
                };

            Assert.False(data.IsValid);
        }

        [Test]
        public void ShouldNotBeValidIfNameIsNull()
        {
            var data = new SepaIbanData
                {
                    Bic = Bic,
                    Iban = Iban
                };

            Assert.False(data.IsValid);
        }

        [Test]
        public void ShouldReduceNameIfGreaterThan70Chars()
        {
            const string longName = "12345678901234567890123456789012345678901234567890123456789012345678901234567890";
            const string expectedName = "1234567890123456789012345678901234567890123456789012345678901234567890";
            var data = new SepaIbanData
                {
                    Bic = Bic,
                    Iban = Iban,
                    Name = longName
                };

            Assert.AreEqual(expectedName, data.Name);
        }

        [Test]
        public void ShouldRejectBadBic()
        {
            Assert.That(() => { new SepaIbanData { Bic = "BIC" }; },
                Throws.TypeOf<SepaRuleException>().With.Property("Message").Contains("Null or Invalid length of BIC"));            
        }

        [Test]
        public void ShouldRejectTooLongIban()
        {
            Assert.That(() => { new SepaIbanData { Iban = "FR012345678901234567890123456789012" }; },
                Throws.TypeOf<SepaRuleException>().With.Property("Message").Contains("Null or Invalid length of IBAN code"));
        }

        [Test]
        public void ShouldRejectTooShortIban()
        {
            Assert.That(() => { new SepaIbanData { Iban = "FR01234567890" }; },
                Throws.TypeOf<SepaRuleException>().With.Property("Message").Contains("Null or Invalid length of IBAN code"));
        }

        [Test]
        public void ShouldRejectInvalidIbanFormat()
        {
            Assert.That(() => { new SepaIbanData { Iban = "FRBABABA2005500000157845Z02" , Bic = Bic}; },
                Throws.TypeOf<SepaRuleException>().With.Property("Message").Contains("Invalid format of IBAN"));
        }

        [Test]
        public void ShouldRejectInvalidBicFormat()
        {
            Assert.That(() => { new SepaIbanData { Iban = Iban, Bic = "SOGE11PPXXX" }; },
                Throws.TypeOf<SepaRuleException>().With.Property("Message").Contains("Invalid format of BIC/swift code"));
        }
    }
}