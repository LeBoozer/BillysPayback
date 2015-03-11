/*
 * Project:	Billy's Payback
 * File:	T_OnObjectsDestroyed.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * The transition will be triggered as soon as all registered objects has been destroyed
 */
public class T_OnObjectsDestroyed : FSMTransition
{
    // List of registered objects
    public GameObject[] m_objects = null;

    // Override: FSMTransition::OnEnable
    void OnEnable()
    {
        // Check length
        if (m_objects == null || m_objects.Length == 0)
            Debug.LogWarning("Transition has no registered objects!");
	}

    // Override: FSMTransition::OnDisable
    void OnDisable()
    {
    }

    // Override: FSMTransition::MonoBehaviour::OnDisable
    void FixedUpdate()
    {
        // Check parameters
        if (m_objects == null)
            return;

        // Validate all registered objects
        foreach(GameObject obj in m_objects)
        {
            if (obj != null)
                return;
        }

        // Trigger transition
        setTargetFSMState();
    }
}
