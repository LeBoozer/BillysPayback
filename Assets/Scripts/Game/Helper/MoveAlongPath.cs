/*
 * Project:	Billy's Payback
 * File:	MoveAlongPath.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Moves the reference object along a certain path. All child game-objects will be used as path nodes!
 */
public class MoveAlongPath : MonoBehaviour, DeActivatable
{
    // Represents a list with all possible end-rules
    public enum EndRule
    {
        // Just stops the animation
        None,

        // Handles the nodes as ring buffer
        Loop,

        // Move the path in the opposite direction
        Return
    };

    // True if activated
    public bool m_isActivated = true;

    // The reference object
    public Transform m_reference = null;

    // The easing type
    public EasingType m_easingType = EasingType.Linear;

    // The specified end-rule
    public EndRule m_endRule = EndRule.None;

    // The max speed (world units per second)
    public float m_maxSpeed = 1.0f;

    // The smoothness factor
    public float m_smoothnessFactor = 100.0f;

    // True to ease in the function
    public bool m_easeIn = true;

    // True to ease out the function
    public bool m_easeOut = true;

    // True if the nodes creates a valid path
    private bool m_validPath = false;

    // True to stop the movement
    private bool m_isMovementStopped = false;

    // List with path nodes (in form of tranforms)
    private List<Transform> m_nodeList = new List<Transform>();

    // Array with all path nodes
    private Transform[] m_nodeArray = null;

    // The current path position
    private float m_currentPathPosition = 0;

    // Override: MonoBehaviour::Awake()
	void Awake () 
    {
        // Retrieve all nodes
        foreach (Transform child in gameObject.transform)
        {
            // Add to list
            m_nodeList.Add(child.gameObject.transform);
        }
        if (m_nodeList.Count == 0)
            return;

	    // Create array from list
        m_nodeArray = new Transform[m_nodeList.Count];
        m_nodeArray = m_nodeList.ToArray();

        // Set flag
        m_validPath = true;
	}

    // Override: MonoBehaviour::Update()
	void Update () 
    {
        // Local variables
        float currentPathPosition = m_currentPathPosition;

        // Activated?
        if (m_isActivated == false)
            return;

        // Valid?
        if (!m_validPath || m_isMovementStopped)
            return;

        // Move reference
        m_reference.position = Spline.MoveOnPath(m_nodeArray, m_reference.position, ref currentPathPosition, m_maxSpeed, m_smoothnessFactor, m_easingType, m_easeIn, m_easeOut);

        // End reached?
        if (currentPathPosition == 1 && m_currentPathPosition != 1)
        {
            // No end-rule set?
            if(m_endRule == EndRule.None)
            {
                m_isMovementStopped = true;
                return;
            }
            else if(m_endRule == EndRule.Loop)
            {
                currentPathPosition = 0.0f;
            }
            else if(m_endRule == EndRule.Return)
            {
                // Reverse path
                currentPathPosition = 0.0f;
                m_nodeList.Reverse();
                m_nodeArray = m_nodeList.ToArray();
            }
        }

        // Copy position
        m_currentPathPosition = currentPathPosition;
	}

    // Override: DeActivatable::isActivated()
    public bool isActivated()
    {
        return m_isActivated;
    }

    // Override: DeActivatable::Start()
    public void onActivate()
    {
        // Set flag
        m_isActivated = true;
    }

    // Override: DeActivatable::Start()
    public void onDeactivate()
    {
        // Clear flag
        m_isActivated = false;
    }
}
