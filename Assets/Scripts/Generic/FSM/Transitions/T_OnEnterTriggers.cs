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
		// The trigger collider
		public Collider	    m_triggerCollider 	= null;
		
		// Number of hits of the assigned collider with this trigger game-object before the transition will be kicked-off (value < 1 -> value = 1)
		public int 			m_hitsToTrigger 	= 0;

        // True to delete the trigger object on transition
        public bool         m_deleteObject      = false;
		
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
			if(t == null || t.m_triggerCollider == null)
				continue;

			// Adjust
			t.m_currentHitCount = 0;
			if(t.m_hitsToTrigger < 1)
				t.m_hitsToTrigger = 1;
		
			// Inject script
			hj = t.m_triggerCollider.gameObject.AddComponent<FSMEventHighjack>();
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
                        // Delete trigger object?
                        if (t.m_deleteObject == true)
                        {
                            // Delete from list
                            m_triggerObjects.Remove(t);

                            // Delete object
                            GameObject.Destroy(t.m_triggerCollider);
                        }
                           
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
			if(t == null || t.m_triggerCollider == null)
				continue;
				
			// Remove
			hj = t.m_triggerCollider.gameObject.GetComponent<FSMEventHighjack>();
			if(hj != null)
				Component.Destroy(hj);
		}
	}
}