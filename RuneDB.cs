using HarmonyLib;
using Runestones.RuneEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones
{
    class RuneDB
    {
        private static RuneDB _instance;
        public static RuneDB Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RuneDB();
                return _instance;
            }
        }

        public List<Rune> AllRunes = new List<Rune>
        {
            // Meadows runes
            new Rune { Name = "Force", EffectClass = typeof(ForceRuneEffect), Desc = "May the force be with you", DiscoveryToken = "$lore_meadows_random06", AssetIndex = 0, Reagents = new List<Dictionary<string, int>>{ new Dictionary<string, int> { { "TrophyBoar", 2 } } } },
            new Rune { Name = "Hearth", EffectClass = typeof(HearthRuneEffect), Desc = "A warm fire is magic in and of itself", DiscoveryToken = "$lore_meadows_random01", AssetIndex = 1, Reagents = new List<Dictionary<string, int>>{ new Dictionary<string, int>{ { "Stone", 20 }, { "Wood", 15 } } } },
            new Rune { Name = "Curse", EffectClass = typeof(CurseRuneEffect), Desc = "Revenge is a dish best served with raspberries", DiscoveryToken = "$lore_meadows_random02", AssetIndex = 2, Reagents = new List<Dictionary<string, int>>{ new Dictionary<string, int> { { "Raspberry", 5 } } } },
            new Rune { Name = "Bees", EffectClass = typeof(BeeRuneEffect), Desc = "My God, it's full of bees...", DiscoveryToken = "$lore_meadows_random03", AssetIndex = 3, Reagents = new List<Dictionary<string, int>>{ new Dictionary<string, int> { { "Honey", 10 } } } },
            new Rune { Name = "Light", EffectClass = typeof(LightRuneEffect), Desc = "Let there be light", DiscoveryToken = "$lore_mountain_random04", AssetIndex = 4, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Resin", 5 }, { "Dandelion", 3 } } } },
            new Rune { Name = "Feather", EffectClass = typeof(FeatherRuneEffect), Desc = "I'm sure Hugin can lend you some", DiscoveryToken = "$lore_meadows_random05", AssetIndex = 5, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Feathers", 10 } } } },
            // Forest runes
            new Rune { Name = "Timber", EffectClass = typeof(TimberRuneEffect), Desc = "Fells trees", DiscoveryToken = "$lore_blackforest_random01", AssetIndex = 7, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "BeechSeeds", 8 }, { "TrophyGreydwarf", 1 } } } },
            new Rune { Name = "Boat", EffectClass = typeof(BoatRuneEffect), Desc = "Conjure a boat", DiscoveryToken = "$lore_blackforest_random02", AssetIndex = 8, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Wood", 50 }, { "LeatherScraps", 30 } } } },
            new Rune { Name = "Sunder", EffectClass = typeof(SunderRuneEffect), Desc = "Weakens enemy resistances to physical damage", DiscoveryToken = "$lore_blackforest_random03", AssetIndex = 9, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Flint", 8 }, { "GreydwarfEye", 3 } } } },
            new Rune { Name = "Harden", _FixedEffect = new AlchemyRuneEffect(){ itemAName="$item_resin", itemAPrefabName="Resin", itemBName="$item_amber", itemBPrefabName="Amber", ratio=25, alertMessage="Hardened Resin=>Amber", reversible=false },
                Desc = "Hardens items in inventory\n25 Resin ===> Amber", DiscoveryToken = "$lore_forest_random04", AssetIndex = 10, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Stone", 20 }, { "GreydwarfEye", 3 } } } },
            new Rune { Name = "Cultivate", EffectClass = typeof(FarmRuneEffect), Desc = "Accelerates crop growth", DiscoveryToken = "$lore_blackforest_random05", AssetIndex = 11, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "TrophyGreydwarf", 1 }, { "AncientSeed", 1 } } } }, //requires aoe
            new Rune { Name = "Index", EffectClass = typeof(IndexRuneEffect), Desc = "Quick and easy spellcasting", DiscoveryToken = "$lore_blackforest_random06", AssetIndex = 12, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "GreydwarfEye", 1 }, { "Thistle", 1 } } } }, //rearranges hotkeys for easier spellcasting, inventory effect
            new Rune { Name = "FinalPlaceHolder1", EffectClass = typeof(LightRuneEffect), Desc = "Placeholder rune 1", DiscoveryToken = "$lore_blackforest_random07", AssetIndex = 13, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Raspberry", 5 }, { "GreydwarfEye", 5 } } } },
            // Swamp runes
            new Rune { Name = "Animate", EffectClass = typeof(AnimateRuneEffect), Desc = "You can necromance if you want to", DiscoveryToken = "$lore_swamp_random01", AssetIndex = 14, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "BoneFragments", 5 }, { "Ruby", 1 } } } },
            new Rune { Name = "FinalPlaceHolder2", EffectClass = typeof(LightRuneEffect), Desc = "Placeholder rune 2", DiscoveryToken = "$lore_swamp_random02", AssetIndex = 15,  Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Raspberry", 5 }, { "GreydwarfEye", 5 } } } },
            new Rune { Name = "Fear", EffectClass = typeof(FearRuneEffect), Desc = "Forces an enemy to flee for 30 seconds", DiscoveryToken = "$lore_swamp_random03", AssetIndex = 16, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "TrophyBlob", 1 }, { "GreydwarfEye", 3 } } } }, //requires aoe
            new Rune { Name = "FinalPlaceHolder3", EffectClass = typeof(LightRuneEffect), Desc = "Placeholder rune 3", DiscoveryToken = "$lore_swamp_random04", AssetIndex = 17,  Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Raspberry", 5 }, { "GreydwarfEye", 5 } } } },
            new Rune { Name = "Revive", EffectClass = typeof(ReviveRuneEffect), Desc = "After a recent death, restores 50% of skill knowledge lost and grants +75% movement speed", DiscoveryToken = "$lore_swamp_random05", AssetIndex = 18, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "TrophyGreydwarfShaman", 1 } } } },
            new Rune { Name = "Pocket", EffectClass = typeof(PocketRuneEffect), Desc = "Extradimensional storage", DiscoveryToken = "$lore_swamp_random06", AssetIndex = 19, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "FineWood", 10 }, { "GreydwarfEye", 3 } } } }, //access small extradimensional chest, inventory effect
            new Rune { Name = "Darkness", EffectClass = typeof(DarknessRuneEffect), Desc = "Hello darkness my old friend...", DiscoveryToken = "$lore_swamp_random07", AssetIndex = 20, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Coal", 10 }, { "Guck", 3 } } } }, //darkens an area, making stealth easier, requires area effect
            // Mountain runes
            new Rune { Name = "Ice", EffectClass = typeof(IceRuneEffect), Desc = "The breath of the dragon", DiscoveryToken = "$lore_mountains_random01", AssetIndex = 21, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "FreezeGland", 10 }, { "GreydwarfEye", 3 } } } },
            new Rune { Name = "Home", EffectClass = typeof(HouseRuneEffect), Desc = "Keeps out your enemies", DiscoveryToken = "$lore_mountains_random02", AssetIndex = 22, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Wood", 50 }, { "GreydwarfEye", 3 } } } },
            new Rune { Name = "Slow", EffectClass = typeof(SlowRuneEffect), Desc = "Slow 90%", DiscoveryToken = "$lore_mountains_random03", AssetIndex = 23, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "FreezeGland", 3 }, { "Silver", 1 } } } },
            new Rune { Name = "Transmute", _FixedEffect = new AlchemyRuneEffect(){ itemAName="$item_tin", itemAPrefabName="Tin", itemBName="$item_copper", itemBPrefabName="Copper", ratio=2, alertMessage="Transmuted Copper/Tin", reversible=true },
                Desc = "Transmutes metals in inventory\n2 Tin <===> Copper", DiscoveryToken = "$lore_mountains_random04", AssetIndex = 24, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Coal", 10 }, { "GreydwarfEye", 3 } } } },
            new Rune { Name = "Heal", EffectClass = typeof(HealRuneEffect), Desc = "Heal", DiscoveryToken = "$lore_mountains_random05", AssetIndex = 25, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "MeadHealthMinor", 3 }, { "Thistle", 3 } } } },
            new Rune { Name = "Weather", EffectClass = typeof(WeatherRuneEffect), Desc = "So, you want to harness lightning. Best start with a stiff breeze", DiscoveryToken = "$lore_mountains_random06", AssetIndex = 26, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "GreydwarfEye", 5 }, { "TrophyDeer", 2 } } } },
            new Rune { Name = "Quake", EffectClass = typeof(QuakeRuneEffect), Desc = "Stagger at range", DiscoveryToken = "$lore_mountains_random07", AssetIndex = 27, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Crystal", 2 } } } }, //stagger at range, requires aoe
            // Plains runes
            new Rune { Name = "Fire", EffectClass = typeof(FireRuneEffect), Desc = "The trouble with fire is not how to make it, but how to stop", DiscoveryToken = "$lore_plains_random01", AssetIndex = 28, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Coal", 10 }, { "AmberPearl", 2 }, { "SurtlingCore", 2 } } } },
            new Rune { Name = "Wall", EffectClass = typeof(WallRuneEffect), Desc = "Keeps out your enemies", DiscoveryToken = "$lore_plains_random02", AssetIndex = 29, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Wood", 20 }, { "ShieldBanded", 1 } } } },
            new Rune { Name = "Charm", EffectClass = typeof(CharmRuneEffect), Desc = "Charms an enemy for 30 seconds", DiscoveryToken = "$lore_plains_random03", AssetIndex = 30, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Honey", 10 }, { "SurtlingCore", 1 } } } }, //requires projectile/aoe
            new Rune { Name = "Pyrolyze", _FixedEffect = new AlchemyRuneEffect(){ itemAName="$item_wood", itemAPrefabName="Wood", itemBName="$item_coal", itemBPrefabName="Coal", ratio=4, alertMessage="Pyrolyzed Wood=>Coal", reversible=false },
                Desc = "Pyrolyzes items in inventory\n4 Wood ===> Coal", DiscoveryToken = "$lore_plains_random04", AssetIndex = 31, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "SurtlingCore", 1 }, { "GreydwarfEye", 3 } } } },
            new Rune { Name = "Ward", EffectClass = typeof(WardRuneEffect), Desc = "Powerful ward", DiscoveryToken = "$lore_plains_random05", AssetIndex = 32, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "FineWood", 10 }, { "SurtlingCore", 1 }, { "TrophyGoblin", 1 } } } },
            new Rune { Name = "Teleport", EffectClass = typeof(TeleportRuneEffect), Desc = "Short-range teleport", DiscoveryToken = "$lore_plains_random06", AssetIndex = 33, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "GreydwarfEye", 15 }, { "TrophyWraith", 1 } } } }, //requires magic projectile
            new Rune { Name = "FinalPlaceHolder4", EffectClass = typeof(LightRuneEffect), Desc = "Placeholder rune 4", DiscoveryToken = "$lore_plains_random07", AssetIndex = 34, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Raspberry", 5 }, { "GreydwarfEye", 5 } } } }
        };

        public Dictionary<string, Rune> RunesByName = new Dictionary<string, Rune>();

        public List<Dictionary<string, int>> RuneBases = new List<Dictionary<string, int>>()
        {
            new Dictionary<string, int>{ { "Stone", 10 } },
            new Dictionary<string, int>{ { "ElderBark", 5 } },
            new Dictionary<string, int>{ { "BlackMetal", 2 } }
        };
        
        private RuneDB()
        {
            foreach(Rune r in AllRunes)
            {
                RunesByName.Add(r.GetToken(), r);
            }
        }

        public Rune GetRune(string runeName)
        {
            RunesByName.TryGetValue(runeName, out Rune r);
            return r;
        }
    }

    [HarmonyPatch(typeof(InventoryGui), "SetRecipe")]
    public static class CraftingVariantMod
    {
        public static void Postfix(InventoryGui __instance, KeyValuePair<Recipe, ItemDrop.ItemData> ___m_selectedRecipe, ref int ___m_selectedVariant)
        {
            var item = ___m_selectedRecipe.Key?.m_item?.m_itemData;
            if ((item?.m_shared?.m_ammoType ?? "") == "rune")
            {
                RuneDB.Instance.RunesByName.TryGetValue(item.m_shared.m_name, out Rune selectedRune);
                item.m_variant = selectedRune.AssetIndex;
                ___m_selectedVariant = selectedRune.AssetIndex;
            }
        }
    }
}
