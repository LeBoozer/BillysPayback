/*
 * Project:	Billy's Payback
 * File:	SA_SaveObjectTransform.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * ToDo
 */
public class SA_SaveObjectTransform : FSMStateAction
{
    public class StoredTransforms
    {
        public Vector3 m_localPosition;
        public Vector3 m_localScale;
        public Quaternion m_localRotation;
    }

    // Represents an entry
    [System.Serializable]
    public class Entry
    {
        public GameObject m_object = null;

        [HideInInspector]
        public StoredTransforms m_transform = null;
    }

    // List with all entries
    public Entry[] m_objects = null;

    // Override: FSMStateAction::onActionEnter()
    override public void onActionEnter()
    {
        // Check
        if (m_objects == null || m_objects.Length == 0)
            return;

        // Safe transforms of the registered objects
        foreach (Entry obj in m_objects)
        {
            // Valid object?
            if (obj.m_object == null)
                continue;
            obj.m_transform = new StoredTransforms();
            
            // Copy values
            obj.m_transform.m_localPosition = obj.m_object.transform.localPosition;
            obj.m_transform.m_localScale = obj.m_object.transform.localScale;
            obj.m_transform.m_localRotation = obj.m_object.transform.localRotation;
        }
    }

    // Override: FSMStateAction::onActionLeave()
    override public void onActionLeave()
    {
    }
}
