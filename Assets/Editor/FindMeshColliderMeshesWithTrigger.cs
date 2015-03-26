/*
 * Project:	Billy's Payback
 * File:	FindMeshColliderMeshesWithTrigger.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

/*
 * ToDo
 */
public class FindMeshColliderMeshesWithTrigger : EditorWindow
{
    // Add the window to the unity menu bar
    [MenuItem("Window/Port to Unity5")]
    static void Init()
    {
        // Add window
        FindMeshColliderMeshesWithTrigger window = (FindMeshColliderMeshesWithTrigger)EditorWindow.GetWindow(typeof(FindMeshColliderMeshesWithTrigger));
        window.minSize = new Vector2(400, 200);

        // Show window
        window.Show();
    }

    // Override: MonoBehaviour::OnGUI()
    void OnGUI()
    {
        // Add label
        GUILayout.Label("Attention should be paid to the following list during the port to Unity5:", EditorStyles.boldLabel);

        // Add label
        GUILayout.Label("# All lights can cast dynamic shadows now.", EditorStyles.label);
        GUILayout.Space(10);

        // Add button: find all triangle meshes with mesh colliders, which are configurated as trigger shapes
        GUILayout.Label("# Meshes with triangle colliders as trigger (not supported):", EditorStyles.label);
        if(GUILayout.Button("Find meshes"))
        {
            Selection.objects = FindObjectsOfType<Enemy>().Where(mc => mc.m_disableRaycast == true).Select(mc => mc.gameObject).ToArray();
            //Selection.objects = FindObjectsOfType<MeshCollider>().Where(mc => mc.isTrigger && !mc.convex).Select(mc => mc.gameObject).ToArray();
        }
    }
}
