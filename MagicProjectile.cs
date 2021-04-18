using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones
{
    public class MagicProjectile
    {
        public GameObject m_spawnOnHit = null;
        public Action<Vector3> m_actionOnHit = null;
        public float m_range = 2;
        public float m_launchAngle = 0;
        public float m_attackSpread = 90; //spread angle in degrees; equivalent to Attack.m_attackAngle for horizontal attacks
        public Vector3 m_attackOffset = Vector3.up * 1.5f; //1.5m approx human height
        public Attack.HitPointType m_hitType = Attack.HitPointType.Average;
        const float degInterval = 4;

        public void Cast(Transform origin, Vector3 direction)
        {
            Vector3 attackOrigin = origin.position + origin.forward * m_attackOffset.x + origin.up * m_attackOffset.y + origin.right * m_attackOffset.z;
            Vector3 localSpaceDir = origin.InverseTransformDirection(direction);
            Debug.Log($"attack direction: {direction} local space direction: {localSpaceDir}");
            float startAngle = -m_attackSpread / 2;
            int mask = (int)typeof(Attack).GetField("m_attackMaskTerrain", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy).GetValue(null);
            List<RaycastHit> hits = new List<RaycastHit>();
            for (float angle = startAngle; angle <= startAngle + m_attackSpread; angle += degInterval)
            {
                Vector3 currentDir = origin.TransformDirection(Quaternion.Euler(m_launchAngle, angle, 0) * localSpaceDir);
                RaycastHit hitInfo;
                Physics.Raycast(attackOrigin, currentDir, out hitInfo, m_range, mask, QueryTriggerInteraction.Ignore);
                Debug.Log($"cast from {attackOrigin} in direction {currentDir}");
                hits.Add(hitInfo);
            }

            Vector3 spawnLoc = origin.position;
            if (hits.Count > 0)
            {
                switch (m_hitType)
                {
                    case Attack.HitPointType.Closest:
                        spawnLoc = hits.OrderBy(hit => hit.distance).FirstOrDefault().point; //get closest
                        break;
                    case Attack.HitPointType.Average:
                        spawnLoc = new Vector3(hits.Average(hit => hit.point.x), hits.Average(hit => hit.point.y), hits.Average(hit => hit.point.z)); //get average
                        break;
                }
            }
            else
            {
                Vector3 dirNoElev = new Vector3(direction.x, 0, direction.z);
                dirNoElev.Normalize();
                spawnLoc = origin.position + dirNoElev * m_range;
            }

            if (m_spawnOnHit != null)
            {
                GameObject.Instantiate(m_spawnOnHit, spawnLoc, Quaternion.identity);
            }
            if (m_actionOnHit != null)
            {
                m_actionOnHit(spawnLoc);
            }
        }

    }
}
