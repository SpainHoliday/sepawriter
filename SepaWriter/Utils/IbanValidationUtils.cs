using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpainHoliday.SepaWriter.Utils
{
    public static class IbanValidationUtils
    {
        /// <summary>
        /// Determines whether [is iban checksum valid] [the specified iban].
        /// </summary>
        /// <param name="iban">The iban.</param>
        /// <returns></returns>
        /// <exception cref="SepaRuleException">Invalid IBAN Checksum</exception>
        public static bool IsIbanChecksumValid(string iban)
        {
            var checksum = 0;
            var ibanLength = iban.Length;
            for (int charIndex = 0; charIndex < ibanLength; charIndex++)
            {
                if (iban[charIndex] == ' ') continue;

                int value;
                var c = iban[(charIndex + 4) % ibanLength];
                if ((c >= '0') && (c <= '9'))
                {
                    value = c - '0';
                }
                else if ((c >= 'A') && (c <= 'Z'))
                {
                    value = c - 'A';
                    checksum = (checksum * 10 + (value / 10 + 1)) % 97;
                    value %= 10;
                }
                else throw new SepaRuleException("Invalid IBAN Checksum");

                checksum = (checksum * 10 + value) % 97;
            }
            return checksum == 1;
        }
    }
}
