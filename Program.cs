using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using Newtonsoft.Json;

namespace ValidateInput
{
    class Program
    {
        //Most input is generted from users. Two categories of users:
        //1) Innocent users: Users trying to use app, but they can still make mistakes
        //2) Users that seek weaknesses in app and exploit them.
        //You can avoid these problems by managing data integrity. There ae 4 types of data integrity:
        //1) Entity Integrity: States that each record must be uniquey identible(Primary key columns like ID's etc.)
        //2) Domain integrity: Validity of the data that entity contains(Type of data and allowed values(Postal code, cellphone numbers)).
        //3) Referential Integrity: Relationships that each entities have with eachother.
        //4) User-defined Integrity: Comprises specific business rules that you need to enforce(Customer not allowed to place order above certian amount).
        static void Main(string[] args)
        {
            //Parse
            Console.WriteLine("Using Parse:");
            UsingParse();

            //TryParse
            Console.WriteLine("Using TryParse:");
            TryParse();

            //Globalization
            Console.WriteLine("Using Globalization configurations:");
            UsingGlobalizationConfigurations();

            //RegularExpressions
            Console.WriteLine("Using RegularExpressions:");
            Console.WriteLine("Validating ZipCode(Dutch): 1234AB");
            var result = RegularExpressionsDuchZipCode("1234AB");
            switch(result){
                case true: {
                    Console.WriteLine("ZipCode is valid!");
                    break;
                }
                case false: {
                    Console.WriteLine("ZipCode is not valid!");
                    break;
                }
            }

            RegularExpressionsCollapseWhiteSpaces();

            //Validating JSON and XML
            Console.WriteLine("Validating JSON: Line 116");
            ValidateJSON("Person.json");
            Console.WriteLine("Validating XML: Line 131");
            ValidateXML();
        }
        
        //Parse and TryParse methods can be used when you have a string that you want to convert to a specific data type
        static void UsingParse(){
            string value = "true";
            //Parse into bool
            bool b = bool.Parse(value);
            Console.WriteLine("{0} Parsed to bool. Result {1}",value, b);
        }
        
        //You use TryParse if you are not sure that the parsing will succeed. You don’t want an exception to be thrown and you want to handle invalid conversion gracefully
        static void TryParse(){
            string value = "1";
            int result;
            bool success = int.TryParse(value, out result);
            if (success)
            { 
                Console.WriteLine("TryParse was successful. {0} parsed into int", value);
            }
            else
            { 
                Console.WriteLine("TryParse was not successful. {0} was not parsed into int", value);
            }
        }

        //When parsing numbers, you can supply extra options for the style of the number and the specific culture that you want to use
        static void UsingGlobalizationConfigurations(){
            var amount = 123.45M;
            //Pounds
            var Pound = CultureInfo.GetCultureInfo("en-GB");
            var PoundAmount = String.Format(Pound, "{0:C}", amount);
            Console.WriteLine("Pounds: {0}", PoundAmount);
            //Dollars
            var Dollar = CultureInfo.GetCultureInfo("en-US");
            var DollarAmount = String.Format(Dollar, "{0:C}", amount);
            Console.WriteLine("Dollars: {0}", DollarAmount);
        }

        //A regular expression is a specific pattern used to parse and find matches in strings. 
        //A regular expression is sometimes called regex or regexp
        static bool RegularExpressionsDuchZipCode(string zipCode){
            // Valid Dutch zipcode examples: 1234AB | 1234 AB | 1001 AB
            Match match = Regex.Match(zipCode, @"^[1-9][0-9]{3}\s?[a-zA-Z]{2}$",
            RegexOptions.IgnoreCase);
            return match.Success;
        }   

        static void RegularExpressionsCollapseWhiteSpaces(){
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex(@"[ ]{2,}", options);

            string input = "1 2 3 4   5";
            Console.WriteLine("Input: {0}", input);
            string result = regex.Replace(input, " ");

            Console.WriteLine("Output: {0}", result); // Displays 1 2 3 4 5
        }

        //Validating XML and JSON Data
        //When exchanging data with other applications, you will often receive JavaScript Object Notation (JSON) orExtensible Markup Language (XML) data
        //Valid JSON starts with { or [, and ends with } or ]
        //You use JavaScriptSerializer or JsonConvert to validate JSON Data
        static void ValidateJSON(string jsonPath){
            using (StreamReader r = new StreamReader(jsonPath)){
                string json = r.ReadToEnd();
                object o = JsonConvert.DeserializeObject(json);
                Console.WriteLine("Result: {0}",o.ToString());
            }
        }

        //An XML file can be described by using an XML Schema Definition (XSD). This XSD can be used to validate an XML file.
        //Once the XML file has been converted into a XSD file, the XML can be validated
        //You can create an XSD file for this schema by using the XML Schema Definition Tool (Xsd.exe)
        //Following Line will generate XSD file(In Visual Studio): Xsd.exe Person.xml
        static void ValidateXML()
        {
            string xsdPath = "Person.xsd";
            string xmlPath = "Person.xml";
            
            XmlReader reader = XmlReader.Create(xmlPath);
            XmlDocument document = new XmlDocument();
            document.Schemas.Add("", xsdPath);
            document.Load(reader);

            ValidationEventHandler eventHandler = 
                new ValidationEventHandler(ValidationEventHandler);
                try {
                    document.Validate(eventHandler);
                    Console.WriteLine("XML is valid!");
                }
                catch {
                    Console.WriteLine("XML is not valid!");
                }
        }

        //If there is something wrong with the XML file, such as a non-existing element, the ValidationEventHandler is called. Depending on the type of validation error, you can decide which action to take.
        static void ValidationEventHandler(object sender,
            ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    Console.WriteLine("Error: {0}", e.Message);
                    break;
                case XmlSeverityType.Warning:
                    Console.WriteLine("Warning {0}", e.Message);
                    break;
            }
        }

    }
}
