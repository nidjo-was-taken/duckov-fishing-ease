using System;
using HarmonyLib;
using UnityEngine;

// Patches for fishing success windows in both Action_Fishing and Action_FishingV2
namespace FishingEase
{
    // v1 fishing: success window defined by private float catchTime; success if currentTime < catchTime
    [HarmonyPatch]
    internal static class Patch_ActionFishing_Catching
    {
        static System.Reflection.MethodBase TargetMethod()
        {
            // Target: private async UniTask<bool> Catching(Func<bool> IsTaskValid)
            var type = AccessTools.TypeByName("Action_Fishing");
            if (type == null) return null;
            // Find by name since it's async; Harmony can patch the original method symbol
            return AccessTools.Method(type, "Catching");
        }

        static void Prefix(object __instance)
        {
            try
            {
                var field = AccessTools.Field(__instance.GetType(), "catchTime");
                if (field != null)
                {
                    float value = (float)field.GetValue(__instance);
                    value *= Config.Multiplier; // scale by config
                    field.SetValue(__instance, value);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[FishingEase] Failed to adjust Action_Fishing.catchTime: {ex.Message}");
            }
        }
    }

    // v2 fishing: success window duration governed by scaleTime computed in TransToRing()
    [HarmonyPatch]
    internal static class Patch_ActionFishingV2_TransToRing
    {
        static System.Reflection.MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("Action_FishingV2");
            if (type == null) return null;
            return AccessTools.Method(type, "TransToRing");
        }

        // Cap preference: YES -> keep original cap when doubling
        static void Postfix(object __instance)
        {
            try
            {
                var field = AccessTools.Field(__instance.GetType(), "scaleTime");
                if (field != null)
                {
                    float value = (float)field.GetValue(__instance);
                    value = Mathf.Min(value * Config.Multiplier, 7f); // scaled but capped at original 7f
                    field.SetValue(__instance, value);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[FishingEase] Failed to adjust Action_FishingV2.scaleTime: {ex.Message}");
            }
        }
    }
}
