using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using childrenGrowFaster;
using TaleWorlds.MountAndBlade;
using System.Reflection;
using static TaleWorlds.Library.VirtualFolders.Win64_Shipping_Client;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.CampaignSystem.Settlements;
using System.Linq;

namespace childrenGrowFaster
{
    public class SubModuleDebugging
    {
        [CommandLineFunctionality.CommandLineArgumentFunction("create_and_marry_hero", "debug")]
        public static string CreateAndMarryHero(List<string> strings)
        {
            if (Hero.MainHero.Spouse == null)
            {
                CreateAndMarryNewHero();
            }
            return "Hero created and married to main hero.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_child_age", "debug")]
        public static string SetChildAge(List<string> strings)
        {
            try
            {
                if (strings.Count < 2)
                {
                    return "Usage: set_child_age [child_name] [new_age]";
                }
                string childName = strings[0];
                float newAge = float.Parse(strings[1]);

                if (newAge < 0 || newAge > 100)
                {
                    return "Error: Age must be between 0 and 100.";
                }
                Hero targetChild = null;
                foreach (Hero h in Hero.AllAliveHeroes)
                {
                    if ((h.Father == Hero.MainHero || h.Mother == Hero.MainHero) && h.Name.ToString().ToLower().Contains(childName.ToLower()))
                    {
                        targetChild = h;
                        break;
                    }
                }

                if (targetChild == null)
                {
                    return $"Error: Could not find child with name '{childName}'";
                }

                // calculate new birthday
                CampaignTime currentTime = CampaignTime.Now;
                CampaignTime newBday = currentTime - CampaignTime.Years(newAge);
                targetChild.SetBirthDay(newBday);
                return $"Set {targetChild.Name} to age {newAge:F1}.";
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("create_random_workshop", "debug")]
        public static string CreateRandomWorkshops(List<string> strings)
        {
            try
            {
                if (Hero.MainHero.OwnedWorkshops.Count == 0)
                {
                    CreateRandomWorkshop();
                    return "Random workshop created and assigned to main hero.";
                }
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
            return "Main hero already owns a workshop.";
        }

            private static void CreateAndMarryNewHero()
        {
            // creating hero stuff (wish it could be more compact ;c )
            TextObject heroFullName = new TextObject("Debug Wife");
            TextObject heroFirstName = new TextObject("Debug");
            Clan heroClan = Clan.PlayerClan;
            CultureObject heroCulture = Hero.MainHero.Clan.Culture;
            CharacterObject templateCharacter = CharacterObject.FindFirst(character => character.Culture == heroCulture && character.Occupation == Occupation.Lord && character.IsFemale == true);
            Hero newHero = HeroCreator.CreateSpecialHero(templateCharacter, Hero.MainHero.HomeSettlement, Hero.MainHero.Clan, null);
            newHero.SetName(heroFullName, heroFirstName);
            newHero.SetBirthDay(Hero.MainHero.BirthDay);
            newHero.Clan = heroClan;
            heroClan.Heroes.Add(newHero);
            newHero.SetNewOccupation(Occupation.Lord); // the occupation is set in the template but you have to call it twice for it to work??? wtf
            newHero.ChangeState(Hero.CharacterStates.Active);
            CampaignEventDispatcher.Instance.OnHeroCreated(newHero, false);

            if (newHero != null)
            {
                MarriageAction.Apply(Hero.MainHero, newHero);
            }
        }

        private static void CreateRandomWorkshop()
        {
            Settlement nearestSettlement = Settlement.All
                .Where(s => s.IsTown && s.Town != null)
                .OrderBy(s => s.GetPosition().DistanceSquared(Hero.MainHero.GetPosition()))
                .FirstOrDefault();

            if (nearestSettlement != null && Hero.MainHero != null)
            {
                WorkshopType randomWorkshopType = WorkshopType.All.GetRandomElement();
                Workshop workshop = nearestSettlement.Town.Workshops.FirstOrDefault(w => w.WorkshopType == randomWorkshopType);
                Hero.MainHero.AddOwnedWorkshop(workshop);
            }
        }
    }
}