﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using MCM.Abstractions.Base.Global;

namespace ChildrenGrowFasterRedux
{
    internal class ChildrenGrowFasterCampaignBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents() => CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTickEvent);

        public override void SyncData(IDataStore dataStore) { }

        private readonly SubModuleSettings settings = AttributeGlobalSettings<SubModuleSettings>.Instance ?? new SubModuleSettings();

        private int daysSinceLastPlayerChildTraitAdded = 0;

        private bool playerChildTraitAdded = false;

        private void OnDailyTickEvent()
        {
            LogMessage("OnDailyTickEvent() Called.");

            if (settings.AffectOnlyPlayerChildren)
                ApplyGrowthRateToPlayerChildren();
            else if (settings.AffectOnlyPlayerClanChildren)
                ApplyGrowthRateToPlayerClanChildren();
            else
                ApplyGrowthRateToAllChildren();

            if (settings.AffectEveryone)
                ApplyGrowthRateToEveryoneElse();

            if (settings.RandomTraitsForPlayerChildren)
            {
                if (playerChildTraitAdded && daysSinceLastPlayerChildTraitAdded < settings.DaysBetweenNextTraitCanBeAdded)
                {
                    daysSinceLastPlayerChildTraitAdded++;
                }
                else
                {
                    daysSinceLastPlayerChildTraitAdded = 0;
                    playerChildTraitAdded = false;
                }

                if (!playerChildTraitAdded && MBRandom.RandomInt(0, 100) < settings.RandomTraitChance)
                {
                    GiveRandomTraitToChild();
                    playerChildTraitAdded = true;
                }
            }
        }

        private void ApplyGrowthRateToPlayerChildren()
        {
            LogMessage("ApplyGrowthRateToPlayerChildren() Called.");
            ApplyGrowthRate(hero => hero.IsChild && hero.Age < Campaign.Current.Models.AgeModel.HeroComesOfAge && (hero.Father == Hero.MainHero || hero.Mother == Hero.MainHero));
        }

        private void ApplyGrowthRateToPlayerClanChildren()
        {
            LogMessage("ApplyGrowthRateToPlayerClan Children() Called.");
            ApplyGrowthRate(hero => hero.IsChild && hero.Age < Campaign.Current.Models.AgeModel.HeroComesOfAge && (hero.Clan == Hero.MainHero.Clan));
        }

        private void ApplyGrowthRateToAllChildren()
        {
            LogMessage("ApplyGrowthRateToAllChildren() Called.");
            ApplyGrowthRate(hero => hero.IsChild && hero.Age < Campaign.Current.Models.AgeModel.HeroComesOfAge);
        }

        

        private void ApplyGrowthRateToEveryoneElse()
        {
            ApplyGrowthRate(hero => hero.Age >= Campaign.Current.Models.AgeModel.HeroComesOfAge);
        }

        private void ApplyGrowthRate(Func<Hero, bool> heroFilter)
        {
            LogMessage("ApplyGrowthRate() Called.");

            if (settings.DoChildGrowToAdultInstantly)
            {
                ApplyInstantGrowthToChildren(Hero.AllAliveHeroes.Where(heroFilter));
                return;
            }

            float additionalDaysPerDay = settings.GrowthRate;
            foreach (Hero hero in Hero.AllAliveHeroes.Where(heroFilter))
            {
                hero.SetBirthDay(hero.BirthDay - CampaignTime.Days(additionalDaysPerDay - 1));
            }
        }

        private void ApplyInstantGrowthToChildren(IEnumerable<Hero> heroes)
        {
            LogMessage("ApplyInstantGrowthToChildren() Called.");

            foreach (Hero hero in heroes)
            {
                if (hero.IsChild && hero.Age < Campaign.Current.Models.AgeModel.HeroComesOfAge)
                {
                    CampaignTime currentTime = CampaignTime.Now;
                    CampaignTime newBday = currentTime - CampaignTime.Years(Campaign.Current.Models.AgeModel.HeroComesOfAge);
                    hero.SetBirthDay(newBday);
                    LogMessage($"{hero.Name} has instantly grown into an adult (Age: {hero.Age}).");
                }
            }
        }

        private void GiveRandomTraitToChild()
        {
            LogMessage("GiveRandomTraitToChild() Called.");

            if (Hero.MainHero.Children == null || Hero.MainHero.Children.Count == 0)
                return;

            Hero randomChild = Hero.MainHero.Children[MBRandom.RandomInt(Hero.MainHero.Children.Count)];

            if (randomChild.GetHeroTraits().ToString().Length > settings.ChildTraitCountCheck)
                return;

            TraitObject[] availableTraits = new TraitObject[]
            {
                DefaultTraits.Mercy,
                DefaultTraits.Generosity,
                DefaultTraits.Honor,
                DefaultTraits.Valor,
                DefaultTraits.Calculating,
                DefaultTraits.ScoutSkills,
                DefaultTraits.RogueSkills,
                DefaultTraits.SergeantCommandSkills,
                DefaultTraits.KnightFightingSkills,
                DefaultTraits.CavalryFightingSkills,
                DefaultTraits.HorseArcherFightingSkills,
                DefaultTraits.ArcherFIghtingSkills,
                DefaultTraits.CrossbowmanStyle
            };

            TraitObject randomTrait = availableTraits[MBRandom.RandomInt(availableTraits.Length)];
            int randomTraitLevel = MBRandom.RandomInt(-1, 3);
            randomChild.SetTraitLevel(randomTrait, randomTraitLevel);
            InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=CGRR_S7CDl3Ac}{randomChild.Name} has gained the trait {randomTrait.Name} with level {randomTraitLevel}!").ToString()));
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