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
        private static Dictionary< Guid, HashSet<ExtendedStatusEffect> > instances = new Dictionary<Guid, HashSet<ExtendedStatusEffect>>();
        private static void Register(ExtendedStatusEffect statusEffect)
        {
            ObjectDB.instance.m_StatusEffects.Add(statusEffect);
        }

        public static T Create<T>() where T : ExtendedStatusEffect, new()
        {
            var newId = Guid.NewGuid();
            if(instances == null)
                instances = new Dictionary<Guid, HashSet<ExtendedStatusEffect>>();
            instances[newId] = new HashSet<ExtendedStatusEffect>();
            var newSE = ScriptableObject.CreateInstance<T>();
            newSE.Id = newId;
            newSE.name = "SEext_"+ newId.ToString();
            Register(newSE);
            return newSE;
        }

        public void Awake()
        {
            instances[Id].Add(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            instances[Id].Remove(this);
            if (instances.Count <= 1)
                ObjectDB.instance.m_StatusEffects.RemoveAll(se => se.name == this.name);
        }

        public override void Stop()
        {
            base.Stop();
            instances[Id].Remove(this);
            if (instances.Count <= 1)
                ObjectDB.instance.m_StatusEffects.RemoveAll(se => se.name == this.name);
        }
    }
}
