using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones
{
    public static class DebuffVfx
    {
        private const string baseStatusVfxPath = "Assets/Runestones/vfx_active_debuff.prefab";
        const string baseAoeVfxPath = "Assets/Runestones/vfx_aoe_debuff.prefab";

        public static GameObject ConstructStatusVfx()
        {
            var basePrefab = RunestonesMod.GetLoadedAssets().LoadAsset<GameObject>(baseStatusVfxPath);

            return GameObject.Instantiate(basePrefab);
        }

        public static GameObject ConstructAoeVfx()
        {
            var basePrefab = RunestonesMod.GetLoadedAssets().LoadAsset<GameObject>(baseAoeVfxPath);

            return GameObject.Instantiate(basePrefab);
        }
    }
}
