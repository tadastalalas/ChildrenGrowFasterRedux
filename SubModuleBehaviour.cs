using MCM.Abstractions.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace childrenGrowFaster
{
    public class SubModuleBehaviour : CampaignBehaviorBase
    {
        private bool isKidnapped = false;
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick2);
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void OnDailyTick2()
        {
            if (GlobalSettings<SubModuleSettings>.Instance.spouseEventsEnabled)
            {
                if (Hero.MainHero.Spouse != null && !Hero.MainHero.Spouse.IsPregnant)
                {
                    FireRandomSpouseEvent();
                }
            }
        }

        private void FireRandomSpouseEvent()
        {
            var events = new List<Action> { SpouseEvent1, SpouseEvent2, SpouseEvent3, SpouseEvent4 };
            if (MBRandom.RandomFloat < GlobalSettings<SubModuleSettings>.Instance.eventChance)
            {
                events[MBRandom.RandomInt(events.Count)]();
            }
        }

        private void SpouseEvent1()
        {
            Hero spouse = Hero.MainHero.Spouse;
            int daysSinceLastKidnapping = 0;
            if (spouse == null || spouse.IsPrisoner || spouse.CurrentSettlement == null)
            {
                return;
            }

            var nearestBanditParty = MobileParty.All.Where(p => p.IsBandit && p.IsActive && p.CurrentSettlement == null)
            .OrderBy(p => p.GetPosition().DistanceSquared(spouse.CurrentSettlement.GetPosition()))
            .FirstOrDefault();

            while (nearestBanditParty != null && spouse.CurrentSettlement != Hero.MainHero.CurrentSettlement)
            {
                if (MBRandom.RandomFloat > GlobalSettings<SubModuleSettings>.Instance.eventChance)
                {
                    isKidnapped = true; 
                    InformationManager.DisplayMessage(new InformationMessage($"{spouse.Name} has been kidnapped by bandits!"));
                }

                if (isKidnapped)
                {
                    daysSinceLastKidnapping++;
                    nearestBanditParty.AddPrisoner(spouse.CharacterObject, 1);
                    Campaign.Current.VisualTrackerManager.RegisterObject(nearestBanditParty);
                    InformationManager.DisplayMessage(new InformationMessage("The bandit party has been marked on your map.", Colors.Red));
                    if (!nearestBanditParty.PrisonRoster.Contains(spouse.CharacterObject))
                    {
                        isKidnapped = false;
                    }

                    if (daysSinceLastKidnapping > 5)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"Your spouse has been in captivity for {daysSinceLastKidnapping} days! You should rescue them soon."));
                    }

                }
            }

        }

        private void SpouseEvent2()
        {
            Hero spouse = Hero.MainHero.Spouse;
            while (spouse.CurrentSettlement != null && spouse.CurrentSettlement != Hero.MainHero.CurrentSettlement && !isKidnapped)
            {
                int currentGold = Hero.MainHero.Gold;
                int gainedAmount = MBRandom.RandomInt(500, 1000);

                if (spouse != null && (currentGold < 1000 || currentGold > 1000))
                {
                    currentGold += gainedAmount;
                    InformationManager.DisplayMessage(new InformationMessage($"Your spouse earned {gainedAmount} gold!", Colors.Green));
                }
            }
        }

        private void SpouseEvent3()
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

        private void SpouseEvent4()
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
    }
}

// remove spouse events from main file later. 