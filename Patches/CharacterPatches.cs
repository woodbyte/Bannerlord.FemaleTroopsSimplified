using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Bannerlord.FemaleTroopsSimplified.Patches
{
    [HarmonyPatch]
    internal class CharacterPatches
    {
        static Random _random = new();
        static bool _enableGenderOverride;

        internal static int GetCultureCoverage(CultureCode cultureCode)
        {
            if (Settings.Instance == null) return 0;

            int coverage = Settings.Instance.DefaultCoverage;

            switch (cultureCode)
            {
                case CultureCode.Nord:
                    if (Settings.Instance.SkolderbrodaToggle) coverage = Settings.Instance.SkolderbrodaCoverage;
                    break;
                case CultureCode.Darshi:
                    if (Settings.Instance.GhilmanToggle) coverage = Settings.Instance.GhilmanCoverage;
                    break;
                case CultureCode.Vakken:
                    if (Settings.Instance.ForestToggle) coverage = Settings.Instance.ForestCoverage;
                    break;
                case CultureCode.Aserai:
                    if (Settings.Instance.AseraiToggle) coverage = Settings.Instance.AseraiCoverage;
                    break;
                case CultureCode.Battania:
                    if (Settings.Instance.BattaniaToggle) coverage = Settings.Instance.BattaniaCoverage;
                    break;
                case CultureCode.Empire:
                    if (Settings.Instance.EmpireToggle) coverage = Settings.Instance.EmpireCoverage;
                    break;
                case CultureCode.Khuzait:
                    if (Settings.Instance.KhuzaitToggle) coverage = Settings.Instance.KhuzaitCoverage;
                    break;
                case CultureCode.Sturgia:
                    if (Settings.Instance.SturgiaToggle) coverage = Settings.Instance.SturgiaCoverage;
                    break;
                case CultureCode.Vlandia:
                    if (Settings.Instance.VlandiaToggle) coverage = Settings.Instance.VlandiaCoverage;
                    break;
            }

            return coverage;
        }

        internal static bool GetCharacterIsFemale(CharacterObject character, int seed = -1)
        {
            if (character.IsHero) return false;

            if (Settings.Instance == null) return false;

            int coverage = GetCultureCoverage(character.Culture.GetCultureCode());

            Random rand = _random;
            if (seed != -1)
                rand = new(seed);

            return rand.NextDouble() < coverage / 100f;
        }

        internal static void EnableGenderOverride(CharacterObject character, int seed = -1)
        {
            _enableGenderOverride = GetCharacterIsFemale(character, seed);
        }

        internal static void EnableGenderOverride()
        {
            _enableGenderOverride = true;
        }

        internal static void DisableGenderOverride()
        {
            _enableGenderOverride = false;
        }

        [HarmonyPatch(typeof(BasicCharacterObject))]
        [HarmonyPatch(nameof(BasicCharacterObject.IsFemale), MethodType.Getter)]
        class Patch01
        {
            internal static void Postfix(ref bool __result)
            {
                if (_enableGenderOverride)
                    __result = true;
            }
        }
    }
}
