/*
 * Project:	Billy's Payback
 * File:	PlayerCharacteristics.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Basic class for manipulations of the player's characteristics
 */
public abstract class PlayerCharacteristics : MonoBehaviour
{	
	// True to auto-load the player object
	public bool				m_autoloadPlayer = true;

	// List of objects which relate to the manipulation of the player's characteristics
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
	
	// Will be called as soon as the manipulation should be executed
	protected void onApplyCharacteristics()
	{
		// Local variables
		PlayerData pd = Game.Instance.PlayerData;
		
		// Commit characteristics
		pd.addPlayerCharacteristics(PlayerData.PlayerType.PT_COLLECTING, (short)m_collecting);
		pd.addPlayerCharacteristics(PlayerData.PlayerType.PT_LIGHTNESS, (short)m_lightness);
		pd.addPlayerCharacteristics(PlayerData.PlayerType.PT_CHALLENGE, (short)m_challenge);
		pd.addPlayerCharacteristics(PlayerData.PlayerType.PT_COMPASSION, (short)m_compassion);
		pd.addPlayerCharacteristics(PlayerData.PlayerType.PT_AUTONOMOUS, (short)m_autonomous);
		pd.addPlayerCharacteristics(PlayerData.PlayerType.PT_PATIENCE, (short)m_patience);
		pd.addPlayerCharacteristics(PlayerData.PlayerType.PT_ATTENTION, (short)m_attention);
	}
	
	// Override: MonoBehaviour::Start()
	void Start()
	{
		// Local variables
		bool  	   playerFound = false;
		GameObject player      = null;
		
		// Auto-load player?
		if(m_autoloadPlayer == true)
		{
			// Player already in list?
			foreach(GameObject obj in m_objectList)
			{
				// Player tag set?
				if(obj.tag.Equals(Tags.TAG_PLAYER) == true)
				{
					playerFound = true;
					break;
				}
			}
			
			// Player found?
			if(playerFound == false)
			{
				// Find object
				player = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);
				if(player != null)
					m_objectList.Add(player);
			}
		}
	}
}
