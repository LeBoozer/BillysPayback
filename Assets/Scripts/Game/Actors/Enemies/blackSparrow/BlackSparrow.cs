/*
 * Project:	Billy's Payback
 * File:	BlackSparrow.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


/**
 * Controll the boss BlackSparrow
 */
public class BlackSparrow : Enemy, Boss
{
    #region Variable

    // death handling
    private bool                m_isActive;
    private LinkedList<Action>  m_deathEvent;

    #endregion

    // Use this for initialization
	void Start () 
    {
        base.Start();
	}

    void FixedUpdate()
    {
        base.FixedUpdate();
    }

	// Update is called once per frame
	void Update () 
    {
        base.Update();
    }

    #region Boss

    // let the boss fight start
    public void StartBossFight()
    {
        m_isActive = true;
    }

    // let the boss fight end
    public void EndBossFight(Action _event)
    {
        m_deathEvent.AddLast(_event);
    }

    // let the boss fight break
    public void BreakBossFight() 
    { 
        m_isActive = false; 
    }

    internal override void die()
    {
        foreach (Action _a in m_deathEvent)
            _a();
        base.die();
    }

    #endregion
}
