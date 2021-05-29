using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    public class BeeRuneEffect : RuneEffect
    {
        const string aoeName = "bee_aoe";
        const string beeName = "QueenBee";
        const float baseDamage = 30;
        public BeeRuneEffect()
        {
            _FlavorText = "My God, it's full of bees...";
            _EffectText = new List<string> { "Summons a swarm of angry bees, bee careful", "Generates a Queen Bee item"};
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "-50% Damage" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "Summon bees at targeted location instead of on self", "10m range" };
            targetLock = true;
        }
        public override void DoMagicAttack(Attack baseAttack)
        {
            var character = baseAttack.GetCharacter();
            var aoePrefab = ZNetScene.instance.GetPrefab(aoeName);
            GameObject aoe;
            if (_Quality == RuneQuality.Dark)
            {
                aoe = GameObject.Instantiate(aoePrefab, targetLocation, character.transform.rotation);
            }
            else
            {
                aoe = GameObject.Instantiate(aoePrefab, character.transform.position, character.transform.rotation);
            }
            aoe.GetComponent<Aoe>().m_damage.m_poison = baseDamage * (_Quality == RuneQuality.Ancient ? 0.5f : 1);
            aoe.transform.position = aoe.transform.position + Vector3.up;
            GameObject.Instantiate(ZNetScene.instance.GetPrefab(beeName), aoe.transform.position, aoe.transform.rotation);
        }
    }
}
