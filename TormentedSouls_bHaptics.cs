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
        public static bool rightFootLast = true;

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
        public static void Postfix()
        {
            if (Plugin.tactsuitVr.suitDisabled)
            {
                return;
            }
            Plugin.tactsuitVr.PlaybackHaptics("Death");
            Plugin.tactsuitVr.StopHeartBeat();
        }
    }

    [HarmonyPatch(typeof(GameplaySceneManager), "UpdateMainPlayerHealth")]
    public class bhaptics_OnUpdateMainPlayerHealth
    {
        [HarmonyPostfix]
        public static void Postfix(GameplaySceneManager __instance,  int addAmount)
        {
            if (Plugin.tactsuitVr.suitDisabled)
            {
                return;
            }
            //low health
            UserDataManager userDataManager = __instance.managerRefs.userDataManager;
            int playerHealth = (int)userDataManager.GetPlayerHealth();
            if (playerHealth < __instance.GetSceneData().mainPlayerData.maximumHealth / 3)
            {
                Plugin.tactsuitVr.StartHeartBeat();
            }
            else
            {
                Plugin.tactsuitVr.StopHeartBeat();
            }
            //damages
            if (addAmount < 0)
            {
                Plugin.tactsuitVr.PlaybackHaptics("Impact");
            }
            //Heal
            else
            {
                Plugin.tactsuitVr.PlaybackHaptics("Heal");
            }
        }
    }

    [HarmonyPatch(typeof(WeaponBase), "Attack")]
    public class bhaptics_PlayerSM_Shoot
    {
        [HarmonyPostfix]
        public static void Postfix(WeaponBase __instance)
        {
            if (Plugin.tactsuitVr.suitDisabled)
            {
                return;
            }
            int weaponType = (int)__instance.GetWeaponType();
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

    [HarmonyPatch(typeof(PlayerStepsReceiver), "TriggerStep")]
    public class bhaptics_OnPlayerStepsReceiver
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (Plugin.tactsuitVr.suitDisabled)
            {
                return;
            }
            if (Plugin.rightFootLast)
            {
                Plugin.rightFootLast = false;
                Plugin.tactsuitVr.PlaybackHaptics("FootStep_L");
            }
            else
            {
                Plugin.rightFootLast = true;
                Plugin.tactsuitVr.PlaybackHaptics("FootStep_R");
            }
        }
    }
}

