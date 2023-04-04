using Bannerlord.FemaleTroopsSimplified.Configuration;
using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Core;

namespace Bannerlord.FemaleTroopsSimplified.Patches
{
    [HarmonyPatch]
    class EncyclopediaPatches
    {
        internal static bool DisableFullPagePatch { get; set; } = false;

        [HarmonyPatch(typeof(EncyclopediaUnitPageVM))]
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(EncyclopediaPageArgs) })]
        class Patch01
        {
            internal static void Prefix(EncyclopediaUnitPageVM __instance, EncyclopediaPageArgs args)
            {
                if (DisableFullPagePatch) return;

                if (CampaignSettings.Instance == null) return;
                if (!CampaignSettings.Instance.RandomizeEncyclopedia) return;

                CharacterObject? character = args.Obj as CharacterObject;
                if (character == null) return;

                CharacterPatches.EnableGenderOverride(character);
            }

            internal static void Postfix(EncyclopediaUnitPageVM __instance, EncyclopediaPageArgs args)
            {
                if (DisableFullPagePatch) return;

                CharacterPatches.DisableGenderOverride();
            }
        }

        [HarmonyPatch(typeof(EncyclopediaUnitVM))]
        [HarmonyPatch(nameof(EncyclopediaUnitVM.ExecuteLink))]
        class Patch02
        {
            internal static void Prefix(EncyclopediaUnitVM __instance)
            {
                if (__instance.ImageIdentifier == null || __instance.ImageIdentifier.Id == "") return;

                DisableFullPagePatch = true;

                CharacterCode code = CharacterCode.CreateFrom(__instance.ImageIdentifier.Id);

                if (code.IsFemale)
                    CharacterPatches.EnableGenderOverride();
            }

            internal static void Postfix(EncyclopediaUnitVM __instance)
            {
                CharacterPatches.DisableGenderOverride();
                DisableFullPagePatch = false;
            }
        }

        [HarmonyPatch(typeof(EncyclopediaUnitVM))]
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(CharacterObject), typeof(bool) })]
        class Patch03
        {
            internal static void Prefix(out bool __state, CharacterObject character, bool isActive)
            {
                __state = CharacterPatches.GenderOverrideEnabled;

                if (CampaignSettings.Instance == null) return;

                if (CharacterPatches.GenderOverrideEnabled && CampaignSettings.Instance.GetCharacterCoverage(character) == 0)
                {
                    CharacterPatches.DisableGenderOverride();
                }
            }

            internal static void Postfix(bool __state, CharacterObject character, bool isActive)
            {
                if (__state)
                    CharacterPatches.EnableGenderOverride();
            }
        }
    }
}
