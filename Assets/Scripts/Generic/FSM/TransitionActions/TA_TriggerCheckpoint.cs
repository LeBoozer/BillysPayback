/*
 * Project:	Billy's Payback
 * File:	TA_TriggerCheckpoint.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * ToDo
 */
public class TA_TriggerCheckpoint : FSMAction
{
    // The checkpoint
    public CheckPoint m_checkPoint = null;

    // Override: FSMStateAction::onAction()
    override public void onAction()
    {
        // Check parameter
        if (m_checkPoint == null)
            return;

        // Activate check point
        m_checkPoint.onActivate();

        // Trigger check point
        m_checkPoint.trigger();
    }
}
