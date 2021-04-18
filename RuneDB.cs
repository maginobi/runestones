using Runestones.RuneEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            new Rune { Name = "Hearth", EffectClass = typeof(HearthRuneEffect), Desc = "A warm fire is magic in and of itself", DiscoveryToken = "$lore_meadows_random01", AssetIndex = 0, SimpleRecipe = new Dictionary<string, int>{ { "Stone", 50 }, { "Wood", 25 } } },
            new Rune { Name = "Force", EffectClass = typeof(ForceRuneEffect), Desc = "FUS RO DAH!!!", DiscoveryToken = "$lore_meadows_random06", AssetIndex = 1, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Weather", EffectClass = typeof(WeatherRuneEffect), Desc = "So, you want to harness lightning. Best start with a stiff breeze", DiscoveryToken = "$lore_meadows_random04", AssetIndex = 2, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 5 }, { "TrophyDeer", 2 }, { "GreydwarfEye", 1 } } },
            new Rune { Name = "Curse", EffectClass = typeof(CurseRuneEffect), Desc = "Revenge is a dish best served with raspberries", DiscoveryToken = "$lore_meadows_random02", AssetIndex = 3, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "Feather", EffectClass = typeof(FeatherRuneEffect), Desc = "I'm sure Hugin can lend you some", DiscoveryToken = "$lore_meadows_random05", AssetIndex = 4, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 20 } } },
            new Rune { Name = "Fire", EffectClass = typeof(FireRuneEffect), Desc = "The trouble with fire is not how to make it, but how to stop", DiscoveryToken = "$lore_plains_random06", AssetIndex = 29, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "Ice", EffectClass = typeof(IceRuneEffect), Desc = "The breath of the dragon", DiscoveryToken = "$lore_mountain_random06", AssetIndex = 22, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "Wall", EffectClass = typeof(WallRuneEffect), Desc = "Keeps out your enemies", DiscoveryToken = "$lore_swamp_random01", AssetIndex = 16, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Home", EffectClass = typeof(HouseRuneEffect), Desc = "Keeps out your enemies", DiscoveryToken = "$lore_swamp_random02", AssetIndex = 17, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Animate", EffectClass = typeof(AnimateRuneEffect), Desc = "Hello darkness, my old friend", DiscoveryToken = "$lore_swamp_random03", AssetIndex = 15, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Bees", EffectClass = typeof(BeeRuneEffect), Desc = "My God, it's full of bees...", DiscoveryToken = "$lore_meadows_random03", AssetIndex = 5, SimpleRecipe =  new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 }, { "Honey", 20 } } },
            new Rune { Name = "Revive", EffectClass = typeof(ReviveRuneEffect), Desc = "After a recent death, restores 50% of skill knowledge lost and grants +75% movement speed", AssetIndex = 10, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Heal", EffectClass = typeof(HealRuneEffect), Desc = "Heal", DiscoveryToken = "disabled", AssetIndex = 12, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Slow", EffectClass = typeof(SlowRuneEffect), Desc = "Slow 90%", DiscoveryToken = "$lore_plains_random01", AssetIndex = 19, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Sunder", EffectClass = typeof(SunderRuneEffect), Desc = "Weakens enemy resistances to physical damage", DiscoveryToken = "$lore_plains_random02", AssetIndex = 20, SimpleRecipe  = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Charm", EffectClass = typeof(CharmRuneEffect), Desc = "Charms an enemy for 30 seconds", DiscoveryToken = "$lore_plains_random03", AssetIndex = 30, SimpleRecipe  = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } }, //requires projectile/aoe
            new Rune { Name = "Fear", EffectClass = typeof(FearRuneEffect), Desc = "Forces an enemy to flee for 30 seconds", DiscoveryToken = "$lore_plains_random04", AssetIndex = 31, SimpleRecipe  = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } }, //requires aoe
            new Rune { Name = "Quake", EffectClass = typeof(QuakeRuneEffect), Desc = "Stagger at range", DiscoveryToken = "$lore_plains_random05", AssetIndex = 32, SimpleRecipe  = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } }, //stagger at range, requires aoe
            new Rune { Name = "Index", EffectClass = typeof(IndexRuneEffect), Desc = "Quick and easy spellcasting", DiscoveryToken = "$lore_plains_random06", AssetIndex = 33, SimpleRecipe  = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } }, //rearranges hotkeys for easier spellcasting, inventory effect
            new Rune { Name = "Transmute", _FixedEffect = new AlchemyRuneEffect(){ itemAName="$item_tin", itemAPrefabName="Tin", itemBName="$item_copper", itemBPrefabName="Copper", ratio=2, alertMessage="Transmuted Copper/Tin", reversible=true },
                Desc = "Transmutes metals in inventory\n2 Tin <===> Copper", DiscoveryToken = "$lore_mountain_random05", AssetIndex = 23, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "Pyrolyze", _FixedEffect = new AlchemyRuneEffect(){ itemAName="$item_wood", itemAPrefabName="Wood", itemBName="$item_coal", itemBPrefabName="Coal", ratio=4, alertMessage="Pyrolyzed Wood=>Coal", reversible=false },
                Desc = "Pyrolyzes items in inventory\n4 Wood ===> Coal", DiscoveryToken = "$lore_mountain_random06", AssetIndex = 24, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "Harden", _FixedEffect = new AlchemyRuneEffect(){ itemAName="$item_resin", itemAPrefabName="Resin", itemBName="$item_amber", itemBPrefabName="Amber", ratio=25, alertMessage="Hardened Resin=>Amber", reversible=false },
                Desc = "Hardens items in inventory\n25 Resin ===> Amber", DiscoveryToken = "$lore_mountain_random07", AssetIndex = 25, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "Cultivate", EffectClass = typeof(FarmRuneEffect), Desc = "Hardens items in inventory\n25 Resin ===> Amber", DiscoveryToken = "$lore_mountain_random08", AssetIndex = 26, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } }, //requires aoe
            new Rune { Name = "Pocket", EffectClass = typeof(PocketRuneEffect), Desc = "Extradimensional storage", DiscoveryToken = "$lore_mountain_random09", AssetIndex = 27, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } }, //access small extradimensional chest, inventory effect
            new Rune { Name = "Teleport", EffectClass = typeof(TeleportRuneEffect), Desc = "Short-range teleport", DiscoveryToken = "$lore_mountain_random09", AssetIndex = 28, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } }, //requires magic projectile
            new Rune { Name = "Boat", EffectClass = typeof(BoatRuneEffect), Desc = "Conjure a boat", DiscoveryToken = "$lore_mountain_random09", AssetIndex = 29, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "Timber", EffectClass = typeof(TimberRuneEffect), Desc = "Fells trees", DiscoveryToken = "$lore_mountain_random09", AssetIndex = 30, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } }, //requires projectile
            new Rune { Name = "Light", EffectClass = typeof(LightRuneEffect), Desc = "Let there be light", DiscoveryToken = "$lore_mountain_random09", AssetIndex = 31, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "Ward", EffectClass = typeof(WardRuneEffect), Desc = "Powerful ward", DiscoveryToken = "$lore_mountain_random09", AssetIndex = 32, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "Darkness", EffectClass = typeof(DarknessRuneEffect), Desc = "Hello darkness my old friend...", DiscoveryToken = "$lore_mountain_random09", AssetIndex = 33, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } }, //darkens an area, making stealth easier, requires area effect
            new Rune { Name = "FinalPlaceHolder1", EffectClass = typeof(LightRuneEffect), Desc = "Placeholder rune 1", DiscoveryToken = "$lore_mountain_random09", AssetIndex = 34, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "FinalPlaceHolder2", EffectClass = typeof(LightRuneEffect), Desc = "Placeholder rune 2", DiscoveryToken = "$lore_mountain_random09", AssetIndex = 34, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "FinalPlaceHolder3", EffectClass = typeof(LightRuneEffect), Desc = "Placeholder rune 3", DiscoveryToken = "$lore_mountain_random09", AssetIndex = 34, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "FinalPlaceHolder4", EffectClass = typeof(LightRuneEffect), Desc = "Placeholder rune 4", DiscoveryToken = "$lore_mountain_random09", AssetIndex = 34, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } }
        };

        public Dictionary<string, Rune> RunesByName = new Dictionary<string, Rune>();

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
}
