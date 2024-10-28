using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
namespace childrenGrowFaster;

public class SubModuleSettings : AttributeGlobalSettings<SubModuleSettings>
{
    [SettingPropertyFloatingInteger("Growth Rate", 0f, 40f, "0.0", Order = 0, RequireRestart = false, HintText = "Adjusts the rate at which all children grow.")]
    [SettingPropertyGroup("Children Growth Rate Settings")]
    public float newGrowthRate { get; set; } = 15f;

    [SettingPropertyBool("Affect player children only", Order = 1, RequireRestart = false, HintText = "Mod only affects growth rate of player children.")]
    [SettingPropertyGroup("Children Growth Rate Settings")]
    public bool affectOnlyPlayerChildren { get; set; } = true;

    [SettingPropertyBool("Instant Growth", Order = 2, RequireRestart = false, HintText = "Children will grow to adulthood instantly.")]
    [SettingPropertyGroup("Children Growth Rate Settings")]
    public bool instantGrowth { get; set; } = false;

    [SettingPropertyInteger("Adjust Pregnancy Duration", minValue: 1, maxValue: 36, Order = 3, RequireRestart = false, HintText = "Children will be born instantly.")]
    [SettingPropertyGroup("Children Growth Rate Settings")]
    public int AdjsutPregnancyDuration { get; set; } = 36;


    public override string Id => "childrenGrowFaster";
    public override string DisplayName => "Children Grow Faster";
    public override string FolderName => "childrenGrowFaster";
    public override string FormatType => "xml";
}
