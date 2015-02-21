/*
 * Project:	Billy's Payback
 * File:	SA_RotateObjects.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * State action which will rotate the specified game objects
 */
public class SA_RotateObjects : FSMStateAction
{
    // Represents an entry
    [System.Serializable]
    public class Entry
    {
        // The game object
        public GameObject m_gameObject = null;

        // The rotation axis
        public Vector3 m_axis = Vector3.up;

        // Degrees for the rotation
        public float m_degrees = 0.0f;

        // True to trigger the activation on enter events, otherwise on leave events
        public bool m_onEnter = true;
    }

    // List with the specified game object
    public Entry[] m_entries;

    // Will activate/deactivate the specified object
    // _enter: True on action enter events
    private void action(bool _enter)
    {
        // Check parameter
        if (m_entries == null || m_entries.Length == 0)
            return;

        // Loop through all entries
        foreach (Entry e in m_entries)
        {
            if (e.m_gameObject == null || e.m_onEnter != _enter)
                continue;
            e.m_gameObject.transform.Rotate(e.m_axis, e.m_degrees, Space.World);
        }
    }

    // Override: FSMStateAction::onActionEnter()
    override public void onActionEnter()
    {
        action(true);
    }

    // Override: FSMStateAction::onActionLeave()
    override public void onActionLeave()
    {
        action(false);
    }
}
