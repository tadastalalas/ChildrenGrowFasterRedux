using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Localization;

namespace ChildrenGrowFasterRedux
{
    public class SubModuleSettings : AttributeGlobalSettings<SubModuleSettings>
    {
        public override string Id => "ChildrenGrowFasterRedux";
        public override string DisplayName => new TextObject("{=CGFR_YYayWvdK}Children Grow Faster Redux").ToString();
        public override string FolderName => "ChildrenGrowFasterRedux";
        public override string FormatType => "json2";

        [SettingPropertyFloatingInteger("{=CGFR_xpz8K0H5}Growth Rate", 0.1f, 100.0f, "0.0", Order = 0, RequireRestart = false, HintText = "{=CGFR_1ZWhgRlk}Children growth rate. 1 = normal rate; 2 = twice as fast; 3 = three times as fast growth rate and so on. Values below 1 slow growth down. [Default: 2]")]
        [SettingPropertyGroup("{=CGFR_5GjemDpY}Children Growth Rate Settings", GroupOrder = 0)]
        public float GrowthRate { get; set; } = 2;

        [SettingPropertyBool("{=CGFR_xJ1yLMuy}Affect Player Clan Children Only", Order = 1, RequireRestart = false, HintText = "{=CGFR_IOUT6bW1}Mod only affects growth rate of player clan children. If Affect Player Children Only below option is also enabled, then this option will not work. Ignored when Affect Everyone is enabled. [Default: false]")]
        [SettingPropertyGroup("{=CGFR_5GjemDpY}Children Growth Rate Settings", GroupOrder = 0)]
        public bool AffectOnlyPlayerClanChildren { get; set; } = false;

        [SettingPropertyBool("{=CGFR_88NRzszQ}Affect Player Children Only", Order = 2, RequireRestart = false, HintText = "{=CGFR_s9e0XtHk}Mod only affects growth rate of player children. Ignored when Affect Everyone is enabled. [Default: false]")]
        [SettingPropertyGroup("{=CGFR_5GjemDpY}Children Growth Rate Settings", GroupOrder = 0)]
        public bool AffectOnlyPlayerChildren { get; set; } = false;

        [SettingPropertyBool("{=CGFR_MYAPs8YR}Instant Children Growth", Order = 3, RequireRestart = false, HintText = "{=CGFR_vMl3gSRM}Children will grow to adulthood instantly. [Default: false]")]
        [SettingPropertyGroup("{=CGFR_5GjemDpY}Children Growth Rate Settings", GroupOrder = 0)]
        public bool DoChildGrowToAdultInstantly { get; set; } = false;

        [SettingPropertyBool("{=CGFR_fdWNNbDe}Affect Everyone", Order = 4, RequireRestart = false, HintText = "{=CGFR_XFONu6ZR}Growth rate affects ALL children AND all adults (overrides the Player/Clan-only toggles for children). The main hero is excluded from adult aging to avoid premature death. [Default: false]")]
        [SettingPropertyGroup("{=CGFR_5GjemDpY}Children Growth Rate Settings", GroupOrder = 0)]
        public bool AffectEveryone { get; set; } = false;

        [SettingPropertyBool("{=CGFR_kYpzWk1A}Exclude Main Hero From Adult Aging", Order = 5, RequireRestart = false, HintText = "{=CGFR_qJk7L9rN}When Affect Everyone is enabled, exclude the main hero from accelerated aging. [Default: false]")]
        [SettingPropertyGroup("{=CGFR_5GjemDpY}Children Growth Rate Settings", GroupOrder = 0)]
        public bool ExcludeMainHeroFromAdultAging { get; set; } = false;


        [SettingPropertyBool("{=CGFR_daNt7nlf}Enable Adjustable Pregnancy Duration", Order = 0, RequireRestart = false, HintText = "{=CGFR_WcwRhJm0}Must be enabled for the below slider to work. This feature applies a Harmony Postfix patch to DefaultPregnancyModel.PregnancyDurationInDays so please check with your other mods that adjust pregnancy duration before enabling it. [Default: false]")]
        [SettingPropertyGroup("{=CGFR_vF2aVt8N}Pregnancy Duration (Harmony Patch)", GroupOrder = 2)]
        public bool EnablePregnancyDuration { get; set; } = false;

        [SettingPropertyInteger("{=CGFR_yvgrrQcd}Adjust Pregnancy Duration", minValue: 1, maxValue: 100, Order = 1, RequireRestart = false, HintText = "{=CGFR_DlU01AjN}Adjust the number of days it takes for children to be born. [Default: 36]")]
        [SettingPropertyGroup("{=CGFR_vF2aVt8N}Pregnancy Duration (Harmony Patch)", GroupOrder = 2)]
        public int AdjustPregnancyDuration { get; set; } = 36;


        [SettingPropertyBool("Logging for debugging", Order = 0, RequireRestart = false, HintText = "Logging for debugging. [Default: disabled]")]
        [SettingPropertyGroup("Technical settings", GroupOrder = 3)]
        public bool LoggingEnabled { get; set; } = false;
    }
}