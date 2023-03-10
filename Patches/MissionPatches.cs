using Bannerlord.FemaleTroopsSimplified.Configuration;
using HarmonyLib;
using SandBox.ViewModelCollection.Tournament;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.CustomBattle;

namespace Bannerlord.FemaleTroopsSimplified.Patches
{
    [HarmonyPatch]
    class MissionPatches
    {
        [HarmonyPatch(typeof(Mission))]
        [HarmonyPatch(nameof(Mission.SpawnAgent))]
        class Patch01
        {
            internal static void Prefix(Mission __instance, AgentBuildData agentBuildData)
            {
                if (agentBuildData.AgentCharacter == null) return;
                if (agentBuildData.AgentCharacter.IsHero) return;

                if (Game.Current.GameType is CustomGame)
                {
                    if (GlobalSettings.Instance == null) return;

                    CharacterPatches.EnableGenderOverride(GlobalSettings.Instance.CustomCoverage, agentBuildData.AgentOrigin.Seed);
                    return;
                }

                if (Game.Current.GameType is Campaign)
                {
                    CharacterObject? character = agentBuildData.AgentCharacter as CharacterObject;
                    if (character == null) return;

                    if (!CampaignSettings.GetCharacterIsValid(character, true)) return;

                    IAgentOriginBase origin = agentBuildData.AgentOrigin;
                    if (origin == null) return;

                    int seed = origin.Seed;
                    if (origin is SimpleAgentOrigin && origin.UniqueSeed != 0)
                        seed = origin.UniqueSeed;

                    CharacterPatches.EnableGenderOverride(character, seed);
                    return;
                }
            }

            internal static void Postfix(Mission __instance, AgentBuildData agentBuildData)
            {
                CharacterPatches.DisableGenderOverride();
            }
        }

        [HarmonyPatch(typeof(TournamentParticipantVM))]
        [HarmonyPatch(nameof(TournamentParticipantVM.Refresh))]
        [HarmonyPatch(new Type[] { typeof(TournamentParticipant), typeof(Color) })]
        class Patch02
        {
            internal static void Prefix(TournamentParticipantVM __instance, TournamentParticipant participant, Color teamColor)
            {
                if (participant == null) return;
                if (participant.Character == null) return;
                if (participant.Character.IsHero) return;

                CharacterPatches.EnableGenderOverride(participant.Character, participant.Descriptor.UniqueSeed);
            }

            internal static void Postfix(TournamentParticipantVM __instance, TournamentParticipant participant, Color teamColor)
            {
                CharacterPatches.DisableGenderOverride();
            }
        }

        [HarmonyPatch(typeof(TournamentParticipantVM))]
        [HarmonyPatch(nameof(TournamentParticipantVM.ExecuteOpenEncyclopedia))]
        class Patch03
        {
            internal static void Prefix(TournamentParticipantVM __instance)
            {
                EncyclopediaPatches.DisableFullPagePatch = true;

                CharacterCode code = CharacterCode.CreateFrom(__instance.Visual.Id);

                if (code.IsFemale)
                    CharacterPatches.EnableGenderOverride();
            }

            internal static void Postfix(TournamentParticipantVM __instance)
            {
                CharacterPatches.DisableGenderOverride();
                EncyclopediaPatches.DisableFullPagePatch = false;
            }
        }
    }
}
