/*
 * Project:	Billy's Payback
 * File:	SpawnObjectOnAreaKeyEvent.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * The declared object will be spawned on an area key-event
 */
public class SpawnObjectOnAreaKeyEvent : MonoBehaviour, DeActivatable
{
    // The area-key-event object
    public OnAreaKeyEvent   m_areaKeyEvent;

    // The object which should be spawned
    public Object           m_objectToSpawn;

    // True if the transition is activated
    public bool             m_isActivated = true;

    // The world offset for the spawned object
    public Vector3          m_worldOffset = Vector3.zero;

    // The world scale
    private Vector3         m_worldScale;

    // Override: MonoBehaviour::Awake
    void Awake()
    {
        // Check area object
        if (m_areaKeyEvent == null)
        {
            Debug.LogWarning("The area-key-event must not be null!");
            return;
        }

        // Add event
        m_areaKeyEvent.OnKeyEventPressed += () =>
        {
            // Spawn object
            spawnObject();

            // Kill area key-event
            if(m_areaKeyEvent != null)
            {
                GameObject.Destroy(m_areaKeyEvent);
                m_areaKeyEvent = null;
            }
        };

        // Calculate world scale
        m_worldScale = HelperFunctions.getWorldScale(gameObject);
    }

    // Override: MonoBehaviour::OnDestroy
    void OnDestroy()
    {
        // Remove key-event
        m_areaKeyEvent = null;
    }

    // Spawns the object
    private void spawnObject()
    {
        // Local variables
        GameObject obj = null;

        // Activated?
        if (m_isActivated == false)
            return;

        // Check object
        if (m_objectToSpawn == null)
            return;

        // Create object
        obj = GameObject.Instantiate(m_objectToSpawn) as GameObject;

        // Set parent
        obj.transform.parent        = transform;
        obj.transform.localPosition = m_worldOffset;
        obj.transform.eulerAngles   = Vector3.zero;
        obj.transform.localScale    = (m_objectToSpawn as GameObject).transform.localScale;
    }

    // Override: DeActivatable::isActivated()
    public bool isActivated()
    {
        return m_isActivated;
    }

    // Override: DeActivatable::onActivate()
    public void onActivate()
    {
        Debug.Log("hure");

        // Set flag
        m_isActivated = true;
    }

    // Override: DeActivatable::onDeactivate()
    public void onDeactivate()
    {
        // Clear flag
        m_isActivated = false;
    }
}
