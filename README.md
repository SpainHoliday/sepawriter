SepaWriter
===
[![sepawriter MyGet Build Status](https://www.myget.org/BuildSource/Badge/sepawriter?identifier=e1d53645-035c-47b4-bf78-48c34f912349)](https://www.myget.org/)

Manage SEPA (Single Euro Payments Area) Credit and Debit Transfer for SEPA or international order.
Only one PaymentInformation is managed but it can manage multiple transactions. 
- The debitor is common to all transactions in a Credit transfer.
- The creditor is common to all transactions in a Debit transfer.

It follows the "Customer Credit Transfer Initiation" &lt;pain.001.001.03&gt; defined in ISO 20022 - the original repository also includes some specific french rules (field used size != allowed size).
Debit uses &lt;pain.008.001.02&gt; defined in ISO 20022 and the same french restrictions.

This version of the SepaWriter repository has automatic conversion to UPPERCASE for Bic/IBAN, with Regex added for Bic and IBAN code validation.


Version 1.0.5
---
Improved IBAN validation, with regex specific to each country.

Version 1.0.3
---
IBAN checksum validation was added, as well as regex for Bic and IBAN validation.


Usage
---
In [English](http://www.swift.com/assets/corporates/documents/our_solution/implementing_your_project_2009_iso20022_usage_guide.pdf) (PDF)

If you need IBAN codes for testing, check out [MobileFish](http://www.mobilefish.com/services/random_iban_generator/random_iban_generator.php) or [GenerateIban](https://www.generateiban.com/test-iban/)

Sample
---

Sample for a quick single transaction :
```csharp
public class MySepaCreditTransfer
{
  private static SepaCreditTransfer GetCreditTransfertSample(decimal amount, string bic, string iban,
    string name, string comment)
  {
      var transfert = new SepaCreditTransfer
      {
          MessageIdentification = "uniqueCreditTransfertId",
          InitiatingPartyName = "Your name",
          // Below, your bank data
          Debtor = new SepaIbanData {Bic = "SOGEFRPPXXX", Iban = "FR7030002005500000157845Z02", Name = "My Corp"}          
      };

      transfert.AddCreditTransfer(new SepaCreditTransferTransaction
		{
			Creditor = new SepaIbanData {
			  Bic = bic,
			  Iban = iban,
			  Name = name
			},
			Amount = amount,
			RemittanceInformation = comment
		});
      return transfert;
  }

  public MySepaCreditTransfer()
  {
    var transfert = GetCreditTransfertSample(123.45m,  "AGRIFRPPXXX", "FR1420041010050500013M02606",
      "THEIR_NAME", "Payment sample");
    transfert.Save("sample.xml");
  }
}
```

Used libraries:
---
- NUnit 3.2.1 for unit tests
- Log4net 2.0.5 for log (used in XML validator)


License:
---

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

