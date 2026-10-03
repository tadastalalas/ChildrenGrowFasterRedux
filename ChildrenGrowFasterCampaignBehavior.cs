using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Library;

namespace ChildrenGrowFasterRedux
{
    internal class ChildrenGrowFasterCampaignBehavior : CampaignBehaviorBase
    {
        private static readonly SubModuleSettings Fallback = new();
        private static SubModuleSettings Settings => AttributeGlobalSettings<SubModuleSettings>.Instance ?? Fallback;

        public override void RegisterEvents() => CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTickEvent);

        public override void SyncData(IDataStore dataStore) { }

        private void OnDailyTickEvent()
        {
            SubModuleSettings settings = Settings;
            bool log = settings.LoggingEnabled;
            if (log) Log("OnDailyTickEvent() Called.");

            bool instant = settings.DoChildGrowToAdultInstantly;
            float additionalDaysPerDay = settings.GrowthRate - 1f;
            if (!instant && additionalDaysPerDay == 0f)
                return;

            AgeModel ageModel = Campaign.Current.Models.AgeModel;
            int comesOfAge = ageModel.HeroComesOfAge;
            int teenAge = ageModel.BecomeTeenagerAge;
            int childAge = ageModel.BecomeChildAge;

            bool affectEveryone = settings.AffectEveryone;
            bool onlyPlayerChildren = !affectEveryone && settings.AffectOnlyPlayerChildren;
            bool onlyPlayerClan = !affectEveryone && !onlyPlayerChildren && settings.AffectOnlyPlayerClanChildren;
            bool ageAdults = affectEveryone && additionalDaysPerDay != 0f;
            bool excludeMain = settings.ExcludeMainHeroFromAdultAging;
            bool fireAgeEvents = !CampaignOptions.IsLifeDeathCycleDisabled;

            Hero main = Hero.MainHero;
            Clan playerClan = main.Clan;
            CampaignTime shift = CampaignTime.Days(additionalDaysPerDay);
            // +1 day so Age is unambiguously >= HeroComesOfAge after float conversion
            CampaignTime adultBirthDay = CampaignTime.Now - CampaignTime.Years(comesOfAge) - CampaignTime.Days(1f);

            MBReadOnlyList<Hero> heroes = Hero.AllAliveHeroes;
            for (int i = 0; i < heroes.Count; i++)
            {
                Hero hero = heroes[i];
                if (hero.IsTemplate)
                    continue;

                float age = hero.Age;
                if (age < comesOfAge)
                {
                    if (onlyPlayerChildren && hero.Father != main && hero.Mother != main) continue;
                    if (onlyPlayerClan && hero.Clan != playerClan) continue;

                    int oldAge = (int)age;
                    hero.SetBirthDay(instant ? adultBirthDay : hero.BirthDay - shift);
                    if (fireAgeEvents)
                        FireSkippedAgeEvents(hero, oldAge, (int)hero.Age, childAge, teenAge);
                    if (log && instant)
                        Log($"{hero.Name} has instantly grown into an adult (Age: {hero.Age}).");
                }
                else if (ageAdults && !(excludeMain && hero == main))
                {
                    hero.SetBirthDay(hero.BirthDay - shift);
                }
            }
        }

        // Vanilla AgingCampaignBehavior.DailyTickHero fires these only when (int)Age == threshold exactly,
        // so a jump past 3 or 14 in one day would skip them. HeroComesOfAge uses >= and needs no help.
        private static void FireSkippedAgeEvents(Hero hero, int oldAge, int newAge, int childAge, int teenAge)
        {
            if (oldAge < childAge && newAge > childAge)
                CampaignEventDispatcher.Instance.OnHeroGrowsOutOfInfancy(hero);
            if (oldAge < teenAge && newAge > teenAge)
                CampaignEventDispatcher.Instance.OnHeroReachesTeenAge(hero);
        }

        private static void Log(string message) => InformationManager.DisplayMessage(new InformationMessage(message, Colors.Yellow));
    }
}