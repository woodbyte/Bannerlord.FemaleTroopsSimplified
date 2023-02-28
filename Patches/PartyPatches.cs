using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;

namespace Bannerlord.FemaleTroopsSimplified.Patches
{
    [HarmonyPatch]
    class PartyPatches
    {
        static Random _random = new();
        static int _seed;

        static bool DisableCharacterPatch { get; set; } = false;

        [HarmonyPatch(typeof(PartyVM))]
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(Game), typeof(PartyScreenLogic), typeof(string), typeof(string) })]
        class Patch01
        {
            internal static void Prefix(Game game, PartyScreenLogic partyScreenLogic, string fiveStackShortcutkeyText, string entireStackShortcutkeyText)
            {
                _seed = _random.Next();
            }
        }

        [HarmonyPatch(typeof(PartyVM))]
        [HarmonyPatch(nameof(PartyVM.CurrentCharacter), MethodType.Setter)]
        class Patch02
        {
            internal static void Prefix(PartyVM __instance, PartyCharacterVM value)
            {
                CharacterCode code = CharacterCode.CreateFrom(value.Code.Id);

                if (code.IsFemale)
                    CharacterPatches.EnableGenderOverride();
            }

            internal static void Postfix(PartyVM __instance)
            {
                CharacterPatches.DisableGenderOverride();
            }
        }

        [HarmonyPatch(typeof(PartyCharacterVM))]
        [HarmonyPatch(nameof(PartyCharacterVM.Character), MethodType.Setter)]
        class Patch03
        {
            internal static void Prefix(PartyCharacterVM __instance, CharacterObject value)
            {
                if (DisableCharacterPatch) return;

                CharacterPatches.EnableGenderOverride(value, _seed + value.GetHashCode());
            }

            internal static void Postfix(PartyCharacterVM __instance)
            {
                if (DisableCharacterPatch) return;

                CharacterPatches.DisableGenderOverride();
            }
        }

        [HarmonyPatch(typeof(PartyCharacterVM))]
        [HarmonyPatch(nameof(PartyCharacterVM.ExecuteOpenTroopEncyclopedia))]
        class Patch04
        {
            internal static void Prefix(PartyCharacterVM __instance)
            {
                EncyclopediaPatches.DisableFullPagePatch = true;

                CharacterCode code = CharacterCode.CreateFrom(__instance.Code.Id);

                if (code.IsFemale)
                    CharacterPatches.EnableGenderOverride();
            }

            internal static void Postfix(PartyCharacterVM __instance)
            {
                CharacterPatches.DisableGenderOverride();
                EncyclopediaPatches.DisableFullPagePatch = false;
            }
        }

        [HarmonyPatch(typeof(UpgradeTargetVM))]
        [HarmonyPatch(nameof(UpgradeTargetVM.ExecuteUpgradeEncyclopediaLink))]
        class Patch05
        {
            internal static void Prefix(UpgradeTargetVM __instance)
            {
                EncyclopediaPatches.DisableFullPagePatch = true;

                CharacterCode code = CharacterCode.CreateFrom(__instance.TroopImage.Id);

                if (code.IsFemale)
                    CharacterPatches.EnableGenderOverride();
            }

            internal static void Postfix(UpgradeTargetVM __instance)
            {
                CharacterPatches.DisableGenderOverride();
                EncyclopediaPatches.DisableFullPagePatch = false;
            }
        }
    }
}
