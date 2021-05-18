using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones
{
    public class ConeVolumeProjectile : MagicProjectile
    {
        float m_exclusionRadius = 0.25f;

        new public void Cast(Transform origin, Vector3 direction)
        {
            Vector3 attackOrigin = origin.position + origin.forward * m_attackOffset.x + origin.up * m_attackOffset.y + origin.right * m_attackOffset.z;

            var hitColliders = from Collider collider in Physics.OverlapSphere(attackOrigin, m_range)
                            where CheckInCone(attackOrigin, direction, collider)
                            select collider;

            Vector3 spawnLoc = origin.position;
            if (hitColliders.Count() > 0)
            {
                switch (m_hitType)
                {
                    case Attack.HitPointType.Closest:
                        spawnLoc = hitColliders.OrderBy(hit => Math.Abs((hit.ClosestPoint(attackOrigin) - attackOrigin).magnitude)).FirstOrDefault().ClosestPoint(attackOrigin); //get closest
                        break;
                    case Attack.HitPointType.Average:
                        spawnLoc = new Vector3(hitColliders.Average(hit => hit.ClosestPoint(attackOrigin).x),
                            hitColliders.Average(hit => hit.ClosestPoint(attackOrigin).y),
                            hitColliders.Average(hit => hit.ClosestPoint(attackOrigin).z)); //get average
                        break;
                }
            }
            else
            {
                Vector3 dirNoElev = new Vector3(direction.x, 0, direction.z);
                dirNoElev.Normalize();
                spawnLoc = attackOrigin + dirNoElev * m_range;
            }

            if (m_spawnOnHit != null)
            {
                InstantiatedObject = GameObject.Instantiate(m_spawnOnHit, spawnLoc, Quaternion.identity);
            }
            if (m_actionOnHit != null)
            {
                m_actionOnHit(spawnLoc);
            }
            if (m_actionOnHitCollider != null)
            {
                foreach(var collider in hitColliders)
                {
                    if (collider)
                        m_actionOnHitCollider(collider);
                }
            }
        }

        private bool CheckInCone(Vector3 origin, Vector3 direction, Collider colliderToCheck)
        {
            var toCollider = colliderToCheck.ClosestPoint(origin) - origin;
            var angleToColliderVec = Vector3.Angle(direction, toCollider);
            return Math.Abs(angleToColliderVec) <= m_attackSpread/2 && toCollider.magnitude > m_exclusionRadius;
        }

    }
}
