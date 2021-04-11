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
        public void DoMagicAttack(Attack baseAttack)
        {
            var character = baseAttack.GetCharacter();
            var aoePrefab = ZNetScene.instance.GetPrefab(aoeName);
            var aoe = GameObject.Instantiate(aoePrefab, character.transform.position + Vector3.up, character.transform.rotation);
            aoe.GetComponent<Aoe>().m_damage.m_poison = 30;
            GameObject.Instantiate(ZNetScene.instance.GetPrefab(beeName), character.transform.position + Vector3.up, character.transform.rotation);
        }
    }
}
