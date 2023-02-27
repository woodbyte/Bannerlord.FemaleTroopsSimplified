using Bannerlord.FemaleTroopsSimplified.Patches;
using HarmonyLib;
using Helpers;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.Localization;

namespace Bannerlord.FemaleTroopsSimplified.Behaviors
{
    internal class FemaleTroopsSimplifiedBehavior : CampaignBehaviorBase
    {
        internal struct TroopRename
        {
            public string Original;
            public string New;
        }

        private Dictionary<string, TroopRename>? _troopRenames;

        public FemaleTroopsSimplifiedBehavior()
        {
            NativeOptions.OnNativeOptionsApplied += NativeOptions_OnNativeOptionsApplied;
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, () =>
            {
                LoadTroopRenames("ModuleData/base_troop_renames.xml");

                if (Settings.Instance != null && Settings.Instance.UseGenderNeutral)
                {
                    ApplyTroopRenames();
                }
            });
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void NativeOptions_OnNativeOptionsApplied()
        {
            if (Settings.Instance == null) return;

            RevertTroopRenames();

            if (Settings.Instance.UseGenderNeutral)
                ApplyTroopRenames();
        }

        internal void RevertTroopRenames()
        {
            AccessTools.FieldRef<TextObject, string> textObjectValue =
                AccessTools.FieldRefAccess<TextObject, string>("Value");

            if (_troopRenames == null) return;

            var characters = CharacterObject.All;

            foreach (var character in characters)
            {
                if (character.IsHero) continue;

                if (_troopRenames.TryGetValue(character.StringId, out TroopRename rename))
                {
                    string name = textObjectValue(character.Name);

                    if (name == rename.New)
                    {
                        textObjectValue(character.Name) = rename.Original;
                    }

                    character.Name.CacheTokens();
                }
            }
        }

        internal void ApplyTroopRenames()
        {
            AccessTools.FieldRef<TextObject, string> textObjectValue =
                AccessTools.FieldRefAccess<TextObject, string>("Value");

            if (_troopRenames == null) return;

            var characters = CharacterObject.All;

            foreach (var character in characters)
            {
                if (character.IsHero) continue;

                if (!character.IsFemale && CharacterPatches.GetCultureCoverage(character.Culture.GetCultureCode()) == 0)
                    continue;

                if (_troopRenames.TryGetValue(character.StringId, out TroopRename rename))
                {
                    string name = textObjectValue(character.Name);

                    if (name == rename.Original)
                    {
                        textObjectValue(character.Name) = rename.New;
                    }

                    character.Name.CacheTokens();
                }
            }
        }

        internal void LoadTroopRenames(string path)
        {
            _troopRenames = new();

            var modulePath = Utilities.GetFullModulePath("Bannerlord.FemaleTroopsSimplified");
            var xmlFile = MiscHelper.LoadXmlFile(modulePath + path);

            var nodes = xmlFile.SelectNodes("renames/rename");

            foreach (XmlNode node in nodes)
            {
                string? id = node.Attributes["id"]?.Value;
                string? original = node.Attributes["original"]?.Value;
                string? @new = node.Attributes["new"]?.Value;

                if (id == null || id == "")
                    continue;

                if (original == null || original == "")
                    continue;

                if (@new == null || @new == "")
                    continue;

                _troopRenames[id] = new() { Original = original, New = @new };
            }
        }
    }
}
