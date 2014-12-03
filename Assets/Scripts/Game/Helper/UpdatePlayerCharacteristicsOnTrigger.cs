/*
 * Project:	Billy's Payback
 * File:	UpdatePlayerCharacteristicsOnTrigger.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Updates the player characteristics on "ontriggerenter" events of the trigger the script is attached to
 */
public class UpdatePlayerCharacteristicsOnTrigger : MonoBehaviour
{
	// The trigger objects
	private Collider 		m_triggerObject = null;

	// List of objects the trigger reacts to
	public List<GameObject>	m_objectList 	= new List<GameObject>();

	// Player characteristic: collecting
	public int 				m_collecting 	= 0;
	
	// Player characteristic: lightness
	public int 				m_lightness 	= 0;
	
	// Player characteristic: challenge
	public int 				m_challenge 	= 0;
	
	// Player characteristic: compassion
	public int 				m_compassion 	= 0;
	
	// Player characteristic: autonomous
	public int 				m_autonomous 	= 0;
	
	// Player characteristic: patience
	public int 				m_patience 		= 0;
	
	// Player characteristic: attention
	public int 				m_attention 	= 0;

	// Will be called as soon as an hit of one registered objects has been triggered
	private void onHit()
	{
		// Local variables
		PlayerData pd = Game.Instance.PlayerData;
	
		// Disable collider
		if(m_triggerObject != null)
			m_triggerObject.gameObject.SetActive(false);
			
		// Commit characteristics
		pd.addPlayerCharacteristics((ushort)PlayerData.PlayerType.PT_COLLECTING, (short)m_collecting);
		pd.addPlayerCharacteristics((ushort)PlayerData.PlayerType.PT_LIGHTNESS, (short)m_lightness);
		pd.addPlayerCharacteristics((ushort)PlayerData.PlayerType.PT_CHALLENGE, (short)m_challenge);
		pd.addPlayerCharacteristics((ushort)PlayerData.PlayerType.PT_COMPASSION, (short)m_compassion);
		pd.addPlayerCharacteristics((ushort)PlayerData.PlayerType.PT_AUTONOMOUS, (short)m_autonomous);
		pd.addPlayerCharacteristics((ushort)PlayerData.PlayerType.PT_PATIENCE, (short)m_patience);
		pd.addPlayerCharacteristics((ushort)PlayerData.PlayerType.PT_ATTENTION, (short)m_attention);
	}

	// Override: MonoBehaviour::Awake()
	void Awake()
	{
		// Get collider shape
		m_triggerObject = gameObject.GetComponent<Collider>();
	}

	// Override: MonoBehaviour::Start()
	void Start()
	{
		// Local variables
		GameObject player = null;
	
		// Not object available? Try to auto locate player object
		if(m_objectList.Count == 0)
		{
			// Find object
			player = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);
			if(player != null)
				m_objectList.Add(player);
		}
	}
	
	// Override: MonoBehaviour::OnTriggerEnter()
	void OnTriggerEnter(Collider _other)
	{
		// Check against all objects
		foreach(GameObject obj in m_objectList)
		{
			if(_other.gameObject.Equals(obj) == true)
			{
				onHit();
				return;
			}
		}
	}
}
