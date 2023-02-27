using HarmonyLib;
using Helpers;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Conversation;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.FemaleTroopsSimplified.Patches
{
    [HarmonyPatch]
    class ConversationPatches
    {
        [HarmonyPatch(typeof(MapConversationTableau))]
        [HarmonyPatch("SpawnOpponentBodyguardCharacter")]
        class Patch01
        {
            static AccessTools.FieldRef<MapConversationTableau, MapConversationTableauData> data =
                AccessTools.FieldRefAccess<MapConversationTableau, MapConversationTableauData>("_data");

            internal static void Prefix(MapConversationTableau __instance, CharacterObject character, int indexOfBodyguard)
            {
                MapConversationTableauData _data = data(__instance);

                if (indexOfBodyguard >= 0 && indexOfBodyguard <= 1)
                {
                    int num = (indexOfBodyguard + 10) * 5;
                    int seed = -1;
                    if (_data.ConversationPartnerData.Party != null)
                    {
                        seed = CharacterHelper.GetPartyMemberFaceSeed(_data.ConversationPartnerData.Party, _data.ConversationPartnerData.Character, num);
                    }

                    CharacterPatches.EnableGenderOverride(character, seed);
                }
            }

            internal static void Postfix(MapConversationTableau __instance, CharacterObject character, int indexOfBodyguard)
            {
                CharacterPatches.DisableGenderOverride();
            }
        }

        [HarmonyPatch(typeof(MissionConversationVM))]
        [HarmonyPatch(nameof(MissionConversationVM.ExecuteConversedHeroLink))]
        class Patch02
        {
            internal static void Prefix(MissionConversationVM __instance)
            {
                EncyclopediaPatches.DisableFullPagePatch = true;

                var agent = Campaign.Current.ConversationManager.OneToOneConversationAgent as Agent;

                if (agent == null) return;
                if (agent.IsHero) return;

                if (agent.IsFemale)
                {
                    CharacterPatches.EnableGenderOverride();
                }
            }

            internal static void Postfix(MissionConversationVM __instance)
            {
                CharacterPatches.DisableGenderOverride();
                EncyclopediaPatches.DisableFullPagePatch = false;
            }
        }
    }
}
