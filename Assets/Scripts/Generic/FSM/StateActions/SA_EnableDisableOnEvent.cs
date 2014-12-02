/*
 * Project:	Billy's Payback
 * File:	SA_EnableDisableOnEvent.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * TODO
 */
public class SA_EnableDisableOnEvent : FSMStateAction
{
	// True to enable the target objects on entering the state. Otherwise the behaviour is the other way around.
	public bool 			m_enableOnEnter = true;

	// True to skip the onenter event
	public bool 			m_skipOnEnter = false;
	
	// True to skip the onleave event
	public bool 			m_skipOnLeave = false;

	// List with the target objects
	public List<GameObject> m_targetList = new List<GameObject>();
	
	// Override: FSMStateAction::onActionEnter()
	override public void onActionEnter()
	{
		// Local variables
		bool enable = m_enableOnEnter;
		
		// Skip?
		if(m_skipOnEnter == true)
			return;
		
		// Set
		foreach(GameObject o in m_targetList)
		{
			if(o != null)
				o.SetActive(enable);
		}
	}
	
	// Override: FSMStateAction::onActionLeave()
	override public void onActionLeave()
	{
		// Local variables
		bool enable = !m_enableOnEnter;
		
		// Skip?
		if(m_skipOnLeave == true)
			return;
		
		// Set
		foreach(GameObject o in m_targetList)
		{
			if(o != null)
				o.SetActive(enable);
		}
	}
}
