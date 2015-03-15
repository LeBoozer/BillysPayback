/*
 * Project:	Billy's Payback
 * File:	AutoTransparent.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * ToDo
 */
public class CP_FlockOfBirds : CheckPoint
{
    // The flock of birds game-object
    public GameObject m_flockOfBirds = null;

    // The activation delay in seconds
    public float      m_activationDelay = 1.0f;

    // Override: MonoBehaviour::Awake()
    new void Awake()
    {
        // Call parental function
        base.Awake();

        // Define check point action
        m_checkPointAction = () =>
        {
            // Disable and re-enable flock of birds
            if (m_flockOfBirds != null)
            {
                m_flockOfBirds.SetActive(false);
                Invoke("activateFlockOfBirds", m_activationDelay);
            }
        };
    }

    // Activates the flock of birds
    void activateFlockOfBirds()
    {
        m_flockOfBirds.SetActive(true);
    }
}
