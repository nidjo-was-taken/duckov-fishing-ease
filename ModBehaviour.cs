using Duckov.Modding;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace FishingEase
{
    public static class Constants
    {
        public const string MOD_ID = "FishingEase";
        public const string MOD_NAME = "FishingEase";
    }

    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private Harmony _harmony;

        void Awake()
        {
            Debug.Log($"[{Constants.MOD_NAME}] Loaded!!!");
        }

        void OnEnable()
        {
            // Load config before applying patches
            Config.Load();

            _harmony = new Harmony(Constants.MOD_ID);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            Debug.Log($"[{Constants.MOD_NAME}] Harmony patches applied");
        }

        void OnDisable()
        {
            if (_harmony != null)
            {
                _harmony.UnpatchAll(Constants.MOD_ID);
                Debug.Log($"[{Constants.MOD_NAME}] Harmony patches removed");
            }
        }
    }
}
