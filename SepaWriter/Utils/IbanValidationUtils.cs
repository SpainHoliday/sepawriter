namespace SpainHoliday.SepaWriter.Utils
{
    public static class IbanValidationUtils
    {
        /// <summary>
        /// An IBAN is validated by converting it into an integer and performing a basic mod-97 operation (as described in ISO 7064) on it. If the IBAN is valid, the remainder equals 1. The algorithm of IBAN validation is as follows:
        ///   1.  Check that the total IBAN length is correct as per the country.If not, the IBAN is invalid
        ///   2.  Move the four initial characters to the end of the string
        ///   3.  Replace each letter in the string with two digits, thereby expanding the string, where A = 10, B = 11, ..., Z = 35
        ///   4.  Interpret the string as a decimal integer and compute the remainder of that number on division by 97
        /// </summary>
        /// <param name="iban">The IBAN.</param>
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
                else throw new SepaRuleException("Invalid IBAN Checksum (" + checksum + ")");

                checksum = (checksum * 10 + value) % 97;
            }
            return checksum == 1;
        }

        /// <summary>
        /// Determines whether IBAN code length is valid, based on country code.
        /// NOTE: If the country code is not in this list, it will not fail validation.
        /// </summary>
        /// <param name="iban">The IBAN code.</param>
        /// <returns>
        ///   <c>true</c> if IBAN length is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIbanLengthValid(string iban)
        {
            return iban.Length == GetValidIbanLength(iban);
        }

        /// <summary>
        /// Gets the length of the valid iban by country code (first 2 characters).
        /// </summary>
        /// <param name="iban">The IBAN code.</param>
        /// <returns></returns>
        public static int GetValidIbanLength(string iban)
        {
            var countryCode = iban.Substring(0, 2);
            switch (countryCode)
            {
                case "AL": return 28;
                case "AD": return 24;
                case "AT": return 20;
                case "AZ": return 28;
                case "BH": return 22;
                case "BE": return 16;
                case "BA": return 20;
                case "BR": return 29;
                case "BG": return 22;
                case "CR": return 22;
                case "HR": return 21;
                case "CY": return 28;
                case "CZ": return 24;
                case "DK": return 18;
                case "DO": return 28;
                case "TL": return 23;
                case "EE": return 20;
                case "FO": return 18;
                case "FI": return 18;
                case "FR": return 27;
                case "GE": return 22;
                case "DE": return 22;
                case "GI": return 23;
                case "GR": return 27;
                case "GL": return 18;
                case "GT": return 28;
                case "HU": return 28;
                case "IS": return 26;
                case "IE": return 22;
                case "IL": return 23;
                case "IT": return 27;
                case "JO": return 30;
                case "KZ": return 20;
                case "XK": return 20;
                case "KW": return 30;
                case "LV": return 21;
                case "LB": return 28;
                case "LI": return 21;
                case "LT": return 20;
                case "LU": return 20;
                case "MK": return 19;
                case "MT": return 31;
                case "MR": return 27;
                case "MU": return 30;
                case "MC": return 27;
                case "MD": return 24;
                case "ME": return 22;
                case "NL": return 18;
                case "NO": return 15;
                case "PK": return 24;
                case "PS": return 29;
                case "PL": return 28;
                case "PT": return 25;
                case "QA": return 29;
                case "RO": return 24;
                case "SM": return 27;
                case "SA": return 24;
                case "RS": return 22;
                case "SK": return 24;
                case "SI": return 19;
                case "ES": return 24;
                case "SE": return 24;
                case "CH": return 21;
                case "TN": return 24;
                case "TR": return 26;
                case "AE": return 23;
                case "GB": return 22;
                case "VG": return 24;
                case "DZ": return 24;
                case "AO": return 25;
                case "BJ": return 28;
                case "BF": return 27;
                case "BI": return 16;
                case "CM": return 27;
                case "CV": return 25;
                case "IR": return 26;
                case "CI": return 28;
                case "MG": return 27;
                case "ML": return 28;
                case "MZ": return 25;
                case "SN": return 28;
                case "UA": return 29;
                default:
                    return 34; // IBAN should not be longer than 34 characters
            }
        }

        /// <summary>
        /// Gets the Regular Expression for validating an IBAN code.
        /// </summary>
        /// <param name="iban">The IBAN code.</param>
        /// <returns></returns>
        public static string GetRegex(string iban)
        {
            var countryCode = iban.Substring(0, 2);
            switch (countryCode)
            {
                case "AL": return @"^(AL[0-9]{2}[A-Z0-9]{24}){0,28}$";
                case "AD": return @"^(AD[0-9]{2}[A-Z0-9]{20}){0,24}$";
                case "AT": return @"^(AT[0-9]{2}[A-Z0-9]{16}){0,20}$";
                case "AZ": return @"^(AZ[0-9]{2}[A-Z0-9]{24}){0,28}$";
                case "BH": return @"^(BH[0-9]{2}[A-Z0-9]{18}){0,22}$";
                case "BE": return @"^(BE[0-9]{2}[A-Z0-9]{12}){0,16}$";
                case "BA": return @"^(BA39[A-Z0-9]{16}){0,20}$";
                case "BR": return @"^(BR[0-9]{2}[A-Z0-9]{25}){0,29}$";
                case "BG": return @"^(BG[0-9]{2}[A-Z0-9]{18}){0,22}$";
                case "CR": return @"^(CR[0-9]{2}[A-Z0-9]{18}){0,22}$";
                case "HR": return @"^(HR[0-9]{2}[A-Z0-9]{17}){0,21}$";
                case "CY": return @"^(CY[0-9]{2}[A-Z0-9]{24}){0,28}$";
                case "CZ": return @"^(CZ[0-9]{2}[A-Z0-9]{20}){0,24}$";
                case "DK": return @"^(DK[0-9]{2}[A-Z0-9]{14}){0,18}$";
                case "DO": return @"^(DO[0-9]{2}[A-Z0-9]{24}){0,28}$";
                case "TL": return @"^(TL38[A-Z0-9]{19}){0,23}$";
                case "EE": return @"^(EE[0-9]{2}[A-Z0-9]{16}){0,20}$";
                case "FO": return @"^(FO[0-9]{2}[A-Z0-9]{14}){0,18}$";
                case "FI": return @"^(FI[0-9]{2}[A-Z0-9]{14}){0,18}$";
                case "FR": return @"^(FR[0-9]{2}[A-Z0-9]{23}){0,27}$";
                case "GE": return @"^(GE[0-9]{2}[A-Z0-9]{18}){0,22}$";
                case "DE": return @"^(DE[0-9]{2}[A-Z0-9]{18}){0,22}$";
                case "GI": return @"^(GI[0-9]{2}[A-Z0-9]{19}){0,23}$";
                case "GR": return @"^(GR[0-9]{2}[A-Z0-9]{23}){0,27}$";
                case "GL": return @"^(GL[0-9]{2}[A-Z0-9]{14}){0,18}$";
                case "GT": return @"^(GT[0-9]{2}[A-Z0-9]{24}){0,28}$";
                case "HU": return @"^(HU[0-9]{2}[A-Z0-9]{24}){0,28}$";
                case "IS": return @"^(IS[0-9]{2}[A-Z0-9]{22}){0,26}$";
                case "IE": return @"^(IE[0-9]{2}[A-Z0-9]{18}){0,22}$";
                case "IL": return @"^(IL[0-9]{2}[A-Z0-9]{19}){0,23}$";
                case "IT": return @"^(IT[0-9]{2}[A-Z0-9]{23}){0,27}$";
                case "JO": return @"^(JO[0-9]{2}[A-Z0-9]{26}){0,30}$";
                case "KZ": return @"^(KZ[0-9]{2}[A-Z0-9]{16}){0,20}$";
                case "XK": return @"^(XK[0-9]{2}[A-Z0-9]{16}){0,20}$";
                case "KW": return @"^(KW[0-9]{2}[A-Z0-9]{26}){0,30}$";
                case "LV": return @"^(LV[0-9]{2}[A-Z0-9]{17}){0,21}$";
                case "LB": return @"^(LB[0-9]{2}[A-Z0-9]{24}){0,28}$";
                case "LI": return @"^(LI[0-9]{2}[A-Z0-9]{17}){0,21}$";
                case "LT": return @"^(LT[0-9]{2}[A-Z0-9]{16}){0,20}$";
                case "LU": return @"^(LU[0-9]{2}[A-Z0-9]{16}){0,20}$";
                case "MK": return @"^(MK07[A-Z0-9]{15}){0,19}$";
                case "MT": return @"^(MT[0-9]{2}[A-Z0-9]{27}){0,31}$";
                case "MR": return @"^(MR13[A-Z0-9]{23}){0,27}$";
                case "MU": return @"^(MU[0-9]{2}[A-Z0-9]{26}){0,30}$";
                case "MC": return @"^(MC[0-9]{2}[A-Z0-9]{23}){0,27}$";
                case "MD": return @"^(MD[0-9]{2}[A-Z0-9]{20}){0,24}$";
                case "ME": return @"^(ME25[A-Z0-9]{18}){0,22}$";
                case "NL": return @"^(NL[0-9]{2}[A-Z0-9]{14}){0,18}$";
                case "NO": return @"^(NO[0-9]{2}[A-Z0-9]{11}){0,15}$";
                case "PK": return @"^(PK[0-9]{2}[A-Z0-9]{20}){0,24}$";
                case "PS": return @"^(PS[0-9]{2}[A-Z0-9]{25}){0,29}$";
                case "PL": return @"^(PL[0-9]{2}[A-Z0-9]{24}){0,28}$";
                case "PT": return @"^(PT50[A-Z0-9]{21}){0,25}$";
                case "QA": return @"^(QA[0-9]{2}[A-Z0-9]{25}){0,29}$";
                case "RO": return @"^(RO[0-9]{2}[A-Z0-9]{20}){0,24}$";
                case "SM": return @"^(SM[0-9]{2}[A-Z0-9]{23}){0,27}$";
                case "SA": return @"^(SA[0-9]{2}[A-Z0-9]{20}){0,24}$";
                case "RS": return @"^(RS[0-9]{2}[A-Z0-9]{18}){0,22}$";
                case "SK": return @"^(SK[0-9]{2}[A-Z0-9]{20}){0,24}$";
                case "SI": return @"^(SI56[A-Z0-9]{15}){0,19}$";
                case "ES": return @"^(ES[0-9]{2}[A-Z0-9]{20}){0,24}$";
                case "SE": return @"^(SE[0-9]{2}[A-Z0-9]{20}){0,24}$";
                case "CH": return @"^(CH[0-9]{2}[A-Z0-9]{17}){0,21}$";
                case "TN": return @"^(TN59[A-Z0-9]{20}){0,24}$";
                case "TR": return @"^(TR[0-9]{2}[A-Z0-9]{22}){0,26}$";
                case "AE": return @"^(AE[0-9]{2}[A-Z0-9]{19}){0,23}$";
                case "GB": return @"^(GB[0-9]{2}[A-Z0-9]{18}){0,22}$";
                case "VG": return @"^(VG[0-9]{2}[A-Z0-9]{20}){0,24}$";
                case "DZ": return @"^(DZ[0-9]{2}[A-Z0-9]{20}){0,24}$";
                case "AO": return @"^(AO[0-9]{2}[A-Z0-9]{21}){0,25}$";
                case "BJ": return @"^(BJ[0-9]{2}[A-Z0-9]{24}){0,28}$";
                case "BF": return @"^(BF[0-9]{2}[A-Z0-9]{23}){0,27}$";
                case "BI": return @"^(BI[0-9]{2}[A-Z0-9]{12}){0,16}$";
                case "CM": return @"^(CM[0-9]{2}[A-Z0-9]{23}){0,27}$";
                case "CV": return @"^(CV[0-9]{2}[A-Z0-9]{21}){0,25}$";
                case "IR": return @"^(IR[0-9]{2}[A-Z0-9]{22}){0,26}$";
                case "CI": return @"^(CI[0-9]{2}[A-Z0-9]{24}){0,28}$";
                case "MG": return @"^(MG[0-9]{2}[A-Z0-9]{23}){0,27}$";
                case "ML": return @"^(ML[0-9]{2}[A-Z0-9]{24}){0,28}$";
                case "MZ": return @"^(MZ[0-9]{2}[A-Z0-9]{21}){0,25}$";
                case "SN": return @"^(SN[0-9]{2}[A-Z0-9]{24}){0,28}$";
                case "UA": return @"^(UA[0-9]{2}[A-Z0-9]{25}){0,29}$";
                default:
                    return @"^[A-Z]{2}[0-9]{2}[A-Z0-9]{10,30}$"; // In case of IBAN codes for other countries, just validate the length
            }
        }
    }
}