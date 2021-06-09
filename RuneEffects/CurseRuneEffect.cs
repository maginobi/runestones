using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    public class CurseRuneEffect : RuneEffect
    {
        public const float baseDuration = 60;
        public const float baseDamageMod = 0.75f;

        public CurseRuneEffect()
        {
            _FlavorText = "Revenge is a dish best served with raspberries";
            _EffectText = new List<string> { "Reduces enemy damage dealt" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "+100% Duration" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "+100% More damage reduction" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Damage", () => $"-{1-(baseDamageMod / _Effectiveness / (_Quality==RuneQuality.Dark ? 2 : 1)) :P1}"},
                                                                    { "Duration", () => $"{baseDuration * _Effectiveness * (_Quality==RuneQuality.Ancient ? 2 : 1) :F1} sec" } };
            targetLock = true;
        }

        public override void DoMagicAttack(Attack baseAttack)
        {
            var vfxPrefab = DebuffVfx.ConstructAoeVfx();
            var gameObject = GameObject.Instantiate(vfxPrefab);
            var aoe = gameObject.AddComponent<Aoe>();

            var curseEffect = ScriptableObject.CreateInstance<SE_Curse>();
            curseEffect.m_ttl = baseDuration * _Effectiveness * (_Quality == RuneQuality.Ancient ? 2 : 1);
            curseEffect.m_damageModifier = baseDamageMod / _Effectiveness / (_Quality == RuneQuality.Dark ? 2 : 1);

            aoe.m_statusEffect = curseEffect.Serialize();
            aoe.m_ttl = 2;
            aoe.m_radius = 5;
            aoe.m_hitOwner = false;

            var go = GameObject.Instantiate(gameObject, targetLocation, Quaternion.identity);
            go.GetComponent<Aoe>().Setup(baseAttack.GetCharacter(), Vector3.zero, 0, null, null);
        }

        public class SE_Curse : RuneStatusEffect
        {
            public SE_Curse() : base()
            {
                name = "SE_Curse";
                m_name = "Cursed";
                m_tooltip = "-90% Damage dealt";
                m_startMessage = "You have been cursed";
                m_time = 0;
                m_repeatInterval = 60;
                m_ttl = baseDuration;
                m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "jackoturnip" select s).FirstOrDefault();
                m_modifyAttackSkill = Skills.SkillType.All;
                m_damageModifier = 0.75f;
                m_startEffects = new EffectList();

                var vfxPrefab = DebuffVfx.ConstructStatusVfx();
                m_startEffects.m_effectPrefabs = new EffectList.EffectData[] { new EffectList.EffectData { m_prefab = vfxPrefab, m_enabled = true, m_attach = true, m_scale = true } };
            }
        }
    }
}
