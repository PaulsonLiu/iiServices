using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;

namespace iiFramework.Util
{
    public class CSVHelper<T> where T : class
    {
        public static string GeneralCSV(List<T> dataList, string csvFileName, string localPath)
        {
            if (dataList == null || dataList.Count == 0)
            {
                Console.WriteLine("no report date");
                return "";
            }
            try
            {
                string localFileName = string.IsNullOrWhiteSpace(csvFileName) ? $"{DateTime.Now.ToString("yyyyMMddhhssmmm")}.csv" : csvFileName;

                if (!Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }
                string csvContent = "";
                using (FileStream fs = new FileStream(Path.Combine(localPath, localFileName), FileMode.Create, FileAccess.ReadWrite))
                using (StreamWriter strW = new StreamWriter(fs, Encoding.UTF8))
                {

                    csvContent = GenerateCsvContent(dataList);

                    if (csvContent == null) return "";
                    strW.Write(csvContent);
                }
                return csvContent;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GeneralCSV",
                                $"csvFileName:{csvFileName}, Path:{localPath}",
                                ex.Message);
                return "";
            }
        }

        static string GenerateCsvContent(List<T> itemList, bool hasHeaderRecord = true)
        {
            try
            {
                using (StringWriter sWriter = new StringWriter())
                {
                    Configuration  config = new Configuration();
                    config.HasHeaderRecord = hasHeaderRecord;
                    //char soh = '\u0001';
                    //config.Delimiter = soh.ToString();
                    using (var csv = new CsvWriter(sWriter, config))
                    {
                        csv.WriteRecords(itemList);

                        return sWriter.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                var itemContent = JsonConvert.SerializeObject(itemList);
                Console.Write("HDFSApiHelper.GenerateCsvFile"
                                  , $"itemList:{itemContent}"
                                  , ex.Message);
                return null;
            }
        }

        public static List<T> GetEntityFromCSV<M>(Stream csvMemory, string delimiter = ",") where M : ClassMap<T>
        {
            var csvReader = new StreamReader(csvMemory);
            var csvConfig = new Configuration();
            csvConfig.HasHeaderRecord = true;
            csvConfig.RegisterClassMap<M>();
            CsvReader csv = new CsvReader(csvReader, csvConfig);
            return csv.GetRecords<T>().ToList();
        }

        public static List<T> GetEntityFromCSV(Stream csvMemory, string delimiter = ",")
        {
            var csvReader = new StreamReader(csvMemory);
            var temp = csvReader.ReadToEnd();
            //var csvConfig = new Configuration()
            //{
            //    HasHeaderRecord = true,
            //    IgnoreHeaderWhiteSpace = true,
            //    IsHeaderCaseSensitive = false,
            //    Delimiter = delimiter
            //};
            var csvConfig = new Configuration()
            {
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                Delimiter = delimiter
            };

            var csv = new CsvReader(csvReader);
            try
            {
                return csv.GetRecords<T>().ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
    }
}
