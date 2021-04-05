using Runestones.RuneEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones
{
    public static class SERegistrar
    {
        public static void RegisterStatusEffects()
        {
            ObjectDB.instance.m_StatusEffects.Add(ScriptableObject.CreateInstance<CurseRuneEffect.SE_Curse>());
            ObjectDB.instance.m_StatusEffects.Add(ScriptableObject.CreateInstance<FeatherRuneEffect.SE_Feather>());
            ObjectDB.instance.m_StatusEffects.Add(ScriptableObject.CreateInstance<AnimateRuneEffect.SE_Necromancer>());
            ObjectDB.instance.m_StatusEffects.Add(ScriptableObject.CreateInstance<ReviveRuneEffect.SE_Revivify>());
        }
    }
}
