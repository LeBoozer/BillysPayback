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
    public static AdvancedDialogue parseDialog(string _text)
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

        // Create dialogue
        dialogue = new AdvancedDialogue(convList);

        return dialogue;
    }

    // Parses a conversation
    private static Conversation parseConversation(XmlNode _node)
    {
        // Local variables
        int conversationID = -1;
        int conversationStartID = -1;
        Conversation conversation = null;
        XmlNode xmlNodeText = null;
        XmlNode xmlNodeAtt = null;
        DialogueText text = null;
        List<DialogueText> textList = new List<DialogueText>();

        // Get attribute: id
        xmlNodeAtt = _node.Attributes.GetNamedItem("id");
        if (xmlNodeAtt == null)
        {
            Debug.LogError("Conversations need IDs!");
            return null;
        }
        conversationID = int.Parse(xmlNodeAtt.Value);

        // Get attribute: start
        xmlNodeAtt = _node.Attributes.GetNamedItem("start");
        if (xmlNodeAtt == null)
        {
            Debug.LogError("Conversations need a start text ID!");
            return null;
        }
        conversationStartID = int.Parse(xmlNodeAtt.Value);

        // Find first text node
        xmlNodeText = _node.SelectSingleNode("text");
        if (xmlNodeText == null)
        {
            Debug.LogError("No texts has been found!");
            return null;
        }

        // Parse all texts
        do
        {
            // Parse
            text = parseText(xmlNodeText);
            if (text == null)
                return null;

            // Add to list
            textList.Add(text);

            // Next sibling
            xmlNodeText = xmlNodeText.NextSibling;
        }
        while (xmlNodeText != null);

        // Create conversation
        conversation = new Conversation(conversationID, conversationStartID, textList);

        return conversation;
    }

    // Parses a text
    private static DialogueText parseText(XmlNode _node)
    {
        // Local variables
        int textID = -1;
        DialogueText text = null;
        XmlNode xmlNode = null;
        XmlNode xmlNodeAtt = null;
        TextPart part = null;
        Choice choice = null;
        List<TextPart> partList = new List<TextPart>();
        List<Choice> choiceList = new List<Choice>();

        // Get attribute: id
        xmlNodeAtt = _node.Attributes.GetNamedItem("id");
        if (xmlNodeAtt == null)
        {
            Debug.LogError("Dialogue texts need IDs!");
            return null;
        }
        textID = int.Parse(xmlNodeAtt.Value);

        // Find first text/choice node
        xmlNode = _node.FirstChild;
        if (xmlNode == null)
        {
            Debug.LogError("No text-parts or choices has been found!");
            return null;
        }

        // Parse all text-parts/choices
        do
        {
            // Text-part?
            if (xmlNode.Name.Equals("part") == true)
            {
                // Parse
                part = parseTextPart(xmlNode);
                if (part == null)
                    return null;

                // Add to list
                partList.Add(part);
            }

            // Choice?
            else if (xmlNode.Name.Equals("choice") == true)
            {
                // Parse
                choice = parseChoice(xmlNode);
                if (choice == null)
                    return null;

                // Add to list
                choiceList.Add(choice);
            }

            // Unknown
            else
                Debug.LogWarning("Unknown XML node with a text");

            // Next sibling
            xmlNode = xmlNode.NextSibling;
        }
        while (xmlNode != null);

        // Create text
        text = new DialogueText(textID, choiceList, partList);

        return text;
    }

    // Parses a text part
    private static TextPart parseTextPart(XmlNode _node)
    {
        // Local variables
        string text = "";
        string character = "";
        double timeInSec = -1;
        TextPart part = null;
        XmlNode xmlNodeAtt = null;

        // Get attribute: id
        xmlNodeAtt = _node.Attributes.GetNamedItem("t");
        if (xmlNodeAtt == null)
        {
            Debug.LogError("Text-parts need a defined display time (in seconds)!");
            return null;
        }
        timeInSec = double.Parse(xmlNodeAtt.Value);

        // Get attribute: character
        xmlNodeAtt = _node.Attributes.GetNamedItem("character");
        if (xmlNodeAtt != null)
            character = xmlNodeAtt.Value;

        // Get text
        text = _node.InnerText;
        if(text == null || text.Length == 0)
        {
            Debug.LogError("Text-parts need text!");
            return null;
        }

        // Create text part
        part = new TextPart(timeInSec, character, text);

        return part;
    }

    // Parses a choice
    private static Choice parseChoice(XmlNode _node)
    {
        // Local variables
        string text = "";
        int choiceID = -1;
        int nextTextID = -1;
        string exitValue = "";
        Choice choice = null;
        XmlNode xmlNodeAtt = null;

        // Get attribute: id
        xmlNodeAtt = _node.Attributes.GetNamedItem("id");
        if (xmlNodeAtt == null)
        {
            Debug.LogError("Choices need IDs!");
            return null;
        }
        choiceID = int.Parse(xmlNodeAtt.Value);

        // Get attribute: next text ID
        xmlNodeAtt = _node.Attributes.GetNamedItem("nextText");
        if (xmlNodeAtt != null)
            nextTextID = int.Parse(xmlNodeAtt.Value);

        // Get attribute: exit value
        xmlNodeAtt = _node.Attributes.GetNamedItem("exit");
        if (xmlNodeAtt != null)
            exitValue = xmlNodeAtt.Value;

        // Get text
        text = _node.InnerText;
        if (text == null || text.Length == 0)
        {
            Debug.LogError("Choices need text!");
            return null;
        }

        // Create choice
        choice = new Choice(choiceID, text, nextTextID, exitValue);

        return choice;
    }
}