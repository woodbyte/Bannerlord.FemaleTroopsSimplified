using HarmonyLib;
using SandBox.ViewModelCollection.Tournament;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

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

                CharacterObject? character = agentBuildData.AgentCharacter as CharacterObject;
                if (character == null) return;

                switch (character.Occupation)
                {
                    case Occupation.Bandit:
                    case Occupation.CaravanGuard:
                    case Occupation.Gangster:
                    case Occupation.Guard:
                    case Occupation.Mercenary:
                    case Occupation.PrisonGuard:
                    case Occupation.Soldier:
                        break;
                    default:
                        return;
                }

                // ignore spy quest character ids
                if (character.StringId.Contains("bold_contender_")) return;
                if (character.StringId.Contains("confident_contender_")) return;
                if (character.StringId.Contains("dignified_contender_")) return;
                if (character.StringId.Contains("hardy_contender_")) return;

                // ignore tutorial character ids
                if (character.StringId.Contains("tutorial_npc_")) return;

                IAgentOriginBase origin = agentBuildData.AgentOrigin;
                if (origin == null) return;

                int seed = origin.Seed;
                if (origin is SimpleAgentOrigin && origin.UniqueSeed != 0)
                    seed = origin.UniqueSeed;

                CharacterPatches.EnableGenderOverride(character, seed);
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
