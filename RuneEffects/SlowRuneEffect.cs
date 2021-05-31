using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    public class SlowRuneEffect : RuneEffect
    {
        private const string vfxName = "vfx_blob_hit";
        public const float baseSpeedMod = 0.5f;
        public const float baseDuration = 30;
        public const float darkSpeedMod = 0.1f;
        public SlowRuneEffect()
        {
            _FlavorText = "Stop and smell the roses";
            _EffectText = new List<string> { "Slows enemies", "1m radius" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "+200% Duration" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "+40% Slow (before spell effectiveness)" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Slow", () => $"{1 - (_Quality==RuneQuality.Dark ? darkSpeedMod : baseSpeedMod) / _Effectiveness :P1}"},
                                                                    { "Duration", () => $"{baseDuration * _Effectiveness * (_Quality==RuneQuality.Ancient ? 3 : 1) :F1} sec" }};
            targetLock = true;
        }
        public override void DoMagicAttack(Attack baseAttack)
        {
            var vfxPrefab = ZNetScene.instance.GetPrefab(vfxName);
            Debug.Log($"fetched prefab {vfxPrefab.name}");
            var gameObject = GameObject.Instantiate(vfxPrefab);
            Debug.Log("vfx instantiated");

            var statusEffect = ScriptableObject.CreateInstance<SE_Slow>();
            statusEffect.m_ttl = baseDuration * _Effectiveness * (_Quality == RuneQuality.Ancient ? 3 : 1);
            statusEffect.speedMod = (_Quality == RuneQuality.Dark ? darkSpeedMod : baseSpeedMod) / _Effectiveness;

            var aoe = gameObject.AddComponent<SlowAoe>();
            aoe.m_statusEffect = statusEffect.Serialize();

            var go = GameObject.Instantiate(gameObject, targetLocation, Quaternion.identity);
            go.GetComponent<Aoe>().Setup(baseAttack.GetCharacter(), Vector3.zero, 0, null, null);
        }


        public class SlowAoe : Aoe
        {
            public SlowAoe() : base()
            {
                m_useAttackSettings = false;
                m_dodgeable = true;
                m_blockable = false;
                m_statusEffect = "SE_Slow";
                m_hitOwner = false;
                m_hitSame = false;
                m_hitFriendly = false;
                m_hitEnemy = true;
                m_skill = Skills.SkillType.None;
                m_hitInterval = -1;
                m_ttl = 1;
            }
        };

        public class SE_Slow : RuneStatusEffect
        {
            public float speedMod = baseSpeedMod;
            public SE_Slow() : base()
            {
                name = "SE_Slow";
                m_name = "Slow";
                m_startMessage = "Slowed";
                m_time = 0;
                m_ttl = baseDuration;
                m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "CorpseRun" select s).FirstOrDefault();

                var vfxPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == CurseRuneEffect.curseVfxName select prefab).FirstOrDefault();
                m_startEffects.m_effectPrefabs = new EffectList.EffectData[] { new EffectList.EffectData { m_prefab = vfxPrefab, m_enabled = true, m_attach = true, m_scale = true } };
            }

            public override string GetTooltipString()
            {
                return $"-{1-speedMod :P0} Speed";
            }

            /*
            public override void SetAttacker(Character attacker)
            {
                base.SetAttacker(attacker);
                float effectiveness = (1 + attacker.GetSkillFactor(MagicSkill.MagicSkillDef.m_skill));
                m_ttl = baseDuration * effectiveness;
                speedMod = baseSpeedMod / effectiveness;
            }
            */

            override public void ModifySpeed(ref float speed)
            {
                speed *= speedMod;
            }
        }
    }
}
