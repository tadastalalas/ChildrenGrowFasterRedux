using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Party;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using System.Collections.Generic;
using System;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using System.Drawing;


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
                daysSinceLastSpouseEvent++;
                if (MBRandom.RandomFloat < GlobalSettings<SubModuleSettings>.Instance.eventChance)
                {
                    var events = new List<Action> { spouseEvent1, spouseEvent2, spouseEvent3, spouseEvent4 };
                    events[MBRandom.RandomInt(4)]();
                    daysSinceLastSpouseEvent = 0;
                }
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
               if (MBRandom.RandomFloat < 0.5f && CampaignTime.Now.IsNightTime) // 5% chance of being kidnapped 
                {
                    isKidnapped = true;
                    InformationManager.DisplayMessage(new InformationMessage($"Bandits snuck into {spouse.Name}`s current settlment and kidnapped {spouse.Name}! get her back!", Colors.Green));
                }

               if (isKidnapped == true)
                {
                    nearestBanditParty.AddPrisoner(spouse.CharacterObject, 1);
                    Campaign.Current.VisualTrackerManager.RegisterObject(nearestBanditParty);
                    InformationManager.DisplayMessage(new InformationMessage("The bandit party has been marked on your map.", Colors.Red));
                    if (nearestBanditParty.PrisonRoster.Contains(spouse.CharacterObject) == false)
                    {
                        isKidnapped = false;
                    }
                }

               while (isKidnapped == true)
                {
                    daysSinceLastKidnapping++;

                    if (daysSinceLastKidnapping > 5)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"Your spouse has been in captivity  for {daysSinceLastKidnapping} days! You should consider rescuing them."));
                    }
                    else if (daysSinceLastKidnapping >= 10)
                    {
                        Hero.MainHero.SetPersonalRelation(spouse, daysSinceLastKidnapping);
                        InformationManager.DisplayMessage(new InformationMessage($"Your spouse has been in captivity for {daysSinceLastKidnapping} days! Their relation with you has descreased by {daysSinceLastKidnapping}"));
                    }
                }
            }
        }

        private void spouseEvent2()
        {
            Hero spouse = Hero.MainHero.Spouse;
            while (spouse.CurrentSettlement != null && spouse.CurrentSettlement != Hero.MainHero.CurrentSettlement && isKidnapped == false)
            {
                int currentGold = Hero.MainHero.Gold;
                int gainedAmount = (int)MBRandom.RandomInt(500, 1000);

                if (spouse != null && currentGold < 1000 || currentGold > 1000)
                {
                    currentGold += gainedAmount;
                    InformationManager.DisplayMessage(new InformationMessage($"Your spouse earned {gainedAmount} gold!", Colors.Green));
                }
            }
        }

        
        private void spouseEvent3()
        {
            List<Workshop> workshops = new List<Workshop>();
            workshops.AddRange(Hero.MainHero.OwnedWorkshops);
            foreach (Workshop workshop in workshops)
            {
                Hero spouse = Hero.MainHero.Spouse; 
                if (workshop.Settlement != Hero.MainHero.CurrentSettlement && workshop.Settlement == spouse.CurrentSettlement && workshop != null)
                {
                    int randomProfit = MBRandom.RandomInt(100, 900);
                    workshop.ChangeGold(randomProfit);
                    InformationManager.DisplayMessage(new InformationMessage($"Your spouse has boosted the profits of {workshop.Name} by {randomProfit} gold!", Colors.Green));
                }
            }
        }

        private void spouseEvent4()
        {
            Hero spouse = Hero.MainHero.Spouse;

            foreach (Settlement s in Settlement.All)
            {
                if (!IsValidSettlement(s, spouse)) continue;

                TroopRoster garrisonRoster = s.Town.GarrisonParty?.MemberRoster;
                if (garrisonRoster == null) continue;
                GiveXP(garrisonRoster, s, spouse);
            }
        }

        private bool IsValidSettlement(Settlement s, Hero spouse)
        {
            return s.OwnerClan == Hero.MainHero.Clan
                && spouse.CurrentSettlement == s
                && Hero.MainHero.CurrentSettlement != spouse.CurrentSettlement
                && s != null;
        }

        private void GiveXP(TroopRoster garrisonRoster, Settlement s, Hero spouse)
        {
            foreach (TroopRosterElement troop in garrisonRoster.GetTroopRoster())
            {
                if (troop.Character == null) continue;

                var randomXP = MBRandom.RandomInt(100, 999);
                var skills = typeof(DefaultSkills).GetFields();
                foreach (var skill in skills)
                {
                    if (skill.GetValue(null) is SkillObject skillObject)
                    {
                        troop.Character.HeroObject?.AddSkillXp(skillObject, randomXP);
                        InformationMessage message = new InformationMessage(
                            $"{spouse.Name}'s leadership & steward skills have increased the xp of garrisoned troops in {s.Name} by {randomXP}",
                            Colors.Green);
                        InformationManager.DisplayMessage(message);
                    }
                }
            }
        }


        private void spouseEvent5()
        {
            Hero spouse = Hero.MainHero.Spouse;

            foreach (Settlement s in Settlement.All)
            {
                if (s.Owner == Hero.MainHero && spouse.CurrentSettlement == s && s != null)
                {
                    int randomGold = MBRandom.RandomInt(100, 1000);
                    s.Town.ChangeGold(randomGold);
                    InformationManager.DisplayMessage(new InformationMessage($"{spouse.Name} has earned {randomGold} for {s.Name}!", Colors.Green));
                }
            }
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
 * do something with keeping track of days since last event. ex : if daysSinceLastSpouseEvent > 30, then do something.
*/