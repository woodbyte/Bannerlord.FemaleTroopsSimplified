using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;

namespace Bannerlord.FemaleTroopsSimplified.Patches
{
    [HarmonyPatch]
    class PartyPatches
    {
        static Random _random = new();
        static int _seed;

        [HarmonyPatch(typeof(PartyCharacterVM))]
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(PartyScreenLogic), typeof(Action<PartyCharacterVM, bool>), typeof(Action<PartyCharacterVM>), typeof(Action<PartyCharacterVM, int, int, PartyScreenLogic.PartyRosterSide>), typeof(Action<PartyCharacterVM>), typeof(Action<PartyCharacterVM>), typeof(PartyVM), typeof(TroopRoster), typeof(int), typeof(PartyScreenLogic.TroopType), typeof(PartyScreenLogic.PartyRosterSide), typeof(bool), typeof(string), typeof(string) })]
        class Patch01
        {
            internal static void Prefix(PartyCharacterVM __instance, PartyScreenLogic partyScreenLogic, Action<PartyCharacterVM, bool> processCharacterLock, Action<PartyCharacterVM> setSelected, Action<PartyCharacterVM, int, int, PartyScreenLogic.PartyRosterSide> onTransfer, Action<PartyCharacterVM> onShift, Action<PartyCharacterVM> onFocus, PartyVM partyVm, TroopRoster troops, int index, PartyScreenLogic.TroopType type, PartyScreenLogic.PartyRosterSide side, bool isTroopTransferrable, string fiveStackShortcutKeyText, string entireStackShortcutKeyText)
            {
                if (troops == null) return;

                CharacterObject character = troops.GetCharacterAtIndex(index);
                if (character == null) return;

                CharacterPatches.EnableGenderOverride(character, _seed + character.GetHashCode());
            }

            internal static void Postfix(PartyCharacterVM __instance, PartyScreenLogic partyScreenLogic, Action<PartyCharacterVM, bool> processCharacterLock, Action<PartyCharacterVM> setSelected, Action<PartyCharacterVM, int, int, PartyScreenLogic.PartyRosterSide> onTransfer, Action<PartyCharacterVM> onShift, Action<PartyCharacterVM> onFocus, PartyVM partyVm, TroopRoster troops, int index, PartyScreenLogic.TroopType type, PartyScreenLogic.PartyRosterSide side, bool isTroopTransferrable, string fiveStackShortcutKeyText, string entireStackShortcutKeyText)
            {
                CharacterPatches.DisableGenderOverride();
            }
        }

        [HarmonyPatch(typeof(PartyCharacterVM))]
        [HarmonyPatch(nameof(PartyCharacterVM.RefreshValues))]
        class Patch02
        {
            internal static void Prefix(PartyCharacterVM __instance)
            {
                CharacterCode code = CharacterCode.CreateFrom(__instance.Code.Id);

                if (code.IsFemale)
                    CharacterPatches.EnableGenderOverride();
            }

            internal static void Postfix(PartyCharacterVM __instance)
            {
                CharacterPatches.DisableGenderOverride();
            }
        }

        [HarmonyPatch(typeof(PartyCharacterVM))]
        [HarmonyPatch(nameof(PartyCharacterVM.ExecuteSetSelected))]
        class Patch03
        {
            internal static void Prefix(PartyCharacterVM __instance)
            {
                CharacterCode code = CharacterCode.CreateFrom(__instance.Code.Id);

                if (code.IsFemale)
                    CharacterPatches.EnableGenderOverride();
            }

            internal static void Postfix(PartyCharacterVM __instance)
            {
                CharacterPatches.DisableGenderOverride();
            }
        }

        [HarmonyPatch(typeof(PartyCharacterVM))]
        [HarmonyPatch("ApplyTransfer")]
        class Patch04
        {
            internal static void Prefix(PartyCharacterVM __instance)
            {
                CharacterCode code = CharacterCode.CreateFrom(__instance.Code.Id);

                if (code.IsFemale)
                    CharacterPatches.EnableGenderOverride();
            }

            internal static void Postfix(PartyCharacterVM __instance)
            {
                CharacterPatches.DisableGenderOverride();
            }
        }

        [HarmonyPatch(typeof(PartyCharacterVM))]
        [HarmonyPatch(nameof(PartyCharacterVM.ExecuteOpenTroopEncyclopedia))]
        class Patch05
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
        class Patch06
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

        [HarmonyPatch(typeof(PartyVM))]
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(Game), typeof(PartyScreenLogic), typeof(string), typeof(string) })]
        class Patch07
        {
            internal static void Prefix(Game game, PartyScreenLogic partyScreenLogic, string fiveStackShortcutkeyText, string entireStackShortcutkeyText)
            {
                _seed = _random.Next();
            }
        }
    }
}
