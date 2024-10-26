using System;
using System.IO;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq; 

namespace ProgAssign1
{
    public class DirWalker
    {
        private int validRows = 0;
        private int skippedRows = 0;
        private Stopwatch stopwatch;
        private StreamWriter logWriter;
        private List<Customer> validCustomers = new List<Customer>();

        public DirWalker()
        {
            string logFilePath = @"C:\Users\Mohammed Thoufiq\Desktop\MSCDA\MCDA5510_Software_dev\Dotnet_AS1\ProgAssign1\logs\directory_log.txt";
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)); 
            logWriter = new StreamWriter(logFilePath, false); // Overwrite the log file each run
        }

        public void Walk(string path)
        {
            stopwatch = Stopwatch.StartNew();
            Console.WriteLine($"Program execution has started please wait, this might take few minutes");

            TraverseDirectory(path);

            stopwatch.Stop();
            Log($"Directory walk completed. Total time: {stopwatch.Elapsed}");
            Console.WriteLine($"Directory walk completed. Total time: {stopwatch.Elapsed}");
            Log($"Total valid rows: {validRows}, Total skipped rows: {skippedRows}");
            Console.WriteLine($"Total valid rows: {validRows}, Total skipped rows: {skippedRows}");

            // Write valid customers to a new CSV file
            WriteValidCustomersToCsv(@"C:\Users\Mohammed Thoufiq\Desktop\MSCDA\MCDA5510_Software_dev\Dotnet_AS1\ProgAssign1\Output\valid_customers.csv");

            logWriter.Close();
        }

        private void TraverseDirectory(string path)
        {
            // Get year directories
            string[] yearDirectories = Directory.GetDirectories(path);
            

            if (yearDirectories.Length == 0)
            {
                Console.WriteLine("No year directories found.");
                return; // Early exit if no directories found
            }

            foreach (string yearDir in yearDirectories)
            {
                string year = Path.GetFileName(yearDir);
                Console.WriteLine($"Processing the files of year: {year}");
                // Get month directories
                string[] monthDirectories = Directory.GetDirectories(yearDir);

                foreach (string monthDir in monthDirectories)
                {
                    //Console.WriteLine($"Processing the files of year: {yearDir} Month {monthDir}");
                    string month = Path.GetFileName(monthDir);
                    Console.WriteLine($"Processing the files of year: {year},  month {month}");


                    // Get day directories
                    string[] dayDirectories = Directory.GetDirectories(monthDir);

                    foreach (string dayDir in dayDirectories)
                    {
                        string day = Path.GetFileName(dayDir); // Ensure day is two digits

                        // Construct the date
                        if (DateTime.TryParse($"{year}/{month}/{day}", out DateTime date))
                        {
                            // Process CSV files in this day directory
                            string[] files = Directory.GetFiles(dayDir, "*.csv");
                            foreach (string filepath in files)
                            {
                                ProcessCsv(filepath, date);
                                //Console.WriteLine("Processing file: " + filepath);
                                //Log($"Processing file: {filepath}");
                            }
                        }
                        else
                        {
                            //FConsole.WriteLine($"Failed to parse date from: {year}/{month}/{day}");
                        }
                    }
                }
            }
        }


        private void ProcessCsv(string filepath, DateTime date)
        {
            try
            {
                using (var reader = new StreamReader(filepath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<CustomerMap>();
                    var records = csv.GetRecords<Customer>().ToList(); // Convert to a list for easier handling

                    foreach (var record in records)
                    {
                        //Log($"Read record: {record.FirstName}, {record.LastName}, {record.EmailAddress}, {record.PhoneNumber}");
                        //Console.WriteLine($"Read record: {record.FirstName}, {record.LastName}, {record.EmailAddress}, {record.PhoneNumber}");

                        if (IsValidRecord(record))
                        {
                            validRows++;

                            // Set the date for the valid record
                            record.Date = date.ToString("yyyy/MM/dd"); // Store formatted date as string

                            validCustomers.Add(record); // Add valid customer to the list

                            // Log and console the record that has been added with formatted date
                            //Log($"Added valid record with date: {record.FirstName}, {record.LastName}, {record.EmailAddress}, {record.PhoneNumber}, Date: {record.Date}");
                            //Console.WriteLine($"Added valid record with date: {record.FirstName}, {record.LastName}, {record.EmailAddress}, {record.PhoneNumber}, Date: {record.Date}");
                        }
                        else
                        {
                            skippedRows++;
                            //Log($"Skipped incomplete record in file: {filepath}. Record: {record.FirstName} {record.LastName}");
                            //Console.WriteLine($"Skipped incomplete record in file: {filepath}. Record: {record.FirstName} {record.LastName}");

                        }
                    }
                }
            }
            catch (CsvHelperException csvEx)
            {
                Log($"CSV processing error for file: {filepath}. Exception: {csvEx.Message}");
                //Console.WriteLine($"CSV processing error for file: {filepath}. Exception: {csvEx.Message}");
            }
            catch (Exception ex)
            {
                Log($"Error processing file: {filepath}. Exception: {ex.Message}");
                //Console.WriteLine($"Error processing file: {filepath}. Exception: {ex.Message}");
            }
        }





        private bool IsValidRecord(Customer record)
        {
            bool isValid = !string.IsNullOrEmpty(record.FirstName) &&
                           !string.IsNullOrEmpty(record.LastName) &&
                           !string.IsNullOrEmpty(record.EmailAddress) &&
                           !string.IsNullOrEmpty(record.PhoneNumber) &&
                           !string.IsNullOrEmpty(record.PostalCode) &&
                           !string.IsNullOrEmpty(record.Country) &&
                           !string.IsNullOrEmpty(record.City) &&
                           !string.IsNullOrEmpty(record.Province) &&
                           !string.IsNullOrEmpty(record.StreetNumber); // Missing semicolon added

            if (!isValid)
            {
                //Log($"Invalid record: {record.FirstName}, {record.LastName}, {record.EmailAddress}, {record.PhoneNumber}");
            }

            return isValid;
        }

        private void WriteValidCustomersToCsv(string outputPath)
        {
            using (var writer = new StreamWriter(outputPath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<CustomerMapWrite>();
                csv.WriteRecords(validCustomers); // Write valid customers to the new CSV
            }
            Log($"Valid customers written to: {outputPath}");
        }

        private void Log(string message)
        {
            logWriter.WriteLine($"{DateTime.Now}: {message}");
        }

        public static void Main(string[] args)
        {
            string path = @"C:\Users\Mohammed Thoufiq\Desktop\MSCDA\MCDA5510_Software_dev\Dotnet_AS1\ProgAssign1\Sample Data"; // Replace with an absolute path if needed
            DirWalker walker = new DirWalker();
            walker.Walk(path);
        }
    }

    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StreetNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Date { get; set; } // The new date attribute
    }
    
    public sealed class CustomerMap : ClassMap<Customer>
    {
        public CustomerMap()
        {
            Map(m => m.FirstName).Name("First Name");
            Map(m => m.LastName).Name("Last Name");
            Map(m => m.StreetNumber).Name("Street Number");
            Map(m => m.Street).Name("Street");
            Map(m => m.City).Name("City");
            Map(m => m.Province).Name("Province");
            Map(m => m.Country).Name("Country");
            Map(m => m.PostalCode).Name("Postal Code");
            Map(m => m.PhoneNumber).Name("Phone Number");
            Map(m => m.EmailAddress).Name("email Address"); // Corrected case
            //Map(m => m.Date).Name("Date");
        }
    }
    public sealed class CustomerMapWrite : ClassMap<Customer>
    {
        public CustomerMapWrite()
        {
            Map(m => m.FirstName).Name("First Name");
            Map(m => m.LastName).Name("Last Name");
            Map(m => m.StreetNumber).Name("Street Number");
            Map(m => m.Street).Name("Street");
            Map(m => m.City).Name("City");
            Map(m => m.Province).Name("Province");
            Map(m => m.Country).Name("Country");
            Map(m => m.PostalCode).Name("Postal Code");
            Map(m => m.PhoneNumber).Name("Phone Number");
            Map(m => m.EmailAddress).Name("email Address"); // Corrected case
            Map(m => m.Date).Name("Date");
        }
    }
}
