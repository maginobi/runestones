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
            new Rune { Name = "Hearth", Effect = new HearthRuneEffect(), Desc = "A warm fire is magic in and of itself", DiscoveryToken = "$lore_meadows_random01", AssetIndex = 0, SimpleRecipe = new Dictionary<string, int>{ { "Stone", 50 }, { "Wood", 25 } } },
            new Rune { Name = "Force", Effect = new ForceRuneEffect(), Desc = "FUS RO DAH!!!", DiscoveryToken = "$lore_meadows_random06", AssetIndex = 1, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Weather", Effect = new WeatherRuneEffect(), Desc = "So, you want to harness lightning. Best start with a stiff breeze", DiscoveryToken = "$lore_meadows_random04", AssetIndex = 2, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 5 }, { "TrophyDeer", 2 }, { "GreydwarfEye", 1 } } },
            new Rune { Name = "Curse", Effect = new CurseRuneEffect(), Desc = "Revenge is a dish best served with raspberries", DiscoveryToken = "$lore_meadows_random02", AssetIndex = 3, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "Feather", Effect = new FeatherRuneEffect(), Desc = "I'm sure Hugin can lend you some", DiscoveryToken = "$lore_meadows_random05", AssetIndex = 4, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 20 } } },
            new Rune { Name = "Fire", Effect = new FireRuneEffect(), Desc = "The trouble with fire is not how to make it, but how to stop", DiscoveryToken = "$lore_plains_random06", AssetIndex = 29, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "Ice", Effect = new IceRuneEffect(), Desc = "The breath of the dragon", DiscoveryToken = "$lore_mountain_random06", AssetIndex = 22, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "Wall", Effect = new WallRuneEffect(), Desc = "Keeps out your enemies", DiscoveryToken = "$lore_swamp_random01", AssetIndex = 16, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Home", Effect = new HouseRuneEffect(), Desc = "Keeps out your enemies", DiscoveryToken = "$lore_swamp_random02", AssetIndex = 17, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Animate", Effect = new AnimateRuneEffect(), Desc = "Hello darkness, my old friend", DiscoveryToken = "$lore_swamp_random03", AssetIndex = 15, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Bees", Effect = new BeeRuneEffect(), Desc = "My God, it's full of bees...", DiscoveryToken = "$lore_meadows_random03", AssetIndex = 5, SimpleRecipe =  new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 }, { "Honey", 20 } } },
            new Rune { Name = "Revive", Effect = new ReviveRuneEffect(), Desc = "After a recent death, restores 50% of skill knowledge lost and grants +75% movement speed", AssetIndex = 10, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Heal", Effect = new HealRuneEffect(), Desc = "Heal", DiscoveryToken = "disabled", AssetIndex = 12, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Slow", Effect = new SlowRuneEffect(), Desc = "Slow 90%", DiscoveryToken = "$lore_plains_random01", AssetIndex = 19, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Sunder", Effect = new SunderRuneEffect(), Desc = "Weakens enemy resistances to physical damage", DiscoveryToken = "$lore_plains_random02", AssetIndex = 20, SimpleRecipe  = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Charm", Effect = new CharmRuneEffect(), Desc = "Charms an enemy for 30 seconds", DiscoveryToken = "$lore_plains_random03", AssetIndex = 30, SimpleRecipe  = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } }, //requires projectile/aoe
            new Rune { Name = "Fear", Effect = new FearRuneEffect(), Desc = "Forces an enemy to flee for 30 seconds", DiscoveryToken = "$lore_plains_random04", AssetIndex = 31, SimpleRecipe  = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } }, //requires aoe
            new Rune { Name = "Quake", Effect = new QuakeRuneEffect(), Desc = "Stagger at range", DiscoveryToken = "$lore_plains_random05", AssetIndex = 32, SimpleRecipe  = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } }, //stagger at range, requires aoe
            new Rune { Name = "Index", Effect = new IndexRuneEffect(), Desc = "Quick and easy spellcasting", DiscoveryToken = "$lore_plains_random06", AssetIndex = 33, SimpleRecipe  = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } }, //rearranges hotkeys for easier spellcasting, inventory effect
            new Rune { Name = "Transmute", Effect = new AlchemyRuneEffect(){ itemAName="$item_tin", itemAPrefabName="Tin", itemBName="$item_copper", itemBPrefabName="Copper", ratio=2, alertMessage="Transmuted Copper/Tin", reversible=true },
                Desc = "Transmutes metals in inventory\n2 Tin <===> Copper", DiscoveryToken = "$lore_mountain_random05", AssetIndex = 23, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "Pyrolyze", Effect = new AlchemyRuneEffect(){ itemAName="$item_wood", itemAPrefabName="Wood", itemBName="$item_coal", itemBPrefabName="Coal", ratio=4, alertMessage="Pyrolyzed Wood=>Coal", reversible=false },
                Desc = "Pyrolyzes items in inventory\n4 Wood ===> Coal", DiscoveryToken = "$lore_mountain_random06", AssetIndex = 24, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "Harden", Effect = new AlchemyRuneEffect(){ itemAName="$item_resin", itemAPrefabName="Resin", itemBName="$item_amber", itemBPrefabName="Amber", ratio=25, alertMessage="Hardened Resin=>Amber", reversible=false },
                Desc = "Hardens items in inventory\n25 Resin ===> Amber", DiscoveryToken = "$lore_mountain_random07", AssetIndex = 25, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "Cultivate", Effect = new FarmRuneEffect(), Desc = "Hardens items in inventory\n25 Resin ===> Amber", DiscoveryToken = "$lore_mountain_random08", AssetIndex = 26, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } } //requires aoe
            //new Rune { Name = "Pocket", Effect = new PocketRuneEffect() } //access small extradimensional chest, inventory effect
            //new Rune { Name = "Teleport", Effect = new TeleportRuneEffect() } //requires magic projectile
            //new Rune { Name = "Timber", Effect = new TimberRuneEffect() } //requires projectile
            //new Rune { Name = "Shadow", Effect = new ShadowRuneEffect() } //darkens an area, making stealth easier, requires aoe
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
