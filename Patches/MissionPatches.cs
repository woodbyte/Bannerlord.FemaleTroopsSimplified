﻿using Bannerlord.FemaleTroopsSimplified.Configuration;
using HarmonyLib;
using SandBox.ViewModelCollection.Tournament;
using StoryMode.GameComponents;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TroopSelection;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.CustomBattle;

namespace Bannerlord.FemaleTroopsSimplified.Patches
{
    [HarmonyPatch]
    class MissionPatches
    {
        static bool ProcessingSentence { get; set; } = false;

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
                if (__instance.Visual == null || __instance.Visual.Id == "") return;

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

        [HarmonyPatch(typeof(TroopSelectionItemVM))]
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(TroopRosterElement), typeof(Action<TroopSelectionItemVM>), typeof(Action<TroopSelectionItemVM>) })]
        class Patch04
        {
            internal static void Prefix(TroopRosterElement troop, Action<TroopSelectionItemVM> onAdd, Action<TroopSelectionItemVM> onRemove)
            {
                if (troop.Character == null) return;

                CharacterPatches.EnableGenderOverride(troop.Character);
            }

            internal static void Postfix(TroopRosterElement troop, Action<TroopSelectionItemVM> onAdd, Action<TroopSelectionItemVM> onRemove)
            {
                CharacterPatches.DisableGenderOverride();
            }
        }

        [HarmonyPatch(typeof(ConversationManager))]
        [HarmonyPatch(nameof(ConversationManager.ProcessSentence))]
        class Patch05
        {
            internal static void Prefix(ConversationSentenceOption conversationSentenceOption)
            {
                if (Game.Current == null) return;

                ProcessingSentence = true;
            }

            internal static void Postfix(ConversationSentenceOption conversationSentenceOption)
            {
                ProcessingSentence = false;

                CharacterPatches.DisableGenderOverride();
            }
        }

        [HarmonyPatch(typeof(ConversationManager))]
        [HarmonyPatch("UpdateSpeakerAndListenerAgents")]
        class Patch06
        {
            internal static void Postfix(ConversationManager __instance, ConversationSentence sentence)
            {
                if (ProcessingSentence)
                {
                    if (__instance.SpeakerAgent != null && __instance.SpeakerAgent is Agent agent && agent.IsFemale)
                        CharacterPatches.EnableGenderOverride();
                }
            }
        }

        [HarmonyPatch(typeof(StoryModeVoiceOverModel))]
        [HarmonyPatch(nameof(StoryModeVoiceOverModel.GetSoundPathForCharacter))]
        class Patch07
        {
            internal static void Postfix(ref string __result, CharacterObject character, VoiceObject voiceObject)
            {
                if (character.IsFemale && CampaignSettings.GetCharacterIsValid(character, includeFemales: true))
                {
                    __result = "";
                }
            }
        }
    }
}
