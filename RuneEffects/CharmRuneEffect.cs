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
        public void DoMagicAttack(Attack baseAttack)
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
            baseAttack.GetWeapon().m_shared.m_attackStatusEffect = ObjectDB.instance.GetStatusEffect("SE_Charm");
            baseAttack.DoProjectileAttack();
            baseAttack.GetWeapon().m_shared.m_attackStatusEffect = null;
        }

        public class SE_Charm : StatusEffect
        {
            private Character.Faction originalFaction;

            public SE_Charm() : base()
            {
                name = "SE_Charm";
                m_name = "Charmed";
                m_tooltip = "Charmed";
                m_startMessage = "Charmed";
                m_time = 0;
                m_ttl = 30;
                m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "CorpseRun" select s).FirstOrDefault();

                var vfxPrefab = ZNetScene.instance.GetPrefab(charmStartVfxName);
                m_startEffects.m_effectPrefabs = new EffectList.EffectData[] { new EffectList.EffectData { m_prefab = vfxPrefab, m_enabled = true, m_attach = true, m_scale = true } };
                Debug.Log("Got charm vfx " + vfxPrefab.ToString());
            }

            public override void Setup(Character character)
            {
                base.Setup(character);
                Debug.Log("Setting up charm status effect");
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
