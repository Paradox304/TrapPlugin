using System.Linq;
using Rocket.Core.Plugins;
using HarmonyLib;
using SDG.Unturned;
using Steamworks;
using Pustalorc.Plugins.BaseClustering.API.Statics;
using UnityEngine;

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
        }

        protected override void Unload()
        {
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

            if (!Provider.isPvP || target.transform.parent.CompareTag("Vehicle")) return false; // PvP is disabled or the player is in a vehicle, so ignore.

            var player = DamageTool.getPlayer(target.transform);
            if (player == null) return true; // Player not found, something went horribly wrong in nelson code

            var build = ReadOnlyGame.GetBuilds(CSteamID.Nil, true, true).FirstOrDefault(k => k.Interactable == __instance);
            if (build == null) return true; // Trap data not found, something went horribly wrong in nelson code.

            if (build.Group == player.quests.groupID.m_SteamID) return false; // Player is in the same group, cancel the trap's trigger code from nelson.

            return false;
        }
    }
}
