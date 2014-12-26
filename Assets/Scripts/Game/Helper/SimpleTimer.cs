/*
 * Project:	Billy's Payback
 * File:	SimpleTimer.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Timers;
using System.Collections;

/*
 * Provides a simple timer 
 */
public class SimpleTimer
{
    // The timer instance
    private Timer m_timer = null;

    // True if the time has been elapsed
    private bool m_isDone = false;
    public bool Done
    {
        get { return m_isDone; }
        private set { }
    }


    // Resets and restart the timer
    public void restart(float _interval)
    {
        // Reset flag
        m_isDone = false;

        // Create timer
        m_timer = new Timer(_interval);
        m_timer.Enabled = true;
        m_timer.Elapsed += (object _sender, ElapsedEventArgs _args) => 
            {
                m_isDone = true;
            };
    }
}
