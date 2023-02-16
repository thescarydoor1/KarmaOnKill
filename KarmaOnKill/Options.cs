using Menu.Remix.MixedUI;
using UnityEngine;

namespace KarmaOnKill
{
    sealed class Options : OptionInterface
    {
        public static Configurable<bool> UseHat;
        public static Configurable<bool> EnableDebugLogs;
        public static Configurable<int> MaxBonusKarma;
        public static Configurable<int> ScavengerKillsPerKarma;

        public Options()
        {
            UseHat = config.Bind("cfgUseHat", false);
            EnableDebugLogs = config.Bind("cfgEnableDebugLogs", false);
            MaxBonusKarma = config.Bind("cfgMaxBonusKarma", 4, new ConfigAcceptableRange<int>(1, 9));
            ScavengerKillsPerKarma = config.Bind("cfgScavengerKillsPerKarma", 1, new ConfigAcceptableRange<int>(1, 10));
        }

        public override void Initialize()
        {
            base.Initialize();

            Tabs = new OpTab[] { new OpTab(this) };

            var titleLabel = new OpLabel(20, 600 - 30, "KarmaOnKill Settings", true);

            var useHatLabel = new OpLabel(70, 600 - 60, "Use Hat");
            var useHatCheckBox = new OpCheckBox(UseHat, 40, 600 - 63);

            var debugLogLabel = new OpLabel(70, 600 - 93, "Enable Debug Logs");
            var debugLogCheckBox = new OpCheckBox(EnableDebugLogs, 40, 600 - 96);

            var maxKarmaLabel = new OpLabel(40, 600 - 123, "Maximum Bonus Karma");
            var maxKarmaSlider = new OpSlider(MaxBonusKarma, new Vector2(50, 600 - 156), 320);

            var scavengerKillsPerKarmaLabel = new OpLabel(40, 600 - 180, "Scavenger kills needed for bonus karma");
            var scavengerKillsPerKarmaSlider = new OpSlider(ScavengerKillsPerKarma, new Vector2(50, 600 - 213), 320);

            Tabs[0].AddItems(
                titleLabel,
                useHatLabel,
                useHatCheckBox,
                debugLogLabel, 
                debugLogCheckBox,
                maxKarmaLabel,
                maxKarmaSlider,
                scavengerKillsPerKarmaLabel,
                scavengerKillsPerKarmaSlider);
        }
    }
}
