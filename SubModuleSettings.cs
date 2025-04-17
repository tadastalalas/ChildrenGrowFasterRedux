using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace ChildrenGrowFaster
{
    public class SubModuleSettings : AttributeGlobalSettings<SubModuleSettings>
    {
        [SettingPropertyFloatingInteger("Growth Rate", 0f, 100f, "0.0", Order = 0, RequireRestart = false, HintText = "Adjusts how many days to add to normal growth day. If you set it to 1 it means children will get older twice as fast. [Default: 1]")]
        [SettingPropertyGroup("Children Growth Rate Settings")]
        public float additionalDaysPerDay { get; set; } = 1f;

        [SettingPropertyInteger("When Hero Comes of Age?", 10, 18, Order = 1, RequireRestart = false, HintText = "Let this mod know when children will come of age. Why? Because if you use 'Bannerlord Expanded - Children Expanded' mod, there is a setting 'Hero Comes of Age', make this setting same as that one. [Default: 18]")]
        [SettingPropertyGroup("Children Growth Rate Settings")]
        public int whenHeroComesOfAge { get; set; } = 18;

        [SettingPropertyBool("Affect player children only", Order = 2, RequireRestart = false, HintText = "Mod only affects growth rate of player children. [Default: false]")]
        [SettingPropertyGroup("Children Growth Rate Settings")]
        public bool affectOnlyPlayerChildren { get; set; } = false;

        [SettingPropertyBool("Instant Children Growth", Order = 3, RequireRestart = false, HintText = "Children will grow to adulthood instantly. [Default: false]")]
        [SettingPropertyGroup("Children Growth Rate Settings")]
        public bool DoChildGrowToAdultInstantly { get; set; } = false;

        [SettingPropertyBool("Affect Everyone", Order = 4, RequireRestart = false, HintText = "Growth rate affects all children & adults. [Default: false]")]
        [SettingPropertyGroup("Children Growth Rate Settings")]
        public bool affectEveryone { get; set; } = false;

        [SettingPropertyBool("Enable Spouse Events", Order = 1, RequireRestart = false, HintText = "Enables spouse events feature. [Default: false]")]
        [SettingPropertyGroup("Spouse Events")]
        public bool spouseEventsEnabled { get; set; } = false;

        [SettingPropertyInteger("Event Chance", 1, 10, Order = 2, RequireRestart = false, HintText = "The chance of the spouse events happening. [Default: 1]")]
        [SettingPropertyGroup("Spouse Events")]
        public int eventChance { get; set; } = 1;

        [SettingPropertyBool("Random Traits for Children", Order = 1, HintText = "Enables random traits for children. [Default: false]")]
        [SettingPropertyGroup("Random Traits")]
        public bool randomTraitsEnabled { get; set; } = false;

        [SettingPropertyFloatingInteger("Trait Chance", 0.05f, 1f, "0.0", Order = 2, RequireRestart = false, HintText = "Chance of children gaining a random trait. [Default: 0.05]")]
        [SettingPropertyGroup("Random Traits")]
        public float traitChance { get; set; } = 0.05f;

        [SettingPropertyBool("Enable adjustable pregnancy duration", Order = 1, RequireRestart = false, HintText = "Must be enabled for the slider to work. This feature will enable Harmony Postfix patch so please check with your other mods that adjust pregnancy duration before enabling. [Default: false]")]
        [SettingPropertyGroup("Pregnancy Duration (Harmony Patch)")]
        public bool enablePregnancyDuration { get; set; } = false;

        [SettingPropertyInteger("Adjust Pregnancy Duration", minValue: 1, maxValue: 100, Order = 2, RequireRestart = false, HintText = "Adjust the number of days it takes for children to be born. [Default: 36]")]
        [SettingPropertyGroup("Pregnancy Duration (Harmony Patch)")]
        public int AdjsutPregnancyDuration { get; set; } = 36;

        public override string Id => "childrenGrowFaster";
        public override string DisplayName => "Children Grow Faster";
        public override string FolderName => "childrenGrowFaster";
        public override string FormatType => "xml";
    }
}