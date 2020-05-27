using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using HarmonyLib;
using SDG.Unturned;
using Rocket.Unturned.Player;
using Rocket.Unturned;
using UnityEngine;
using Steamworks;
using Pustalorc.Plugins.BaseClustering.API.Statics;

namespace TrapPlugin
{
    public class TrapPlugin : RocketPlugin
    {
        protected override void Load()
        {
            Rocket.Core.Logging.Logger.Log("Trap Plugin has been loaded!");
            Rocket.Core.Logging.Logger.Log($"Version: {Assembly.GetName().Version}");
            Rocket.Core.Logging.Logger.Log("Made by Paradox");

            var harmony = new Harmony("xyz.u6s.unturnedsixsiege.trapplugin");
            
            BarricadeManager.onDeployBarricadeRequested += onBarricadeDeploy;

        }

        private void onBarricadeDeploy(Barricade barricade, ItemBarricadeAsset asset, Transform hit, ref Vector3 point, ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            if (barricade.id.ToString() == "12403" || barricade.id.ToString() == "27110")
            {
                BarricadeGroup barricadeGroup = new BarricadeGroup() { group = group, barricade = barricade };
                Storage.traps.Add(barricadeGroup);
            }
        }

        protected override void Unload()
        {
            BarricadeManager.onDeployBarricadeRequested -= onBarricadeDeploy;
            Rocket.Core.Logging.Logger.Log("Reset Plugin has been unloaded!");
        }

    }

    [HarmonyPatch(typeof(InteractableTrap), "OnTriggerEnter")]
    public static class OnTriggerEnter_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Collider target, InteractableTrap __instance)
        {
            if (!target.transform.CompareTag("Player")) return true; // Not a player, we dont care, it can activate.

            if (!Provider.isPvP || target.transform.parent.CompareTag("Vehicle")) return true; // PvP is disabled or the player is in a vehicle, so ignore.

            var player = DamageTool.getPlayer(target.transform);
            if (player == null) return true; // Player not found, something went horribly wrong in nelson code

            var build = ReadOnlyGame.GetBuilds(CSteamID.Nil, true, true).FirstOrDefault(k => k.Interactable == __instance); // Note you need https://github.com/Pustalorc/BaseClustering/blob/master/API/Statics/ReadOnlyGame.cs for this to work.
            if (build == null) return true; // Trap data not found, something went horribly wrong in nelson code.

            if (build.Group == player.quests.groupId) return false; // Player is in the same group, cancel the trap's trigger code from nelson.
        }
    }

    public static class Storage {
        public static List<BarricadeGroup> traps = new List<BarricadeGroup>();
    }

    public class BarricadeGroup
    {
        public ulong group { get; set; }
        public Barricade barricade { get; set; }
    }
}
