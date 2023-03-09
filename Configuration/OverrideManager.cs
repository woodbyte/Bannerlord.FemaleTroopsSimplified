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

                overrideCharacter = GetCharacterRoot(character);

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

        static CharacterObject GetCharacterRoot(CharacterObject character)
        {
            var roots = new List<CharacterObject>();

            FindAllCharacterRoots(character, roots);

            roots.Sort((x, y) =>
            {
                if (x.IsBasicTroop && !y.IsBasicTroop) return -1;
                else if (!x.IsBasicTroop && y.IsBasicTroop) return 1;
                else return x.Tier - y.Tier;
            });

            return roots[0];
        }

        static void FindAllCharacterRoots(CharacterObject character, List<CharacterObject> roots, CharacterObject? searchStart = null, CharacterObject? searchCurrent = null)
        {
            if (searchStart == null)
            {
                roots.Add(character); // add self

                foreach (CharacterObject rootCharacter in CharacterObject.All)
                {
                    if (!Settings.GetCharacterIsValid(rootCharacter)) continue;

                    FindAllCharacterRoots(character, roots, rootCharacter, rootCharacter);
                }

                return;
            }

            if (searchCurrent == null) return;

            foreach (var upgrade in searchCurrent.UpgradeTargets)
            {
                if (upgrade == character)
                {
                    roots.Add(searchStart);
                    return;
                }

                FindAllCharacterRoots(character, roots, searchStart, upgrade);
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
    }
}
