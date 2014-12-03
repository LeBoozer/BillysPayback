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
#region (Characteristics)
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
	
	// Returns the player characteristics for a certain player type
	public short getPlayerCharacteristics(PlayerType _type)
	{
		// Check type
		if(_type == PlayerType.PT_COUNT || _type == PlayerType.PT_NONE)
			return 0;	
		return m_rawPlayerCharacteristics[(int)_type];
	}
	
	// Updates the player characteristics (add the value to the indexed value)
	public void addPlayerCharacteristics(PlayerType _type, short _value)
	{
		// Check type
		if(_type == PlayerType.PT_COUNT || _type == PlayerType.PT_NONE)
			return;
			
		// Update
		m_rawPlayerCharacteristics[(int)_type] += _value;
	}
#endregion

#region (Attributes)
	// Represents a power-up
	private class PowerUp
	{
		// True if this power-up is available
		public bool 	m_available 	= false;
		
		// Number of units left for this power-up
		public int 		m_stockSize 	= 0;
	}

	// Represents a list with all supported power-ups
	public enum PowerUpType
	{
		// Internal use only!
		PUT_NONE 		= -1,
		
		// The cherry gets access to a protective casing made from stings
		PUT_KIWANO 		= 0,
		
		// The cherry can "shoot" little raspberries
		PUT_RASPBERRY 	= 1,
		
		// The cherry can "fly"
		PUT_ORANGE      = 2,
		
		// Internal use only!
		PUT_COUNT 		= 2
	}

	// Life points of billy
	private int			m_lifePoints 	= GameConfig.BILLY_LIFE_POINT;
	
	// List of all power-ups
	private PowerUp[] 	m_powerUps 		= new PowerUp[(int)PowerUpType.PUT_COUNT];

	// Returns the number of remaining life points
	public int getLifePoints()
	{
		return m_lifePoints;
	}
	
	// Sets a new number of life points
	public void setLifePoints(int _num)
	{
		m_lifePoints = _num;
	}
	
	// Increases the life points by the defined value
	public void increaseLifePointsByValue(PowerUpType _type, int _v)
	{
		// Decrease
		m_lifePoints += _v;	
	}
	
	// Decreases the life points by the defined value
	public void decreaseLifePointsByValue(PowerUpType _type, int _v)
	{
		// Decrease
		m_lifePoints -= _v;
		if(m_lifePoints < 0)
			m_lifePoints = 0;
	}	

	// Returns whether the specified power-up is available
	public bool isPowerUpAvailable(PowerUpType _type)
	{
		// Check type
		if(_type == PowerUpType.PUT_NONE || _type == PowerUpType.PUT_COUNT)
			return false;
		return m_powerUps[(int)_type].m_available;
	}
	
	// Sets whether the specified power-up is available
	public void setPowerUpAvailable(PowerUpType _type, bool _t)
	{
		// Check type
		if(_type == PowerUpType.PUT_NONE || _type == PowerUpType.PUT_COUNT)
			return;
		m_powerUps[(int)_type].m_available = _t;
	}
	
	// Returns the number of left units for the specified power-up
	public int getPowerUpStockSize(PowerUpType _type)
	{
		// Check type
		if(_type == PowerUpType.PUT_NONE || _type == PowerUpType.PUT_COUNT)
			return 0;
		return m_powerUps[(int)_type].m_stockSize;
	}
	
	// Sets a new stock size for the specified power-type
	public void setPowerUpStockSize(PowerUpType _type, int _size)
	{
		// Check type
		if(_type == PowerUpType.PUT_NONE || _type == PowerUpType.PUT_COUNT)
			return;
		m_powerUps[(int)_type].m_stockSize = _size;
		if(m_powerUps[(int)_type].m_stockSize <= 0)
		{
			m_powerUps[(int)_type].m_stockSize = 0;
			m_powerUps[(int)_type].m_available = false;
		}
	}
	
	// Increases the stock size of the specified power-up by the defined value
	public void increaseStockSizeByValue(PowerUpType _type, int _v)
	{
		// Check type
		if(_type == PowerUpType.PUT_NONE || _type == PowerUpType.PUT_COUNT)
			return;
		m_powerUps[(int)_type].m_stockSize += _v;
		m_powerUps[(int)_type].m_available = true;
	}
	
	// Decreases the stock size of the specified power-up by the defined value
	public void decreaseStockSizeByValue(PowerUpType _type, int _v)
	{
		// Check type
		if(_type == PowerUpType.PUT_NONE || _type == PowerUpType.PUT_COUNT)
			return;
		m_powerUps[(int)_type].m_stockSize -= _v;
		if(m_powerUps[(int)_type].m_stockSize <= 0)
		{
			m_powerUps[(int)_type].m_stockSize = 0;
			m_powerUps[(int)_type].m_available = false;
		}
	}
#endregion
}
