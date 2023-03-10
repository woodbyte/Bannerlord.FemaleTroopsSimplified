using MCM.Abstractions.Base.Global;
using MCM.Abstractions.FluentBuilder;
using MCM.Common;

namespace Bannerlord.FemaleTroopsSimplified.Configuration
{
    internal class GlobalSettings
    {
        ISettingsBuilder? _builder;
        FluentGlobalSettings? _settings;

        public int CustomCoverage { get; set; } = 50;
        public bool EnableMilitiaConfig { get; set; } = false;
        public bool EnableCaravanConfig { get; set; } = false;
        public bool EnableGuardConfig { get; set; } = false;
        public bool EnableHiddenConfig { get; set; } = false;

        public static GlobalSettings? Instance { get; private set; }
        public static void Initialize() => Instance = new();

        public GlobalSettings()
        {
            _builder = BaseSettingsBuilder.Create("FemaleTroopsSimplifiedGlobals", $"Female Troops Simplified {typeof(CampaignSettings).Assembly.GetName().Version.ToString(3)}");

            if (_builder == null) return;

            // General settings
            _builder.SetFolderName("FemaleTroopsSimplified");
            _builder.SetFormat("json");

            _builder.CreateGroup("Custom Battle", groupBuider => groupBuider
                .SetGroupOrder(0)
                .AddInteger("CustomCoverage", "Custom Battle Female Percentage", 0, 100, new ProxyRef<int>(() => CustomCoverage, x => CustomCoverage = x), integerBuilder => integerBuilder
                    .SetOrder(0)));

            _builder.CreateGroup("Campaign and Sandbox", groupBuilder => groupBuilder
                .SetGroupOrder(1)
                .AddBool("EnableMilitiaConfig", "Enable Militia Customization", new ProxyRef<bool>(() => EnableMilitiaConfig, x => EnableMilitiaConfig = x), boolBuilder => boolBuilder
                    .SetOrder(0))
                .AddBool("EnableCaravanConfig", "Enable Caravan Customization", new ProxyRef<bool>(() => EnableCaravanConfig, x => EnableCaravanConfig = x), boolBuilder => boolBuilder
                    .SetOrder(1))
                .AddBool("EnableGuardConfig", "Enable Guard Customization", new ProxyRef<bool>(() => EnableGuardConfig, x => EnableGuardConfig = x), boolBuilder => boolBuilder
                    .SetOrder(2))
                .AddBool("EnableHiddenConfig", "Enable Hidden Troops Customization", new ProxyRef<bool>(() => EnableHiddenConfig, x => EnableHiddenConfig = x), boolBuilder => boolBuilder
                    .SetOrder(3)
                    .SetHintText("Customize troops not shown in encyclopedia.")));

            _builder.CreatePreset("default", "Default", presetBuilder => presetBuilder
                .SetPropertyValue("CustomCoverage", CustomCoverage)
                .SetPropertyValue("EnableMilitiaConfig", EnableMilitiaConfig)
                .SetPropertyValue("EnableCaravanConfig", EnableCaravanConfig)
                .SetPropertyValue("EnableGuardConfig", EnableGuardConfig)
                .SetPropertyValue("EnableHiddenConfig", EnableHiddenConfig));

            _settings = _builder?.BuildAsGlobal();
            _settings?.Register();
        }

        public void Unregister()
        {
            _settings?.Unregister();
        }
    }
}
