using System;
using System.IO;
using NUnit.Framework;
using SpainHoliday.SepaWriter.Utils;

namespace SpainHoliday.SepaWriter.Test
{
    [TestFixture]
    public class SepaDebitTransferTest
    {
        // See valid generated test IBAN codes http://www.mobilefish.com/services/random_iban_generator/random_iban_generator.php
        // See also https://ssl.ibanrechner.de/sample_accounts.html?&L=0

        private static readonly SepaIbanData Creditor = new SepaIbanData
        {
            Bic = "SOGEFRPPXXX",
            Iban = "FR7030002005500000157845Z02",
            Name = "My Corp"
        };

        private const string FILENAME = "sepa_test_result.xml";

        private const string ONE_ROW_RESULT =
            "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?><Document xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:iso:std:iso:20022:tech:xsd:pain.008.001.02\"><CstmrDrctDbtInitn><GrpHdr><MsgId>transferID</MsgId><CreDtTm>2013-02-17T22:38:12</CreDtTm><NbOfTxs>1</NbOfTxs><CtrlSum>23.45</CtrlSum><InitgPty><Nm>Me</Nm><Id><OrgId><Othr><Id>MyId</Id></Othr></OrgId></Id></InitgPty></GrpHdr><PmtInf><PmtInfId>paymentInfo</PmtInfId><PmtMtd>DD</PmtMtd><NbOfTxs>1</NbOfTxs><CtrlSum>23.45</CtrlSum><PmtTpInf><SvcLvl><Cd>SEPA</Cd></SvcLvl><LclInstrm><Cd>CORE</Cd></LclInstrm><SeqTp>OOFF</SeqTp></PmtTpInf><ReqdColltnDt>2013-02-17</ReqdColltnDt><Cdtr><Nm>My Corp</Nm></Cdtr><CdtrAcct><Id><IBAN>FR7030002005500000157845Z02</IBAN></Id><Ccy>EUR</Ccy></CdtrAcct><CdtrAgt><FinInstnId><BIC>SOGEFRPPXXX</BIC></FinInstnId></CdtrAgt><ChrgBr>SLEV</ChrgBr><CdtrSchmeId><Id><PrvtId><Othr><Id /><SchmeNm><Prtry>SEPA</Prtry></SchmeNm></Othr></PrvtId></Id></CdtrSchmeId><DrctDbtTxInf><PmtId><InstrId>Transaction Id 1</InstrId><EndToEndId>paymentInfo/1</EndToEndId></PmtId><InstdAmt Ccy=\"EUR\">23.45</InstdAmt><DrctDbtTx><MndtRltdInf><MndtId>First mandate</MndtId><DtOfSgntr>2012-12-07</DtOfSgntr></MndtRltdInf></DrctDbtTx><DbtrAgt><FinInstnId><BIC>AGRIFRPPXXX</BIC></FinInstnId></DbtrAgt><Dbtr><Nm>THEIR_NAME</Nm></Dbtr><DbtrAcct><Id><IBAN>FR1420041010050500013M02606</IBAN></Id></DbtrAcct><RmtInf><Ustrd>Transaction description</Ustrd></RmtInf></DrctDbtTxInf></PmtInf></CstmrDrctDbtInitn></Document>";

        private string MULTIPLE_ROW_RESULT (string iban){
            return "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?><Document xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:iso:std:iso:20022:tech:xsd:pain.008.001.02\"><CstmrDrctDbtInitn><GrpHdr><MsgId>transferID</MsgId><CreDtTm>2013-02-17T22:38:12</CreDtTm><NbOfTxs>3</NbOfTxs><CtrlSum>63.36</CtrlSum><InitgPty><Nm>Me</Nm></InitgPty></GrpHdr><PmtInf><PmtInfId>paymentInfo</PmtInfId><PmtMtd>DD</PmtMtd><NbOfTxs>3</NbOfTxs><CtrlSum>63.36</CtrlSum><PmtTpInf><SvcLvl><Cd>SEPA</Cd></SvcLvl><LclInstrm><Cd>CORE</Cd></LclInstrm><SeqTp>OOFF</SeqTp></PmtTpInf><ReqdColltnDt>2013-02-18</ReqdColltnDt><Cdtr><Nm>My Corp</Nm></Cdtr><CdtrAcct><Id><IBAN>FR7030002005500000157845Z02</IBAN></Id><Ccy>EUR</Ccy></CdtrAcct><CdtrAgt><FinInstnId><BIC>SOGEFRPPXXX</BIC></FinInstnId></CdtrAgt><ChrgBr>SLEV</ChrgBr><CdtrSchmeId><Id><PrvtId><Othr><Id /><SchmeNm><Prtry>SEPA</Prtry></SchmeNm></Othr></PrvtId></Id></CdtrSchmeId><DrctDbtTxInf><PmtId><InstrId>Transaction Id 1</InstrId><EndToEndId>multiple1</EndToEndId></PmtId><InstdAmt Ccy=\"EUR\">23.45</InstdAmt><DrctDbtTx><MndtRltdInf><MndtId>First mandate</MndtId><DtOfSgntr>2012-12-07</DtOfSgntr></MndtRltdInf></DrctDbtTx><DbtrAgt><FinInstnId><BIC>AGRIFRPPXXX</BIC></FinInstnId></DbtrAgt><Dbtr><Nm>THEIR_NAME</Nm></Dbtr><DbtrAcct><Id><IBAN>FR1420041010050500013M02606</IBAN></Id></DbtrAcct><RmtInf><Ustrd>Transaction description</Ustrd></RmtInf></DrctDbtTxInf><DrctDbtTxInf><PmtId><InstrId>Transaction Id 2</InstrId><EndToEndId>paymentInfo/2</EndToEndId></PmtId><InstdAmt Ccy=\"EUR\">12.56</InstdAmt><DrctDbtTx><MndtRltdInf><MndtId>First mandate</MndtId><DtOfSgntr>2012-12-07</DtOfSgntr></MndtRltdInf></DrctDbtTx><DbtrAgt><FinInstnId><BIC>AGRIFRPPXXX</BIC></FinInstnId></DbtrAgt><Dbtr><Nm>THEIR_NAME</Nm></Dbtr><DbtrAcct><Id><IBAN>FR1420041010050500013M02606</IBAN></Id></DbtrAcct><RmtInf><Ustrd>Transaction description 2</Ustrd></RmtInf></DrctDbtTxInf><DrctDbtTxInf><PmtId><InstrId>Transaction Id 3</InstrId><EndToEndId>paymentInfo/3</EndToEndId></PmtId><InstdAmt Ccy=\"EUR\">27.35</InstdAmt><DrctDbtTx><MndtRltdInf><MndtId /><DtOfSgntr>0001-01-01</DtOfSgntr></MndtRltdInf></DrctDbtTx><DbtrAgt><FinInstnId><BIC>AAAAAA2A</BIC></FinInstnId></DbtrAgt><Dbtr><Nm>NAME</Nm></Dbtr><DbtrAcct><Id><IBAN>"+iban+"</IBAN></Id></DbtrAcct><RmtInf><Ustrd>Transaction description 3</Ustrd></RmtInf></DrctDbtTxInf></PmtInf></CstmrDrctDbtInitn></Document>"; }
        
        private static SepaDebitTransferTransaction CreateTransaction(string id, decimal amount, string information)
        {
            return new SepaDebitTransferTransaction
            {
                Id = id,
                Debtor = new SepaIbanData
                {
                    Bic = "AGRIFRPPXXX",
                    Iban = "FR1420041010050500013M02606",
                    Name = "THEIR_NAME"
                },
                MandateIdentification = "First mandate",
                DateOfSignature = new DateTime(2012, 12, 7),
                Amount = amount,
                RemittanceInformation = information
            };
        }

        private static SepaDebitTransfer GetEmptyDebitTransfert()
        {
            return new SepaDebitTransfer
            {
                CreationDate = new DateTime(2013, 02, 17, 22, 38, 12),
                RequestedExecutionDate = new DateTime(2013, 02, 17),
                MessageIdentification = "transferID",
                PaymentInfoId = "paymentInfo",
                InitiatingPartyName = "Me",
                Creditor = Creditor
            };
        }

        private static SepaDebitTransfer GetOneTransactionDebitTransfert(decimal amount)
        {
            var transfert = GetEmptyDebitTransfert();
            transfert.InitiatingPartyId = "MyId";
            transfert.AddDebitTransfer(CreateTransaction("Transaction Id 1",amount,"Transaction description"));
            return transfert;
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(FILENAME))
                File.Delete(FILENAME);
        }

        [Test]
        public void ShouldAllowMultipleNullIdTransations()
        {
            const decimal amount = 23.45m;

            SepaDebitTransfer transfert = GetOneTransactionDebitTransfert(amount);

            transfert.AddDebitTransfer(new SepaDebitTransferTransaction
                {
                    Id = null,
                    Debtor = new SepaIbanData
                        {
                            Bic = "AGRIFRPPXXX",
                            Iban = "FR1420041010050500013M02606",
                            Name = "THEIR_NAME"
                        },
                    Amount = amount,
                    RemittanceInformation = "Transaction description 1",
                    MandateIdentification = "mandate 1",
                    DateOfSignature = new DateTime(2010, 12, 7),
                });

            transfert.AddDebitTransfer(new SepaDebitTransferTransaction
                {
                    Id = null,
                    Debtor = new SepaIbanData
                        {
                            Bic = "AGRIFRPPXXX",
                            Iban = "FR1420041010050500013M02606",
                            Name = "THEIR_NAME"
                        },
                    Amount = amount,
                    RemittanceInformation = "Transaction description 2",
                    MandateIdentification = "mandate 2",
                    DateOfSignature = new DateTime(2011, 12, 7),
                });
        }

        [Test]
        public void ShouldKeepEndToEndIdIfSet()
        {
            const decimal amount = 23.45m;

            SepaDebitTransfer transfert = GetOneTransactionDebitTransfert(amount);

            var trans = CreateTransaction(null, amount, "Transaction description 2");
            trans.EndToEndId = "endToendId1";
            transfert.AddDebitTransfer(trans);

            trans = CreateTransaction(null, amount, "Transaction description 3");
            trans.EndToEndId = "endToendId2";
            transfert.AddDebitTransfer(trans);

            string result = transfert.AsXmlString();

            Assert.True(result.Contains("<EndToEndId>endToendId1</EndToEndId>"));
            Assert.True(result.Contains("<EndToEndId>endToendId2</EndToEndId>"));
        }

        [TestCase("AL47212110090000000235698741")] // Albania
        [TestCase("AD1200012030200359100100")] // Andorra
        [TestCase("AT611904300234573201")] // Austria
        [TestCase("AZ21NABZ00000000137010001944")] // Azerbaijan, Republic of
        //[TestCase("BH67BMAG00001299123456")] // Bahrain not a SEPA member
        [TestCase("BE68539007547034")] // Belgium
        [TestCase("BA391290079401028494")] // Bosnia and Herzegovina
        //[TestCase("BR7724891749412660603618210F3")] // Brazil not a SEPA member
        [TestCase("BG80BNBG96611020345678")] // Bulgaria
        //[TestCase("CR0515202001026284066")] // Costa Rica not a SEPA member
        [TestCase("HR1210010051863000160")] // Croatia
        [TestCase("CY17002001280000001200527600")] // Cyprus
        [TestCase("CZ6508000000192000145399")] // Czech Republic
        [TestCase("DK5000400440116243")] // Denmark
        //[TestCase("DO28BAGR00000001212453611324")] // Dominican Republic not a SEPA member
        [TestCase("EE382200221020145685")] // Estonia
        [TestCase("FO6264600001631634")] // Faroe Islands
        [TestCase("FI2112345600000785")] // Finland
        [TestCase("FR1420041010050500013M02606")] // France
        [TestCase("GE29NB0000000101904917")] // Georgia
        [TestCase("DE89370400440532013000")] // Germany
        [TestCase("GI75NWBK000000007099453")] // Gibraltar
        [TestCase("GR1601101250000000012300695")] // Greece
        [TestCase("GL8964710001000206")] // Greenland
        //[TestCase("GT82TRAJ01020000001210029690")] // Guatemala not a SEPA member
        [TestCase("HU42117730161111101800000000")] // Hungary
        [TestCase("IS140159260076545510730339")] // Iceland
        [TestCase("IE29AIBK93115212345678")] // Ireland
        [TestCase("IL620108000000099999999")] // Israel
        [TestCase("IT60X0542811101000000123456")] // Italy
        [TestCase("JO94CBJO0010000000000131000302")] // Jordan
        //[TestCase("KZ86125KZT5004100100")] // Kazakhstan not a SEPA member
        //[TestCase("KW81CBKU0000000000001234560101")] // Kuwait not a SEPA member
        [TestCase("LV80BANK0000435195001")] // Latvia
        //[TestCase("LB62099900000001001901229114")] // Lebanon not a SEPA member
        [TestCase("LI21088100002324013AA")] // Liechtenstein (Principality of)
        [TestCase("LT121000011101001000")] // Lithuania
        [TestCase("LU280019400644750000")] // Luxembourg
        [TestCase("MK07250120000058984")] // Macedonia
        [TestCase("MT84MALT011000012345MTLCAST001S")] // Malta
        //[TestCase("MR1300020001010000123456753")] // Mauritania not a SEPA member
        //[TestCase("MU17BOMM0101101030300200000MUR")] // Mauritius not a SEPA member
        [TestCase("MD24AG000225100013104168")] // Moldova
        [TestCase("MC5811222000010123456789030")] // Monaco
        [TestCase("ME25505000012345678951")] // Montenegro
        [TestCase("NL91ABNA0417164300")] // Netherlands
        [TestCase("NO9386011117947")] // Norway
        //[TestCase("PK36SCBL0000001123456702")] // Pakistan not a SEPA member
        [TestCase("PL61109010140000071219812874")] // Poland
        //[TestCase("PS92PALS000000000400123456702")] // Palestinian Territory, Occupied not a SEPA member
        [TestCase("PT50000201231234567890154")] // Portugal
        //[TestCase("QA58DOHB00001234567890ABCDEFG")] // Qatar not a SEPA member
        [TestCase("XK051212012345678906")] // Republic of Kosovo
        [TestCase("RO49AAAA1B31007593840000")] // Romania
        //[TestCase("LC55HEMM000100010012001200023015")] // Saint Lucia
        [TestCase("SM86U0322509800000000270100")] // San Marino
        //[TestCase("ST68000100010051845310112")] // Sao Tome And Principe not a SEPA member
        //[TestCase("SA0380000000608010167519")] // Saudi Arabia not a SEPA member
        [TestCase("RS35260005601001611379")] // Serbia
        //[TestCase("SC18SSCB11010000000000001497USD")] // Seychelles not a SEPA member
        [TestCase("SK3112000000198742637541")] // Slovak Republic
        [TestCase("SI56263300012039086")] // Slovenia
        [TestCase("ES9121000418450200051332")] // Spain
        [TestCase("SE4550000000058398257466")] // Sweden
        [TestCase("CH9300762011623852957")] // Switzerland
        //[TestCase("TL380080012345678910157")] // Timor-Leste not a SEPA member
        //[TestCase("TN5910006035183598478831")] // Tunisia not a SEPA member
        [TestCase("TR330006100519786457841326")] // Turkey
        [TestCase("UA213996220000026007233566001")] // Ukraine
        [TestCase("AE070331234567890123456")] // United Arab Emirates
        [TestCase("GB29NWBK60161331926819")] // United Kingdom
        [TestCase("VG96VPVG0000012345678901")] // Virgin Islands, British
        public void ShouldManageMultipleTransactionsTransfer(string iban)
        {
            var transfert = new SepaDebitTransfer
                {
                    CreationDate = new DateTime(2013, 02, 17, 22, 38, 12),
                    RequestedExecutionDate = new DateTime(2013, 02, 18),
                    MessageIdentification = "transferID",
                    PaymentInfoId = "paymentInfo",
                    InitiatingPartyName = "Me",
                    Creditor = Creditor
                };

            const decimal amount = 23.45m;
            var trans = CreateTransaction("Transaction Id 1", amount, "Transaction description");
            trans.EndToEndId = "multiple1";
            transfert.AddDebitTransfer(trans);

            const decimal amount2 = 12.56m;
            transfert.AddDebitTransfer(CreateTransaction("Transaction Id 2", amount2, "Transaction description 2"));


            const decimal amount3 = 27.35m;
            transfert.AddDebitTransfer(new SepaDebitTransferTransaction
                {
                    Id = "Transaction Id 3",
                    Debtor = new SepaIbanData
                        {
                            Bic = "AAAAAA2A",
                            Iban = iban,//"AA1234567890123",
                            Name = "NAME"
                        },
                    Amount = amount3,
                    RemittanceInformation = "Transaction description 3"
                });

            const decimal total = (amount + amount2 + amount3)*100;

            Assert.AreEqual(total, transfert.HeaderControlSumInCents);
            Assert.AreEqual(total, transfert.PaymentControlSumInCents);

            Assert.AreEqual(MULTIPLE_ROW_RESULT(iban), transfert.AsXmlString());
            
        }


        [TestCase("AL47212110090000000235698741")] // Albania
        [TestCase("AD1200012030200359100100")] // Andorra
        [TestCase("AT611904300234573201")] // Austria
        [TestCase("AZ21NABZ00000000137010001944")] // Azerbaijan, Republic of
        //[TestCase("BH67BMAG00001299123456")] // Bahrain not a SEPA member
        [TestCase("BE68539007547034")] // Belgium
        [TestCase("BA391290079401028494")] // Bosnia and Herzegovina
        //[TestCase("BR7724891749412660603618210F3")] // Brazil not a SEPA member
        [TestCase("BG80BNBG96611020345678")] // Bulgaria
        //[TestCase("CR0515202001026284066")] // Costa Rica not a SEPA member
        [TestCase("HR1210010051863000160")] // Croatia
        [TestCase("CY17002001280000001200527600")] // Cyprus
        [TestCase("CZ6508000000192000145399")] // Czech Republic
        [TestCase("DK5000400440116243")] // Denmark
        //[TestCase("DO28BAGR00000001212453611324")] // Dominican Republic not a SEPA member
        [TestCase("EE382200221020145685")] // Estonia
        [TestCase("FO6264600001631634")] // Faroe Islands
        [TestCase("FI2112345600000785")] // Finland
        [TestCase("FR1420041010050500013M02606")] // France
        [TestCase("GE29NB0000000101904917")] // Georgia
        [TestCase("DE89370400440532013000")] // Germany
        [TestCase("GI75NWBK000000007099453")] // Gibraltar
        [TestCase("GR1601101250000000012300695")] // Greece
        [TestCase("GL8964710001000206")] // Greenland
        //[TestCase("GT82TRAJ01020000001210029690")] // Guatemala not a SEPA member
        [TestCase("HU42117730161111101800000000")] // Hungary
        [TestCase("IS140159260076545510730339")] // Iceland
        [TestCase("IE29AIBK93115212345678")] // Ireland
        [TestCase("IL620108000000099999999")] // Israel
        [TestCase("IT60X0542811101000000123456")] // Italy
        [TestCase("JO94CBJO0010000000000131000302")] // Jordan
        //[TestCase("KZ86125KZT5004100100")] // Kazakhstan not a SEPA member
        //[TestCase("KW81CBKU0000000000001234560101")] // Kuwait not a SEPA member
        [TestCase("LV80BANK0000435195001")] // Latvia
        //[TestCase("LB62099900000001001901229114")] // Lebanon not a SEPA member
        [TestCase("LI21088100002324013AA")] // Liechtenstein (Principality of)
        [TestCase("LT121000011101001000")] // Lithuania
        [TestCase("LU280019400644750000")] // Luxembourg
        [TestCase("MK07250120000058984")] // Macedonia
        [TestCase("MT84MALT011000012345MTLCAST001S")] // Malta
        //[TestCase("MR1300020001010000123456753")] // Mauritania not a SEPA member
        //[TestCase("MU17BOMM0101101030300200000MUR")] // Mauritius not a SEPA member
        [TestCase("MD24AG000225100013104168")] // Moldova
        [TestCase("MC5811222000010123456789030")] // Monaco
        [TestCase("ME25505000012345678951")] // Montenegro
        [TestCase("NL91ABNA0417164300")] // Netherlands
        [TestCase("NO9386011117947")] // Norway
        //[TestCase("PK36SCBL0000001123456702")] // Pakistan not a SEPA member
        [TestCase("PL61109010140000071219812874")] // Poland
        //[TestCase("PS92PALS000000000400123456702")] // Palestinian Territory, Occupied not a SEPA member
        [TestCase("PT50000201231234567890154")] // Portugal
        //[TestCase("QA58DOHB00001234567890ABCDEFG")] // Qatar not a SEPA member
        [TestCase("XK051212012345678906")] // Republic of Kosovo
        [TestCase("RO49AAAA1B31007593840000")] // Romania
        //[TestCase("LC55HEMM000100010012001200023015")] // Saint Lucia
        [TestCase("SM86U0322509800000000270100")] // San Marino
        //[TestCase("ST68000100010051845310112")] // Sao Tome And Principe not a SEPA member
        //[TestCase("SA0380000000608010167519")] // Saudi Arabia not a SEPA member
        [TestCase("RS35260005601001611379")] // Serbia
        //[TestCase("SC18SSCB11010000000000001497USD")] // Seychelles not a SEPA member
        [TestCase("SK3112000000198742637541")] // Slovak Republic
        [TestCase("SI56263300012039086")] // Slovenia
        [TestCase("ES9121000418450200051332")] // Spain
        [TestCase("SE4550000000058398257466")] // Sweden
        [TestCase("CH9300762011623852957")] // Switzerland
        //[TestCase("TL380080012345678910157")] // Timor-Leste not a SEPA member
        //[TestCase("TN5910006035183598478831")] // Tunisia not a SEPA member
        [TestCase("TR330006100519786457841326")] // Turkey
        [TestCase("UA213996220000026007233566001")] // Ukraine
        [TestCase("AE070331234567890123456")] // United Arab Emirates
        [TestCase("GB29NWBK60161331926819")] // United Kingdom
        [TestCase("VG96VPVG0000012345678901")] // Virgin Islands, British
        public void ShouldValidateThePain00800102XmlSchema(string iban)
        {
            var transfert = new SepaDebitTransfer
            {
                CreationDate = new DateTime(2013, 02, 17, 22, 38, 12),
                RequestedExecutionDate = new DateTime(2013, 02, 18),
                MessageIdentification = "transferID",
                PaymentInfoId = "paymentInfo",
                InitiatingPartyName = "Me",
                Creditor = Creditor
            };

            const decimal amount = 23.45m;
            var trans = CreateTransaction("Transaction Id 1", amount, "Transaction description");
            trans.EndToEndId = "multiple1";
            transfert.AddDebitTransfer(trans);

            const decimal amount2 = 12.56m;
            transfert.AddDebitTransfer(CreateTransaction("Transaction Id 2", amount2, "Transaction description 2"));


            const decimal amount3 = 27.35m;
            transfert.AddDebitTransfer(new SepaDebitTransferTransaction
            {
                Id = "Transaction Id 3",
                Debtor = new SepaIbanData
                {
                    Bic = "AAAAAA2A",
                    Iban = iban,//"AA1234567890123",
                    Name = "NAME"
                },
                Amount = amount3,
                RemittanceInformation = "Transaction description 3"
            });

            var validator = XmlValidator.GetValidator(transfert.Schema);
            validator.Validate(transfert.AsXmlString());
        }


        [TestCase("AL47212110090000000235698741")] // Albania
        [TestCase("AD1200012030200359100100")] // Andorra
        [TestCase("AT611904300234573201")] // Austria
        [TestCase("AZ21NABZ00000000137010001944")] // Azerbaijan, Republic of
        //[TestCase("BH67BMAG00001299123456")] // Bahrain not a SEPA member
        [TestCase("BE68539007547034")] // Belgium
        [TestCase("BA391290079401028494")] // Bosnia and Herzegovina
        //[TestCase("BR7724891749412660603618210F3")] // Brazil not a SEPA member
        [TestCase("BG80BNBG96611020345678")] // Bulgaria
        //[TestCase("CR0515202001026284066")] // Costa Rica not a SEPA member
        [TestCase("HR1210010051863000160")] // Croatia
        [TestCase("CY17002001280000001200527600")] // Cyprus
        [TestCase("CZ6508000000192000145399")] // Czech Republic
        [TestCase("DK5000400440116243")] // Denmark
        //[TestCase("DO28BAGR00000001212453611324")] // Dominican Republic not a SEPA member
        [TestCase("EE382200221020145685")] // Estonia
        [TestCase("FO6264600001631634")] // Faroe Islands
        [TestCase("FI2112345600000785")] // Finland
        [TestCase("FR1420041010050500013M02606")] // France
        [TestCase("GE29NB0000000101904917")] // Georgia
        [TestCase("DE89370400440532013000")] // Germany
        [TestCase("GI75NWBK000000007099453")] // Gibraltar
        [TestCase("GR1601101250000000012300695")] // Greece
        [TestCase("GL8964710001000206")] // Greenland
        //[TestCase("GT82TRAJ01020000001210029690")] // Guatemala not a SEPA member
        [TestCase("HU42117730161111101800000000")] // Hungary
        [TestCase("IS140159260076545510730339")] // Iceland
        [TestCase("IE29AIBK93115212345678")] // Ireland
        [TestCase("IL620108000000099999999")] // Israel
        [TestCase("IT60X0542811101000000123456")] // Italy
        [TestCase("JO94CBJO0010000000000131000302")] // Jordan
        //[TestCase("KZ86125KZT5004100100")] // Kazakhstan not a SEPA member
        //[TestCase("KW81CBKU0000000000001234560101")] // Kuwait not a SEPA member
        [TestCase("LV80BANK0000435195001")] // Latvia
        //[TestCase("LB62099900000001001901229114")] // Lebanon not a SEPA member
        [TestCase("LI21088100002324013AA")] // Liechtenstein (Principality of)
        [TestCase("LT121000011101001000")] // Lithuania
        [TestCase("LU280019400644750000")] // Luxembourg
        [TestCase("MK07250120000058984")] // Macedonia
        [TestCase("MT84MALT011000012345MTLCAST001S")] // Malta
        //[TestCase("MR1300020001010000123456753")] // Mauritania not a SEPA member
        //[TestCase("MU17BOMM0101101030300200000MUR")] // Mauritius not a SEPA member
        [TestCase("MD24AG000225100013104168")] // Moldova
        [TestCase("MC5811222000010123456789030")] // Monaco
        [TestCase("ME25505000012345678951")] // Montenegro
        [TestCase("NL91ABNA0417164300")] // Netherlands
        [TestCase("NO9386011117947")] // Norway
        //[TestCase("PK36SCBL0000001123456702")] // Pakistan not a SEPA member
        [TestCase("PL61109010140000071219812874")] // Poland
        //[TestCase("PS92PALS000000000400123456702")] // Palestinian Territory, Occupied not a SEPA member
        [TestCase("PT50000201231234567890154")] // Portugal
        //[TestCase("QA58DOHB00001234567890ABCDEFG")] // Qatar not a SEPA member
        [TestCase("XK051212012345678906")] // Republic of Kosovo
        [TestCase("RO49AAAA1B31007593840000")] // Romania
        //[TestCase("LC55HEMM000100010012001200023015")] // Saint Lucia
        [TestCase("SM86U0322509800000000270100")] // San Marino
        //[TestCase("ST68000100010051845310112")] // Sao Tome And Principe not a SEPA member
        //[TestCase("SA0380000000608010167519")] // Saudi Arabia not a SEPA member
        [TestCase("RS35260005601001611379")] // Serbia
        //[TestCase("SC18SSCB11010000000000001497USD")] // Seychelles not a SEPA member
        [TestCase("SK3112000000198742637541")] // Slovak Republic
        [TestCase("SI56263300012039086")] // Slovenia
        [TestCase("ES9121000418450200051332")] // Spain
        [TestCase("SE4550000000058398257466")] // Sweden
        [TestCase("CH9300762011623852957")] // Switzerland
        //[TestCase("TL380080012345678910157")] // Timor-Leste not a SEPA member
        //[TestCase("TN5910006035183598478831")] // Tunisia not a SEPA member
        [TestCase("TR330006100519786457841326")] // Turkey
        [TestCase("UA213996220000026007233566001")] // Ukraine
        [TestCase("AE070331234567890123456")] // United Arab Emirates
        [TestCase("GB29NWBK60161331926819")] // United Kingdom
        [TestCase("VG96VPVG0000012345678901")] // Virgin Islands, British
        public void ShouldValidateThePain00800103XmlSchema(string iban)
        {
            var transfert = new SepaDebitTransfer
            {
                CreationDate = new DateTime(2013, 02, 17, 22, 38, 12),
                RequestedExecutionDate = new DateTime(2013, 02, 18),
                MessageIdentification = "transferID",
                PaymentInfoId = "paymentInfo",
                InitiatingPartyName = "Me",
                Creditor = Creditor,
                Schema = SepaSchema.Pain00800103
            };

            const decimal amount = 23.45m;
            var trans = CreateTransaction("Transaction Id 1", amount, "Transaction description");
            trans.EndToEndId = "multiple1";
            transfert.AddDebitTransfer(trans);

            const decimal amount2 = 12.56m;
            transfert.AddDebitTransfer(CreateTransaction("Transaction Id 2", amount2, "Transaction description 2"));


            const decimal amount3 = 27.35m;
            transfert.AddDebitTransfer(new SepaDebitTransferTransaction
            {
                Id = "Transaction Id 3",
                Debtor = new SepaIbanData
                {
                    Bic = "AAAAAA2A",
                    Iban = iban,//"AA1234567890123",
                    Name = "NAME"
                },
                Amount = amount3,
                RemittanceInformation = "Transaction description 3"
            });

            var validator = XmlValidator.GetValidator(transfert.Schema);
            validator.Validate(transfert.AsXmlString());
        }

        [Test]
        public void ShouldRejectNotAllowedXmlSchema()
        {
            Assert.That(() => { new SepaDebitTransfer { Schema = SepaSchema.Pain00100103 }; },
                Throws.TypeOf<ArgumentException>().With.Property("Message").Contains("schema is not allowed!"));
        }

        [Test]
        public void ShouldManageOneTransactionTransfer()
        {
            const decimal amount = 23.45m;
            SepaDebitTransfer transfert = GetOneTransactionDebitTransfert(amount);

            const decimal total = amount*100;
            Assert.AreEqual(total, transfert.HeaderControlSumInCents);
            Assert.AreEqual(total, transfert.PaymentControlSumInCents);

            Assert.AreEqual(ONE_ROW_RESULT, transfert.AsXmlString());
        }

        [Test]
        public void ShouldAllowTransactionWithoutRemittanceInformation()
        {
            var transfert = GetEmptyDebitTransfert();
            transfert.AddDebitTransfer(CreateTransaction(null, 12m, null));
            transfert.AddDebitTransfer(CreateTransaction(null, 13m, null));

            string result = transfert.AsXmlString();
            Assert.False(result.Contains("<RmtInf>"));
        }

        [Test]
        public void ShouldRejectIfNoCreditor()
        {
            var transfert = new SepaDebitTransfer
                {
                    MessageIdentification = "transferID",
                    PaymentInfoId = "paymentInfo",
                    InitiatingPartyName = "Me"
                };
            transfert.AddDebitTransfer(CreateTransaction("Transaction Id 1", 100m, "Transaction description"));
            Assert.That(() => { transfert.AsXmlString(); },
                Throws.TypeOf<SepaRuleException>().With.Property("Message").EqualTo("The creditor is mandatory."));
            
        }

        [Test]
        public void ShouldRejectIfNoInitiatingPartyName()
        {
            SepaDebitTransfer transfert = GetOneTransactionDebitTransfert(100m);
            transfert.InitiatingPartyName = null;
            Assert.That(() => { transfert.AsXmlString(); },
                Throws.TypeOf<SepaRuleException>().With.Property("Message").EqualTo("The initial party name is mandatory."));
        }

        [Test]
        public void ShouldRejectIfNoMessageIdentification()
        {
            SepaDebitTransfer transfert = GetOneTransactionDebitTransfert(100m);
            transfert.MessageIdentification = null;
            Assert.That(() => { transfert.AsXmlString(); },
                Throws.TypeOf<SepaRuleException>().With.Property("Message").EqualTo("The message identification is mandatory."));
        }

        [Test]
        public void ShouldUseMessageIdentificationAsPaymentInfoIdIfNotDefined()
        {
            SepaDebitTransfer transfert = GetOneTransactionDebitTransfert(100m);
            transfert.PaymentInfoId = null;

            string result = transfert.AsXmlString();

            Assert.True(result.Contains("<PmtInfId>"+ transfert.MessageIdentification + "</PmtInfId>"));
        }

        [Test]
        public void ShouldRejectIfNoTransaction()
        {
            var transfert = new SepaDebitTransfer
                {
                    MessageIdentification = "transferID",
                    PaymentInfoId = "paymentInfo",
                    InitiatingPartyName = "Me",
                    Creditor = Creditor
                };

            Assert.That(() => { transfert.AsXmlString(); },
                Throws.TypeOf<SepaRuleException>().With.Property("Message").EqualTo("At least one transaction is needed in a transfer."));
        }

        [Test]
        public void ShouldRejectInvalidCreditor()
        {
            Assert.That(() => { new SepaDebitTransfer { Creditor = new SepaIbanData() }; },
                Throws.TypeOf<SepaRuleException>().With.Property("Message").EqualTo("Creditor IBAN data are invalid."));            
        }

        [Test]
        public void ShouldRejectCreditorWithoutBic()
        {
            var iban = (SepaIbanData)Creditor.Clone();
            iban.UnknownBic = true;
            Assert.That(() => { new SepaDebitTransfer { Creditor = iban }; },
                Throws.TypeOf<SepaRuleException>().With.Property("Message").EqualTo("Creditor IBAN data are invalid."));
            
        }

        [Test]
        public void ShouldRejectNullTransactionTransfer()
        {
            var transfert = new SepaDebitTransfer();

            
            Assert.That(() => { transfert.AddDebitTransfer(null); },
                Throws.TypeOf<ArgumentNullException>().With.Property("Message").Contains("transfer"));
        }

        [Test]
        public void ShouldRejectTwoTransationsWithSameId()
        {
            SepaDebitTransfer transfert = GetOneTransactionDebitTransfert(100m);
            var debit = CreateTransaction("Transaction Id 1", 23.45m, "Transaction description 2");

            Assert.That(() => { transfert.AddDebitTransfer(debit); },
                Throws.TypeOf<SepaRuleException>().With.Property("Message").Contains("must be unique in a transfer"));
        }
        
        [Test]
        public void ShouldUseADefaultCurrency()
        {
            var transfert = new SepaDebitTransfer();
            Assert.AreEqual("EUR", transfert.CreditorAccountCurrency);
        }

        [Test]
        public void ShouldUseADefaultLocalInstrumentCode()
        {
            var transfert = new SepaDebitTransfer();
            Assert.AreEqual("CORE", transfert.LocalInstrumentCode);
        }

        [Test]
        public void ShouldNotAllowNullCurrencyString()
        {
            Assert.Throws<SepaRuleException>(() => new SepaDebitTransfer((string)null));
        }

        [Test]
        public void ShouldNotAllowShortCurrencyString()
        {
            Assert.Throws<SepaRuleException>(() => new SepaDebitTransfer("DK"));
        }

        [Test]
        public void ShouldNotAllowLongCurrencyString()
        {
            Assert.Throws<SepaRuleException>(() => new SepaDebitTransfer("EURO"));
        }

        [Test]
        public void ShouldSetCurrency()
        {
            var transfer = new SepaDebitTransfer("DKK");
            Assert.That(transfer.CreditorAccountCurrency, Is.EqualTo("DKK"));
        }

        [Test]
        public void ShouldSetCurrencyCaseInsensitive()
        {
            var transfer = new SepaDebitTransfer("Dkk");
            Assert.That(transfer.CreditorAccountCurrency, Is.EqualTo("DKK"));
        }
    }
}