/*
 * Project:	Billy's Payback
 * File:	ControlMultipleLevelPlanes.cs
 * Authors:	Dominique Kasper
 * Editors:	Byron Worms, Raik Dankworth
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlMultipleLevelPlanes : MonoBehaviour {
    
	//the upper plane control can be set off or on
	public bool m_isOn = true;

    // DeActived game objects
    public List<GameObject> m_gameObjects = new List<GameObject>();

    // control if the player enter the trigger -> game object get invisible
    void OnTriggerEnter(Collider _other)
    {
        OnTrigger(_other, false);
    }

    // control if the player exit the trigger -> game object get visible
    void OnTriggerExit(Collider _other)
    {
        OnTrigger(_other, true);
    }


    // DeActived the renderer of the game objects
    private void OnTrigger(Collider _other, bool _hiddenValue)
    {    
        // ignore all except the player
        if (!m_isOn || _other.tag != Tags.TAG_PLAYER)
            return;

        // Actived the game objects
        foreach (GameObject obj in m_gameObjects)
        {
            //obj.SetActive(_hiddenValue);
            // seek the renderer in the game object
            MeshRenderer[] renderer = obj.GetComponentsInChildren<MeshRenderer>();

            // dont find renderer?
            if (renderer == null || renderer.Length == 0)
                continue;

            // set the hidden value
            foreach (MeshRenderer r in renderer)
                r.enabled = _hiddenValue;
        }
    }
}