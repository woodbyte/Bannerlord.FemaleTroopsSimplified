using MCM.Abstractions.FluentBuilder;
using MCM.Common;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.FemaleTroopsSimplified.Configuration
{
    internal class CampaignSettings
    {
        public static bool BaseTreesArePresent { get; private set; } = false;
        public static CampaignSettings? Instance { get; private set; }

        OverrideManager? _overrideManager;
        ISettingsBuilder? _builder;

        public int DefaultCoverage { get; set; } = 50;
        public bool UseGenderNeutral { get; set; } = true;
        public bool RandomizeEncyclopedia { get; set; } = true;

        public static bool GetCharacterIsConfigurable(CharacterObject character)
        {
            if (!GetCharacterIsValid(character)) return false;

            var culture = character.Culture;
            if (culture == null) return false;

            if (GlobalSettings.Instance == null) return false;

            if (!GlobalSettings.Instance.EnableHiddenConfig && character.HiddenInEncylopedia) return false;

            if (!GlobalSettings.Instance.EnableCaravanConfig)
            {
                if (culture.CaravanGuard == character) return false;
                if (culture.CaravanMaster == character) return false;
                if (culture.CaravanPartyTemplate != null &&
                    culture.CaravanPartyTemplate.Stacks != null &&
                    culture.CaravanPartyTemplate.Stacks.Any((x) => x.Character == character)) return false;
                if (culture.EliteCaravanPartyTemplate != null &&
                    culture.EliteCaravanPartyTemplate.Stacks != null &&
                    culture.EliteCaravanPartyTemplate.Stacks.Any((x) => x.Character == character)) return false;
            }

            if (!GlobalSettings.Instance.EnableMilitiaConfig)
            {
                if (culture.RangedMilitiaTroop == character) return false;
                if (culture.MeleeMilitiaTroop == character) return false;
                if (culture.RangedEliteMilitiaTroop == character) return false;
                if (culture.MeleeEliteMilitiaTroop == character) return false;
                if (culture.MilitiaArcher == character) return false;
                if (culture.MilitiaSpearman == character) return false;
                if (culture.MilitiaVeteranArcher == character) return false;
                if (culture.MilitiaVeteranSpearman == character) return false;
                if (culture.MilitiaPartyTemplate != null &&
                    culture.MilitiaPartyTemplate.Stacks != null &&
                    culture.MilitiaPartyTemplate.Stacks.Any((x) => x.Character == character)) return false;
            }

            if (!GlobalSettings.Instance.EnableGuardConfig)
            {
                if (culture.Guard == character) return false;
                if (culture.PrisonGuard == character) return false;
            }

            return true;
        }

        public static bool GetCharacterIsValid(CharacterObject character, bool includeObsolete = false)
        {
            if (character.IsObsolete && !includeObsolete) return false;
            if (character.IsHero) return false;
            if (character.IsFemale) return false;

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
                    return false;
            }

            // ignore spy quest character ids
            if (character.StringId.Contains("bold_contender")) return false;
            if (character.StringId.Contains("confident_contender")) return false;
            if (character.StringId.Contains("dignified_contender")) return false;
            if (character.StringId.Contains("hardy_contender")) return false;

            // ignore tutorial character ids
            if (character.StringId.Contains("tutorial_npc")) return false;

            return true;
        }

        public int GetCharacterCoverage(CharacterObject character)
        {
            if (_overrideManager == null) return 0;

            return _overrideManager.GetCharacterCoverage(character);
        }

        public static void Initialize()
        {
            var nordUnit = CharacterObject.All.FirstOrDefault((x) => x.StringId == "skolderbrotva_tier_1");
            var darshiUnit = CharacterObject.All.FirstOrDefault((x) => x.StringId == "ghilman_tier_1");

            if (nordUnit != null && darshiUnit != null)
            {
                if (nordUnit.Culture.StringId == "nord" && darshiUnit.Culture.StringId == "darshi" &&
                    !nordUnit.HiddenInEncylopedia && !darshiUnit.HiddenInEncylopedia)
                    BaseTreesArePresent = true;
            }

            Instance = new CampaignSettings();
        }

        public CampaignSettings()
        {
            _overrideManager = new OverrideManager();

            _builder = BaseSettingsBuilder.Create("FemaleTroopsSimplified", $"Female Troops Simplified {typeof(CampaignSettings).Assembly.GetName().Version.ToString(3)}");

            if (_builder == null) return;

            // General settings
            _builder.SetFolderName("FemaleTroopsSimplified");
            _builder.CreateGroup("Global", groupBuider => groupBuider
                .SetGroupOrder(0)
                .AddInteger("DefaultCoverage", "Default Female Troop Percentage", 0, 100, new ProxyRef<int>(() => DefaultCoverage, x => DefaultCoverage = x), integerBuilder => integerBuilder
                    .SetOrder(0))
                .AddBool("UseGenderNeutral", "Use Gender Neutral Troop Names", new ProxyRef<bool>(() => UseGenderNeutral, x => UseGenderNeutral = x), boolBuilder => boolBuilder
                    .SetOrder(1)
                    .SetHintText("Only works when language is set to English."))
                .AddBool("RandomizeEncyclopedia", "Randomize Troop Gender in the Encyclopedia", new ProxyRef<bool>(() => RandomizeEncyclopedia, x => RandomizeEncyclopedia = x), boolBuilder => boolBuilder
                    .SetOrder(2)));

            // Culture settings
            _overrideManager.CreateGroups(_builder, 1);

            _builder.CreatePreset("default", "Default", presetBuilder =>
            {
                // General preset values
                presetBuilder
                    .SetPropertyValue("DefaultCoverage", DefaultCoverage)
                    .SetPropertyValue("UseGenderNeutral", UseGenderNeutral)
                    .SetPropertyValue("RandomizeEncyclopedia", RandomizeEncyclopedia);

                // Culture preset values
                _overrideManager.CreatePresets(presetBuilder, "default");
            });

            var perCampaignSettings = _builder?.BuildAsPerCampaign();
            perCampaignSettings?.Register();
        }
    }
}