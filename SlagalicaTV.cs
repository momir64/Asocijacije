using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Asocijacije {
    static class SlagalicaTV {
        static CookieContainer cookies = new CookieContainer();
        static HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookies };
        static HttpClient client = new HttpClient(handler);

        public static string[][][] ParseData(string result) {
            string[][][] asocijacije = new string[5][][];
            for (int i = 0; i < 4; i++) {
                asocijacije[i] = new string[5][];
                for (int j = 0; j < 4; j++)
                    asocijacije[i][j] = ParseString(getStringBetween(result, "\"" + (char)('a' + i) + (j + 1) + "\":", ","));
                asocijacije[i][4] = ParseString(getStringBetween(result, "\"" + (char)('a' + i) + "5\":[", "]"));
            }
            asocijacije[4] = new string[][] { ParseString(getStringBetween(result, "\"rr\":[", "]")) };
            return asocijacije;
        }

        static string[] ParseString(string result) {
            string[] niz = new string[result.Count(c => c == ',') + 1];
            for (int i = 0; i < niz.Length; i++) {
                int start = result.IndexOf('"') + 1;
                int end = result.Substring(start).IndexOf('"');
                niz[i] = result.Substring(start, end);
                result = result.Substring(start + end + 1);
            }
            return niz;
        }

        public static string getRandomDate() {
            Random gen = new Random();
            DateTime start = new DateTime(2020, 1, 1);
            int range = (DateTime.Today - start).Days - 1;
            return start.AddDays(gen.Next(range)).ToString("yyyy-MM-dd");
        }

        static async Task LogIn() {
            Uri uri = new Uri("http://www.slagalica.tv/korisnik/prijava_submit/");
            var values = new Dictionary<string, string> { { "openid", "ovojeime" }, { "lozinka", "ovojesifra" } };
            await client.PostAsync(uri, new FormUrlEncodedContent(values));
        }

        public static async Task<string> GetData(string date) {
            await LogIn();
            Uri uri = new Uri("http://www.slagalica.tv/play/asocijacije/" + date);
            string response = await client.GetAsync(uri).Result.Content.ReadAsStringAsync();
            string cypher = getStringBetween(response, "ajax_rpc_loader('", "'));");
            string key = "F81A23D45E67B09C";
            string result = "";
            for (int i = 0; i < cypher.Length; i += 2)
                result += (char)((key.IndexOf(cypher[i]) * 16) + key.IndexOf(cypher[i + 1]));
            return getStringBetween(Regex.Unescape(result), "{", "}");
        }

        static string getStringBetween(string str, string a, string b) {
            int start = str.IndexOf(a) + a.Length;
            return str.Substring(start, str.Substring(start).IndexOf(b));
        }
    }
}
