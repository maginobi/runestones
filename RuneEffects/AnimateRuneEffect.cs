using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    public class AnimateRuneEffect : RuneEffect
    {
        const string skeletonName = "Skeleton";
        const string vfxName = "vfx_skeleton_death";
        public void DoMagicAttack(Attack baseAttack)
        {
            //Spawn skeleton
            var character = baseAttack.GetCharacter();
            var spawnPos = character.transform.position + character.transform.forward * 2f;
            var spawnRot = character.transform.rotation;
            var skeletonPrefab = ZNetScene.instance.GetPrefab(skeletonName);
            var skeleton = GameObject.Instantiate(skeletonPrefab, spawnPos, spawnRot);
            skeleton.GetComponent<Humanoid>().m_faction = Character.Faction.Players;

            //Play vfx
            var vfx = ZNetScene.instance.GetPrefab(vfxName);
            GameObject.Instantiate(vfx, spawnPos, spawnRot);

            //Apply status effect
            if (!character.GetSEMan().HaveStatusEffect("SE_Necromancer"))
                ((SE_Necromancer)character.GetSEMan().AddStatusEffect("SE_Necromancer")).minionList.Add(skeleton);
            else
                ((SE_Necromancer)character.GetSEMan().GetStatusEffect("SE_Necromancer")).minionList.Add(skeleton);
        }

        public class SE_Necromancer : SE_Stats
        {
            public List<GameObject> minionList = new List<GameObject>();
            private const int skeleMaxHealth = 40;
            private const float baseDurationSec = 120;
            public float degenRate = skeleMaxHealth/baseDurationSec;
            public SE_Necromancer() : base()
            {
                name = "SE_Necromancer";
                m_name = "Necromancer";
                m_tooltip = "You have summoned undead minions. They will decay over time.\nEvery additional skeleton animated accelerates the decay process.\n-25% Health regeneration";
                m_startMessage = "Skeleton summoned";
                m_time = 0;
                m_repeatInterval = 60;
                m_ttl = baseDurationSec;
                m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "TrophySkeleton" select s).FirstOrDefault();
                m_healthRegenMultiplier = 0.75f;
            }

            public override void UpdateStatusEffect(float dt)
            {
                base.UpdateStatusEffect(dt);
                degenRate = CalcDegen(minionList.Count);
                SortedSet<float> sortedHealth = new SortedSet<float>();
                GameObject[] origMinionList = new GameObject[minionList.Count];
                minionList.CopyTo(origMinionList);
                foreach(var minion in origMinionList)
                {
                    if(minion == null || minion.GetComponent<Humanoid>()?.GetHealth() == null || minion.GetComponent<Humanoid>().GetHealth() <= 0)
                    {
                        minionList.Remove(minion);
                        continue;
                    }
                    var skele = minion.GetComponent<Humanoid>();
                    HitData hit = new HitData { m_damage = new HitData.DamageTypes { m_damage = degenRate * dt } };
                    skele.ApplyDamage(hit, false, false);
                    if (skele != null)
                        sortedHealth.Add(skele.GetHealth());
                }
                int index = 0;
                float totalDamage = 0;
                float totalTime = 0;
                foreach (var origHp in sortedHealth)
                {
                    var hp = origHp - totalDamage;
                    var numMinions = sortedHealth.Count - index;
                    totalTime += hp / CalcDegen(numMinions);
                    totalDamage += hp;
                    index++;
                }
                m_time = 0;
                m_ttl = totalTime;
                if (m_ttl <= 0)
                {
                    Stop();
                    m_character.GetSEMan().RemoveStatusEffect(this);
                }
            }

            private float CalcDegen(int numMinions)
            {
                // 1 skele: base rate,   2 skele: 2x rate,   3 skele: 4x rate,   4 skele: 8x rate
                return (float)(skeleMaxHealth / baseDurationSec * Math.Pow(2, numMinions-1)); 
            }
        }
    }
}
