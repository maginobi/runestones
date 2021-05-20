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
            new Rune { Name = "Force", EffectClass = typeof(ForceRuneEffect), DiscoveryToken = "$lore_meadows_random06", AssetIndex = 0, Reagents =
                    new List<Dictionary<string, int>>{
                        new Dictionary<string, int> { { "TrophyBoar", 2 } },
                        new Dictionary<string, int> { { "TrophyBoar", 5 }, { "Flint", 5 } },
                        new Dictionary<string, int> { { "TrophyForestTroll", 1 }, { "Obsidian", 5 } }
                    }
                },
            new Rune { Name = "Hearth", EffectClass = typeof(HearthRuneEffect), DiscoveryToken = "$lore_meadows_random01", AssetIndex = 1, Reagents =
                    new List<Dictionary<string, int>>{
                        new Dictionary<string, int>{ { "Stone", 20 }, { "Wood", 15 } },
                        new Dictionary<string, int>{ { "ElderBark", 5 }, { "RoundLog", 10 }, { "FineWood", 10 }, { "SurtlingCore", 1 } },
                        new Dictionary<string, int>{ { "Coal", 20 }, { "Obsidian", 5 } }
                    }
                },
            new Rune { Name = "Curse", EffectClass = typeof(CurseRuneEffect), DiscoveryToken = "$lore_meadows_random02", AssetIndex = 2, Reagents =
                    new List<Dictionary<string, int>>{
                        new Dictionary<string, int> { { "Raspberry", 5 } },
                        new Dictionary<string, int> { { "Raspberry", 10 }, { "GreydwarfEye", 3 } },
                        new Dictionary<string, int> { { "Raspberry", 15 }, { "GreydwarfEye", 5 }, { "Ooze", 1 } }
                    }
                },
            new Rune { Name = "Index", EffectClass = typeof(IndexRuneEffect), DiscoveryToken = "$lore_meadows_random03", AssetIndex = 3, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "GreydwarfEye", 1 }, { "Feathers", 1 } },
                        new Dictionary < string, int > { { "GreydwarfEye", 10 }, { "Thistle", 10 } },
                        new Dictionary < string, int > { { "GreydwarfEye", 20 }, { "Thistle", 20 }, { "LinenThread", 20 } }
                    }
                }, //rearranges hotkeys for easier spellcasting, inventory effect
            new Rune { Name = "Light", EffectClass = typeof(LightRuneEffect), DiscoveryToken = "$lore_mountain_random04", AssetIndex = 4, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "Resin", 5 }, { "Dandelion", 3 } },
                        new Dictionary < string, int > { { "Coal", 5 }, { "Dandelion", 3 } },
                        new Dictionary < string, int > { { "Coal", 5 }, { "SurtlingCore", 3 } }
                    }
                },
            new Rune { Name = "Feather", EffectClass = typeof(FeatherRuneEffect), DiscoveryToken = "$lore_meadows_random05", AssetIndex = 5, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "Feathers", 10 } },
                        new Dictionary < string, int > { { "Feathers", 20 }, { "FineWood", 5 } },
                        new Dictionary < string, int > { { "Feathers", 30 }, { "Thistle", 5 }, { "TrophyHatchling", 1 } }
                    }
                },
            // Forest runes
            new Rune { Name = "Timber", EffectClass = typeof(TimberRuneEffect), DiscoveryToken = "$lore_blackforest_random01", AssetIndex = 7, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "BeechSeeds", 8 }, { "GreydwarfEye", 10 } },
                        new Dictionary < string, int > { { "CarrotSeeds", 8 }, { "TrophyGreydwarf", 1 } },
                        new Dictionary < string, int > { { "TurnipSeeds", 8 }, { "Bronze", 1 } }
                    }
                },
            new Rune { Name = "Boat", EffectClass = typeof(BoatRuneEffect), DiscoveryToken = "$lore_blackforest_random02", AssetIndex = 8, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "Wood", 50 }, { "LeatherScraps", 30 } },
                        new Dictionary < string, int > { { "FineWood", 50 }, { "DeerHide", 30 }, { "Bronze", 10 } },
                        new Dictionary < string, int > { { "FineWood", 50 }, { "DeerHide", 30 }, { "Iron", 10 } }
                    }
                },
            new Rune { Name = "Sunder", EffectClass = typeof(SunderRuneEffect), DiscoveryToken = "$lore_blackforest_random03", AssetIndex = 9, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "Flint", 8 }, { "GreydwarfEye", 3 } },
                        new Dictionary < string, int > { { "Iron", 2 }, { "GreydwarfEye", 6 } },
                        new Dictionary < string, int > { { "Silver", 1 }, { "SurtlingCore", 1 }, { "FreezeGland", 1 }, { "Ooze", 1 } }
                    }
                },
            new Rune { Name = "Pyrolyze", _FixedEffect = new AlchemyRuneEffect(){
                _EffectText = new List<string>{ "Makes items more flamable" },
                _FlavorText = "The alchemic prime of sulphur governs combustion and the fire of the soul",
                conversionList = new List<AlchemyRuneEffect.Conversion> {
                    new AlchemyRuneEffect.Conversion { itemAName="$item_wood", itemAPrefabName="Wood", itemBName="$item_resin", itemBPrefabName="Resin", ratio=1, reversible=false },
                    new AlchemyRuneEffect.Conversion { itemAName="$item_wood", itemAPrefabName="Wood", itemBName="$item_coal", itemBPrefabName="Coal", ratio=3, reversible=false },
                    new AlchemyRuneEffect.Conversion { itemAName="$item_iron", itemAPrefabName="Iron", itemBName="$item_flametal", itemBPrefabName="Flametal", ratio=3, reversible=false }
                } },
                DiscoveryToken = "$lore_forest_random04", AssetIndex = 10, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "SurtlingCore", 1 }, { "GreydwarfEye", 3 } },
                        new Dictionary < string, int > { { "SurtlingCore", 1 }, { "GreydwarfEye", 6 } },
                        new Dictionary < string, int > { { "SurtlingCore", 3 }, { "GreydwarfEye", 9 } }
                    }
                },
            new Rune { Name = "Cultivate", EffectClass = typeof(FarmRuneEffect), DiscoveryToken = "$lore_blackforest_random05", AssetIndex = 11, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "AncientSeed", 1 }, { "BeechSeeds", 10 } },
                        new Dictionary < string, int > { { "AncientSeed", 1 }, { "CarrotSeeds", 10 }, { "TrophyGreydwarf", 1 } },
                        new Dictionary < string, int > { { "AncientSeed", 1 }, { "TurnipSeeds", 10 }, { "TrophyGreydwarfBrute", 1 } }
                    }
                },
            new Rune { Name = "Bees", EffectClass = typeof(BeeRuneEffect), DiscoveryToken = "$lore_blackforest_random06", AssetIndex = 12, Reagents =
                    new List<Dictionary<string, int>>{
                        new Dictionary<string, int> { { "Honey", 10 }, { "Thistle", 2 } },
                        new Dictionary<string, int> { { "Honey", 8 }, { "Thistle", 4 } },
                        new Dictionary<string, int> { { "Honey", 8 }, { "Thistle", 6 }, { "Ooze", 1 } }
                    }
                },
            new Rune { Name = "FinalPlaceHolder1", EffectClass = typeof(LightRuneEffect), DiscoveryToken = "$lore_blackforest_random07", AssetIndex = 13, Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Raspberry", 5 }, { "GreydwarfEye", 5 } } } },
            // Swamp runes
            new Rune { Name = "Animate", EffectClass = typeof(AnimateRuneEffect), DiscoveryToken = "$lore_swamp_random01", AssetIndex = 14, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "BoneFragments", 5 }, { "Ruby", 1 } },
                        new Dictionary < string, int > { { "BoneFragments", 5 }, { "Ruby", 2 }, { "TrophySkeleton", 1 } },
                        new Dictionary < string, int > { { "Entrails", 10 }, { "Ruby", 3 }, { "WitheredBone", 1 } }
                    }
                },
            new Rune { Name = "FinalPlaceHolder2", EffectClass = typeof(LightRuneEffect), DiscoveryToken = "$lore_swamp_random02", AssetIndex = 15,  Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Raspberry", 5 }, { "GreydwarfEye", 5 } } } },
            new Rune { Name = "Fear", EffectClass = typeof(FearRuneEffect), DiscoveryToken = "$lore_swamp_random03", AssetIndex = 16, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "TrophyBlob", 1 }, { "GreydwarfEye", 3 } },
                        new Dictionary < string, int > { { "TrophyBlob", 1 }, { "TrophyDraugr", 1 }, { "GreydwarfEye", 6 } },
                        new Dictionary < string, int > { { "TrophyBonemass", 1 }, { "GreydwarfEye", 9 } }
                    }
                }, //requires aoe
            new Rune { Name = "FinalPlaceHolder3", EffectClass = typeof(LightRuneEffect), DiscoveryToken = "$lore_swamp_random04", AssetIndex = 17,  Reagents = new List < Dictionary < string, int > > { new Dictionary < string, int > { { "Raspberry", 5 }, { "GreydwarfEye", 5 } } } },
            new Rune { Name = "Revive", EffectClass = typeof(ReviveRuneEffect), DiscoveryToken = "$lore_swamp_random05", AssetIndex = 18, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "TrophyGreydwarfShaman", 1 } },
                        new Dictionary < string, int > { { "TrophyGreydwarfShaman", 1 }, { "AncientSeed", 3 } },
                        new Dictionary < string, int > { { "TrophyTheElder", 1 }, { "AncientSeed", 6 } }
                    }
                },
            new Rune { Name = "Pocket", EffectClass = typeof(PocketRuneEffect), DiscoveryToken = "$lore_swamp_random06", AssetIndex = 19, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "FineWood", 10 }, { "GreydwarfEye", 3 } },
                        new Dictionary < string, int > { { "FineWood", 10 }, { "GreydwarfEye", 6 }, { "Iron", 3 } },
                        new Dictionary < string, int > { { "FineWood", 10 }, { "GreydwarfEye", 9 }, { "Iron", 6 } }
                    }
                }, //access small extradimensional chest, inventory effect
            new Rune { Name = "Darkness", EffectClass = typeof(DarknessRuneEffect), DiscoveryToken = "$lore_swamp_random07", AssetIndex = 20, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "Coal", 10 }, { "Guck", 3 } },
                        new Dictionary < string, int > { { "Coal", 20 }, { "Guck", 3 }, { "TrollHide", 3 } },
                        new Dictionary < string, int > { { "Obsidian", 6 }, { "Guck", 9 }, { "Feathers", 12 } }
                    }
                }, //darkens an area, making stealth easier, requires area effect
            // Mountain runes
            new Rune { Name = "Ice", EffectClass = typeof(IceRuneEffect), DiscoveryToken = "$lore_mountains_random01", AssetIndex = 21, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "FreezeGland", 10 }, { "GreydwarfEye", 3 } },
                        new Dictionary < string, int > { { "FreezeGland", 15 }, { "GreydwarfEye", 6 }, { "Obsidian", 1 } },
                        new Dictionary < string, int > { { "TrophyHatchling", 1 }, { "GreydwarfEye", 9 }, { "Obsidian", 3 } }
                    }
                },
            new Rune { Name = "Home", EffectClass = typeof(HouseRuneEffect), DiscoveryToken = "$lore_mountains_random02", AssetIndex = 22, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "Wood", 50 }, { "GreydwarfEye", 3 } },
                        new Dictionary < string, int > { { "Wood", 50 }, { "RoundLog", 10 }, { "GreydwarfEye", 10 } },
                        new Dictionary < string, int > { { "FineWood", 50 }, { "RoundLog", 50 }, { "Iron", 3 } }
                    }
                },
            new Rune { Name = "Slow", EffectClass = typeof(SlowRuneEffect), DiscoveryToken = "$lore_mountains_random03", AssetIndex = 23, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "FreezeGland", 3 }, { "Silver", 1 } },
                        new Dictionary < string, int > { { "FreezeGland", 6 }, { "Silver", 3 } },
                        new Dictionary < string, int > { { "FreezeGland", 9 }, { "Needle", 3 } }
                    }
                },
            new Rune { Name = "Transmute", _FixedEffect = new AlchemyRuneEffect(){
                _EffectText = new List<string>{ "Transmutes metals" },
                _FlavorText = "The alchemic prime of mercury governs transformation and the fluidity of the mind",
                conversionList = new List<AlchemyRuneEffect.Conversion> {
                    new AlchemyRuneEffect.Conversion { itemAName="$item_tin", itemAPrefabName="Tin", itemBName="$item_copper", itemBPrefabName="Copper", ratio=2, reversible=true },
                    new AlchemyRuneEffect.Conversion { itemAName="$item_coins", itemAPrefabName="Coins", itemBName="$item_silver", itemBPrefabName="Silver", ratio=100, reversible=true },
                    new AlchemyRuneEffect.Conversion { itemAName="$item_flametal", itemAPrefabName="Flametal", itemBName="$item_blackmetal", itemBPrefabName="BlackMetal", ratio=3, reversible=true }
                } },
                DiscoveryToken = "$lore_mountains_random04", AssetIndex = 24, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "Coal", 10 }, { "FineWood", 5 } },
                        new Dictionary < string, int > { { "Chitin", 5 }, { "Entrails", 3 } },
                        new Dictionary < string, int > { { "DragonTear", 1 }, { "Crystal", 1 } }
                    }
                },
            new Rune { Name = "Heal", EffectClass = typeof(HealRuneEffect), DiscoveryToken = "$lore_mountains_random05", AssetIndex = 25, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "MeadHealthMinor", 3 }, { "Thistle", 3 } },
                        new Dictionary<string, int> { { "MeadHealthMinor", 3 }, { "Thistle", 6 }, { "TrophyGreydwarfShaman", 1 } },
                        new Dictionary<string, int> { { "MeadHealthMinor", 3 }, { "Thistle", 9 }, { "TrophyLeech", 3 } }
                    }
                },
            new Rune { Name = "Weather", EffectClass = typeof(WeatherRuneEffect), DiscoveryToken = "$lore_mountains_random06", AssetIndex = 26, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "GreydwarfEye", 5 }, { "TrophyDeer", 2 } },
                        new Dictionary < string, int > { { "GreydwarfEye", 10 }, { "TrophyDeer", 5 }, { "Silver", 1 } },
                        new Dictionary < string, int > { { "GreydwarfEye", 15 }, { "TrophyEikthyr", 1 }, { "Silver", 5 } }
                    }
                },
            new Rune { Name = "Quake", EffectClass = typeof(QuakeRuneEffect), DiscoveryToken = "$lore_mountains_random07", AssetIndex = 27, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "Crystal", 2 } },
                        new Dictionary < string, int > { { "Obsidian", 10 }, { "Crystal", 2 } },
                        new Dictionary < string, int > { { "TrophySGolem", 1 } }
                    }
                }, //stagger at range, requires aoe
            // Plains runes
            new Rune { Name = "Fire", EffectClass = typeof(FireRuneEffect), DiscoveryToken = "$lore_plains_random01", AssetIndex = 28, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "Coal", 10 }, { "AmberPearl", 2 }, { "SurtlingCore", 2 } },
                        new Dictionary < string, int > { { "Coal", 20 }, { "AmberPearl", 4 }, { "SurtlingCore", 4 } },
                        new Dictionary < string, int > { { "Flametal", 3 }, { "Ruby", 1 }, { "TrophySurtling", 1 } }
                    }
                },
            new Rune { Name = "Wall", EffectClass = typeof(WallRuneEffect), DiscoveryToken = "$lore_plains_random02", AssetIndex = 29, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "ShieldWood", 1 } },
                        new Dictionary < string, int > { { "RoundLog", 5 }, { "ShieldBanded", 1 } },
                        new Dictionary < string, int > { { "Obsidian", 10 }, { "ShieldSilver", 1 } }
                    }
                },
            new Rune { Name = "Charm", EffectClass = typeof(CharmRuneEffect), DiscoveryToken = "$lore_plains_random03", AssetIndex = 30, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "Honey", 10 }, { "SurtlingCore", 1 } },
                        new Dictionary < string, int > { { "Honey", 10 }, { "SurtlingCore", 3 } },
                        new Dictionary < string, int > { { "Honey", 10 }, { "SurtlingCore", 3 }, { "Bloodbag", 3 } }
                    }
                },
            new Rune { Name = "Harden", _FixedEffect = new AlchemyRuneEffect(){
                _EffectText = new List<string>{ "Hardens items" },
                _FlavorText = "The alchemic prime of salt governs fixation and the strength of the body",
                conversionList = new List<AlchemyRuneEffect.Conversion> {
                    new AlchemyRuneEffect.Conversion { itemAName="$item_resin", itemAPrefabName="Resin", itemBName="$item_amber", itemBPrefabName="Amber", ratio=25, reversible=false },
                    new AlchemyRuneEffect.Conversion { itemAName="$item_copper", itemAPrefabName="Copper", itemBName="$item_bronze", itemBPrefabName="Bronze", ratio=4, reversible=false },
                    new AlchemyRuneEffect.Conversion { itemAName="$item_silver", itemAPrefabName="Silver", itemBName="$item_iron", itemBPrefabName="Iron", ratio=2, reversible=false }
                } },
                DiscoveryToken = "$lore_plains_random04", AssetIndex = 31, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "Stone", 20 }, { "Thistle", 3 } },
                        new Dictionary < string, int > { { "Flint", 15 }, { "Thistle", 3 } },
                        new Dictionary < string, int > { { "Obsidian", 10 }, { "Thistle", 3 } }
                    }
                },
            new Rune { Name = "Ward", EffectClass = typeof(WardRuneEffect), DiscoveryToken = "$lore_plains_random05", AssetIndex = 32, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "FineWood", 10 }, { "GoblinTotem", 1 } },
                        new Dictionary < string, int > { { "FineWood", 10 }, { "DragonEgg", 1 }, { "GoblinTotem", 1 } },
                        new Dictionary < string, int > { { "ShieldSerpentscale", 1 }, { "DragonEgg", 1 }, { "GoblinTotem", 1 } }
                    }
                },
            new Rune { Name = "Teleport", EffectClass = typeof(TeleportRuneEffect), DiscoveryToken = "$lore_plains_random06", AssetIndex = 33, Reagents =
                    new List < Dictionary < string, int > > {
                        new Dictionary < string, int > { { "GreydwarfEye", 10 }, { "SurtlingCore", 2 } },
                        new Dictionary < string, int > { { "GreydwarfEye", 20 }, { "TrophyWraith", 1 } },
                        new Dictionary < string, int > { { "GreydwarfEye", 40 }, { "YmirRemains", 1 } }
                    }
                }, //requires magic projectile
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
