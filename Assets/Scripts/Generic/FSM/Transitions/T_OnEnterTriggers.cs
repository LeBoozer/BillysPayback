/*
 * Project:	Billy's Payback
 * File:	T_OnEnterTriggers.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * TODO
 */
public class T_OnEnterTriggers : FSMTransition
{	
	// Class to store informations about a trigger object
	[System.Serializable]
	public class Trigger
	{
		// The trigger game-object
		public GameObject	m_triggerObject 	= null;
		
		// Number of hits of the assigned collider with this trigger game-object before the transition will be kicked-off (value < 1 -> value = 1)
		public int 			m_hitsToTrigger 	= 0;
		
		// Number of hits between the collider and the trigger game-object
		[HideInInspector]
		public uint 		m_currentHitCount 	= 0;
	}

	// The collider object (e.g. the player)
	public GameObject 		m_collider;
	
	// List of trigger objects
	public List<Trigger> 	m_triggerObjects = new List<Trigger>();
	
	// Override: FSMTransition::OnEnable
	void OnEnable()
	{	
		// Local variables
		FSMEventHighjack hj = null;
	
		// Check parameter
		if(m_triggerObjects.Count == 0 || m_collider == null)
			return;
		
		// Highjack trigger objects
		foreach(Trigger t in m_triggerObjects)
		{
			// Check
			if(t == null || t.m_triggerObject == null)
				continue;
		
			// Adjust
			t.m_currentHitCount = 0;
			if(t.m_hitsToTrigger < 1)
				t.m_hitsToTrigger = 1;
		
			// Inject script
			hj = t.m_triggerObject.AddComponent<FSMEventHighjack>();
			hj.FSMOnTriggerEnter += (Collider _other) => 
			{
				// Collider hits one of the triggers?
				if(_other.tag.Equals(m_collider.tag) == true)
				{
					// Update hit count
					++t.m_currentHitCount;
				
					// Change state
					if(t.m_currentHitCount >= t.m_hitsToTrigger)
					{
						// Set state
						setTargetFSMState();
					}
				}
			};	
		}		
	}
	
	// Override: FSMTransition::OnDisable
	void OnDisable()
	{
		// Local variables
		FSMEventHighjack hj = null;	
	
		// Delete highjack from trigger objects
		foreach(Trigger t in m_triggerObjects)
		{
			// Check
			if(t == null || t.m_triggerObject == null)
				continue;
				
			// Remove
			hj = t.m_triggerObject.GetComponent<FSMEventHighjack>();
			if(hj != null)
				Component.Destroy(hj);
		}
	}
}