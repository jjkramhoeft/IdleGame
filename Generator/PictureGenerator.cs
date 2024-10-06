using System.Text;
using System.Text.Json;
using Model;

namespace Generator
{
    public static class PictureGenerator
    {
        private const string comfyPath = $"D:\\ComfyUiStandalone\\ComfyUI_windows_portable\\ComfyUI\\output";
        private const string picturePath = $"D:\\VisualStudioProjects\\IdleGame\\Storage\\Data\\Pictures";
        private const string pictureUriStart = "https://localhost:7210/pictures/";
        private const string templatePath = $"D:\\VisualStudioProjects\\IdleGame\\Generator\\PictureGeneratorTemplates";
        private const string locationFolder = "Locations";
        private const string portraitFolder = "Portraits";
        private const string locationTemplate = "location-template.json";
        private const string portraitTemplate = "portrait-template.json";
        private const string locationPrefix = "l";
        private const string portraitPrefix = "p";
        private const string seedReplacementKey = "1234567654321";
        private const string positivePromptReplacementKey = "XX-insert-positive-XX";
        private const string negativePromptReplacementKey = "XX-insert-negative-XX";
        private const string fileNamePrefixReplacementKey = "XX-insert-filename-XX";
        private static string LogPath() =>
            $"D:\\VisualStudioProjects\\IdleGame\\Generator\\PictureGeneratorLogs\\picGen{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.txt";

        private static void Log(string textLine)
        {
            using StreamWriter sw = File.AppendText(LogPath());
            sw.WriteLine(textLine);
        }

        public static async Task<Guid> QueuePicture(MotiveType motive, int id, int seed, string positivePrompt)
        {
            string uniqe = GetUniqe(id, motive);
            string negativePrompt = GetNegativePrompt(motive);
            using StreamReader templateReader = new(templatePath + "\\" + ToTemplate(motive));
            string templateText = templateReader.ReadToEnd();
            templateText = templateText.Replace(seedReplacementKey, seed.ToString());
            templateText = templateText.Replace(positivePromptReplacementKey, positivePrompt);
            templateText = templateText.Replace(negativePromptReplacementKey, negativePrompt);
            templateText = templateText.Replace(fileNamePrefixReplacementKey, uniqe);
            string prompt = "{\"prompt\": " + templateText + "}";
            using HttpClient client = new();
            client.BaseAddress = new Uri("http://127.0.0.1:8188/prompt");
            var httpRequestContent = new StringContent(prompt, Encoding.UTF8, "application/json");
            var httpResponse = await client.PostAsync("", httpRequestContent);
            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            var comfyPromptResponse = JsonSerializer.Deserialize<Model.ComfyPromptResponse>(jsonResponse);
            if(comfyPromptResponse is not null && comfyPromptResponse.prompt_id.HasValue)
            {
                Log($"Queued picture {uniqe} Id:{comfyPromptResponse.prompt_id}");
                return comfyPromptResponse.prompt_id.Value;
            }
            else
            {
                Log($"Picture {uniqe}failed to queue!");
                return Guid.Empty;
            }
        }

        private static string GetNegativePrompt(MotiveType motive)
        {
            if (motive == MotiveType.location)
                return TextGenerator.CreateNegativeLocationPrompt();
            else if (motive == MotiveType.portrait)
                return TextGenerator.CreateNegativePersonPrompt();
            return "text, watermark, border, frame";
        }

        public static bool RetrievePicture(string comfyFileName, MotiveType motive, int id)
        {
            string comfyName = comfyPath + "\\" + comfyFileName;
            string uniqe = GetUniqe(id, motive);
            if (File.Exists(comfyName))
            {
                string storeName = picturePath + "\\" + ToFolder(motive) + "\\" + uniqe + ".png";
                try
                {
                    File.Copy(comfyName, storeName, true);
                    File.Delete(comfyName);
                    Log($"Retrieved picture {uniqe}");
                    return true;
                }
                catch (Exception e)
                {
                    Log($"Error while retrieving file {uniqe} {e.Message}");
                    return false;
                }
            }
            Log($"Picture not ready for retrieval {uniqe}");
            return false;
        }

        public static string GetUniqe(int id, MotiveType motive) =>
            ToPrefix(motive) + id.ToString().PadLeft(8, '0');

        public static string GetFullPath(int id, MotiveType motive) =>
            $"{picturePath}\\{ToFolder(motive)}\\{ToPrefix(motive)}{id.ToString().PadLeft(8, '0')}.png";

        public static Uri? GetUri(int id, MotiveType motive) =>
            new($"{pictureUriStart}{ToPrefix(motive)}/{id.ToString().PadLeft(8, '0')}");

        public static string ToFolder(MotiveType motive) => motive switch
        {
            MotiveType.location => locationFolder,
            MotiveType.portrait => portraitFolder,
            _ => throw new Exception($"PictureGenerator.MotiveType {motive} not supported for folder.")
        };

        public static string ToPrefix(MotiveType motive) => motive switch
        {
            MotiveType.location => locationPrefix,
            MotiveType.portrait => portraitPrefix,
            _ => throw new Exception($"PictureGenerator.MotiveTyped {motive} not supported for prefix.")
        };

        public static string ToTemplate(MotiveType motive) => motive switch
        {
            MotiveType.location => locationTemplate,
            MotiveType.portrait => portraitTemplate,
            _ => throw new Exception($"PictureGenerator.MotiveTyped {motive} not supported for prefix.")
        };

        public static async Task<ComfyHistoryResponse?> RetrievePictureHistory(Guid comfyId)
        {
            string id = comfyId.ToString().Replace("{","").Replace("}","");
            using HttpClient client = new();
            client.BaseAddress = new Uri("http://127.0.0.1:8188");
            var httpResponse = await client.GetStringAsync($"/history/{id}", CancellationToken.None);
            var histDict = JsonSerializer.Deserialize<Dictionary<Guid,ComfyHistoryResponse>>(httpResponse);
            if(histDict is null || histDict.Count==0)
            {
                // got nothing - potential error
                return null;
            }
            else
            {
                return histDict.First().Value;
            }
        }

        public enum MotiveType
        {
            location,
            portrait
        }
    }
}