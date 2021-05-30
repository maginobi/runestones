using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    public class VineRuneEffect : RuneEffect
    {
        const string minionName = "TentaRoot";
        const string vfxName = "fx_gdking_rootspawn";
        const float baseDuration = 20;
        public VineRuneEffect()
        {
            _FlavorText = "Druids are broken even without Entangle";
            _EffectText = new List<string> { "Summons a friendly stationary vine", "Appears at a random location within 10m", "Low attack range and speed" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "Summons 3 vines instead" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "Summons at your target location instead", "Increased attack range and speed" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Duration", () => $"{baseDuration * _Effectiveness:F1} sec" } };
            speed = CastingAnimations.CastSpeed.Medium;
            targetLock = true;
        }

        public override void DoMagicAttack(Attack baseAttack)
        {
            var character = baseAttack.GetCharacter();
            var rand = new System.Random();
            var statusEffect = (SE_Druid)character.GetSEMan().AddStatusEffect("SE_Druid");
            if (statusEffect == null)
            {
                statusEffect = (SE_Druid)character.GetSEMan().GetStatusEffect("SE_Druid");
                statusEffect.ResetTime();
            }
            statusEffect.m_ttl = baseDuration * _Effectiveness;
            if (statusEffect.minionList.Count > 0)
            {
                statusEffect.DestroyAll();
            }

            for (int i = 0; i < (_Quality == RuneQuality.Ancient ? 3 : 1); i++)
            {
                if (_Quality == RuneQuality.Dark)
                {
                    var root = SpawnRoot(targetLocation, true);
                    statusEffect.minionList.Add(root);
                }
                else
                {
                    var randSpawnPos = character.transform.position + Quaternion.Euler(0, (float)(360 * rand.NextDouble()), 0) * new Vector3((float)(9 * rand.NextDouble() + 1), 0, 0);
                    var root = SpawnRoot(randSpawnPos);
                    statusEffect.minionList.Add(root);
                }
            }
        }

        private GameObject SpawnRoot(Vector3 location, bool upgraded = false)
        {
            //Play vfx
            var vfx = ZNetScene.instance.GetPrefab(vfxName);
            GameObject.Instantiate(vfx, location, Quaternion.identity);

            //Spawn root
            var tentarootPrefab = ZNetScene.instance.GetPrefab(minionName);
            var tentaroot = GameObject.Instantiate(tentarootPrefab, location, Quaternion.identity);
            var timedDestruct = tentaroot.GetComponent<CharacterTimedDestruction>();
            timedDestruct.CancelInvoke();
            timedDestruct.m_timeoutMin = baseDuration * _Effectiveness;
            timedDestruct.m_timeoutMax = baseDuration * _Effectiveness;
            timedDestruct.Trigger();
            var characterSettings = tentaroot.GetComponent<Humanoid>();
            characterSettings.m_faction = Character.Faction.Players;
            var weapon = GameObject.Instantiate(characterSettings.m_randomWeapon[0]);
            var attack = weapon.GetComponent<ItemDrop>();
            attack.m_itemData.m_shared.m_aiAttackInterval = upgraded ? 2 : 5;
            attack.m_itemData.m_shared.m_attack.m_attackRange = upgraded ? 4 : 1;
            attack.m_itemData.m_shared.m_damages.m_blunt = 30;
            characterSettings.m_randomWeapon[0] = weapon;

            return tentaroot;
        }

        public class SE_Druid : StatusEffect
        {
            public List<GameObject> minionList = new List<GameObject>();
            public SE_Druid() : base()
            {
                name = "SE_Druid";
                m_name = "Druid";
                m_tooltip = "You have summoned nature to your aid\nNew vines will replace existing ones";
                m_startMessage = "The forest moves";
                m_time = 0;
                m_repeatInterval = 60;
                m_ttl = baseDuration;
                m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "ancientseed" select s).FirstOrDefault();
            }

            public override void UpdateStatusEffect(float dt)
            {
                base.UpdateStatusEffect(dt);
                var toRemove = new List<GameObject>();
                foreach (var minion in minionList)
                {
                    if (minion == null || minion.GetComponent<Humanoid>()?.GetHealth() == null || minion.GetComponent<Humanoid>().GetHealth() <= 0)
                        toRemove.Add(minion);
                }
                minionList = minionList.Except(toRemove).ToList();
            }

            public void DestroyAll()
            {
                foreach (var minion in minionList)
                {
                    GameObject.Destroy(minion);
                }
            }
        }
    }
}
