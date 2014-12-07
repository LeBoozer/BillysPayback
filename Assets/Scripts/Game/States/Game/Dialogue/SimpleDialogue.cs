/*
 * Project:	Billy's Payback
 * File:	SimpleDialogue.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Represents a simple dialog
 */
public class SimpleDialogue
{
	// Represents a list with all available actions
	[System.Serializable]
	public enum Action
	{
		YES,
		NO,
		CUSTOM0,
		CUSTOM1,
		CUSTOM2
	}
	
	// Represents a simple text slot
	public class TextSlot
	{
		public int		m_slotID 		= 0;
		public string	m_text   		= "";
		public int 		m_timeInSeconds = 0;
	}
	
	// Represent a simple action slot
	public class ActionSlot
	{
		public Action 	m_type	= Action.YES;
		public string 	m_text	= "";
	}
	
	// List of text slots
	private TextSlot[] 		m_textSlots		= null;
	
	// List of action slots
	private ActionSlot[]	m_actionSlots	= null;
	
	// Constructor
	public SimpleDialogue(TextSlot[] _textSlots, ActionSlot[] _actionSlots)
	{
		// Copy
		m_textSlots 	= _textSlots;
		m_actionSlots	= _actionSlots;
	}
	
	// Determines whether the dialogue is valid
	public bool isValid()
	{
		return m_textSlots != null && m_textSlots.Length > 0;
	}
	
	// Returns the number of text slots
	public int getNumTextSlots()	{ return m_textSlots.Length; }
	
	// Returns the number of action slots
	public int getNumActionSlots()	{ return m_actionSlots.Length; }
	
	// Returns a text slot specified by its index or null
	public TextSlot getTextSlotByIndex(int _index)
	{
		if(_index < 0 || _index >= m_textSlots.Length)
			return null;
		return m_textSlots[_index];
	}
	
	// Returns an action slot specified by its index or null
	public ActionSlot getActionSlotByIndex(int _index)
	{
		if(_index < 0 || _index >= m_actionSlots.Length)
			return null;
		return m_actionSlots[_index];
	}
}
