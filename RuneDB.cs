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
            new Rune { Name = "Force", EffectClass = typeof(ForceRuneEffect), DiscoveryToken = "$lore_meadows_random06", AssetIndex = 0, Reagents = new List<Dictionary<string, int>>{ new Dictionary<string, int> { { "TrophyBoar", 2 } } } },
            new Rune { Name = "Hearth", EffectClass = typeof(HearthRuneEffect), DiscoveryToken = "$lore_meadows_random01", AssetIndex = 1, Reagents = new List<Dictionary<string, int>>{ new Dictionary<string, int>{ { "Stone", 20 }, { "Wood", 15 } } } },
            new Rune { Name = "Curse", EffectClass = typeof(CurseRuneEffect), DiscoveryToken = "$lore_meadows_random02", AssetIndex = 2, Reagents = new List<Dictionary<string, int>>{ new Dictionary<string, int> { { "Raspberry", 5 } } } },
            new Rune { Name = "Bees", EffectClass = typeof(BeeRuneEffect), DiscoveryToken = "$lore_meadows_random03", AssetIndex = 3, Reagents = new List<Dictionary<string, int>>{ new Dictionary<string, int> { { "Honey", 10 } } } },
            new Rune { Name = "Light", EffectClass = typeof(LightRuneEffect), DiscoveryToken = "$lore_mountain_random04", AssetIndex = 4, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Resin", 5 }, { "Dandelion", 3 } } } },
            new Rune { Name = "Feather", EffectClass = typeof(FeatherRuneEffect), DiscoveryToken = "$lore_meadows_random05", AssetIndex = 5, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Feathers", 10 } } } },
            // Forest runes
            new Rune { Name = "Timber", EffectClass = typeof(TimberRuneEffect), DiscoveryToken = "$lore_blackforest_random01", AssetIndex = 7, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "BeechSeeds", 8 }, { "TrophyGreydwarf", 1 } } } },
            new Rune { Name = "Boat", EffectClass = typeof(BoatRuneEffect), DiscoveryToken = "$lore_blackforest_random02", AssetIndex = 8, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Wood", 50 }, { "LeatherScraps", 30 } } } },
            new Rune { Name = "Sunder", EffectClass = typeof(SunderRuneEffect), DiscoveryToken = "$lore_blackforest_random03", AssetIndex = 9, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Flint", 8 }, { "GreydwarfEye", 3 } } } },
            new Rune { Name = "Harden", _FixedEffect = new AlchemyRuneEffect(){ itemAName="$item_resin", itemAPrefabName="Resin", itemBName="$item_amber", itemBPrefabName="Amber", ratio=25, alertMessage="Hardened Resin=>Amber", reversible=false },
                DiscoveryToken = "$lore_forest_random04", AssetIndex = 10, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Stone", 20 }, { "GreydwarfEye", 3 } } } },
            new Rune { Name = "Cultivate", EffectClass = typeof(FarmRuneEffect), DiscoveryToken = "$lore_blackforest_random05", AssetIndex = 11, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "TrophyGreydwarf", 1 }, { "AncientSeed", 1 } } } }, //requires aoe
            new Rune { Name = "Index", EffectClass = typeof(IndexRuneEffect), DiscoveryToken = "$lore_blackforest_random06", AssetIndex = 12, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "GreydwarfEye", 1 }, { "Thistle", 1 } } } }, //rearranges hotkeys for easier spellcasting, inventory effect
            new Rune { Name = "FinalPlaceHolder1", EffectClass = typeof(LightRuneEffect), DiscoveryToken = "$lore_blackforest_random07", AssetIndex = 13, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Raspberry", 5 }, { "GreydwarfEye", 5 } } } },
            // Swamp runes
            new Rune { Name = "Animate", EffectClass = typeof(AnimateRuneEffect), DiscoveryToken = "$lore_swamp_random01", AssetIndex = 14, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "BoneFragments", 5 }, { "Ruby", 1 } } } },
            new Rune { Name = "FinalPlaceHolder2", EffectClass = typeof(LightRuneEffect), DiscoveryToken = "$lore_swamp_random02", AssetIndex = 15,  Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Raspberry", 5 }, { "GreydwarfEye", 5 } } } },
            new Rune { Name = "Fear", EffectClass = typeof(FearRuneEffect), DiscoveryToken = "$lore_swamp_random03", AssetIndex = 16, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "TrophyBlob", 1 }, { "GreydwarfEye", 3 } } } }, //requires aoe
            new Rune { Name = "FinalPlaceHolder3", EffectClass = typeof(LightRuneEffect), DiscoveryToken = "$lore_swamp_random04", AssetIndex = 17,  Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Raspberry", 5 }, { "GreydwarfEye", 5 } } } },
            new Rune { Name = "Revive", EffectClass = typeof(ReviveRuneEffect), DiscoveryToken = "$lore_swamp_random05", AssetIndex = 18, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "TrophyGreydwarfShaman", 1 } } } },
            new Rune { Name = "Pocket", EffectClass = typeof(PocketRuneEffect), DiscoveryToken = "$lore_swamp_random06", AssetIndex = 19, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "FineWood", 10 }, { "GreydwarfEye", 3 } } } }, //access small extradimensional chest, inventory effect
            new Rune { Name = "Darkness", EffectClass = typeof(DarknessRuneEffect), DiscoveryToken = "$lore_swamp_random07", AssetIndex = 20, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Coal", 10 }, { "Guck", 3 } } } }, //darkens an area, making stealth easier, requires area effect
            // Mountain runes
            new Rune { Name = "Ice", EffectClass = typeof(IceRuneEffect), DiscoveryToken = "$lore_mountains_random01", AssetIndex = 21, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "FreezeGland", 10 }, { "GreydwarfEye", 3 } } } },
            new Rune { Name = "Home", EffectClass = typeof(HouseRuneEffect), DiscoveryToken = "$lore_mountains_random02", AssetIndex = 22, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Wood", 50 }, { "GreydwarfEye", 3 } } } },
            new Rune { Name = "Slow", EffectClass = typeof(SlowRuneEffect), DiscoveryToken = "$lore_mountains_random03", AssetIndex = 23, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "FreezeGland", 3 }, { "Silver", 1 } } } },
            new Rune { Name = "Transmute", _FixedEffect = new AlchemyRuneEffect(){ itemAName="$item_tin", itemAPrefabName="Tin", itemBName="$item_copper", itemBPrefabName="Copper", ratio=2, alertMessage="Transmuted Copper/Tin", reversible=true },
                DiscoveryToken = "$lore_mountains_random04", AssetIndex = 24, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Coal", 10 }, { "GreydwarfEye", 3 } } } },
            new Rune { Name = "Heal", EffectClass = typeof(HealRuneEffect), DiscoveryToken = "$lore_mountains_random05", AssetIndex = 25, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "MeadHealthMinor", 3 }, { "Thistle", 3 } } } },
            new Rune { Name = "Weather", EffectClass = typeof(WeatherRuneEffect), DiscoveryToken = "$lore_mountains_random06", AssetIndex = 26, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "GreydwarfEye", 5 }, { "TrophyDeer", 2 } } } },
            new Rune { Name = "Quake", EffectClass = typeof(QuakeRuneEffect), DiscoveryToken = "$lore_mountains_random07", AssetIndex = 27, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Crystal", 2 } } } }, //stagger at range, requires aoe
            // Plains runes
            new Rune { Name = "Fire", EffectClass = typeof(FireRuneEffect), DiscoveryToken = "$lore_plains_random01", AssetIndex = 28, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Coal", 10 }, { "AmberPearl", 2 }, { "SurtlingCore", 2 } } } },
            new Rune { Name = "Wall", EffectClass = typeof(WallRuneEffect), DiscoveryToken = "$lore_plains_random02", AssetIndex = 29, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Wood", 20 }, { "ShieldBanded", 1 } } } },
            new Rune { Name = "Charm", EffectClass = typeof(CharmRuneEffect), DiscoveryToken = "$lore_plains_random03", AssetIndex = 30, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Honey", 10 }, { "SurtlingCore", 1 } } } }, //requires projectile/aoe
            new Rune { Name = "Pyrolyze", _FixedEffect = new AlchemyRuneEffect(){ itemAName="$item_wood", itemAPrefabName="Wood", itemBName="$item_coal", itemBPrefabName="Coal", ratio=4, alertMessage="Pyrolyzed Wood=>Coal", reversible=false },
                DiscoveryToken = "$lore_plains_random04", AssetIndex = 31, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "SurtlingCore", 1 }, { "GreydwarfEye", 3 } } } },
            new Rune { Name = "Ward", EffectClass = typeof(WardRuneEffect), DiscoveryToken = "$lore_plains_random05", AssetIndex = 32, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "FineWood", 10 }, { "SurtlingCore", 1 }, { "TrophyGoblin", 1 } } } },
            new Rune { Name = "Teleport", EffectClass = typeof(TeleportRuneEffect), DiscoveryToken = "$lore_plains_random06", AssetIndex = 33, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "GreydwarfEye", 15 }, { "TrophyWraith", 1 } } } }, //requires magic projectile
            new Rune { Name = "FinalPlaceHolder4", EffectClass = typeof(LightRuneEffect), DiscoveryToken = "$lore_plains_random07", AssetIndex = 34, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Raspberry", 5 }, { "GreydwarfEye", 5 } } } }
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
