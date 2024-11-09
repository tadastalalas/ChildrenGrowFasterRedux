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

        [CommandLineFunctionality.CommandLineArgumentFunction("fire_spouse_event", "debug")]
        public static string FireSpouseEvent(List<string> strings)
        {
            if (Hero.MainHero.Spouse != null)
            {
                SubModule subModule = new SubModule();

                if (subModule == null)
                {
                    return "Error: SubModule instance is null.";
                }

                Type type = subModule.GetType();

                List<MethodInfo> spouseEventMethods = new List<MethodInfo>
                {
                    type.GetMethod("spouseEvent1", BindingFlags.NonPublic | BindingFlags.Instance),
                    type.GetMethod("spouseEvent2", BindingFlags.NonPublic | BindingFlags.Instance),
                    type.GetMethod("spouseEvent3", BindingFlags.NonPublic | BindingFlags.Instance),
                    type.GetMethod("spouseEvent4", BindingFlags.NonPublic | BindingFlags.Instance)

                };

                foreach (var method in spouseEventMethods)
                {
                    if (method == null)
                    {
                        return "Error: Could not find spouse event method.";
                    }
                }

                MethodInfo selectedEvent = spouseEventMethods[MBRandom.RandomInt(4)];
                selectedEvent.Invoke(subModule, null);
                return "Spouse event fired.";
            }
            return "Error: Main hero has no spouse.";
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

        
        
    }
}