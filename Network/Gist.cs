using System.Web;
using System.Linq;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Network {
    public static class Gist {
        public class File {
            public string Id;
            public string Name;
            public string Content;
        }

        const string token = "ghp_8Q9jOy" + "8YzVTVR2AQ" + "Picy1Dxf8g" + "UblD3fYwLO";
        static string EscapeJSON(string str) => HttpUtility.JavaScriptStringEncode(str);
        public static File GetFile(string name) => ListFiles().FirstOrDefault(file => file.Name == name) ?? new File() { Name = name };

        public static File[] ListFiles() {
            using (var httpClient = new HttpClient()) {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://api.github.com/gists")) {
                    request.Headers.Add("User-Agent", "gist-dispecer");
                    request.Headers.Add("Authorization", "token " + token);
                    HttpResponseMessage response = httpClient.SendAsync(request).Result;
                    JArray result = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                    File[] files = new File[result.Count];
                    for (int i = 0; i < files.Length; i++) {
                        files[i] = new File {
                            Id = result[i]["id"].ToString(),
                            Name = (result[i]["files"].First as JProperty).Name
                        };
                    }
                    return files;
                }
            }
        }

        public static File CreateFile(string name, string content) {
            using (var httpClient = new HttpClient()) {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.github.com/gists")) {
                    request.Headers.Add("User-Agent", "gist-dispecer");
                    request.Headers.Add("Authorization", "token " + token);
                    string json = "{\"files\":{\"" + name + "\":{\"content\":\"" + EscapeJSON(content) + "\"}}}";
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = httpClient.SendAsync(request).Result;
                    string result = response.Content.ReadAsStringAsync().Result;
                    return new File { Id = JObject.Parse(result)["id"].ToString(), Name = name, Content = content };
                }
            }
        }

        public static File ReadFile(string name) => ReadFile(GetFile(name));
        public static File ReadFile(File file) {
            if (file.Id == null) return file;
            using (var httpClient = new HttpClient()) {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://api.github.com/gists/" + file.Id)) {
                    request.Headers.Add("User-Agent", "gist-dispecer");
                    request.Headers.Add("Authorization", "token " + token);
                    HttpResponseMessage response = httpClient.SendAsync(request).Result;
                    JObject result = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    file.Content = result["files"].First.First["content"].ToString();
                    return file;
                }
            }
        }

        public static void EditFile(string name, string content) => EditFile(GetFile(name), content);
        public static void EditFile(File file, string content) {
            if (file.Id == null) {
                CreateFile(file.Name, content);
                return;
            }
            using (var httpClient = new HttpClient()) {
                using (var request = new HttpRequestMessage(new HttpMethod("PATCH"), "https://api.github.com/gists/" + file.Id)) {
                    request.Headers.Add("User-Agent", "gist-dispecer");
                    request.Headers.Add("Authorization", "token " + token);
                    string json = "{\"files\":{\"" + file.Name + "\":{\"content\":\"" + EscapeJSON(content) + "\"}}}";
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    httpClient.SendAsync(request).Wait();
                }
            }
        }

        public static void DeleteFile(string name) => DeleteFile(GetFile(name));
        public static void DeleteFile(File file) {
            if (file.Id == null) return;
            using (var httpClient = new HttpClient()) {
                using (var request = new HttpRequestMessage(new HttpMethod("DELETE"), "https://api.github.com/gists/" + file.Id)) {
                    request.Headers.Add("User-Agent", "gist-dispecer");
                    request.Headers.Add("Authorization", "token " + token);
                    httpClient.SendAsync(request).Wait();
                }
            }
        }

        public static void DeleteAllFiles() {
            foreach (File file in ListFiles())
                DeleteFile(file);
        }
    }
}
