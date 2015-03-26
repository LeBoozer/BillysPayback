/*
 * Project:	Billy's Payback
 * File:	RestoreObjectTransform.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * ToDo
 */
public class RestoreObjectTransform : MonoBehaviour
{
    // The saved object transforms
    public SA_SaveObjectTransform m_savedTransforms = null;

    // Override: MonoBehaviour::Start()
	void Start() 
    {
	    // Check
        if (m_savedTransforms == null || m_savedTransforms.m_objects == null || m_savedTransforms.m_objects.Length == 0)
            return;

        // Restore transforms
        for (int i = 0; i < m_savedTransforms.m_objects.Length; ++i)
        {
            if (m_savedTransforms.m_objects[i] != null && m_savedTransforms.m_objects[i].m_transform != null)
            {
                m_savedTransforms.m_objects[i].m_object.transform.localPosition = m_savedTransforms.m_objects[i].m_transform.m_localPosition;
                m_savedTransforms.m_objects[i].m_object.transform.localScale = m_savedTransforms.m_objects[i].m_transform.m_localScale;
                m_savedTransforms.m_objects[i].m_object.transform.localRotation = m_savedTransforms.m_objects[i].m_transform.m_localRotation;
            }
        }
	}
}
