using HarmonyLib;
using Helpers;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.Core;
using static TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.ClanPartyItemVM;

namespace Bannerlord.FemaleTroopsSimplified.Patches
{
    [HarmonyPatch]
    class SettlementPatches
    {
        [HarmonyPatch(typeof(GameMenuPartyItemVM))]
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(Action<GameMenuPartyItemVM>), typeof(PartyBase), typeof(bool) })]
        class Patch01
        {
            internal static void Prefix(GameMenuPartyItemVM __instance, Action<GameMenuPartyItemVM> onSetAsContextMenuActiveItem, PartyBase item, bool canShowQuest)
            {
                CharacterObject visualPartyLeader = PartyBaseHelper.GetVisualPartyLeader(item);

                if (visualPartyLeader == null) return;
                if (visualPartyLeader.IsHero) return;

                CharacterPatches.EnableGenderOverride(visualPartyLeader);
            }

            internal static void Postfix(GameMenuPartyItemVM __instance, Action<GameMenuPartyItemVM> onSetAsContextMenuActiveItem, PartyBase item, bool canShowQuest)
            {
                CharacterPatches.DisableGenderOverride();
            }
        }

        [HarmonyPatch(typeof(ClanPartyItemVM))]
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(PartyBase), typeof(Action<ClanPartyItemVM>), typeof(Action), typeof(Action), typeof(ClanPartyType), typeof(IDisbandPartyCampaignBehavior), typeof(ITeleportationCampaignBehavior) })]
        class Patch02
        {
            internal static void Prefix(ClanPartyItemVM __instance, PartyBase party, Action<ClanPartyItemVM> onAssignment, Action onExpenseChange, Action onShowChangeLeaderPopup, ClanPartyType type, IDisbandPartyCampaignBehavior disbandBehavior, ITeleportationCampaignBehavior teleportationBehavior)
            {
                CharacterObject? leader = CampaignUIHelper.GetVisualPartyLeader(party);
                if (leader == null)
                {
                    TroopRosterElement troopRosterElement = party.MemberRoster.GetTroopRoster().FirstOrDefault();
                    if (!troopRosterElement.Equals(default))
                    {
                        leader = troopRosterElement.Character;
                    }
                    else
                    {
                        leader = party.MapFaction?.BasicTroop;
                    }
                }

                if (leader == null) return;

                CharacterPatches.EnableGenderOverride(leader);
            }

            internal static void Postfix(ClanPartyItemVM __instance, PartyBase party, Action<ClanPartyItemVM> onAssignment, Action onExpenseChange, Action onShowChangeLeaderPopup, ClanPartyType type, IDisbandPartyCampaignBehavior disbandBehavior, ITeleportationCampaignBehavior teleportationBehavior)
            {
                CharacterPatches.DisableGenderOverride();
            }
        }

        [HarmonyPatch(typeof(ClanPartyItemVM))]
        [HarmonyPatch(nameof(ClanPartyItemVM.UpdateProperties))]
        class Patch03
        {
            internal static void Prefix(ClanPartyItemVM __instance)
            {
                if (__instance.LeaderVisual == null) return;

                var code = CharacterCode.CreateFrom(__instance.LeaderVisual.Id);

                if (code.IsFemale)
                    CharacterPatches.EnableGenderOverride();
            }

            internal static void Postfix(ClanPartyItemVM __instance)
            {
                CharacterPatches.DisableGenderOverride();
            }
        }
    }
}
