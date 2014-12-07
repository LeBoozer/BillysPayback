/*
 * Project:	Billy's Payback
 * File:	SimpleDialogueParser.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

/*
 * Parses simple dialogue streams
 */
public abstract class SimpleDialogueParser
{
	// Parses a input stream in form of a string. 
	public static SimpleDialogue parseText(string _text)
	{
		// Local variables
		SimpleDialogue d = null;
		XmlDocument xmlDoc = null;
		SimpleDialogue.TextSlot textSlot = null;
		SimpleDialogue.ActionSlot actionSlot = null;
		List<SimpleDialogue.TextSlot> textSlots = new List<SimpleDialogue.TextSlot>();
		List<SimpleDialogue.ActionSlot> actionSlots = new List<SimpleDialogue.ActionSlot>();
		
		// Check parameter
		if(_text == null || _text.Length == 0)
			return null;
			
		// Open string reader
		using(StringReader stringReader = new StringReader(_text))
		{
			// Create xml document
			xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(_text);
			
			// Get text
			for(int i = 0; ;)
			{
				// Get text slot
				XmlNode xmlNode = xmlDoc.SelectSingleNode("text/text" + i);
				if(xmlNode == null)
					break;
				XmlNode attTime = xmlNode.Attributes.GetNamedItem("time");
				if(attTime == null)
					break;
					
				// Create slot
				textSlot = new SimpleDialogue.TextSlot();
				textSlot.m_slotID 			= i;
				textSlot.m_text   			= xmlNode.InnerText;
				if(int.TryParse(attTime.Value, out textSlot.m_timeInSeconds) == false)
					break;
				textSlots.Add(textSlot);
				++i;
			}
			
			// Action available?
			XmlNode xmlActions = xmlDoc.SelectSingleNode("text/actions");
			if(xmlActions != null)
			{
				// Get actions
				for(int i = 0; ;)
				{
					// Get text slot
					XmlNode xmlAction = xmlDoc.SelectSingleNode("text/actions/action" + i);
					if(xmlAction == null)
						break;
					XmlNode attType = xmlAction.Attributes.GetNamedItem("type");
					if(attType == null)
						break;
					
					// Create action
					actionSlot = new SimpleDialogue.ActionSlot();
					actionSlot.m_type	= (SimpleDialogue.Action)Enum.Parse(typeof(SimpleDialogue.Action), attType.Value, true);
					actionSlot.m_text   = xmlAction.InnerText;
					actionSlots.Add(actionSlot);
					++i;
				}
			}
		}
		
		// Create dialog
		d = new SimpleDialogue(textSlots.ToArray(), actionSlots.ToArray());
	
		return d;
	}
}