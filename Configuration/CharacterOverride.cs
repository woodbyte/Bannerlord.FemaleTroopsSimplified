using MCM.Abstractions.FluentBuilder;
using MCM.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace Bannerlord.FemaleTroopsSimplified.Configuration
{
    internal class CharacterOverride : IComparable<CharacterOverride>
    {
        CultureOverride _parent;
        CharacterObject _character;

        public CharacterObject Character => _character;
        public string SettingId { get; private set; }

        public bool Enabled { get; set; } = false;
        public int Coverage { get; set; } = 50;

        public CharacterOverride(CharacterObject character, CultureOverride parent)
        {
            _character = character;
            _parent = parent;

            var lowerId = _character.StringId.Replace("_", " ");
            var upperId = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(lowerId);
            SettingId = upperId.Replace(" ", String.Empty);

            if (!CampaignSettings.BaseTreesArePresent) return;

            if (SettingId == "SkolderbrotvaTier1" || SettingId == "GhilmanTier1")
            {
                Enabled = true;
                Coverage = 0;
            }
        }

        public int GetFinalCoverage()
        {
            if (CampaignSettings.Instance == null) return 0;

            int coverage = CampaignSettings.Instance.DefaultCoverage;

            if (_parent.Enabled)
            {
                if (Enabled)
                    coverage = Coverage;
                else if (_parent.CoverageEnabled)
                    coverage = _parent.CoverageValue;
            }

            return coverage;
        }

        public void CreateGroup(ISettingsBuilder builder, int order = 0, bool includeId = false)
        {
            string suffix = "";
            if (includeId) suffix = $" \u2039{_character.StringId}\u203A";

            builder.CreateGroup($"{_parent}/{_parent.groupNameCustomizeTroops}/{ToString()}{suffix}", groupBuider => groupBuider
                .SetGroupOrder(order)
                .AddToggle($"{SettingId}Enabled", "{=FTSOpt004}Toggle", new ProxyRef<bool>(() => Enabled, x => Enabled = x), boolBuilder => { })
                .AddInteger($"{SettingId}Coverage", new TextObject("{=FTSOpt010}{NAME} Female Percentage").SetTextVariable("NAME", ToString()).ToString(), 0, 100, new ProxyRef<int>(() => Coverage, x => Coverage = x), integerBuilder => integerBuilder
                    .SetOrder(0))
                );
        }

        public void CreatePreset(ISettingsPresetBuilder builder, string presetName)
        {
            switch (presetName)
            {
                case "default":
                    builder.SetPropertyValue($"{SettingId}Enabled", Enabled);
                    builder.SetPropertyValue($"{SettingId}Coverage", Coverage);
                    break;
            }
        }

        public override string ToString() => _character.Name.ToString();
        public override int GetHashCode() => _character.GetHashCode();
        public override bool Equals(object obj) => _character.StringId == ((CharacterOverride)obj)._character.StringId;
        public int CompareTo(CharacterOverride other)
        {
            if (_character.IsBasicTroop && !other._character.IsBasicTroop) return -1;
            else if (!_character.IsBasicTroop && other._character.IsBasicTroop) return 1;
            else if (_character.IsSoldier && !other._character.IsSoldier) return -1;
            else if (!_character.IsSoldier && other._character.IsSoldier) return 1;
            else if (!_character.IsRegular && other._character.IsRegular) return -1;
            else if (_character.IsRegular && !other._character.IsRegular) return 1;
            else return ToString().CompareTo(other.ToString());
        }
    }
}