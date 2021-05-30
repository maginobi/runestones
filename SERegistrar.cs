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
            ObjectDB.instance.m_StatusEffects.Add(ScriptableObject.CreateInstance<FeatherRuneEffect.SE_Feather>());
            ObjectDB.instance.m_StatusEffects.Add(ScriptableObject.CreateInstance<FeatherRuneEffect.SE_Flight>());
            ObjectDB.instance.m_StatusEffects.Add(ScriptableObject.CreateInstance<AnimateRuneEffect.SE_Necromancer>());
            ObjectDB.instance.m_StatusEffects.Add(ScriptableObject.CreateInstance<ReviveRuneEffect.SE_Revivify>());
            ObjectDB.instance.m_StatusEffects.Add(ScriptableObject.CreateInstance<FearRuneEffect.SE_Fear>());
            ObjectDB.instance.m_StatusEffects.Add(ScriptableObject.CreateInstance<LightRuneEffect.SE_Light>());
            ObjectDB.instance.m_StatusEffects.Add(ScriptableObject.CreateInstance<DarknessRuneEffect.SE_DarknessStealth>());
            ObjectDB.instance.m_StatusEffects.Add(ScriptableObject.CreateInstance<DarknessRuneEffect.SE_DarknessStealthQuiet>());
            ObjectDB.instance.m_StatusEffects.Add(ScriptableObject.CreateInstance<IndexRuneEffect.SE_Study>());
            ObjectDB.instance.m_StatusEffects.Add(ScriptableObject.CreateInstance<IndexRuneEffect.SE_Scholar>());
            ObjectDB.instance.m_StatusEffects.Add(ScriptableObject.CreateInstance<VineRuneEffect.SE_Druid>());
            ObjectDB.instance.m_StatusEffects.Add(ScriptableObject.CreateInstance<DispelRuneEffect.SE_DispelResist>());
            ObjectDB.instance.m_StatusEffects.Add(ScriptableObject.CreateInstance<WraithRuneEffect.SE_Wraith>());
            /*
            Debug.Log("Status effects registered, listing all status effects:");
            foreach(var statusEffect in ObjectDB.instance.m_StatusEffects)
            {
                Debug.Log(statusEffect.name);
            }
            */
        }
    }
}
