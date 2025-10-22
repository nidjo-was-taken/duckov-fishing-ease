using System;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace FishingEase
{
    internal static class Config
    {
        private const string FileName = "FishingEase.cfg";
        private const string SectionHeader = "# FishingEase Configuration";
        private const string KeyName = "multiplier";
        private const float DefaultMultiplier = 1.33f;

        public static float Multiplier { get; private set; } = DefaultMultiplier;

        public static void Load()
        {
            try
            {
                string modsDir = Path.Combine(Application.dataPath, "Mods");
                string modDir = Path.Combine(modsDir, "FishingEase");
                string cfgPath = Path.Combine(modDir, FileName);

                if (!File.Exists(cfgPath))
                {
                    Directory.CreateDirectory(modDir);
                    WriteDefault(cfgPath);
                    Multiplier = DefaultMultiplier;
                    Debug.Log($"[FishingEase] Created default config at {cfgPath}");
                    return;
                }

                string[] lines = File.ReadAllLines(cfgPath);
                float parsed = DefaultMultiplier;
                bool found = false;
                foreach (var raw in lines)
                {
                    if (string.IsNullOrWhiteSpace(raw)) continue;
                    var line = raw.Trim();
                    if (line.StartsWith("#")) continue;
                    int eq = line.IndexOf('=');
                    if (eq <= 0) continue;
                    string key = line.Substring(0, eq).Trim();
                    string val = line.Substring(eq + 1).Trim();
                    if (string.Equals(key, KeyName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (float.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed))
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    Debug.LogWarning("[FishingEase] multiplier not found in config, using default 1.33");
                    Multiplier = DefaultMultiplier;
                    return;
                }

                if (parsed <= 0f)
                {
                    Debug.LogWarning("[FishingEase] multiplier <= 0 in config, clamping to 0.01");
                    parsed = 0.01f;
                }

                // Reasonable upper clamp to avoid absurd values
                if (parsed > 10f) parsed = 10f;

                Multiplier = parsed;
                Debug.Log($"[FishingEase] Loaded multiplier = {Multiplier.ToString(CultureInfo.InvariantCulture)}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[FishingEase] Failed to load config: {ex.Message}. Using default {DefaultMultiplier}");
                Multiplier = DefaultMultiplier;
            }
        }

        private static void WriteDefault(string path)
        {
            string[] content = new[]
            {
                SectionHeader,
                "# multiplier is a float factor applied to the fishing success window",
                "# 1.0 = original timing, 1.33 = +33%, 2.0 = x2",
                "multiplier=1.33"
            };
            File.WriteAllLines(path, content);
        }
    }
}
