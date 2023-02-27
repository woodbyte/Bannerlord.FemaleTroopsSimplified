using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.PerCampaign;

namespace Bannerlord.FemaleTroopsSimplified
{
    internal sealed class Settings : AttributePerCampaignSettings<Settings>
    {
        public override string Id => "FemaleTroopsSimplified";
        public override string FolderName => "FemaleTroopsSimplified";
        public override string DisplayName => $"Female Troops Simplified {typeof(Settings).Assembly.GetName().Version.ToString(3)}";

        [SettingPropertyInteger("Default Female Troop Percentage", 0, 100, RequireRestart = false, Order = 1)]
        [SettingPropertyGroup("General", GroupOrder = 1)]
        public int DefaultCoverage { get; set; } = 50;

        [SettingPropertyBool("Use Gender Neutral Troop Names", RequireRestart = false, Order = 2, HintText = "Only for the english language.")]
        [SettingPropertyGroup("General", GroupOrder = 1)]
        public bool UseGenderNeutral { get; set; } = true;

        [SettingPropertyBool("Randomize Troop Gender in the Encyclopedia", RequireRestart = false, Order = 3)]
        [SettingPropertyGroup("General", GroupOrder = 1)]
        public bool RandomizeEncyclopedia { get; set; } = true;

        [SettingPropertyBool("Skolderbroda Toggle", RequireRestart = false, Order = 1, IsToggle = true)]
        [SettingPropertyGroup("Skolderbroda Overrides", GroupOrder = 2)]
        public bool SkolderbrodaToggle { get; set; } = true;

        [SettingPropertyInteger("Skolderbroda Female Troop Percentage", 0, 100, RequireRestart = false, Order = 2)]
        [SettingPropertyGroup("Skolderbroda Overrides", GroupOrder = 2)]
        public int SkolderbrodaCoverage { get; set; } = 0;

        [SettingPropertyBool("Ghilman Toggle", RequireRestart = false, Order = 1, IsToggle = true)]
        [SettingPropertyGroup("Ghilman Overrides", GroupOrder = 3)]
        public bool GhilmanToggle { get; set; } = true;

        [SettingPropertyInteger("Ghilman Female Troop Percentage", 0, 100, RequireRestart = false, Order = 2)]
        [SettingPropertyGroup("Ghilman Overrides", GroupOrder = 3)]
        public int GhilmanCoverage { get; set; } = 0;

        [SettingPropertyBool("Forest People Toggle", RequireRestart = false, Order = 1, IsToggle = true)]
        [SettingPropertyGroup("Forest People Overrides", GroupOrder = 4)]
        public bool ForestToggle { get; set; } = false;

        [SettingPropertyInteger("Forest People Female Troop Percentage", 0, 100, RequireRestart = false, Order = 2)]
        [SettingPropertyGroup("Forest People Overrides", GroupOrder = 4)]
        public int ForestCoverage { get; set; } = 50;

        [SettingPropertyBool("Aserai Toggle", RequireRestart = false, Order = 1, IsToggle = true)]
        [SettingPropertyGroup("Aserai Overrides", GroupOrder = 5)]
        public bool AseraiToggle { get; set; } = false;

        [SettingPropertyInteger("Aserai Female Troop Percentage", 0, 100, RequireRestart = false, Order = 2)]
        [SettingPropertyGroup("Aserai Overrides", GroupOrder = 5)]
        public int AseraiCoverage { get; set; } = 50;

        [SettingPropertyBool("Battania Toggle", RequireRestart = false, Order = 1, IsToggle = true)]
        [SettingPropertyGroup("Battania Overrides", GroupOrder = 6)]
        public bool BattaniaToggle { get; set; } = false;

        [SettingPropertyInteger("Battania Female Troop Percentage", 0, 100, RequireRestart = false, Order = 2)]
        [SettingPropertyGroup("Battania Overrides", GroupOrder = 6)]
        public int BattaniaCoverage { get; set; } = 50;

        [SettingPropertyBool("Empire Toggle", RequireRestart = false, Order = 1, IsToggle = true)]
        [SettingPropertyGroup("Empire Overrides", GroupOrder = 7)]
        public bool EmpireToggle { get; set; } = false;

        [SettingPropertyInteger("Empire Female Troop Percentage", 0, 100, RequireRestart = false, Order = 2)]
        [SettingPropertyGroup("Empire Overrides", GroupOrder = 7)]
        public int EmpireCoverage { get; set; } = 50;

        [SettingPropertyBool("Khuzait Toggle", RequireRestart = false, Order = 1, IsToggle = true)]
        [SettingPropertyGroup("Khuzait Overrides", GroupOrder = 8)]
        public bool KhuzaitToggle { get; set; } = false;

        [SettingPropertyInteger("Khuzait Female Troop Percentage", 0, 100, RequireRestart = false, Order = 2)]
        [SettingPropertyGroup("Khuzait Overrides", GroupOrder = 8)]
        public int KhuzaitCoverage { get; set; } = 50;

        [SettingPropertyBool("Sturgia Toggle", RequireRestart = false, Order = 1, IsToggle = true)]
        [SettingPropertyGroup("Sturgia Overrides", GroupOrder = 9)]
        public bool SturgiaToggle { get; set; } = false;

        [SettingPropertyInteger("Sturgia Female Troop Percentage", 0, 100, RequireRestart = false, Order = 2)]
        [SettingPropertyGroup("Sturgia Overrides", GroupOrder = 9)]
        public int SturgiaCoverage { get; set; } = 50;

        [SettingPropertyBool("Vlandia Toggle", RequireRestart = false, Order = 1, IsToggle = true)]
        [SettingPropertyGroup("Vlandia Overrides", GroupOrder = 10)]
        public bool VlandiaToggle { get; set; } = false;

        [SettingPropertyInteger("Vlandia Female Troop Percentage", 0, 100, RequireRestart = false, Order = 2)]
        [SettingPropertyGroup("Vlandia Overrides", GroupOrder = 10)]
        public int VlandiaCoverage { get; set; } = 50;
    }
}