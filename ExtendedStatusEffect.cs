using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones
{
    public class ExtendedStatusEffect : SE_Stats
    {
        private Guid Id;
        private static void Register(ExtendedStatusEffect statusEffect)
        {
            ObjectDB.instance.m_StatusEffects.Add(statusEffect);
        }

        public static T Create<T>() where T : ExtendedStatusEffect, new()
        {
            var newSE = ScriptableObject.CreateInstance<T>();
            newSE.Id = Guid.NewGuid();
            newSE.name = "SEext_"+newSE.Id.ToString();
            Register(newSE);
            return newSE;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            ObjectDB.instance.m_StatusEffects.RemoveAll(se => se.name == this.name);
        }

        public override void Stop()
        {
            base.Stop();
            ObjectDB.instance.m_StatusEffects.RemoveAll(se => se.name == this.name);
        }
    }
}
