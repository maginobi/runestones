using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    class CharmRuneEffect : RuneEffect
    {
        const string projectileName = "projectile_beam";
        const string charmStartVfxName = "vfx_boar_love";
        const float baseDuration = 30;
        public CharmRuneEffect()
        {
            _FlavorText = "You can catch more flies with honey than with vinegar";
            _EffectText = new List<string> { "Charms an enemy with less health than you" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "+100% Duration" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "+200% Duration" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Duration", () => $"{baseDuration * _Effectiveness * ((int)_Quality+1) :F1} sec" } };
        }

        public override void DoMagicAttack(Attack baseAttack)
        {
            baseAttack.m_attackType = Attack.AttackType.Projectile;
            baseAttack.m_attackProjectile = GameObject.Instantiate(ZNetScene.instance.GetPrefab(projectileName)); //(from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == projectileName select prefab).FirstOrDefault();
            baseAttack.m_attackProjectile.GetComponent<Projectile>().m_hitEffects.m_effectPrefabs = new EffectList.EffectData[0];
            baseAttack.m_projectileAccuracy = 0;
            baseAttack.m_projectileAccuracyMin = 0;
            baseAttack.m_useCharacterFacing = true;
            baseAttack.m_useCharacterFacingYAim = true;
            baseAttack.m_launchAngle = 0;
            baseAttack.m_projectileVel = 50;
            baseAttack.m_projectileVelMin = 50;
            baseAttack.m_attackHeight = 1.5f;
            baseAttack.m_attackRange = 1;
            var duration = baseDuration * (1 + baseAttack.GetCharacter().GetSkillFactor(MagicSkill.MagicSkillDef.m_skill)) * (int)_Quality;
            var charmEffect = ExtendedStatusEffect.Create<SE_Charm>();
            charmEffect.m_ttl = duration;
            baseAttack.GetWeapon().m_shared.m_attackStatusEffect = charmEffect;
            baseAttack.DoProjectileAttack();
            baseAttack.GetWeapon().m_shared.m_attackStatusEffect = null;
        }

        public class SE_Charm : ExtendedStatusEffect
        {
            private Character.Faction originalFaction;
            public RuneQuality runeQuality;

            public SE_Charm() : base()
            {
                m_name = "Charmed";
                m_tooltip = "Charmed";
                m_startMessage = "Charmed";
                m_time = 0;
                m_ttl = baseDuration;
                m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "CorpseRun" select s).FirstOrDefault();

                var vfxPrefab = ZNetScene.instance.GetPrefab(charmStartVfxName);
                m_startEffects.m_effectPrefabs = new EffectList.EffectData[] { new EffectList.EffectData { m_prefab = vfxPrefab, m_enabled = true, m_attach = true, m_scale = true } };
                Debug.Log("Got charm vfx " + vfxPrefab.ToString());
            }

            public override void Setup(Character character)
            {
                base.Setup(character);
                originalFaction = m_character.m_faction;
                m_character.m_faction = Character.Faction.Players;
                typeof(BaseAI).GetMethod("SetAlerted", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(character.GetBaseAI(), new object[] { false });
            }

            public override void Stop()
            {
                base.Stop();
                m_character.m_faction = originalFaction;
            }
        }
    }
}
