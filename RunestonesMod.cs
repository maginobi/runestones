using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Runestones
{
    [BepInPlugin("Maginobi.Runestones", "Runestones", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class RunestonesMod : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("Maginobi.Runestones");

        public const string AssetBundleName = "runestones";
        public static AssetBundle ModAssetBundle;

        void Awake()
        {
            LoadAssets();
            harmony.PatchAll();
        }

        static void LoadAssets()
        {
            ModAssetBundle = GetAssetBundleFromResources(AssetBundleName);

            Staff.Load(ModAssetBundle);
            RuneLoader.Instance.Load();
            ItemHelper.Instance.OnAllItemsLoaded(SERegistrar.RegisterStatusEffects);
        }

        public static AssetBundle GetLoadedAssets()
        {
            return ModAssetBundle;
        }

        public static AssetBundle GetAssetBundleFromResources(string fileName)
        {
            var execAssembly = Assembly.GetExecutingAssembly();

            var resourceName = execAssembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(fileName));

            using (var stream = execAssembly.GetManifestResourceStream(resourceName))
            {
                return AssetBundle.LoadFromStream(stream);
            }
        }

    }
}