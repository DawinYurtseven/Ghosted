using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BufferProjectile))]
public class BufferProjectileEditor : Editor
{
    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
    static void DrawGizmosSelected(BufferProjectile projectile, GizmoType gizmoType)
    {
        Gizmos.DrawSphere(projectile.transform.position, 0.125f);
    }
    void OnSceneGUI()
        {
            var projectile = target as BufferProjectile;
            var transform = projectile.transform;
            projectile.damageRadius = Handles.RadiusHandle(
                transform.rotation, 
                transform.position, 
                projectile.damageRadius);
        }
}
