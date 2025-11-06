using FFXIVClientStructs.FFXIV.Component.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PassportCheckpoint
{
    public class DataFetcher
    {
        private readonly Plugin plugin;

        public DataFetcher(Plugin plugin)
        {
            this.plugin = plugin;
        }
        public async Task<string> FetchURL(string playerName, string playerWorld)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var splitName = playerName.Split(' ');
                    string url = $"https://tomestone.gg/search/autocomplete?term={splitName[0]}%20{splitName[1]}&type=&scope=";
                    using (HttpResponseMessage res = await client.GetAsync(url))
                    {
                        using (HttpContent content = res.Content)
                        {
                            var json = await content.ReadAsStringAsync();
                            var data = System.Text.Json.JsonSerializer.Deserialize<AutoCompleteResponse>(json);
                            if (data?.characters.Count == 0)
                            {
                                Plugin.Log.Info($"{playerName}@{playerWorld} does not have a Tomestone profile");
                                Plugin.ChatGui.PrintError($"{playerName}@{playerWorld} does not have a Tomestone profile");
                                return string.Empty;
                            }
                            Character character = data.characters.FirstOrDefault(ch => ch.item.serverName.Equals(playerWorld, StringComparison.OrdinalIgnoreCase))
                                ?? throw new Exception($"No player {playerName} in {playerWorld} exists");
                            var tomestoneCharUrl = @"https://tomestone.gg" + character.href + "/";
                            Plugin.Log.Info(@$"Found url: {tomestoneCharUrl}");
                            return tomestoneCharUrl;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Plugin.ChatGui.PrintError("Something went wrong checking the passport.");
                Plugin.Log.Error(e, "Could not fetch passport URL");
                throw;
            }            
        }
    }
}
