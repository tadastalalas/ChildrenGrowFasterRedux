using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace ChildrenGrowFasterRedux
{
    public class SubModuleSettings : AttributeGlobalSettings<SubModuleSettings>
    {
        public override string Id => "ChildrenGrowFasterRedux";
        public override string DisplayName => "Children Grow Faster Redux";
        public override string FolderName => "ChildrenGrowFasterRedux";
        public override string FormatType => "json2";

        [SettingPropertyFloatingInteger("Growth Rate", 0f, 100f, "0.0", Order = 0, RequireRestart = false, HintText = "Adjusts how many days to add to normal growth day. If you set it to 1 it means children will get older twice as fast. [Default: 1]")]
        [SettingPropertyGroup("Children Growth Rate Settings")]
        public float AdditionalDaysPerDay { get; set; } = 1f;

        [SettingPropertyInteger("When Hero Comes of Age?", 10, 18, Order = 1, RequireRestart = false, HintText = "Let this mod know when children will come of age. Why? Because if you use 'Bannerlord Expanded - Children Expanded' mod, there is a setting 'Hero Comes of Age', make this setting same as that one. [Default: 18]")]
        [SettingPropertyGroup("Children Growth Rate Settings")]
        public int WhenHeroComesOfAge { get; set; } = 18;

        [SettingPropertyBool("Affect Player Children Only", Order = 2, RequireRestart = false, HintText = "Mod only affects growth rate of player children. [Default: false]")]
        [SettingPropertyGroup("Children Growth Rate Settings")]
        public bool AffectOnlyPlayerChildren { get; set; } = false;

        [SettingPropertyBool("Instant Children Growth", Order = 3, RequireRestart = false, HintText = "Children will grow to adulthood instantly. [Default: false]")]
        [SettingPropertyGroup("Children Growth Rate Settings")]
        public bool DoChildGrowToAdultInstantly { get; set; } = false;

        [SettingPropertyBool("Affect Everyone", Order = 4, RequireRestart = false, HintText = "Growth rate affects all children & adults. [Default: false]")]
        [SettingPropertyGroup("Children Growth Rate Settings")]
        public bool AffectEveryone { get; set; } = false;


        [SettingPropertyBool("Random Traits For Player Children", Order = 0, HintText = "Enables random traits for player children. [Default: false]")]
        [SettingPropertyGroup("Random Traits")]
        public bool RandomTraitsForPlayerChildren { get; set; } = false;

        [SettingPropertyInteger("Trait Chance", 0, 100, Order = 1, RequireRestart = false, HintText = "Chance of children gaining a random trait (in percent). [Default: 5]")]
        [SettingPropertyGroup("Random Traits")]
        public int RandomTraitChance { get; set; } = 5;

        [SettingPropertyInteger("Days Between Next Trait Can Be Added", 0, 100, Order = 2, RequireRestart = false, HintText = "How many days must pass since the last trait was added before attempting to add the next trait? Keep in mind that for the next trait to be added, the 'Trait Chance' value must be met. [Default: 10]")]
        [SettingPropertyGroup("Random Traits")]
        public int DaysBetweenNextTraitCanBeAdded { get; set; } = 10;


        [SettingPropertyBool("Enable Adjustable Pregnancy Duration", Order = 0, RequireRestart = false, HintText = "Must be enabled for the slider to work. This feature will enable Harmony Postfix patch so please check with your other mods that adjusts pregnancy duration before enabling it. [Default: false]")]
        [SettingPropertyGroup("Pregnancy Duration (Harmony Patch)")]
        public bool enablePregnancyDuration { get; set; } = false;

        [SettingPropertyInteger("Adjust Pregnancy Duration", minValue: 1, maxValue: 100, Order = 1, RequireRestart = false, HintText = "Adjust the number of days it takes for children to be born. [Default: 36]")]
        [SettingPropertyGroup("Pregnancy Duration (Harmony Patch)")]
        public int AdjsutPregnancyDuration { get; set; } = 36;

        
    }
}