﻿using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;

namespace Bannerlord.FemaleTroopsSimplified.Patches
{
    [HarmonyPatch]
    class PartyPatches
    {
        static Random _random = new();
        static int _seed;

        [HarmonyPatch(typeof(PartyVM))]
        [HarmonyPatch(nameof(PartyVM.PartyScreenLogic), MethodType.Setter)]
        class Patch01
        {
            internal static void Prefix()
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
                if (value.Code == null || value.Code.Id == "") return;

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
                CharacterPatches.EnableGenderOverride(value, _seed + value.GetHashCode());
            }

            internal static void Postfix(PartyCharacterVM __instance)
            {
                CharacterPatches.DisableGenderOverride();
            }
        }

        [HarmonyPatch(typeof(PartyCharacterVM))]
        [HarmonyPatch(nameof(PartyCharacterVM.ExecuteOpenTroopEncyclopedia))]
        class Patch04
        {
            internal static void Prefix(PartyCharacterVM __instance)
            {
                if (__instance.Code == null || __instance.Code.Id == "") return;

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
                if (__instance.TroopImage == null || __instance.TroopImage.Id == "") return;

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
