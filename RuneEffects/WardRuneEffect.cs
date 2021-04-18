using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    class WardRuneEffect : RuneEffect
    {
        private const string forcefieldName = "ForceField";
        private const float size = 5;

        public override void DoMagicAttack(Attack baseAttack)
        {
            var player = baseAttack.GetCharacter();
            GameObject forcefieldPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == forcefieldName select prefab).FirstOrDefault();
            var sphere = GameObject.Instantiate(forcefieldPrefab, player.GetCenterPoint(), Quaternion.identity);
            sphere.transform.localScale = new Vector3(size, size, size);
            var invertedSphere = GameObject.Instantiate(forcefieldPrefab, player.GetCenterPoint(), Quaternion.identity);
            invertedSphere.transform.localScale = new Vector3(size - 0.05f, size - 0.05f, size - 0.05f);

            var sphereBase = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var mf = sphereBase.GetComponent<MeshFilter>();
            Mesh mesh = mf.mesh;
            
            // Reverse the triangles
            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                int t = mesh.triangles[i];
                mesh.triangles[i] = mesh.triangles[i+2];
                mesh.triangles[i+2] = t;
            }

            // Reverse the normals;
            for (int i = 0; i < mesh.normals.Length; i++)
            {
                mesh.normals[i] = -mesh.normals[i];
            }

            invertedSphere.GetComponent<MeshFilter>().mesh = mesh;
        }

    }
}
