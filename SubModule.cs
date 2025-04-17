using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using System.Collections.Generic;
using System.Linq;


namespace ChildrenGrowFaster
{
    public class SubModule : MBSubModuleBase
    {
        private Harmony? _harmony;
        private int daysSinceLastSpouseEvent = 0;
        private int daysSinceLastTraitEvent = 0;
        private readonly SubModuleSettings settings = AttributeGlobalSettings<SubModuleSettings>.Instance ?? new SubModuleSettings();

        protected override void OnSubModuleLoad()
        {
            _harmony = new Harmony("ChildrenGrowFaster");
            _harmony.PatchAll();
            InformationManager.DisplayMessage(new InformationMessage("Children Grow Faster loaded succesfully.", Colors.Green));
        }
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (game.GameType is Campaign)
            {
                CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
                CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarterObject;
                campaignStarter.AddBehavior(new SubModuleBehaviour());
            }
        }

        private void OnDailyTick()
        {
            if (settings.affectOnlyPlayerChildren)
                ApplyGrowthRateToPlayerChildren();
            else
                ApplyGrowthRateToAllChildren();

            if (settings.affectEveryone)
                ApplyGrowthRateToEveryoneElse();

            if (settings.randomTraitsEnabled)
            {
                if (MBRandom.RandomFloat < settings.traitChance)
                {
                    daysSinceLastTraitEvent++;
                    GiveRandomTraitToChild();
                }
                daysSinceLastTraitEvent = 0;
            }
        }

        private void ApplyGrowthRateToPlayerChildren()
        {
            float additionalDaysPerDay = settings.additionalDaysPerDay;
            foreach (Hero hero in Hero.AllAliveHeroes)
            {
                if (hero.IsChild && hero.Age < settings.whenHeroComesOfAge && (hero.Father == Hero.MainHero || hero.Mother == Hero.MainHero))
                {
                    hero.SetBirthDay(hero.BirthDay - CampaignTime.Days(additionalDaysPerDay));
                }
            }

            if (settings.DoChildGrowToAdultInstantly)
                ApplyInstantGrowthToChildren(Hero.AllAliveHeroes.Where(hero => hero.Father == Hero.MainHero || hero.Mother == Hero.MainHero));
        }

        private void ApplyGrowthRateToAllChildren()
        {
            float additionalDaysPerDay = settings.additionalDaysPerDay;
            foreach (Hero hero in Hero.AllAliveHeroes)
            {
                if (hero.IsChild && hero.Age < settings.whenHeroComesOfAge)
                {
                    hero.SetBirthDay(hero.BirthDay - CampaignTime.Days(additionalDaysPerDay));
                }
            }

            if (settings.DoChildGrowToAdultInstantly)
                ApplyInstantGrowthToChildren(Hero.AllAliveHeroes);
        }

        private void ApplyInstantGrowthToChildren(IEnumerable<Hero> heroes)
        {
            foreach (Hero hero in heroes)
            {
                if (hero.IsChild && hero.Age < settings.whenHeroComesOfAge)
                {
                    float yearsLeftToAdulthood = settings.whenHeroComesOfAge - hero.Age;
                    float daysLeftToAdulthood = yearsLeftToAdulthood * 84;

                    CampaignTime newBirthDay = hero.BirthDay - CampaignTime.Days(daysLeftToAdulthood);
                    hero.SetBirthDay(newBirthDay);
                }
            }
        }

        private void ApplyGrowthRateToEveryoneElse()
        {
            float additionalDaysPerDay = settings.additionalDaysPerDay;

            foreach (Hero hero in Hero.AllAliveHeroes)
            {
                if (hero.Age >= settings.whenHeroComesOfAge)
                    hero?.SetBirthDay(hero.BirthDay - CampaignTime.Days(additionalDaysPerDay));
            }
        }

        private void GiveRandomTraitToChild()
        {
            if (Hero.MainHero.Children == null || Hero.MainHero.Children.Count == 0)
                return;

            Hero randomChild = Hero.MainHero.Children[MBRandom.RandomInt(Hero.MainHero.Children.Count)];

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
            InformationManager.DisplayMessage(new InformationMessage($"{randomChild.Name} has gained the trait {randomTrait.Name} with level {randomTraitLevel}!"));
            
            if (randomChild.GetHeroTraits().ToString().Length > 3)
                return;
        }
    }
}