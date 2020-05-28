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
            harmony.PatchAll(Assembly);
        }
        protected override void Unload()
        {
            Rocket.Core.Logging.Logger.Log("Reset Plugin has been unloaded!");
        }

        /*
        private void BarricadeSpawned(BarricadeRegion region, BarricadeDrop drop)
        {
            var interactable = drop.interactable;
            if (interactable is InteractableTrap)
            {
                Storage.barricadeDrops.Add(drop);
                
            }
        }

        private void SalvageBarricade(CSteamID steamID, byte x, byte y, ushort plant, ushort index, ref bool shouldAllow)
        {
            if (Storage.barricadeDrops.Count == 0) return;

            var drop = Storage.barricadeDrops.FirstOrDefault(k => BarricadeManager.regions[x, y].drops[index].model == k.model);

            if (drop == null) return;

            Storage.barricadeDrops.Remove(drop);
        }

        
        private void DamageBarricade(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if (!BarricadeManager.tryGetInfo(barricadeTransform, out byte x, out byte y, out ushort plant, out ushort index, out BarricadeRegion region)) return;
            if (BarricadeManager.regions[x, y].barricades[index].barricade.health < pendingTotalDamage)
            {
                if (Storage.barricadeDrops.Count == 0) return;

                var drop = Storage.barricadeDrops.FirstOrDefault(k => BarricadeManager.regions[x, y].drops[index].model == k.model);

                if (drop == null) return;

                Storage.barricadeDrops.Remove(drop);
            }

        } 
        */
    }

    [HarmonyPatch(typeof(InteractableTrap), "OnTriggerEnter")]
    public static class OnTriggerEnter_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Collider target, InteractableTrap __instance)
        {
            Rocket.Core.Logging.Logger.Log("1");
            if (!target.transform.CompareTag("Player")) return true; // Not a player, we dont care, it can activate.
            Rocket.Core.Logging.Logger.Log("2");
            if (!Provider.isPvP || target.transform.parent.CompareTag("Vehicle")) return false; // PvP is disabled or the player is in a vehicle, so ignore.
            Rocket.Core.Logging.Logger.Log("3");
            var player = DamageTool.getPlayer(target.transform);
            if (player == null) return true; // Player not found, something went horribly wrong in nelson code
            Rocket.Core.Logging.Logger.Log("4");
            BarricadeManager.tryGetInfo(__instance.transform, out byte _, out byte _, out ushort _, out ushort index, out BarricadeRegion region);
            Rocket.Core.Logging.Logger.Log("5");
            var barricadeData = region.barricades[index];
            Rocket.Core.Logging.Logger.Log("6");
            if (barricadeData.group == player.quests.groupID.m_SteamID) return false;
            Rocket.Core.Logging.Logger.Log("7");
            return true;
        }
    }

    /* public static class Storage
    {
        public static List<BarricadeDrop> barricadeDrops = new List<BarricadeDrop>();
    } */
}



