/*
 * Project:	Billy's Payback
 * File:	AdvancedDialogueParser.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/*
 * Parses advanced dialogue streams
 */
public abstract class AdvancedDialogueParser
{
    // Parses an input stream in form of a string
    public static AdvancedDialogue parseAdvancedDialog(string _text)
    {
        // Local variables
        AdvancedDialogue dialogue = null;
        XmlDocument xmlDoc = null;
        XmlNode xmlNodeConv = null;
        Conversation conv = null;
        List<Conversation> convList = new List<Conversation>();

        // Check parameter
        if(_text == null || _text.Length == 0)
        {
            Debug.LogError("Empty input!");
            return null;
        }

        // Open XML document
        xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(_text);

        // Find first conversation node
        xmlNodeConv = xmlDoc.SelectSingleNode("bw/conversation");
        if (xmlNodeConv == null)
        {
            Debug.LogError("No conversations has been found!");
            return null;
        }

        // Parse all conversations
        do
        {
            // Parse
            conv = parseConversation(xmlNodeConv);
            if (conv == null)
                return null;

            // Add to list
            convList.Add(conv);

            // Next sibling
            xmlNodeConv = xmlNodeConv.NextSibling;
        }
        while (xmlNodeConv != null);

        return dialogue;
    }

    // Parses a conversation
    private static Conversation parseConversation(XmlNode _node)
    {
        return null;
    }
}
