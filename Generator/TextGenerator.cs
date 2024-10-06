using static System.Net.Http.HttpClient;
using Model;
using System.Text.Json;
using System.Text;

namespace Generator
{
    public static class TextGenerator
    {
        private const string locationSystemPromt =
            "Your task is to provide a very brief description of photos and landscapes. " +
            "All your descriptions must match the theme which is fantasy and adventure, it takes place in a middle aged fantasy world. " +
            "You must be concise and to the point. Do not give reasoning. You never mention yourself and you never mention me, you only describe the photo.";
        private const string oldLocationSystemPromt =
            "You are an expert photographer and art critic. " +
            "Your task is to provide a very brief description of photos and landscapes for a generative AI diffusion model. " +
            "Your specialty is fantasy worlds and nature photography. " +
            "You never mention yourself and you never mention me, you only describe the photo.";


        private const string portraitSystemPromt =
            "Your assignment is to describe characters photos from a fantasy movie. " +
            "All your descriptions must match the theme which is fantasy and adventure, it takes place in a middle aged fantasy world. " +
            "You must be concise and to the point, focus on face and clothing. Describe the person in a generic way, do not use names.";
        private const string old2portraitSystemPromt =
            "You are a helpful agent. " +
            "Your assignment is to describe characters photos from a fantasy movie. " +
            "All your descriptions must match the theme which is fantasy and adventure, it takes place in a middle aged fantasy world. " +
            "You must be concise and to the point, focus on face and clothing.";
        private const string oldPortraitSystemPromt =
            "you are a expert portrait photographer and famous fantasy novel writer. " +
            "Your assignment is to describe characters from a fantasy movie, your descriptions will be used to make portrait photos of the characters. " +
            "All your descriptions should match the theme of fantasy and adventure set in a middle aged fantasy world. " +
            "You must be concise and focus on face and costume.";

        private static LMStudioConfig config = new("localhost", 1234);

        private static string LogPath() =>
            $"D:\\VisualStudioProjects\\IdleGame\\Generator\\TextGeneratorLogs\\textGen{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.txt";

        private static void Log(string textLine)
        {
            using StreamWriter sw = File.AppendText(LogPath());
            sw.WriteLine(textLine);
        }

        public static async Task<string> CreateLocationPrompt(Location location)
        {
            World.InitWorld();
            string requestToLLM = $"Describe {location.Description.ToLower()}";
            DateTime t = DateTime.Now;
            var m = new
            {
                messages = new[]{
                    new { role="assistant", content=locationSystemPromt },
                    new { role="user", content=requestToLLM }},
                temperature = 0.8,
                max_tokens = -1,
                stream = false
            };
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(m),
                Encoding.UTF8,
                "application/json");
            var client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:1234")
            };
            using HttpResponseMessage response = await client.PostAsync(
                "/v1/chat/completions",
                jsonContent);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"{jsonResponse}\n");
            var r = JsonSerializer.Deserialize<LMStudioResponse>(jsonResponse);
            var responseFromLLM = r?.choices[0].message.content ?? throw new Exception("No LLM response!");
            int llmDuration = (int)(DateTime.Now - t).TotalSeconds;
            responseFromLLM = Clean(SplitByColonAndPickLongest(responseFromLLM));
            Log($"Id ;{location.Id}; LLM-in ;{requestToLLM}; LLM-response ; {responseFromLLM.Length} ; {responseFromLLM} ; Elapsed seconds; {llmDuration}");//;systemPromt;{locationSystemPromt}");
            return responseFromLLM;
        }

        public static async Task<string> CreatePortraitPrompt(Person person)
        {
            World.InitWorld();
            string requestToLLM = $"{person.Description}";
            DateTime t = DateTime.Now;
            var m = new
            {
                messages = new[]{
                    new { role="assistant", content=portraitSystemPromt },
                    new { role="user", content=requestToLLM }},
                temperature = 0.8,
                max_tokens = -1,
                stream = false
            };
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(m),
                Encoding.UTF8,
                "application/json");
            var client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:1234")
            };
            using HttpResponseMessage response = await client.PostAsync(
                "/v1/chat/completions",
                jsonContent);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"{jsonResponse}\n");
            var r = JsonSerializer.Deserialize<LMStudioResponse>(jsonResponse);
            var responseFromLLM = r?.choices[0].message.content ?? throw new Exception("No LLM response!");
            int llmDuration = (int)(DateTime.Now - t).TotalSeconds;
            responseFromLLM = Clean(SplitByColonAndPickLongest(responseFromLLM));
            Log($"Id ;{person.Id}; LLM-in ;{requestToLLM}; LLM-response ; {responseFromLLM.Length} ; {responseFromLLM} ; Elapsed seconds; {llmDuration}");//;systemPromt;{locationSystemPromt}");
            return responseFromLLM;
        }

        public static string Clean(string s) =>
            s.Replace("\"", "").Replace(";", "").Replace(":", "").Replace("  ", " ").Replace("\t", "").Replace("\r", "").Replace("\n", "").Trim();

        public static string SplitByColonAndPickLongest(string s)
        {
            var parts = s.Split(':');
            if (parts.Length == 1)
                return s;
            else if (parts.Length == 2)
            {
                if (parts[0].Length < parts[1].Length)
                    return parts[1];
                else
                    return parts[0];
            }
            else
                return string.Join(" ", parts[1..]);
        }

        public static string CreateNegativeLocationPrompt() =>
            "painting, drawing, text, watermark, border, frame, planes, cars, ferries";

        public static string CreateNegativePersonPrompt() =>
            "painting, drawing, text, watermark, border, frame, wrist watch, mobile phone, glasses";
    }
}