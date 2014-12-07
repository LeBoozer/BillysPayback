/*
 * Project:	Billy's Payback
 * File:	PlayerCharacteristicsWayGroup.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * TODO
 */
public class PlayerCharacteristicsWayGroup : MonoBehaviour
{
	// The start trigger
	public BoxCollider			m_startTrigger = null;
	
	// The end trigger
	public BoxCollider  		m_endTrigger = null;
	
	// The container game-object for the ways
	public GameObject			m_containerWays = null;
	
	// The player object
	private GameObject 			m_player = null;
	
	// The event high-jack for the start trigger
	private FSMEventHighjack	m_highJackStart = null;
	
	// The event high-jack for the end trigger
	private FSMEventHighjack   	m_highJackEnd = null;
	
	// Override: MonoBehaviour::start()
	void Start()
	{
		// Find player
		m_player = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);
		if(m_player == null)
		{
			Debug.LogError("No player found!");
			return;
		}
	
		// High-jack start and end trigger
		m_highJackStart = m_startTrigger.gameObject.AddComponent<FSMEventHighjack>();
		m_highJackEnd   = m_endTrigger.gameObject.AddComponent<FSMEventHighjack>();
	
		// Add event-handler
		m_highJackStart.FSMOnTriggerEnter += (Collider _other) => 
		{
			// Player?
			if(_other.tag.Equals(m_player.tag) == true)
			{			
				// Deactivate start, activate ways and end
				m_startTrigger.gameObject.SetActive(false);
				m_endTrigger.gameObject.SetActive(true);
				m_containerWays.gameObject.SetActive(true);
				
				// Destroy high-jack
				GameObject.Destroy(m_highJackStart);
				m_highJackStart = null;
			}
		};
		m_highJackEnd.FSMOnTriggerEnter += (Collider _other) => 
		{
			// Player?
			if(_other.tag.Equals(m_player.tag) == true)
			{
				// Deactivate everything
				gameObject.SetActive(false);
				
				// Destroy high-jack
				GameObject.Destroy(m_highJackEnd);
				m_highJackEnd = null;
			}
		};
	
		// Activate start, the rest will be deactived
		m_startTrigger.gameObject.SetActive(true);
		m_endTrigger.gameObject.SetActive(false);
		m_containerWays.gameObject.SetActive(false);
	}
	
	// Override: MonoBehaviour::OnDestroy()
	void OnDestroy()
	{
		// Delete high-jacks
		if(m_highJackStart != null)
		{
			GameObject.Destroy(m_highJackStart);
			m_highJackStart = null;
		}
		if(m_highJackEnd != null)
		{
			GameObject.Destroy(m_highJackEnd);
			m_highJackEnd = null;
		}
	}
}
