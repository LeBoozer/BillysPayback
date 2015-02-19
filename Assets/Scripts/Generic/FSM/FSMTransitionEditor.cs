/*
 * Project:	Billy's Payback
 * File:	FSMTransitionEditor.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using UnityEditor;

/*
 * Displays advanced controls for FSM transition within the editor's inspector
 */
[CustomEditor(typeof(FSMTransition),true)]
public class FSMTransitionEditor : Editor
{
    // Override: MonoBehaviour::OnInspectorGUI()
    public override void OnInspectorGUI()
    {
        // Local variables
        FSMTransition script = target as FSMTransition;

        // Draw default stuff
        DrawDefaultInspector();

        // Add button: manuel trigger transition
        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("Trigger"))
                script.setTargetFSMState();
        }
    }
}
