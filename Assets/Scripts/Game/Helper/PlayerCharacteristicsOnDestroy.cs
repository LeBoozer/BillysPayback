/*
 * Project:	Billy's Payback
 * File:	PlayerCharacteristicsOnDestroy.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Updates the player characteristics on "OnDestroy" events
 */
public class PlayerCharacteristicsOnDestroy : PlayerCharacteristics
{
	// Override: MonoBehaviour::Start()
	void Start()
	{
		// Local variables
		FSMEventHighjack hj = null;
	
		// High-jack all objects
		foreach(GameObject obj in m_objectList)
		{
			hj = obj.gameObject.AddComponent<FSMEventHighjack>();
			hj.FSMOnDestroy += () => 
			{
				// Remove from list
				m_objectList.Remove(obj);
				if(m_objectList.Count <= 0)
				{
					onApplyCharacteristics();
					if(gameObject != null)
						gameObject.SetActive(false);
				}
			};
		}
	}
	
	// Override: MonoBehaviour::OnDestroy()
	void OnDestroy()
	{
		// Local variables
		FSMEventHighjack hj = null;
	
		// Remove high-jack
		foreach(GameObject obj in m_objectList)
		{
			if(obj == null)
				continue;
			hj = obj.GetComponent<FSMEventHighjack>();
			if(hj != null)
				GameObject.Destroy(hj);
		}
	}
}