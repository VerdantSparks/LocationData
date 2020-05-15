using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace LocationData
{
    public interface IDataCrawlingService
    {
        ICollection Crawl(string url);
    }

    public interface IDataTransformer
    {
        ICollection Transform(string input);
    }

    //pipeline: Crawl > Transform > Required Objects

    public class A
    {
        private string downloadedStr = string.Empty;
        private readonly WebClient webClient = new WebClient();

        public A()
        {
            webClient.DownloadStringCompleted += (sender, args) =>
            {
                if (args.Cancelled)
                    throw new Exception("user cancelled.");

                if (args.Error != null)
                    throw new Exception(args.Error.Message);

                downloadedStr = args.Result;
            };
        }

        //Command
        public async Task<string> DoSomething(string url) {

            var resultStr = await webClient.DownloadStringTaskAsync(url);

            return resultStr;
        }

        //Query

        public static async Task Main()
        {
            const string url = "https://ahis.baphiq.gov.tw/veter/veterQuery2.jsp";

            // TextReader tr = File.OpenText("/Users/ferrywlto/Desktop/result.txt");
            // var str = await tr.ReadToEndAsync();

            var ab = new AB();
            var result = ab.Go(await ab.Go2()).ToList();
            Console.WriteLine($@"Total clinics: {result.Count}");

            foreach (var clinic in result)
            {
                Console.WriteLine($"Name: {clinic.Name},\t Address: {clinic.Address},\t License: {clinic.LicenseNumber},\t Phone: {clinic.Phone},\t Doctor: {clinic.Doctor} ");
            }
        }
    }

    public class VetClinicGovData
    {
        public string Name { get; set; }
        public string LicenseNumber { get; set; }
        public string Address { get; set; }
        public string Phone{ get; set; }
        public string Doctor { get; set; }

    }

    public class AB
    {
        public static readonly IEnumerable<string> Cities = new[]
        {
            // disabled
            // "宜蘭市", "台東市", "桃園市", "苗栗市", "彰化市", "南投市", "屏東市", "花蓮市"
            "臺北市", "基隆市", "新北市", "宜蘭縣", "臺東縣", "新竹市", "新竹縣",
            "桃園市", "苗栗縣", "臺中市", "彰化縣", "南投縣", "嘉義市", "嘉義縣",
            "雲林縣", "臺南市", "高雄市", "澎湖縣", "金門縣", "屏東縣", "花蓮縣"
        };
        public static readonly IEnumerable<string> FieldNames = new[] {"獸醫院名稱", "開業執照字號", "負責獸醫師", "開業機構地址", "開業機構電話"};
        public static readonly string DelimiterPattern = @"共有\d*份資料符合條件&nbsp;診療機構?";

        protected readonly IEnumerable<string> _Cities;
        protected readonly IEnumerable<string> _FieldNames;
        protected readonly string _DelimiterPattern;

        public AB(IEnumerable<string> cities, IEnumerable<string> fieldNames, string delimiterPattern)
        {
            _Cities = cities;
            _FieldNames = fieldNames;
            _DelimiterPattern = delimiterPattern;
        }

        public AB()
        {
            _Cities = Cities;
            _FieldNames = FieldNames;
            _DelimiterPattern = DelimiterPattern;
        }

        /// <summary>
        /// For unit test hospital data purpose
        /// </summary>
        /// <param name="dataSourceUrl"></param>
        /// <returns></returns>
        public async Task<IEnumerable<KeyValuePair<string, byte[]>>> Go2()
        {
            var runningPath = Environment.CurrentDirectory;
            var bytes = new List<KeyValuePair<string,byte[]>>();
            foreach (var city in _Cities)
            {
                Console.WriteLine($"Downloading {city} data...");
                var testDataPath = Path.Combine(runningPath, $"Test Data/Hospitals/{city}.bin");
                Console.WriteLine(testDataPath);
                var testCityBytes = await File.ReadAllBytesAsync(testDataPath);
                bytes.Add(new KeyValuePair<string, byte[]>(city, testCityBytes));
            }

            return bytes;
        }

        public async Task<IEnumerable<KeyValuePair<string, byte[]>>> Go2(string dataSourceUrl)
        {
            var contentCreator = new GovVetDataQueryParamCreator();
            var downloader = new GovVetDataDownloader();

            var bytes = new List<KeyValuePair<string,byte[]>>();
            foreach (var city in _Cities)
            {
                Console.WriteLine($"Downloading {city} data...");

                bytes.Add(new KeyValuePair<string, byte[]>(city,
                                                           await downloader.DownloadBytesAsync(
                                                               dataSourceUrl,
                                                               contentCreator.Create(city))));
            }

            return bytes;
        }

        public IEnumerable<VetClinicGovData> Go(IEnumerable<KeyValuePair<string,byte[]>> pairs)
        {
            var resultList = new List<VetClinicGovData>();

            foreach (var (key, value) in pairs)
            {
                Console.WriteLine($"Working on {key}...");

                var subResultList =
                    X.Start(StringExtractor.GetStringFromBytes(value, 950), _DelimiterPattern, _FieldNames).ToList();

                Console.WriteLine($"{subResultList.Count} clinics extracted.");
                resultList.AddRange(subResultList);
            }

            return resultList;
        }
    }
    public static class X
    {
        public static IEnumerable<VetClinicGovData> Start(string input,
                                                          string delimiterPattern,
                                                          IEnumerable<string> fieldNames)
        {
            var iFieldNames = fieldNames as string[] ?? fieldNames.ToArray();

            return X.Split(X.Cleanse(input), delimiterPattern)
             .Select(p => X.Z(p, iFieldNames))
             .ToList();
        }
        public static IEnumerable<string> Split(string input, string pattern)
        {
            var regex = new Regex(pattern);
            return regex.Split(input).Where(s => !string.IsNullOrEmpty(s)).ToList();
        }
        /// <summary>
        /// Remove newline, tab, white spaces and html tags from downloaded text
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Cleanse(string input)
        {
            var beforeSize = input.Length;

            var result = new Regex(@"<[^>]*>")
                .Replace(new StringBuilder(input)
                         .Replace(" ", string.Empty)
                         .Replace("\n", string.Empty)
                         .Replace("\r", string.Empty)
                         .Replace("\t", string.Empty)
                         .ToString(), string.Empty);

            var afterSize = result.Length;

            Console.WriteLine($@"Text size before cleanse: {beforeSize}, after cleanse: {afterSize}, reduction rate:{(float)(beforeSize-afterSize)/beforeSize}");

            return result;
        }
        public static VetClinicGovData Z(string input, IEnumerable<string> fieldNames)
        {
            var regex3 = new Regex(X.ConstructRegexStr(fieldNames));
            var matcher = regex3.Match(input);

            if (!matcher.Success)
                throw new Exception("Invalid data entry. Fields not found.");

            return new VetClinicGovData()
            {
                Name = matcher.Groups[1].Value,
                LicenseNumber = matcher.Groups[2].Value,
                Doctor = matcher.Groups[3].Value,
                Address = matcher.Groups[4].Value,
                Phone = matcher.Groups[5].Value,
            };
        }

        public static string ConstructRegexStr(IEnumerable<string> fields)
        {
            var sb = new StringBuilder();

            foreach (var field in fields) { sb.Append(field + "(.*)"); }

            return sb.ToString();
        }
    }

        // Singleton
        public static class StringExtractor
        {
            static StringExtractor()
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            }

            public static string GetStringFromBytes(byte[] input, int encodingCode = 950)
            {
                return Encoding.GetEncoding(encodingCode).GetString(input);
            }
        }

        public class GovVetDataQueryParamCreator
        {
            //Query
            public static KeyValuePair<string, string> CreateUrlEncodedParam(string name, string value)
            {
                return new KeyValuePair<string, string>(name, HttpUtility.UrlEncode(value));
            }

            public FormUrlEncodedContent Create(string city, string mode = "1",  string hospital = "", string doctorName = "", string doctorNum = "", string localDocNum = "")
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    CreateUrlEncodedParam("city", city),
                    CreateUrlEncodedParam("mode",mode),
                    CreateUrlEncodedParam("h_name", hospital),
                    CreateUrlEncodedParam("dr_name", doctorName),
                    CreateUrlEncodedParam("doc_num", doctorNum),
                    CreateUrlEncodedParam("l_doc_num", localDocNum)
                });

                return content;
            }
        }
        public class GovVetDataDownloader
        {
            //Command
            public async Task<byte[]> DownloadBytesAsync(string url, FormUrlEncodedContent content)
            {
                using var httpClient = new HttpClient();
                var result = await httpClient.PostAsync(url, content);
                return await result.Content.ReadAsByteArrayAsync();
            }
        }
}
