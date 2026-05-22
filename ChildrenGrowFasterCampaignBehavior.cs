using System;
using System.Collections.Generic;
using System.Linq;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace ChildrenGrowFasterRedux
{
    internal class ChildrenGrowFasterCampaignBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents() => CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTickEvent);

        public override void SyncData(IDataStore dataStore) { }

        private readonly SubModuleSettings settings = AttributeGlobalSettings<SubModuleSettings>.Instance ?? new SubModuleSettings();

        private void OnDailyTickEvent()
        {
            LogMessage("OnDailyTickEvent() Called.");

            if (settings.AffectEveryone)
            {
                ApplyGrowthRateToAllChildren();
                ApplyGrowthRateToEveryoneElse();
                return;
            }

            if (settings.AffectOnlyPlayerChildren)
                ApplyGrowthRateToPlayerChildren();
            else if (settings.AffectOnlyPlayerClanChildren)
                ApplyGrowthRateToPlayerClanChildren();
            else
                ApplyGrowthRateToAllChildren();
        }

        private void ApplyGrowthRateToPlayerChildren()
        {
            LogMessage("ApplyGrowthRateToPlayerChildren() Called.");
            int comesOfAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;
            Hero player = Hero.MainHero;
            ApplyGrowthRate(hero => hero.IsChild && hero.Age < comesOfAge && (hero.Father == player || hero.Mother == player));
        }

        private void ApplyGrowthRateToPlayerClanChildren()
        {
            LogMessage("ApplyGrowthRateToPlayerClanChildren() Called.");
            int comesOfAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;
            Clan playerClan = Hero.MainHero.Clan;
            ApplyGrowthRate(hero => hero.IsChild && hero.Age < comesOfAge && hero.Clan == playerClan);
        }

        private void ApplyGrowthRateToAllChildren()
        {
            LogMessage("ApplyGrowthRateToAllChildren() Called.");
            int comesOfAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;
            ApplyGrowthRate(hero => hero.IsChild && hero.Age < comesOfAge);
        }

        private void ApplyGrowthRateToEveryoneElse()
        {
            LogMessage("ApplyGrowthRateToEveryoneElse() Called.");
            int comesOfAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;
            bool excludeMain = settings.ExcludeMainHeroFromAdultAging;
            ApplyGrowthRate(hero => hero.Age >= comesOfAge && !(excludeMain && hero == Hero.MainHero));
        }

        private void ApplyGrowthRate(Func<Hero, bool> heroFilter)
        {
            LogMessage("ApplyGrowthRate() Called.");

            if (settings.DoChildGrowToAdultInstantly)
            {
                ApplyInstantGrowthToChildren(Hero.AllAliveHeroes.Where(heroFilter));
                return;
            }

            float additionalDaysPerDay = settings.GrowthRate - 1f;
            if (additionalDaysPerDay == 0f)
                return;

            foreach (Hero hero in Hero.AllAliveHeroes.Where(heroFilter))
            {
                hero.SetBirthDay(hero.BirthDay - CampaignTime.Days(additionalDaysPerDay));
            }
        }

        private void ApplyInstantGrowthToChildren(IEnumerable<Hero> heroes)
        {
            LogMessage("ApplyInstantGrowthToChildren() Called.");

            int comesOfAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;
            CampaignTime newBday = CampaignTime.Now - CampaignTime.Years(comesOfAge);

            foreach (Hero hero in heroes)
            {
                hero.SetBirthDay(newBday);
                LogMessage($"{hero.Name} has instantly grown into an adult (Age: {hero.Age}).");
            }
        }

        private void LogMessage(string message)
        {
            if (settings.LoggingEnabled)
            {
                InformationManager.DisplayMessage(new InformationMessage(message, Colors.Yellow));
            }
        }
    }
}