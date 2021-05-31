using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    public class MeteorRuneEffect : RuneEffect
    {
        const string spawnerName = "spawn_meteors";
        const float baseDelay = 3f;
        public MeteorRuneEffect()
        {
            _FlavorText = "'It's OK guys; I've called in an airstrike on our exact location.' 'What do you mean our *exact* location?'";
            _EffectText = new List<string> { "Summons a meteor to your location after a 3 second delay", "Meteor will hurt you", "40 Blunt, 140 Fire damage" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "No more delay after spell completion. Run." };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "Dear gods! Ragnarok approaches" };
            speed = CastingAnimations.CastSpeed.Slow;
            targetLock = true;
        }

        public override void Precast(Attack baseAttack)
        {
            targetLocation = baseAttack.GetCharacter().transform.position;
        }

        public override void DoMagicAttack(Attack baseAttack)
        {
            var character = baseAttack.GetCharacter();
            var spawnerPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == spawnerName select prefab).FirstOrDefault();
            var spawner = GameObject.Instantiate(spawnerPrefab, targetLocation, Quaternion.identity);
            var spawnEffect = spawner.GetComponent<SpawnAbility>();
            if (_Quality == RuneQuality.Dark)
            {
                spawnEffect.m_spawnDelay = 1;
                spawnEffect.m_minToSpawn = 20;
                spawnEffect.m_maxToSpawn = 40;
                spawnEffect.m_spawnAtTarget = false;
                spawnEffect.m_spawnRadius = 150;
            }
            else
            {
                spawnEffect.m_minToSpawn = 1;
                spawnEffect.m_maxToSpawn = 1;
                spawnEffect.m_spawnAtTarget = true;
            }
            spawnEffect.m_targetType = SpawnAbility.TargetType.Position;
            if (_Quality == RuneQuality.Common)
            {
                var delayer = spawner.AddComponent<ExtraDelay>();
                delayer.actionToDelay = () => spawnEffect.Setup(null, Vector3.zero, 0, null, null);
                delayer.Invoke("DelayedAction", baseDelay);
            }
            else
            {
                spawnEffect.Setup(null, Vector3.zero, 0, null, null);
            }
        }
    }

    public class ExtraDelay : MonoBehaviour
    {
        public Action actionToDelay;
        public void DelayedAction()
        {
            actionToDelay.Invoke();
        }
    }
}
