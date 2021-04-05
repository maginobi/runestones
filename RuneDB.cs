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
            new Rune { Name = "Transmute", Effect = new TransmuteRuneEffect(), Desc = "Transmutes metals in inventory\nCopper &lt;---&gt; Tin", DiscoveryToken = "$lore_mountain_random05", AssetIndex = 23, SimpleRecipe = new Dictionary<string, int> { { "Stone", 20 }, { "Raspberry", 5 }, { "GreydwarfEye", 5 } } },
            new Rune { Name = "Wall", Effect = new WallRuneEffect(), Desc = "Keeps out your enemies", DiscoveryToken = "$lore_swamp_random01", AssetIndex = 16, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Home", Effect = new HouseRuneEffect(), Desc = "Keeps out your enemies", DiscoveryToken = "$lore_swamp_random02", AssetIndex = 17, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Animate", Effect = new AnimateRuneEffect(), Desc = "Hello darkness, my old friend", DiscoveryToken = "$lore_swamp_random03", AssetIndex = 15, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } },
            new Rune { Name = "Bees", Effect = new BeeRuneEffect(), Desc = "My God, it's full of bees...", DiscoveryToken = "$lore_meadows_random03", AssetIndex = 5, SimpleRecipe =  new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 }, { "Honey", 20 } } },
            new Rune { Name = "Revive", Effect = new ReviveRuneEffect(), Desc = "After a recent death, restores 50% of skill knowledge lost and grants +75% movement speed", AssetIndex = 10, SimpleRecipe = new Dictionary<string, int> { { "Stone", 10 }, { "Feathers", 10 } } }
            //new Rune { Name = "Timber", Effect = new TimberRuneEffect() } //requires projectile/aoe
            //new Rune { Name = "Slow", Effect = new SlowRuneEffect() } //requires projectile/aoe
            //new Rune { Name = "Sunder", Effect = new SunderRuneEffect() } //requires projectile/aoe
            //new Rune { Name = "Fear", Effect = new FearRuneEffect() } //requires projectile/aoe
            //new Rune { Name = "Charm", Effect = new CharmRuneEffect() } //requires projectile/aoe
            //new Rune { Name = "Cultivate", Effect = new FarmRuneEffect() } //requires projectile/aoe
            //new Rune { Name = "Teleport", Effect = new TeleportRuneEffect() } //requires projectile/aoe
            //new Rune { Name = "Shadow", Effect = new ShadowRuneEffect() } //darkens an area, making stealth easier
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
