/*
 * Project:	Billy's Payback
 * File:	PlayerCharacteristicsWay.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Updates the player characteristics on "ontriggerenter" events of the trigger the script is attached to
 */
public class PlayerCharacteristicsWay : PlayerCharacteristics
{
	// The trigger object
	private Collider m_triggerObject = null;

	// Override: MonoBehaviour::Awake()
	void Awake()
	{
		// Get collider shape
		m_triggerObject = gameObject.GetComponent<Collider>();
	}
	
	// Override: MonoBehaviour::OnTriggerEnter()
	void OnTriggerEnter(Collider _other)
	{
		// Check against all objects
		foreach(GameObject obj in m_objectList)
		{
			if(_other.gameObject.Equals(obj) == true)
			{
				// Apply characteristics
				onApplyCharacteristics();
				
				// Deactivate trigger
				if(m_triggerObject != null)
					m_triggerObject.gameObject.SetActive(false);
				return;
			}
		}
	}
}
