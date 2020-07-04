using System.Linq;
using Rocket.Core.Plugins;
using HarmonyLib;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using System.Collections.Generic;
using System;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Core;
using Rocket.API;
using Rocket.Unturned.Player;

namespace TrapPlugin
{
    public class TrapPlugin : RocketPlugin
    {
        protected override void Load()
        {
            Rocket.Core.Logging.Logger.Log("Trap Plugin has been loaded!");
            Rocket.Core.Logging.Logger.Log("Version: 1.0");
            Rocket.Core.Logging.Logger.Log("Made by Paradox");
            
            var harmony = new Harmony("xyz.u6s.unturnedsixsiege.trapplugin");
            harmony.PatchAll(Assembly);
        }
        protected override void Unload()
        {
            Rocket.Core.Logging.Logger.Log("Trap Plugin has been unloaded!");
        }
    }

    [HarmonyPatch(typeof(InteractableTrap), "OnTriggerEnter")]
    public static class OnTriggerEnter_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Collider other, InteractableTrap __instance)
        {
            if (!other.transform.CompareTag("Player")) return true; // Not a player, we dont care, it can activate.
            if (!Provider.isPvP || other.transform.parent.CompareTag("Vehicle")) return false; // PvP is disabled or the player is in a vehicle, so ignore.
            var player = DamageTool.getPlayer(other.transform);
            if (player == null) return true; // Player not found, something went horribly wrong in nelson code
            if (!BarricadeManager.tryGetInfo(__instance.transform.parent, out byte x, out byte y, out ushort plant, out ushort index, out BarricadeRegion region)) return true;
            var barricadeData = region.barricades[index];
            if (barricadeData.owner == player.channel.owner.playerID.steamID.m_SteamID) return false;
            if (barricadeData.group == player.quests.groupID.m_SteamID) return false;
            if (UnturnedPlayer.FromPlayer(player).HasPermission("onDuty")) return false;
            return true;
        }
    }
}



