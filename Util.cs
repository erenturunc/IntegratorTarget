using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace IntegratorTarget
{
    class Util
    {
        public static string SendHttpGetRequest(string Url)
        {
            var request = (HttpWebRequest)WebRequest.Create(Url);

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseString;
        }

        public static string SendHttpPostRequest(string Url, string Body)
        {
            var request = (HttpWebRequest)WebRequest.Create(Url);

            var data = Encoding.ASCII.GetBytes(Body);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseString;
        }

        internal static string ReadFromUri(string providerProductsXmlUri, string provider)
        {
            string folder = "tmp";
            string filePath = folder + "/" + provider + DateTime.Now.ToString("yyyyMMddHHmmss");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            WebClient client = new WebClient();
            client.DownloadFile(providerProductsXmlUri, filePath);

            StreamReader reader = new StreamReader(filePath);
            string DataXML = reader.ReadToEnd();
            reader.Close();

            return DataXML;

        }

        public static CurrencyRates ParseOpenExchangeRateCurrencies(string Path)
        {
            CurrencyRates rates = new CurrencyRates();

            StreamReader reader = new StreamReader(Path);
            string jsonData = reader.ReadToEnd();
            reader.Close();

            dynamic data = JsonConvert.DeserializeObject(jsonData);
            rates.USD = Convert.ToDouble(data.rates.USD);
            rates.TRY = Convert.ToDouble(data.rates.TRY);
            rates.IRR = Convert.ToDouble(data.rates.IRR);
            rates.AED = Convert.ToDouble(data.rates.AED);

            return rates;
        }

        public static Product DeepClone(Product obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (Product)formatter.Deserialize(ms);
            }
        }

        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        internal static DataTable ConvertProductDictionaryToTable(Dictionary<string, Product> Products)
        {
            DataTable dt = new DataTable();

            foreach (var field in typeof(Product).GetFields())
            {
                dt.Columns.Add(field.Name);
            }

            foreach (var item in Products)
            {
                DataRow dr = dt.NewRow();
                foreach (var attr in item.Value.GetType().GetFields())
                {
                    dr[attr.Name] = attr.GetValue(item.Value);
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }

        internal static void WriteOutputFile(string member, string provider, string target, string fileFormat, string content)
        {
            string folder = string.Format("Output/{0}/{1}/{2}", member, provider, target);
            string fileName = string.Format("{0}/{1}_{2}_{3}_{4}{5}", folder, member, provider, target, DateTime.Now.ToString("yyyyMMddHHmmss"), fileFormat);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            StreamWriter writer = new StreamWriter(fileName, false, Encoding.GetEncoding("iso-8859-9"));
            writer.Write(content);
            writer.Close();
        }


        public static string GetHashSha256(string Message, string ApiKey)
        {
            var shaKeyBytes = System.Text.Encoding.UTF8.GetBytes(ApiKey);
            string signatureHashHex;
            using (var shaAlgorithm = new System.Security.Cryptography.HMACSHA256(shaKeyBytes))
            {
                var signatureBytes = System.Text.Encoding.UTF8.GetBytes(Message);
                var signatureHashBytes = shaAlgorithm.ComputeHash(signatureBytes);
                signatureHashHex = string.Concat(Array.ConvertAll(signatureHashBytes, b => b.ToString("X2"))).ToLower();

            }

            return signatureHashHex;
        }
    }
}
