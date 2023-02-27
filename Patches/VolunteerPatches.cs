using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Recruitment;
using TaleWorlds.Core;

namespace Bannerlord.FemaleTroopsSimplified.Patches
{
    [HarmonyPatch]
    class VolunteerPatches
    {
        [HarmonyPatch(typeof(RecruitVolunteerTroopVM))]
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(RecruitVolunteerVM), typeof(CharacterObject), typeof(int), typeof(Action<RecruitVolunteerTroopVM>), typeof(Action<RecruitVolunteerTroopVM>) })]
        class Patch01
        {
            internal static void Prefix(RecruitVolunteerTroopVM __instance, RecruitVolunteerVM owner, CharacterObject character, int index, Action<RecruitVolunteerTroopVM> onClick, Action<RecruitVolunteerTroopVM> onRemoveFromCart)
            {
                if (character == null) return;

                int seed = (int)CampaignTime.Zero.ElapsedDaysUntilNow + owner.OwnerHero.Name.GetValueHashCode() + index;

                CharacterPatches.EnableGenderOverride(character, seed);
            }

            internal static void Postfix(RecruitVolunteerTroopVM __instance, RecruitVolunteerVM owner, CharacterObject character, int index, Action<RecruitVolunteerTroopVM> onClick, Action<RecruitVolunteerTroopVM> onRemoveFromCart)
            {
                CharacterPatches.DisableGenderOverride();
            }
        }

        [HarmonyPatch(typeof(RecruitVolunteerTroopVM))]
        [HarmonyPatch(nameof(RecruitVolunteerTroopVM.ExecuteOpenEncyclopedia))]
        class Patch02
        {
            internal static void Prefix(RecruitVolunteerTroopVM __instance)
            {
                if (__instance.ImageIdentifier == null) return;

                EncyclopediaPatches.DisableFullPagePatch = true;

                CharacterCode code = CharacterCode.CreateFrom(__instance.ImageIdentifier.Id);

                if (code.IsFemale)
                    CharacterPatches.EnableGenderOverride();
            }

            internal static void Postfix(RecruitVolunteerTroopVM __instance)
            {
                CharacterPatches.DisableGenderOverride();
                EncyclopediaPatches.DisableFullPagePatch = false;
            }
        }
    }
}
