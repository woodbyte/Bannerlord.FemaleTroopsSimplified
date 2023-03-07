using Bannerlord.FemaleTroopsSimplified.Configuration;
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

        internal static bool GenderOverrideEnabled => _enableGenderOverride;

        internal static bool GetRandomCharacterOverride(CharacterObject character, int seed = -1)
        {
            if (character.IsHero) return false;
            if (character.IsFemale) return false;

            if (Settings.Instance == null) return false;

            int coverage = Settings.Instance.GetCharacterCoverage(character);

            Random rand = _random;
            if (seed != -1)
                rand = new(seed);

            return rand.NextDouble() < coverage / 100f;
        }

        internal static void EnableGenderOverride(CharacterObject character, int seed = -1)
        {
            _enableGenderOverride = GetRandomCharacterOverride(character, seed);
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
