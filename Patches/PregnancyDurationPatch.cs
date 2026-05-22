using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem.GameComponents;

namespace ChildrenGrowFasterRedux.Patches
{
    [HarmonyPatch(typeof(DefaultPregnancyModel), nameof(DefaultPregnancyModel.PregnancyDurationInDays), MethodType.Getter)]
    internal static class PregnancyDurationPatch
    {
        private static void Postfix(ref float __result)
        {
            SubModuleSettings? settings = AttributeGlobalSettings<SubModuleSettings>.Instance;
            if (settings != null && settings.EnablePregnancyDuration)
            {
                __result = settings.AdjustPregnancyDuration;
            }
        }
    }
}