using MCM.Abstractions.FluentBuilder;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.FemaleTroopsSimplified.Configuration
{
    internal class OverrideManager
    {
        SortedSet<CultureOverride> _sortedCultureOverrides;
        Dictionary<CultureObject, CultureOverride> _cultureOverrides;
        Dictionary<CharacterObject, CharacterOverride> _characterOverrides;

        public OverrideManager()
        {
            _cultureOverrides = new();
            _sortedCultureOverrides = new();
            _characterOverrides = new();

            foreach (var character in CharacterObject.All)
            {
                if (!Settings.GetCharacterIsValid(character)) continue;

                CharacterObject overrideCharacter;

                if (character.IsBasicTroop)
                    overrideCharacter = character;
                else
                {
                    var root = FindUpgradeRootOf(character, true);

                    if (root != character)
                        overrideCharacter = root;
                    else
                    {
                        root = FindUpgradeRootOf(character, false);

                        overrideCharacter = root;
                    }
                }

                var culture = overrideCharacter.Culture;

                if (!_cultureOverrides.TryGetValue(culture, out var cultureOverride))
                {
                    cultureOverride = new CultureOverride(culture);
                    _cultureOverrides.Add(culture, cultureOverride);
                    _sortedCultureOverrides.Add(cultureOverride);
                }

                var characterOverride = cultureOverride.AddTroopOverride(overrideCharacter);

                _characterOverrides.Add(character, characterOverride);
            }
        }

        public int GetCharacterCoverage(CharacterObject character)
        {
            if (character == null) return 0;

            if (_characterOverrides.TryGetValue(character, out var characterOverride))
                return characterOverride.GetFinalCoverage();

            if (_cultureOverrides.TryGetValue(character.Culture, out var cultureOverride))
                if (cultureOverride.Enabled && cultureOverride.CoverageEnabled)
                    return cultureOverride.CoverageValue;

            if (Settings.Instance == null) return 0;

            return Settings.Instance.DefaultCoverage;
        }

        public void CreateGroups(ISettingsBuilder builder, int startingOrder)
        {
            int order = startingOrder;

            foreach (var cultureOverride in _sortedCultureOverrides)
            {
                cultureOverride.CreateGroup(builder, order);
                order++;
            }
        }

        public void CreatePresets(ISettingsPresetBuilder builder, string presetName)
        {
            foreach (var cultureOverride in _sortedCultureOverrides)
            {
                cultureOverride.CreatePreset(builder, presetName);
            }
        }

        public static CharacterObject FindUpgradeRootOf(CharacterObject character, bool basicTroopOnly = false)
        {
            foreach (CharacterObject item in CharacterObject.All)
            {
                if ((item.IsBasicTroop || !basicTroopOnly) && UpgradeTreeContains(item, item, character))
                {
                    return item;
                }
            }

            return character;
        }

        private static bool UpgradeTreeContains(CharacterObject rootTroop, CharacterObject baseTroop, CharacterObject character)
        {
            if (baseTroop == character)
            {
                return true;
            }

            for (int i = 0; i < baseTroop.UpgradeTargets.Length; i++)
            {
                if (baseTroop.UpgradeTargets[i] == rootTroop)
                {
                    return false;
                }

                if (UpgradeTreeContains(rootTroop, baseTroop.UpgradeTargets[i], character))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
