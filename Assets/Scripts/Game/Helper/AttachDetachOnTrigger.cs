/*
 * Project:	Billy's Payback
 * File:	AttachDetachOnTrigger.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Attaches the defined target objects on trigger enter events (change parent temporally).
 * Detaches the defined target objects on trigger leave events (insert previous parent).
 * Overlapping trigger colliders are NOT supported by now!
 */
public class AttachDetachOnTrigger : MonoBehaviour 
{
    // True to auto-detect the player (billy) as target object
    public bool                     m_autoDetectBilly = true;

    // List with all target objects
    public GameObject[]             m_targetObjects = null;

    // List with all trigger colliders 
    public Collider[]               m_triggerColliders = null;

    // List with the information about the target objects
    private List<GameObjectHolder>  m_holders = new List<GameObjectHolder>();

    // Override: MonoBehaviour::Start
	void Start ()
    {
        // Local variables
        GameObject obj = null;
        GameObjectHolder holder = null;
        FSMEventHighjack hj = null;
        bool triggerAvailable = false;

        // Trigger collders defined?
        if (m_triggerColliders == null || m_triggerColliders.Length <= 0)
        {
            Debug.LogWarning("No colliders has been defined!");
            return;
        }
        foreach (Collider c in m_triggerColliders)
        {
            if(c.isTrigger == true)
            {
                triggerAvailable = true;
                break;
            }
        }
        if (triggerAvailable == false)
        {
            Debug.LogWarning("At least one trigger collider is required!");
            return;
        }

	    // Auto detect player?
        if(m_autoDetectBilly == true)
        {
            obj = GameObject.Find("Billy");
            if (obj != null)
            {
                // Create holder
                holder = new GameObjectHolder();
                holder.m_targetObject = obj;
                holder.m_parent = obj.transform.parent.gameObject;
                m_holders.Add(holder);
            }   
        }

        // Add defined target objects to list
        if(m_targetObjects != null && m_targetObjects.Length > 0)
        {
            // Loop through all entries
            foreach(GameObject g in m_targetObjects)
            {
                // Create holder
                holder = new GameObjectHolder();
                holder.m_targetObject = g;
                holder.m_parent = g.transform.parent.gameObject;
                m_holders.Add(holder);
            }

            // Clear
            m_targetObjects = null;
        }
        if (m_holders.Count <= 0)
        {
            Debug.LogWarning("At least one target object is required!");
            return;
        }

        // Highjack all target objects
        foreach(GameObjectHolder h in m_holders)
        {
            // Inject script
            hj = h.m_targetObject.AddComponent<FSMEventHighjack>();
            hj.FSMOnTriggerEnter += (Collider _other) =>
            {
                // Detect hit collider
                foreach (Collider c in m_triggerColliders)
                {
                    if(_other.gameObject.Equals(c.gameObject) == true)
                    {
                        // Found! Change parent
                        h.m_targetObject.transform.parent = _other.transform;
                    }
                }
            };
            hj.FSMOnTriggerLeave += (Collider _other) =>
            {
                // Detect hit collider
                foreach (Collider c in m_triggerColliders)
                {
                    if (_other.gameObject.Equals(c.gameObject) == true)
                    {
                        // Found! Change parent to the original one
                        h.m_targetObject.transform.parent = h.m_parent.transform;
                    }
                }
            };
        }
	}

    // Override: MonoBehaviour::OnDestroy
    void OnDestroy()
    {
        // Local variables
        FSMEventHighjack hj = null;

        // Remove highjack
        foreach(GameObjectHolder h in m_holders)
        {
            // Get script
            if (h != null && h.m_targetObject != null)
            {
                hj = h.m_targetObject.GetComponent<FSMEventHighjack>();
                if (hj != null)
                    GameObject.Destroy(hj);
            }
        }
    }

    /**
     * Provides information about a certain target object
     */
    private class GameObjectHolder
    {
        // The original parent object
        public GameObject m_parent       = null;

        // The actual target object
        public GameObject m_targetObject = null;
    }
}
