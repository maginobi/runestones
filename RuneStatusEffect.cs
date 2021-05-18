using HarmonyLib;
using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace Runestones
{
    public class RuneStatusEffect : SE_Stats
    {
        private static XmlAttributeOverrides GetSerializationOverrides()
        {
            XmlAttributeOverrides xOver = new XmlAttributeOverrides();

            XmlAttributes attrs = new XmlAttributes();
            attrs.XmlIgnore = true;
            xOver.Add(typeof(StatusEffect), "m_icon", attrs);

            attrs = new XmlAttributes();
            attrs.XmlIgnore = true;
            xOver.Add(typeof(StatusEffect), "m_startEffects", attrs);

            attrs = new XmlAttributes();
            attrs.XmlIgnore = true;
            xOver.Add(typeof(StatusEffect), "m_stopEffects", attrs);

            return xOver;
        }

        public string Serialize()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(this.GetType(), GetSerializationOverrides());
                using (var writer = new StringWriter())
                {
                    serializer.Serialize(writer, this);
                    return $"{this.GetType().FullName}|{writer.ToString()}";
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError(e.InnerException);
                Debug.LogError(e.InnerException.Message);
                throw e;
            }
        }

        public static RuneStatusEffect Deserialize(Type effectType, string data)
        {
            XmlSerializer serializer = new XmlSerializer(effectType, GetSerializationOverrides());
            using (var reader = new StringReader(data))
            {
                return (RuneStatusEffect)serializer.Deserialize(reader);
            }
        }
    }

    [HarmonyPatch(typeof(ObjectDB), "GetStatusEffect")]
    public static class GetRuneStatusEffectMod
    {
        public static void Postfix(ref StatusEffect __result, string name)
        {
            if (name.Contains("|"))
            {
                var effectType = Type.GetType(name.Substring(0, name.IndexOf("|")));
                if (effectType != null && effectType.IsSubclassOf(typeof(RuneStatusEffect)))
                {
                    __result = RuneStatusEffect.Deserialize(effectType, name.Substring(name.IndexOf("|")+1));
                }
            }
        }
    }
}
