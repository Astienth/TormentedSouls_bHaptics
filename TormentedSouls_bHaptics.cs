using System;
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
    /*
    [HarmonyPatch(typeof(Player), "OnDestroy")]
    public class bhaptics_OnPlayerDestroy
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (Plugin.tactsuitVr.suitDisabled)
            {
                return;
            }
            Plugin.tactsuitVr.StopThreads();
        }
    }
    */
}

