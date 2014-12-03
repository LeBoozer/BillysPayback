/*
 * Project:	Billy's Payback
 * File:	PlayerData.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Helper class holding all player relevant data 
 */
public class PlayerData
{
	// Represents a list of all supported types of players
	public enum PlayerType
	{
		// For internal use only!
		PT_NONE			= -1,
		
		PT_COLLECTING	= 0,
		PT_LIGHTNESS 	= 1,
		PT_CHALLENGE	= 2,
		PT_COMPASSION	= 3,
		PT_AUTONOMOUS   = 4,
		PT_PATIENCE		= 5,
		PT_ATTENTION	= 6,
		
		// Internal use only!
		PT_COUNT		= 7
	};
	
	// The raw player characteristics which has been collected during the analysis of the player
	private short[]	m_rawPlayerCharacteristics = new short[(int)PlayerType.PT_COUNT];
	
	// Returns the raw player characteristics
	public short[] getRawPlayerCharacteristics() { return m_rawPlayerCharacteristics; }
	
	// Updates the player characteristics (add the value to the indexed value)
	public void addPlayerCharacteristics(ushort _index, short _value)
	{
		// Check index
		if(_index >= (ushort)PlayerType.PT_COUNT)
			return;
			
		// Update
		m_rawPlayerCharacteristics[_index] += _value;
	}
}
