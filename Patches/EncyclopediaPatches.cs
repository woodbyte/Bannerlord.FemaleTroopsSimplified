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

                if (Settings.Instance == null) return;
                if (!Settings.Instance.RandomizeEncyclopedia) return;

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
    }
}
