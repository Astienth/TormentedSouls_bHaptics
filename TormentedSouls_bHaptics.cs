﻿using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using MyBhapticsTactsuit;

namespace TormentedSouls_bHaptics
{
    [BepInPlugin("org.bepinex.plugins.TormentedSouls_bHaptics", "TormentedSouls_bHaptics integration", "1.0")]
    public class Plugin : BaseUnityPlugin
    {
#pragma warning disable CS0109 // Remove unnecessary warning
        internal static new ManualLogSource Log;
#pragma warning restore CS0109
        public static TactsuitVR tactsuitVr;

        private void Awake()
        {
            // Make my own logger so it can be accessed from the Tactsuit class
            Log = base.Logger;
            // Plugin startup logic
            Logger.LogMessage("Plugin TormentedSouls_bHaptics is loaded!");
            tactsuitVr = new TactsuitVR();
            // one startup heartbeat so you know the vest works correctly
            tactsuitVr.PlaybackHaptics("HeartBeat");
            // patch all functions
            var harmony = new Harmony("bhaptics.patch.TormentedSouls_bHaptics");
            harmony.PatchAll();
        }
    }
    
    [HarmonyPatch(typeof(PlayerController), "OnPlayerDeath")]
    public class bhaptics_OnPlayerDeath
    {
        [HarmonyPostfix]
        public static void Postfix(float deathDelay)
        {
            if (Plugin.tactsuitVr.suitDisabled)
            {
                return;
            }
            Plugin.tactsuitVr.PlaybackHaptics("Death");
            Plugin.tactsuitVr.StopHeartBeat();
        }
    }
    
    [HarmonyPatch(typeof(PlayerController), "ReceiveDamage")]
    public class bhaptics_OnPlayerDamage
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (Plugin.tactsuitVr.suitDisabled)
            {
                return;
            }
            Plugin.tactsuitVr.PlaybackHaptics("Impact");
        }
    }

    [HarmonyPatch(typeof(PlayerController), "Update")]
    public class bhaptics_OnPlayerLowHealth
    {
        [HarmonyPostfix]
        public static void Postfix(PlayerController __instance)
        {
            if (Plugin.tactsuitVr.suitDisabled)
            {
                return;
            }
            if(__instance.m_characterData.CurrentHealth < __instance.m_characterData.MaximumHealth / 6)
            {
                Plugin.tactsuitVr.StartHeartBeat();
            }
            else
            {
                Plugin.tactsuitVr.StopHeartBeat();
            }
        }
    }

    [HarmonyPatch(typeof(PlayerSM_Shoot), "StartDamage")]
    public class bhaptics_PlayerSM_Shoot
    {
        [HarmonyPostfix]
        public static void Postfix(PlayerSM_Shoot __instance)
        {
            if (Plugin.tactsuitVr.suitDisabled)
            {
                return;
            }
            int weaponType = (int)Traverse.Create(__instance).Property("m_weaponBase").GetValue<WeaponBase>().GetWeaponType();
            if (weaponType == 0)
            {
                Plugin.tactsuitVr.PlaybackHaptics("RecoilVest_R");
                Plugin.tactsuitVr.PlaybackHaptics("RecoilArm_R");
            }
            if (weaponType == 1)
            {
                Plugin.tactsuitVr.PlaybackHaptics("RecoilVest_R");
                Plugin.tactsuitVr.PlaybackHaptics("RecoilArm_R");
                Plugin.tactsuitVr.PlaybackHaptics("RecoilVest_L");
                Plugin.tactsuitVr.PlaybackHaptics("RecoilArm_L");
            }
        }
    }
}

