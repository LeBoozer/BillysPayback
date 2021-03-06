﻿/*
 * Project:	Billy's Payback
 * File:	FSMTransition.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Represents the basic transition for the FSM
 */
public abstract class FSMTransition : MonoBehaviour, DeActivatable
{	    
    // True if the transition is activated
    public bool             m_isActivated = true;

	// The host FSM-state
	protected FSMState 		m_hostState = null;
	
	// The FSM instance
	protected FSM 			m_fsm = null;

    // True of the start method has been called
    protected bool          m_startWasCalled = false;
	
	// The target FSM-state
	public FSMState 		m_targetState = null;

    // True to delete this transition as soon as this transition has been used
    public bool             m_deleteOnUse = false;
	
	// List of actions which will be activated as soon as the transition is being activated
	public List<FSMAction>	m_actionsOnTransition = new List<FSMAction>();
	
	// Sets the target FSM-state 
	public virtual void setTargetFSMState()
	{
        // Deactivated?
        if (m_isActivated == false)
            return;

		// Trigger actions
		foreach(FSMAction a in m_actionsOnTransition)
			a.onAction();

        // Set state
        if (m_fsm != null)
            m_fsm.setFSMState(m_targetState);

        // Kill itself?
        if (m_deleteOnUse == true)
            GameObject.Destroy(gameObject);
	}

    // Enables/disables the state
    public void setEnabled(bool _onOff)
    {
        if (gameObject != null)
            gameObject.SetActive(_onOff);
    }

    // Returns true if start has been called
    public bool wasStartCalled()
    {
        return m_startWasCalled;
    }
	
	// Will be called as soon as the host-state has finished its work (this call is not guaranteed and depends heavily on the host state!)
	public virtual void onHostStateDone(object _param)
	{
        // Set target state
        setTargetFSMState();
	}
	
	// Override: MonoBehaviour::Awake()
	void Awake()
	{
		// Local variables
		FSMAction[] action = null;

        // Retrieve all actions
        action = gameObject.GetComponentsInChildren<FSMAction>();
        m_actionsOnTransition.AddRange(action);
	}
	
	// Override: MonoBehaviour::Start()
    protected virtual void Start()
	{
		// Retrieve FSM instance
        m_fsm = gameObject.GetComponentInParent<FSM>();	
		
		// Retrieve host FSM-state
		m_hostState = gameObject.transform.parent.gameObject.GetComponent<FSMState>();

        // Set flag
        m_startWasCalled = true;
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