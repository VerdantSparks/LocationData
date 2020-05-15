using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace LocationData
{
    //pipeline: Crawl > Transform > Required Objects

    public class GovVetHospital
    {
        public string Name { get; set; }
        public string LicenseNumber { get; set; }
        public string Address { get; set; }
        public string Phone{ get; set; }
        public string Doctor { get; set; }
    }

    public class GovVetPerson
    {
        public string Name { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseCity { get; set; }
        public string HospitalName { get; set; }
        public string HospitalAddress { get; set; }
        // Below are optional fields?
        public DateTime EduPeriodStart { get; set; }
        public DateTime EduPeriodEnd { get; set; }
        public string IsEduStillValid { get; set; }
    }

    public class GovVetDataService
    {
        protected static readonly IEnumerable<string> Cities = new[]
        {
            // disabled
            // "宜蘭市", "台東市", "桃園市", "苗栗市", "彰化市", "南投市", "屏東市", "花蓮市"
            "臺北市", "基隆市", "新北市", "宜蘭縣", "臺東縣", "新竹市", "新竹縣",
            "桃園市", "苗栗縣", "臺中市", "彰化縣", "南投縣", "嘉義市", "嘉義縣",
            "雲林縣", "臺南市", "高雄市", "澎湖縣", "金門縣", "屏東縣", "花蓮縣",
        };

        protected readonly Dictionary<string, (string, string[])> Settings =
            new Dictionary<string, (string, string[])>()
            {
                {"hospital", (@"共有\d*份資料符合條件&nbsp;診療機構?", new[] {"獸醫院名稱", "開業執照字號", "負責獸醫師", "開業機構地址", "開業機構電話"})},
                {"doctor", (@"共有\d*份資料符合條件&nbsp;獸醫師?", new[] {"獸醫師姓名", "執業執照字號", "執業縣市", "執業機構名稱", "執業機構地址", "繼續教育區間", "是否符合繼續教育"})},
                {"assistant", (@"共有\d*份資料符合條件&nbsp;獸醫佐.*第\d*筆?", new[] {"獸醫佐姓名", "執業執照字號", "執業縣市", "執業機構名稱", "執業機構地址", "繼續教育區間", "是否符合繼續教育"})},
            };

        protected readonly GovVetDataDownloader Downloader = new GovVetDataDownloader();
        protected readonly GovVetDataParser Parser = new GovVetDataParser();

        private const string Url = "https://ahis.baphiq.gov.tw/veter/veterQuery{0}.jsp";
        public async Task<IEnumerable<GovVetHospital>> GetHospitals()
        {
            // return await Get("hospital", string.Format(Url, 2));
            return Parser.MapHospital(await Get("hospital","./Test Data/Hospitals"));
        }
        public async Task<IEnumerable<GovVetPerson>> GetDoctors()
        {
            // return await Get("doctor", string.Format(Url, 2), "2");
            return Parser.MapPerson(await Get("doctor","./Test Data/Doctors"));
        }
        public async Task<IEnumerable<GovVetPerson>> GetAssistants()
        {
            // return await Get("assistant", string.Format(Url, 3));
            return Parser.MapPerson(await Get("assistant","./Test Data/Assistants"));
        }

        public async Task<IEnumerable<Match>> Get(string settingName, string source, string mode = "1")
        {
            // var bytes = await Downloader.LoadDataSource(source, Cities, mode);
            var (delimiterPattern, fieldNames) = Settings[settingName];
            var bytes = await Downloader.LoadDataSource(source, Cities);
            // var bytes = await Downloader.LoadDataSource(source, Cities, mode);

            return Parser.Parse(bytes, delimiterPattern, fieldNames).ToList();;
        }

                public static async Task Main()
        {
            var ds = new GovVetDataService();
            var hospitals = (await ds.GetHospitals()).ToList();
            var doctors = (await ds.GetDoctors()).ToList();
            var assistants = (await ds.GetAssistants()).ToList();
            Console.WriteLine($@"Total clinics: {hospitals.Count}");
            Console.WriteLine($@"Total doctors: {doctors.Count}");
            Console.WriteLine($@"Total assistants: {assistants.Count}");

            foreach (var clinic in hospitals)
            {
                Console.WriteLine($"Name:\t{clinic.Name},\n" +
                                  $"Address:\t{clinic.Address},\n" +
                                  $"License:\t{clinic.LicenseNumber},\n" +
                                  $"Phone:\t{clinic.Phone},\n" +
                                  $"Doctor:\t{clinic.Doctor}");
            }
            foreach (var doctor in doctors)
            {
                Console.WriteLine($"Name:\t{doctor.Name},\n" +
                                  $"LicenseNumber:\t{doctor.LicenseNumber},\n" +
                                  $"LicenseCity:\t{doctor.LicenseCity},\n" +
                                  $"HospitalName:\t{doctor.HospitalName},\n" +
                                  $"HospitalAddress:\t{doctor.HospitalAddress},\n" +
                                  $"PeriodStart:\t{doctor.EduPeriodStart},\n" +
                                  $"PeriodEnd:\t{doctor.EduPeriodEnd},\n" +
                                  $"StillValid:\t{doctor.IsEduStillValid}");
            }
            foreach (var assistant in assistants)
            {
                Console.WriteLine($"Name:\t{assistant.Name},\n" +
                                  $"LicenseNumber:\t{assistant.LicenseNumber},\n" +
                                  $"LicenseCity:\t{assistant.LicenseCity},\n" +
                                  $"HospitalName:\t{assistant.HospitalName},\n" +
                                  $"HospitalAddress:\t{assistant.HospitalAddress},\n" +
                                  $"PeriodStart:\t{assistant.EduPeriodStart},\n" +
                                  $"PeriodEnd:\t{assistant.EduPeriodEnd},\n" +
                                  $"StillValid:\t{assistant.IsEduStillValid}");
            }
        }
    }
    public class GovVetDataParser
    {
        static GovVetDataParser()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public IEnumerable<Match> Parse(IEnumerable<KeyValuePair<string,byte[]>> pairs, string delimiterPattern, IEnumerable<string> fieldNames)
        {
            var resultList = new List<Match>();
            var enumerable = fieldNames.ToList();
            foreach (var (key, value) in pairs)
            {
                Console.WriteLine($"Working on {key}...");

                var subResultList =
                    ParseStructuredTextToVetDataList(GetStringFromBytes(value, 950), delimiterPattern, enumerable).ToList();

                Console.WriteLine($"{subResultList.Count} items extracted.");
                resultList.AddRange(subResultList);
            }

            return resultList;
        }

        public IEnumerable<GovVetHospital> MapHospital(IEnumerable<Match> matches)
        {
            return matches.Select(ParseHospital).ToList();
        }

        public IEnumerable<GovVetPerson> MapPerson(IEnumerable<Match> matches)
        {
            return matches.Select(ParsePerson).ToList();
        }

        public static IEnumerable<Match> ParseStructuredTextToVetDataList(string input,
                                                                      string delimiterPattern,
                                                                      IEnumerable<string> fieldNames)
        {
            return Split(Cleanse(input), delimiterPattern)
                   .Select(p => MatchFields(p, fieldNames))
                   .ToList();
        }

        public static Match MatchFields(string input, IEnumerable<string> fieldNames)
        {
            var matcher =
                new Regex(fieldNames.Aggregate(new StringBuilder(),
                                               (builder, s) => builder.Append(s + "(.*)"),
                                               s => s.ToString())).Match(input);

            if (!matcher.Success)
                throw new Exception("Invalid data entry. Fields not found.");

            return matcher;
        }

        public static GovVetHospital ParseHospital(Match match)
        {
            return new GovVetHospital()
            {
                Name = match.Groups[1].Value,
                LicenseNumber = match.Groups[2].Value,
                Doctor = match.Groups[3].Value,
                Address = match.Groups[4].Value,
                Phone = match.Groups[5].Value,
            };
        }

        public static GovVetPerson ParsePerson(Match match)
        {
            // if(match.Groups.Count == 0)
            //     throw new Exception("wtf");
            //
            // Console.WriteLine(@"{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}",
            //                   match.Groups.Count,
            //                   match.Groups[1].Value,
            //                   match.Groups[2].Value,
            //                   match.Groups[3].Value,
            //                   match.Groups[4].Value,
            //                   match.Groups[5].Value,
            //                   match.Groups[6].Value,
            //                   match.Groups[7].Value);

            var periodValue = match.Groups[6].Value;
            DateTime periodStart;
            DateTime periodEnd;

            if (string.IsNullOrEmpty(periodValue))
            {
                periodStart = DateTime.UnixEpoch;
                periodEnd = DateTime.UnixEpoch;
            }
            else
            {
                var period = periodValue.Split("—");
                periodStart = period[0].Equals("null") ?
                    DateTime.UnixEpoch :
                    DateTime.ParseExact(period[0], "yyyy/MM/dd", CultureInfo.InvariantCulture);
                periodEnd = period[1].Equals("null") ?
                    DateTime.UnixEpoch :
                    DateTime.ParseExact(period[0], "yyyy/MM/dd", CultureInfo.InvariantCulture);
            }

            return new GovVetPerson()
            {
                Name = match.Groups[1].Value,
                LicenseNumber = match.Groups[2].Value,
                LicenseCity = match.Groups[3].Value,
                HospitalName = match.Groups[4].Value,
                HospitalAddress = match.Groups[5].Value,
                EduPeriodStart = periodStart,
                EduPeriodEnd = periodEnd,
                IsEduStillValid = match.Groups[7].Value,
            };
        }

        protected static string GetStringFromBytes(byte[] input, int encodingCode = 950)
        {
            return Encoding.GetEncoding(encodingCode).GetString(input);
        }
        /// <summary>
        /// Remove the sentence appear before each record, remove empty entry.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static IEnumerable<string> Split(string input, string pattern)
        {
            return new Regex(pattern).Split(input).Where(s => !string.IsNullOrEmpty(s)).ToList();
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
    }

    /// <summary>
    /// This class is for getting Veterinary doctor, assistant and hospital data
    /// from AHIS (Animal Health Information System) of
    /// BAPHIQ (Bureau of Animal and Plant Health Inspection and Quarantine) under
    /// Taiwan Council of Agriculture Executive Yuan
    /// </summary>
    public class GovVetDataDownloader
    {
        //Query
        public static KeyValuePair<string, string> CreateUrlEncodedParam(string name, string value)
        {
            return new KeyValuePair<string, string>(name, HttpUtility.UrlEncode(value));
        }

        public static FormUrlEncodedContent CreateQueryParam(string city, string mode = "1",  string hospital = "", string doctorName = "", string hospitalLicenseNumber = "", string doctorLicenseNumber = "")
        {
            var content = new FormUrlEncodedContent(new[]
            {
                CreateUrlEncodedParam("city", city),
                CreateUrlEncodedParam("mode",mode),
                CreateUrlEncodedParam("h_name", hospital),
                CreateUrlEncodedParam("dr_name", doctorName),
                //they really use doc_num as hospital license number (Business Registration Number)
                CreateUrlEncodedParam("doc_num", hospitalLicenseNumber),
                CreateUrlEncodedParam("l_doc_num", doctorLicenseNumber)
            });

            return content;
        }

        //Command
        public static async Task<byte[]> DownloadBytesAsync(string url, FormUrlEncodedContent content)
        {
            using var httpClient = new HttpClient();
            var result = await httpClient.PostAsync(url, content);
            return await result.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// For unit test
        /// </summary>
        /// <param name="testDataPath"></param>
        /// <param name="cities"></param>
        /// <returns></returns>
        public async Task<IEnumerable<KeyValuePair<string, byte[]>>> LoadDataSource(string testDataPath, IEnumerable<string> cities)
        {
            var runningPath = Environment.CurrentDirectory;
            var bytes = new List<KeyValuePair<string,byte[]>>();

            foreach (var city in cities)
            {
                Console.WriteLine($"Downloading {city} data...");
                var path = Path.Combine(runningPath, $"{testDataPath}/{city}.bin");
                Console.WriteLine(testDataPath);
                var testCityBytes = await File.ReadAllBytesAsync(path);
                bytes.Add(new KeyValuePair<string, byte[]>(city, testCityBytes));
            }

            return bytes;
        }

        public async Task<IEnumerable<KeyValuePair<string, byte[]>>> LoadDataSource(string dataSourceUrl, IEnumerable<string> cities, string mode)
        {
            var bytes = new List<KeyValuePair<string,byte[]>>();

            foreach (var city in cities)
            {
                Console.WriteLine($"Downloading {city} data...");
                var b = await DownloadBytesAsync(dataSourceUrl, CreateQueryParam(city, mode));
                // File.WriteAllBytes($"./{city}.bin", b);
                bytes.Add(new KeyValuePair<string, byte[]>(city, b));
            }

            return bytes;
        }
    }
}
