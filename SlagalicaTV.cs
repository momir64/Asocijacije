using System;
using Flurl.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Asocijacije {
    class SlagalicaTV {
        readonly CookieJar cookies = new CookieJar();

        public async Task<SlagalicaTV> InitializeAsync() {
            await "http://www.slagalica.tv/korisnik/prijava_submit/".WithCookies(cookies).PostUrlEncodedAsync(new { openid = "ovojeime", lozinka = "ovojesifra" });
            return this;
        }

        public async Task<string> FindData(string date, params string[] data) {
            string response = await ("http://www.slagalica.tv/play/asocijacije/" + date).WithCookies(cookies).GetStringAsync();
            string cypher = GetStringBetween(response, "ajax_rpc_loader('", "'));");
            string key = "F81A23D45E67B09C";
            string result = "";
            for (int i = 0; i < cypher.Length; i += 2)
                result += (char)((key.IndexOf(cypher[i]) * 16) + key.IndexOf(cypher[i + 1]));
            result = GetStringBetween(Regex.Unescape(result), "{", "}");
            if (data.Any(result.Contains))
                return result;
            return "";
        }

        public static string[][][] ParseData(string result) {
            string[][][] asocijacije = new string[5][][];
            for (int i = 0; i < 4; i++) {
                asocijacije[i] = new string[5][];
                for (int j = 0; j < 4; j++)
                    asocijacije[i][j] = ParseString(GetStringBetween(result, "\"" + (char)('a' + i) + (j + 1) + "\":", ","));
                asocijacije[i][4] = ParseString(GetStringBetween(result, "\"" + (char)('a' + i) + "5\":[", "]"));
            }
            asocijacije[4] = new string[][] { ParseString(GetStringBetween(result, "\"rr\":[", "]")) };
            return asocijacije;
        }

        private static string[] ParseString(string result) {
            string[] niz = new string[result.Count(c => c == ',') + 1];
            for (int i = 0; i < niz.Length; i++) {
                int start = result.IndexOf('"') + 1;
                int end = result.Substring(start).IndexOf('"');
                niz[i] = Lat2Cir(result.Substring(start, end));
                result = result.Substring(start + end + 1);
            }
            return niz;
        }

        static readonly Random random = new Random();
        public static string GetRandomDate() {
            DateTime start = new DateTime(2020, 1, 1);
            int range = (DateTime.Today - start).Days - 1;
            return start.AddDays(random.Next(range)).ToString("yyyy-MM-dd");
        }

        public static async Task<string> GetData(string date) {
            CookieJar cookies = new CookieJar();
            await "http://www.slagalica.tv/korisnik/prijava_submit/".WithCookies(cookies).PostUrlEncodedAsync(new { openid = "ovojeime", lozinka = "ovojesifra" });
            string response = await ("http://www.slagalica.tv/play/asocijacije/" + date).WithCookies(cookies).GetStringAsync();
            string cypher = GetStringBetween(response, "ajax_rpc_loader('", "'));");
            string key = "F81A23D45E67B09C";
            string result = "";
            for (int i = 0; i < cypher.Length; i += 2)
                result += (char)((key.IndexOf(cypher[i]) * 16) + key.IndexOf(cypher[i + 1]));
            return GetStringBetween(Regex.Unescape(result), "{", "}");
        }

        private static string GetStringBetween(string str, string a, string b) {
            int start = str.IndexOf(a) + a.Length;
            return str.Substring(start, str.Substring(start).IndexOf(b));
        }

        private readonly static string[] special = { "Q", "W", "X", "Y" };
        private readonly static string[] lat = { "DŽ", "NJ", "LJ", "A", "B", "V", "G", "D", "Đ", "E", "Ž", "Z", "I", "J", "K", "L", "M", "N", "O", "P", "R", "S", "T", "Ć", "U", "F", "H", "C", "Č", "Š" };
        private readonly static string[] cir = { "Џ", "Њ", "Љ", "А", "Б", "В", "Г", "Д", "Ђ", "Е", "Ж", "З", "И", "Ј", "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т", "Ћ", "У", "Ф", "Х", "Ц", "Ч", "Ш" };
        private static string Lat2Cir(string text) {
            text = text.ToUpper();
            if (!special.Any(text.Contains))
                for (int i = 0; i < lat.Length; i++)
                    text = text.Replace(lat[i], cir[i]);
            return text;
        }
    }
}
