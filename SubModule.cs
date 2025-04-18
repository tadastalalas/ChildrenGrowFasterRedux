using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Localization;


namespace ChildrenGrowFasterRedux
{
    public class SubModule : MBSubModuleBase
    {
        private Harmony? _harmony;
        private readonly SubModuleSettings settings = AttributeGlobalSettings<SubModuleSettings>.Instance ?? new SubModuleSettings();

        private int daysSinceLastPlayerChildTraitAdded = 0;

        private bool playerChildTraitAdded = false;

        protected override void OnSubModuleLoad()
        {
            _harmony = new Harmony("ChildrenGrowFasterRedux");
            _harmony.PatchAll();
            InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=CGRR_ufs59VVy}Children Grow Faster Redux loaded succesfully.").ToString(), Colors.Green));
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            if (game.GameType is Campaign)
            {
                CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTickEvent);
            }
        }

        private void OnDailyTickEvent()
        {
            if (settings.AffectOnlyPlayerChildren)
                ApplyGrowthRateToPlayerChildren();
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
            if (settings.DoChildGrowToAdultInstantly)
            {
                ApplyInstantGrowthToChildren(Hero.AllAliveHeroes.Where(hero => hero.Father == Hero.MainHero || hero.Mother == Hero.MainHero));
                return;
            }

            float additionalDaysPerDay = settings.AdditionalDaysPerDay;
            foreach (Hero hero in Hero.AllAliveHeroes)
            {
                if (hero.IsChild && hero.Age < settings.WhenHeroComesOfAge && (hero.Father == Hero.MainHero || hero.Mother == Hero.MainHero))
                {
                    hero.SetBirthDay(hero.BirthDay - CampaignTime.Days(additionalDaysPerDay));
                }
            }
        }

        private void ApplyGrowthRateToAllChildren()
        {
            if (settings.DoChildGrowToAdultInstantly)
            {
                ApplyInstantGrowthToChildren(Hero.AllAliveHeroes);
                return;
            }

            float additionalDaysPerDay = settings.AdditionalDaysPerDay;
            foreach (Hero hero in Hero.AllAliveHeroes)
            {
                if (hero.IsChild && hero.Age < settings.WhenHeroComesOfAge)
                {
                    hero.SetBirthDay(hero.BirthDay - CampaignTime.Days(additionalDaysPerDay));
                }
            }
        }

        private void ApplyInstantGrowthToChildren(IEnumerable<Hero> heroes)
        {
            foreach (Hero hero in heroes)
            {
                if (hero.IsChild && hero.Age < settings.WhenHeroComesOfAge)
                {
                    float yearsLeftToAdulthood = settings.WhenHeroComesOfAge - hero.Age;
                    float daysLeftToAdulthood = yearsLeftToAdulthood * 84;

                    CampaignTime newBirthDay = hero.BirthDay - CampaignTime.Days(daysLeftToAdulthood);
                    hero.SetBirthDay(newBirthDay);
                }
            }
        }

        private void ApplyGrowthRateToEveryoneElse()
        {
            float additionalDaysPerDay = settings.AdditionalDaysPerDay;

            foreach (Hero hero in Hero.AllAliveHeroes)
            {
                if (hero.Age >= settings.WhenHeroComesOfAge)
                    hero?.SetBirthDay(hero.BirthDay - CampaignTime.Days(additionalDaysPerDay));
            }
        }

        private void GiveRandomTraitToChild()
        {
            if (Hero.MainHero.Children == null || Hero.MainHero.Children.Count == 0)
                return;

            Hero randomChild = Hero.MainHero.Children[MBRandom.RandomInt(Hero.MainHero.Children.Count)];

            if (randomChild.GetHeroTraits().ToString().Length > 3)
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
    }
}