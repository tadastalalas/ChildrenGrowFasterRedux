using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Party;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using System.Diagnostics.CodeAnalysis;



namespace childrenGrowFaster
{
    public class SubModule : MBSubModuleBase
    {
        private Harmony _harmony;
        private int daysSinceLastSpouseEvent = 0;
        private int daysSinceLastTraitEvent = 0;
        protected override void OnSubModuleLoad()
        {
            _harmony = new Harmony("childrenGrowFaster"); 
            _harmony.PatchAll(); 
        }
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (game.GameType is Campaign)
            {
                CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
            }
        }

        private void OnDailyTick()
        {
            if (GlobalSettings<SubModuleSettings>.Instance.affectOnlyPlayerChildren)
            {
                applyGrowthRateToPlayerChildren();
            }
            else
            {
                applyGrowthRateToAllChildren();
            }
            if (GlobalSettings<SubModuleSettings>.Instance.instantGrowth)
            {
                float growthRate = GlobalSettings<SubModuleSettings>.Instance.newGrowthRate;
                foreach (Hero h in Hero.AllAliveHeroes)
                {
                    if (h.IsChild && h.Age < 18 && (h.Father == Hero.MainHero || h.Mother == Hero.MainHero))
                    {
                        h.SetBirthDay(h.BirthDay - CampaignTime.Days(growthRate * 100));
                    }
                }
            }
            if (GlobalSettings<SubModuleSettings>.Instance.affectEveryone)
            {
                applyGrowthRateToEveryone();
            }
        
            if (GlobalSettings<SubModuleSettings>.Instance.spouseEventsEnabled && Hero.MainHero.Spouse.IsPregnant == false) 
            {
                if (MBRandom.RandomFloat < GlobalSettings<SubModuleSettings>.Instance.eventChance)
                {
                    daysSinceLastSpouseEvent++;
                    spouseEvent1();
                }
                daysSinceLastSpouseEvent = 0;
            }

            if (GlobalSettings<SubModuleSettings>.Instance.randomTraitsEnabled)
            {
                if (MBRandom.RandomFloat < GlobalSettings<SubModuleSettings>.Instance.traitChance)
                {
                    daysSinceLastTraitEvent++;
                    giveRandomTraitToChild();
                }
                daysSinceLastTraitEvent = 0; 
            }  
        }

        private void applyGrowthRateToPlayerChildren()
        {
            float growthRate = GlobalSettings<SubModuleSettings>.Instance.newGrowthRate;
            foreach (Hero h in Hero.AllAliveHeroes)
            {
                if (h.IsChild && h.Age < 18 && (h.Father == Hero.MainHero || h.Mother == Hero.MainHero))
                {
                    h.SetBirthDay(h.BirthDay - CampaignTime.Days(growthRate + 1f)); // +1f triples growth rate essentially
                }
            }
        }

        private void applyGrowthRateToAllChildren()
        {
            float growthRate = GlobalSettings<SubModuleSettings>.Instance.newGrowthRate;
            foreach (Hero h in Hero.AllAliveHeroes)
            {
                if (h.IsChild && h.Age < 18)
                {
                    h.SetBirthDay(h.BirthDay - CampaignTime.Days(growthRate + 1f));
                }
            }
        }

        private void applyGrowthRateToEveryone()
        {
            float growthRate = GlobalSettings<SubModuleSettings>.Instance.newGrowthRate;
            foreach (Hero h in Hero.AllAliveHeroes)
            {
                if (h != null)
                {
                    h.SetBirthDay(h.BirthDay - CampaignTime.Days(growthRate + 1f));
                }
            }
        }

        public bool isKidnapped { get; set; } = false;
        private void spouseEvent1()
        {
            Hero spouse = Hero.MainHero.Spouse;
            int daysSinceLastKidnapping = 0;
            if (spouse == null  || spouse.IsPrisoner || spouse.CurrentSettlement == null)
            {
                return;
            }

            // find nearest bandit party
            var nearestBanditParty = MobileParty.All
                .Where(party => party.IsBandit && party.IsActive && party.CurrentSettlement == null)
                .OrderBy(party => party.GetPosition().DistanceSquared(spouse.GetPosition()))
                .FirstOrDefault();


            if (nearestBanditParty != null && spouse.CurrentSettlement != Hero.MainHero.CurrentSettlement)
            {
               if (MBRandom.RandomFloat < 0.5f) // 5% chance of being kidnapped 
                {
                    isKidnapped = true;
                    InformationManager.DisplayMessage(new InformationMessage($"Bandits snuck into {spouse.Name}`s current settlment and kidnapped {spouse.Name}! get her back!"));
                }

               if (isKidnapped == true)
                {
                    nearestBanditParty.AddPrisoner(spouse.CharacterObject, 1);
                    Campaign.Current.VisualTrackerManager.RegisterObject(nearestBanditParty);
                    InformationManager.DisplayMessage(new InformationMessage("The bandit party has been marked on your map."));
                    if (nearestBanditParty.PrisonRoster.Contains(spouse.CharacterObject) == false)
                    {
                        isKidnapped = false;
                    }
                }
            }
        }

        private void spouseEvent2()
        {
            throw new System.NotImplementedException(); 
        }

        private void spouseEvent3()
        {
            throw new System.NotImplementedException();
        }

        private void giveRandomTraitToChild()
        {
            // make sure main hero has children 
            if (Hero.MainHero.Children == null || Hero.MainHero.Children.Count == 0)
            {
                return;
            }

            Hero randomChild = Hero.MainHero.Children[MBRandom.RandomInt(Hero.MainHero.Children.Count)];

            TraitObject[] availableTraits = new TraitObject[]
            {
                DefaultTraits.Mercy,
                DefaultTraits.Generosity,
                DefaultTraits.Honor,
                DefaultTraits.Valor,
                DefaultTraits.Calculating,
                
                // skill traits 
                DefaultTraits.ScoutSkills,
                DefaultTraits.RogueSkills,
                DefaultTraits.SergeantCommandSkills,
                DefaultTraits.KnightFightingSkills,
                DefaultTraits.CavalryFightingSkills,
                DefaultTraits.HorseArcherFightingSkills,
                DefaultTraits.ArcherFIghtingSkills,
                DefaultTraits.CrossbowmanStyle

                // https://docs.bannerlordmodding.lt/modding/heroes/
            };

            TraitObject randomTrait = availableTraits[MBRandom.RandomInt(availableTraits.Length)];
            int randomTraitLevel = MBRandom.RandomInt(-1, 3);
            randomChild.SetTraitLevel(randomTrait, randomTraitLevel);
            InformationManager.DisplayMessage(new InformationMessage($"{randomChild.Name} has gained the trait {randomTrait.Name} with level {randomTraitLevel}!"));
        }
    }
}

/* TODO:
 * implement a way to manipulate where the bandit party roams. 
 * create a round popup that notifies the player that their spouse has been kidnapped.. https://docs.bannerlordmodding.lt/guides/custom_round_popup/
 * come up with ideas for 2 more spouse events. 
*/