/*
 * Project:	Billy's Payback
 * File:	ChangeCameraDistanceOnTrigger.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Script which will be called as soon as a power-up has been destroyed or collected
 */
public class ChangeCameraDistanceOnTrigger : MonoBehaviour
{
    // Represents an entry
    [System.Serializable]
    public class Entry
    {
        public float m_newDistance = -1.0f;
        public bool  m_saveRestore = false;

        [HideInInspector]
        public float m_oldDistance = -1.0f;
    }

    // Represents an entry for a trigger
    [System.Serializable]
    public class Trigger
    {
        // The trigger collider
        public Collider m_collider;

        public Entry    m_onEnter = new Entry();
        public Entry    m_onLeave = new Entry();
    }

    // True to auto-detect the player camera
    public bool         m_autoDetectCamera = true;

    // True to auto-detect the player
    public bool         m_autoDetectPlayer = true;

    // The player camera
    public PlayerCamera m_playerCamera = null;

    // The target collider (e.g. the player)
    public Collider     m_targetCollider = null;

    // List of registered triggers
    public Trigger[]    m_triggers;

    // Override: MonoBehaviour::Awake()
    void Awake()
    {
        // Local variables
        GameObject obj = null;
        FSMEventHighjack hj = null;

        // Check parameter
        if (m_triggers == null || m_triggers.Length == 0)
            return;

        // Validate triggers
        foreach(Trigger t in m_triggers)
        {
            // Check
            if (t == null)
                continue;
            else if(t.m_collider == null)
            {
                Debug.LogError("No collider object has been set!");
                return;
            }
            else if(t.m_collider.isTrigger == false)
            {
                Debug.LogError("Collider object must be declared as trigger!");
                return;
            }
        }

        // Check camera
        if (m_autoDetectCamera == true)
        {
            obj = GameObject.FindGameObjectWithTag(Tags.TAG_MAIN_CAMERA);
            if(obj != null)
                m_playerCamera = obj.GetComponent<PlayerCamera>();
        }
        if (m_playerCamera == null)
        {
            Debug.LogError("Camera is null or invalid!");
            return;
        }

        // Check player
        if (m_autoDetectPlayer == true)
        {
            obj = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);
            if (obj != null)
                m_targetCollider = obj.GetComponent<Collider>();
        }
        if (m_targetCollider == null)
        {
            Debug.LogError("Target collider has not been set!");
            return;
        }

        // Highjack registered triggers
        foreach(Trigger t in m_triggers)
        {
            // Check
            if (t == null)
                continue;

            // Inject script
            hj = t.m_collider.gameObject.AddComponent<FSMEventHighjack>();
            hj.FSMOnTriggerEnter += (Collider _other) =>
            {
				// Collider hits one of the triggers?
                if (_other.tag.Equals(m_targetCollider.tag) == true)
                {
                    // Save current distance
                    if (t.m_onEnter.m_saveRestore == true)
                        t.m_onEnter.m_oldDistance = m_playerCamera.m_distance_Z;

                    // Set new distance
                    m_playerCamera.m_distance_Z = t.m_onEnter.m_newDistance;
                }
            };
            hj.FSMOnTriggerLeave += (Collider _other) =>
            {
                // Collider hits one of the triggers?
                if (_other.tag.Equals(m_targetCollider.tag) == true)
                {
                    // Restore distance?
                    if (t.m_onEnter.m_saveRestore == true && t.m_onLeave.m_saveRestore == true)
                        m_playerCamera.m_distance_Z = t.m_onEnter.m_oldDistance;
                    else
                        m_playerCamera.m_distance_Z = t.m_onLeave.m_newDistance;
                }
            };
        }
    }

    // Override: MonoBehaviour::OnDestroy()
    void OnDestroy()
    {
        // Local variables
        FSMEventHighjack hj = null;

        // Delete highjack from triggers
        foreach (Trigger t in m_triggers)
        {
            // Check
            if (t == null || t.m_collider == null)
                continue;

            // Remove
            hj = t.m_collider.gameObject.GetComponent<FSMEventHighjack>();
            if (hj != null)
                Component.Destroy(hj);
        }
    }
}
