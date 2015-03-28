/*
 * Project:	Billy's Payback
 * File:	MeshRendererOnVisible.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * ToDo
 */
[RequireComponent(typeof(MeshRenderer))]
public class MeshRendererOnVisible : MonoBehaviour 
{
    // List of all supported delegate functions
    public delegate void Delegate_OnWillRenderObject(Camera _camera);

    // List of all supported events
    public event Delegate_OnWillRenderObject OnWillRender = delegate { };

    // Override: MonoBehaviour::OnWillRenderObject()
    void OnWillRenderObject()
    {
        // Notify events
        OnWillRender(Camera.current);
    }

    // Tries to attach the script to the mesh renderer with the biggest bounding box
    public static bool attachScriptToRenderer(GameObject _object, Delegate_OnWillRenderObject _callback)
    {
        // Local variables
        MeshRenderer[] rendererList = null;
        MeshRendererOnVisible script = null;

        // Validate parameter
        if (_object == null || _callback == null)
            return false;

        // Get list with all renderer
        rendererList = _object.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in rendererList)
        {
            script = r.gameObject.AddComponent<MeshRendererOnVisible>();
            script.OnWillRender += _callback;
        }

        return true;
    }
}
