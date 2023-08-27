﻿using MCM.Abstractions.FluentBuilder;
using MCM.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace Bannerlord.FemaleTroopsSimplified.Configuration
{
    internal class CultureOverride : IComparable<CultureOverride>
    {
        CultureObject _culture;
        SortedSet<CharacterOverride> _troopOverrides = new();

        public CultureObject Culture => _culture;
        public SortedSet<CharacterOverride> TroopOverrides => _troopOverrides;
        public string SettingId { get; private set; }

        public bool Enabled { get; set; } = false;
        public bool CoverageEnabled { get; set; } = false;
        public int CoverageValue { get; set; } = 50;
        public bool TroopsEnabled { get; set; } = false;

        public CultureOverride(CultureObject culture)
        {
            _culture = culture;

            var lowerId = _culture.StringId.Replace("_", " ");
            var upperId = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(lowerId);
            SettingId = upperId.Replace(" ", String.Empty);

            if (!CampaignSettings.BaseTreesArePresent) return;

            if (SettingId == "Nord" || SettingId == "Darshi")
            {
                Enabled = true;
                TroopsEnabled = true;
            }
        }

        public CharacterOverride AddTroopOverride(CharacterObject troop)
        {
            if (!_troopOverrides.TryGetValue(new CharacterOverride(troop, this), out var troopOverride))
            {
                troopOverride = new CharacterOverride(troop, this);
                _troopOverrides.Add(troopOverride);
            }

            return troopOverride;
        }

        public void CreateGroup(ISettingsBuilder builder, int order = 0)
        {
            Random a = new();
            TextObject groupName = new TextObject("{=FTSOptG004}{NAME}").SetTextVariable("NAME", _culture.Name);

            builder.CreateGroup(groupName.ToString(), groupBuider => groupBuider
                .SetGroupOrder(order)
                .AddToggle($"{SettingId}Enabled", "{=FTSOpt004}Toggle", new ProxyRef<bool>(() => Enabled, x => Enabled = x), boolBuilder => { })
                );

            builder.CreateGroup($"{ToString()}/{this.groupNamePercentageOverride}", groupBuider => groupBuider
                .SetGroupOrder(0)
                .AddToggle($"{SettingId}CoverageEnabled", "{=FTSOpt004}Toggle", new ProxyRef<bool>(() => CoverageEnabled, x => CoverageEnabled = x), boolBuilder => { })
                .AddInteger($"{SettingId}CoverageValue", new TextObject("{=FTSOpt011}{NAME} Female Troop Percentage").SetTextVariable("NAME", ToString()).ToString(), 0, 100, new ProxyRef<int>(() => CoverageValue, x => CoverageValue = x), integerBuilder => integerBuilder
                    .SetOrder(0))
                );

            builder.CreateGroup($"{ToString()}/{this.groupNameCustomizeTroops}", groupBuider => groupBuider
                .SetGroupOrder(1)
                .AddToggle($"{SettingId}TroopsEnabled", "{=FTSOpt004}Toggle", new ProxyRef<bool>(() => TroopsEnabled, x => TroopsEnabled = x), boolBuilder => { })
                );

            var troopNames = _troopOverrides.ToList().ConvertAll((x) => x.ToString());
            var duplicates = troopNames.GroupBy(x => x).Where(g => g.Count() > 1).Select(x => x.Key);

            int troopOrder = 0;
            foreach (var troopOverride in _troopOverrides)
            {
                troopOverride.CreateGroup(builder, troopOrder, duplicates.Contains(troopOverride.ToString()));
                troopOrder++;
            }
        }

        public void CreatePreset(ISettingsPresetBuilder builder, string presetName)
        {
            switch (presetName)
            {
                case "default":
                    builder.SetPropertyValue($"{SettingId}Enabled", Enabled);
                    builder.SetPropertyValue($"{SettingId}CoverageEnabled", CoverageEnabled);
                    builder.SetPropertyValue($"{SettingId}CoverageValue", CoverageValue);
                    builder.SetPropertyValue($"{SettingId}TroopsEnabled", TroopsEnabled);
                    break;
            }

            foreach (var troopOverride in _troopOverrides)
                troopOverride.CreatePreset(builder, presetName);
        }

        public override string ToString() => _culture.Name.ToString();
        public override int GetHashCode() => _culture.GetHashCode();
        public override bool Equals(object obj) => _culture.StringId == ((CultureOverride)obj)._culture.StringId;
        public readonly TextObject groupNamePercentageOverride = new("{=FTSOptG005}Enable Percentage Override");
        public readonly TextObject groupNameCustomizeTroops = new("{=FTSOptG006}Customize Troops");
        public int CompareTo(CultureOverride other)
        {
            if (_culture.IsMainCulture && !other._culture.IsMainCulture) return -1;
            else if (!_culture.IsMainCulture && other._culture.IsMainCulture) return 1;
            else if (!_culture.IsBandit && other._culture.IsBandit) return -1;
            else if (_culture.IsBandit && !other._culture.IsBandit) return 1;
            else return ToString().CompareTo(other.ToString());
        }
    }
}