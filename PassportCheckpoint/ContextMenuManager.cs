using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using Lumina.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace PassportCheckpoint
{
    public class ContextMenuManager
    {
        private static readonly string[] ValidAddons =
[
    null,
            "PartyMemberList",
            "FriendList",
            "FreeCompany",
            "LinkShell",
            "CrossWorldLinkshell",
            "_PartyList",
            "ChatLog",
            "LookingForGroup",
            "BlackList",
            "ContentMemberList",
            "SocialList",
            "ContactList",
        ];
        private readonly Plugin plugin;

        public ContextMenuManager(Plugin plugin)
        {
            this.plugin = plugin;

            Plugin.ContextMenu.OnMenuOpened += OpenContextMenu;
        }

        public void Dispose()
        {
            Plugin.ContextMenu.OnMenuOpened -= OpenContextMenu;
        }
        private void OpenContextMenu(IMenuOpenedArgs args)
        {
            if (!plugin.Configuration.EnableContextMenu)
            {
                return;
            }

            if (ValidAddons.Contains(args.AddonName) && args.Target is MenuTargetDefault def
                && def.TargetName != null && !Utils.GetWorldName(def.TargetHomeWorld.RowId).IsNullOrEmpty())
            {
                string worldName = Utils.GetWorldName(def.TargetHomeWorld.RowId);
                args.AddMenuItem(new()
                {
                    OnClicked = (args) =>
                    {
                        _ = plugin.Actions.OpenTomestonePage(def.TargetName, worldName);
                    },
                    PrefixChar = 'P',
                    Priority = 420,
                    Name = "Check Tomestone"
                });

                args.AddMenuItem(new()
                {
                    OnClicked = (args) =>
                    {
                        _ = plugin.Actions.OpenPartyTomestonePages();
                    },
                    PrefixChar = 'P',
                    Priority = 421,
                    Name = "Check Tomestone for all party members"
                });
            }
        }


    }
}
