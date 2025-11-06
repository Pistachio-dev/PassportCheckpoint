using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using static Lumina.Data.Parsing.Layer.LayerCommon;

namespace PassportCheckpoint
{
    public class Actions
    {
        private readonly Plugin plugin;

        public Actions(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public async Task OpenTomestonePage(string playerName, string playerWorld)
        {
            var url = await plugin.DataFetcher.FetchURL(playerName, playerWorld);
            ShellStart(url);
        }        

        public static void ShellStart(string s)
        {
            Safe(delegate
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = s,
                    UseShellExecute = true
                });
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Safe(System.Action a, bool suppressErrors = false)
        {
            try
            {
                a();
            }
            catch (Exception ex)
            {
                if (!suppressErrors)
                {
                    Plugin.Log.Error(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
    }
}
