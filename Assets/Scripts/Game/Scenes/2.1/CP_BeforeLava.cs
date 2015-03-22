/*
 * Project:	Billy's Payback
 * File:	CP_BeforeLava.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * ToDo
 */
public class CP_BeforeLava : CheckPoint
{
    // Saved transforms
    public TA_SaveObjectTransform m_savedTransforms = null;

    // The way controller of the lava
    public MoveAlongPath m_lavaWay = null;

    // The button event for the red one
    public OnAreaKeyEvent m_redButtonEvent = null;

    // The button event for the blue one
    public OnAreaKeyEvent m_blueButtonEvent = null;

    // Override: MonoBehaviour::Awake()
    new void Awake()
    {
        // Call parental function
        base.Awake();

        // Define check point action
        m_checkPointAction = () =>
        {
            // Check
            if (m_savedTransforms == null || m_savedTransforms.m_objects == null || m_savedTransforms.m_objects.Length == 0)
                return;

            // Activate buttons events
            if (m_redButtonEvent != null)
                m_redButtonEvent.onActivate();
            if (m_blueButtonEvent != null)
                m_blueButtonEvent.onActivate();

            // Deactivate way controller
            if (m_lavaWay != null)
                m_lavaWay.onDeactivate();

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
        };
    }
}
