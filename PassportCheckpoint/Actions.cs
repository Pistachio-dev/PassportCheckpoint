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

        public async Task OpenPartyTomestonePages()
        {
            var players = GetPlayersInPartyOrAlliance();
            List<Task> tasks = new();

            foreach ((var name, var world) in players)
            {
                Plugin.Log.Info($"Opening tomestone page for {name}@{world}");
                tasks.Add(OpenTomestonePage(name, world));
            }

            await Task.WhenAll(tasks);
        }

        private unsafe List<(string playerName, string playerWorld)> GetPlayersInPartyOrAlliance()
        {
            var groupManager = GroupManager.Instance();

            List<(string name, string world)> players = new();

            Plugin.Log.Info($"Starting party data retrieval");

            if (!TryGetAddonByName<AddonPartyList>("_PartyList", out AddonPartyList* addonPartyListPtr))
            {
                Plugin.Log.Warning("Can't find party list addon");
                return players;
            }

            foreach (var player in addonPartyListPtr->PartyMembers)
            {
                Plugin.Log.Warning(player.Name->GetText().ExtractText());
            }

            return players;

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

        private unsafe Span<PartyMember> GetPartyMemberSpan()
        {
            return GroupManager.Instance()->MainGroup.PartyMembers[..GroupManager.Instance()->MainGroup.MemberCount];
        }

        public static unsafe bool TryGetAddonByName<T>(string Addon, out T* AddonPtr) where T : unmanaged
        {
            var a = Plugin.GameGui.GetAddonByName(Addon, 1);
            if (a == IntPtr.Zero)
            {
                AddonPtr = null;
                return false;
            }
            else
            {
                AddonPtr = (T*)a.Address;
                return true;
            }
        }
    }
}
