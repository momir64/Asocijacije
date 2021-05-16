using Flurl;
using Flurl.Http;
using System.Web;
using System.Linq;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Network {
    public static class Gist {
        public class File {
            public string Id;
            public string Name;
            public string Content;
        }

        const string GistApi = "https://api.github.com/gists";
        const string token = "ghp_8Q9jOy" + "8YzVTVR2AQ" + "Picy1Dxf8g" + "UblD3fYwLO";
        static string EscapeJSON(string str) => HttpUtility.JavaScriptStringEncode(str);
        public static async Task<File> GetFileAsync(string name) => (await ListFilesAsync()).FirstOrDefault(file => file.Name == name) ?? new File() { Name = name };

        public static async Task<File[]> ListFilesAsync() {
            JArray result = JArray.Parse(await GistApi.WithHeaders(new { User_Agent = "X", Authorization = "token " + token }).GetStringAsync());
            List<File> files = new List<File>();
            for (int i = 0; i < result.Count; i++) {
                if (result[i]["files"].First == null)
                    continue;
                files.Add(new File {
                    Id = result[i]["id"].ToString(),
                    Name = (result[i]["files"].First as JProperty).Name
                });
            }
            return files.ToArray();
        }

        public static async Task<File> CreateFileAsync(string name, string content) {
            string json = "{\"files\":{\"" + name + "\":{\"content\":\"" + EscapeJSON(content) + "\"}}}";
            var response = await GistApi.WithHeaders(new { User_Agent = "X", Authorization = "token " + token }).PostAsync(new StringContent(json, Encoding.UTF8, "application/json"));
            return new File { Id = JObject.Parse(await response.GetStringAsync())["id"].ToString(), Name = name, Content = content };
        }

        public static async Task<File> ReadFileAsync(string name) => await ReadFileAsync(await GetFileAsync(name));
        public static async Task<File> ReadFileAsync(File file) {
            if (file.Id == null) return file;
            JObject result;
            try {
                result = JObject.Parse(await GistApi.AppendPathSegment("/" + file.Id).WithHeaders(new { User_Agent = "X", Authorization = "token " + token }).GetStringAsync());
            }
            catch (FlurlHttpException ex) when (ex.StatusCode == 404) {
                return null;
            }
            file.Content = result["files"]?.First.First["content"].ToString() ?? (await ReadFileAsync(file)).Content;
            return file;
        }

        public static async Task EditFileAsync(string name, string content) => await EditFileAsync(await GetFileAsync(name), content);
        public static async Task EditFileAsync(File file, string content) {
            if (file.Id == null) {
                await CreateFileAsync(file.Name, content);
                return;
            }
            string json = "{\"files\":{\"" + file.Name + "\":{\"content\":\"" + EscapeJSON(content) + "\"}}}";
            await GistApi.AppendPathSegment("/" + file.Id).WithHeaders(new { User_Agent = "X", Authorization = "token " + token }).PostAsync(new StringContent(json, Encoding.UTF8, "application/json"));
        }

        public static async Task DeleteFileAsync(string name) => await DeleteFileAsync(await GetFileAsync(name));
        public static async Task DeleteFileAsync(File file) {
            if (file.Id != null)
                await GistApi.AppendPathSegment("/" + file.Id).AllowAnyHttpStatus().WithHeaders(new { User_Agent = "X", Authorization = "token " + token }).DeleteAsync();
        }

        public static async Task DeleteAllFilesAsync() {
            File[] files = await ListFilesAsync();
            List<Task> tasks = new List<Task>();
            foreach (File file in files)
                tasks.Add(DeleteFileAsync(file));
            await Task.WhenAll(tasks);
        }
    }
}
