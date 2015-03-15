/*
 * Project:	Billy's Payback
 * File:	T_OnInvalidDistance.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * The transition will be triggered as soon as the target objects invalidates the distance parameter
 */
public class T_OnInvalidDistance : FSMTransition
{
    // The reference object
    public GameObject m_reference = null;

    // List of target objects
    public GameObject[] m_targetObjects = null;

    // The min distance between the reference and the targets
    public float m_minDistance = 0.0f;

    // The max distance between the reference and the targets
    public float m_maxDistance = 1.0f;

    // The time interval for the distance checking (in seconds)
    public float m_checkInterval = 1.0f;

    // True to check all distances before triggering the transition
    public bool  m_checkAgainstAll = true;

    // True to debug draw the distances
    public bool  m_debugDrawDistances = false;

    // The started coroutine for the distance checking
    private Coroutine m_coroutine = null;

    // Override: FSMTransition::OnEnable
    void OnEnable()
    {
        // Validate reference
        if(m_reference == null)
        {
            Debug.LogError("No reference object has been specified!");
            return;
        }

        // Validate target objects
        if(m_targetObjects == null || m_targetObjects.Length == 0)
        {
            Debug.LogError("No target objects has been specified!");
            return;
        }

        // Start distance checking
        m_coroutine = StartCoroutine("proc_checkDistance");
    }

    // Override: FSMTransition::OnDisable
    void OnDisable()
    {
        // Stop the distance checking
        if (m_coroutine != null)
            StopCoroutine(m_coroutine);
    }

    // Checks the distance
    private bool checkDistance()
    {
        // Local variables
        Vector3 direction = Vector3.zero;
        float distance = 0.0f;
        bool distanceSuccess = false;

        // Reference deleted?
        if (m_reference == null)
            return false;

        // Loop through all target objects
        foreach(GameObject obj in m_targetObjects)
        {
            // Valid?
            if (obj == null)
                continue;

            // Debug draw?
            if (m_debugDrawDistances == true)
            {
                Debug.DrawLine(m_reference.transform.position, obj.transform.position, Color.green, m_checkInterval);
            }

            // Get direction and distance
            direction = obj.transform.position - m_reference.transform.position;
            distance = direction.magnitude;
            direction.Normalize();

            // Check distance
            if (distance <= m_minDistance || distance >= m_maxDistance)
            {
                // Check all?
                if(m_checkAgainstAll == false)
                {
                    // Set target state
                    setTargetFSMState();
                    return false;
                }
            }
            else
            {
                distanceSuccess = true;
                if (m_checkAgainstAll == false)
                    return true;
            }
        }

        // All tests failed?
        if (distanceSuccess == false)
        {
            setTargetFSMState();
            return false;
        }

        return true;
    }

    // Coroutine for the distance checking
    private IEnumerator proc_checkDistance()
    {
        // Loop endless
        while(true)
        {
            if(checkDistance() == false)
                yield break;
            else
                yield return new WaitForSeconds(m_checkInterval);
        }
    }
}
