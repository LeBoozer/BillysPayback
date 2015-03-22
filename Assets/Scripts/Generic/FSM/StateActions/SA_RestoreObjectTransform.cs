/*
 * Project:	Billy's Payback
 * File:	SA_RestoreObjectTransform.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * ToDo
 */
public class SA_RestoreObjectTransform : FSMStateAction
{
    // The saved object transforms
    public SA_SaveObjectTransform m_savedTransforms = null;

    // Override: FSMStateAction::onActionEnter()
    override public void onActionEnter()
    {
        // Check
        if (m_savedTransforms == null || m_savedTransforms.m_objects == null || m_savedTransforms.m_objects.Length == 0)
            return;

        // Restore transforms of the registered objects
        foreach (var ent in m_savedTransforms.m_objects)
        {
            // Valid object?
            if (ent.m_object == null || ent.m_transform == null)
                continue;

            // Restore values
            ent.m_object.transform.localPosition = ent.m_transform.m_localPosition;
            ent.m_object.transform.localScale = ent.m_transform.m_localScale;
            ent.m_object.transform.localRotation = ent.m_transform.m_localRotation;
        }
    }

    // Override: FSMStateAction::onActionLeave()
    override public void onActionLeave()
    {
    }
}
