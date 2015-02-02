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
    // Determines whether a certain XML node can be skipped (e.g. comments)
    public static bool isNodeSkipable(XmlNode _node)
    {
        // Validate parameter
        if (_node == null)
            return true;

        // Name?
        if(_node.Name != null)
        {
            // Comment?
            if (_node.Name.Equals("#comment") == true)
                return true;
        }

        return false;
    }

    // Parses an input stream in form of a string
    public static AdvancedDialogue parseDialog(string _text)
    {
        // Local variables
        AdvancedDialogue dialogue = null;
        XmlDocument xmlDoc = null;
        XmlNode xmlNode = null;
        XmlNode xmlNodeAtt = null;
        int importStandardAssets = 0;
        Conversation conv = null;
        DynamicCode dyncode = null;
        List<Conversation> convList = new List<Conversation>();
        List<DynamicCode> dyncodeList = new List<DynamicCode>();

        // Check parameter
        if(_text == null || _text.Length == 0)
        {
            Debug.LogError("Empty input!");
            return null;
        }

        // Open XML document
        xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(_text);

        // Get first node (bw)
        xmlNode = xmlDoc.SelectSingleNode("bw");
        if (xmlNode == null)
        {
            Debug.LogError("Invalid XML format!");
            return null;
        }

        // Import standard assets?
        xmlNodeAtt = xmlNode.Attributes.GetNamedItem("importStandardAssets");
        if (xmlNodeAtt != null)
            importStandardAssets = int.Parse(xmlNodeAtt.Value);

        // Find first conversation or dynamic code node
        xmlNode = xmlNode.FirstChild;
        if (xmlNode == null)
        {
            Debug.LogError("No conversations or dynamic codes has been found!");
            return null;
        }

        // Parse all conversations
        do
        {
            // Skipable?
            if (isNodeSkipable(xmlNode) == false)
            {
                // Dynamic code?
                if (xmlNode.Name.Equals("dyncode") == true)
                {
                    // Parse
                    dyncode = parseDynamicCode(xmlNode);
                    if (dyncode == null)
                        return null;

                    // Add to list
                    dyncodeList.Add(dyncode);
                }

                // Conversation?
                else if (xmlNode.Name.Equals("conversation") == true)
                {
                    // Parse
                    conv = parseConversation(xmlNode);
                    if (conv == null)
                        return null;

                    // Add to list
                    convList.Add(conv);
                }
            }

            // Next sibling
            xmlNode = xmlNode.NextSibling;
        }
        while (xmlNode != null);

        // Create dialogue
        dialogue = new AdvancedDialogue(dyncodeList, convList);

        return dialogue;
    }

    // Parses a dynamic code
    private static DynamicCode parseDynamicCode(XmlNode _node)
    {
        // Local variables
        int codeID = -1;
        string classPath = "";
        string entryPoint = "";
        string code = "";
        DynamicCode dyncode = null;
        XmlNode xmlNodeAtt = null;

        // Get attribute: id
        xmlNodeAtt = _node.Attributes.GetNamedItem("id");
        if (xmlNodeAtt == null)
        {
            Debug.LogError("Dynamic codes need IDs!");
            return null;
        }
        codeID = int.Parse(xmlNodeAtt.Value);

        // Get attribute: class path
        xmlNodeAtt = _node.Attributes.GetNamedItem("class");
        if (xmlNodeAtt == null)
        {
            Debug.LogError("Dynamic codes need class path definitions!");
            return null;
        }
        classPath = xmlNodeAtt.Value;

        // Get attribute: entry point
        xmlNodeAtt = _node.Attributes.GetNamedItem("entry");
        if (xmlNodeAtt == null)
        {
            Debug.LogError("Dynamic codes need entry point definitions!");
            return null;
        }
        entryPoint = xmlNodeAtt.Value;

        // Get text
        code = _node.InnerText;
        if (code == null || code.Length == 0)
        {
            Debug.LogError("Dynamic codes need text!");
            return null;
        }

        // Create choice
        dyncode = new DynamicCode(codeID, classPath, entryPoint, code);

        return dyncode;
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
            // Skipable?
            if (isNodeSkipable(xmlNodeText) == false)
            {
                // Parse
                text = parseText(xmlNodeText);
                if (text == null)
                    return null;

                // Add to list
                textList.Add(text);
            }

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
        int nextTextID = -1;
        string exitValue = "";
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

        // Get attribute: next text ID
        xmlNodeAtt = _node.Attributes.GetNamedItem("nextText");
        if (xmlNodeAtt != null)
            nextTextID = int.Parse(xmlNodeAtt.Value);

        // Get attribute: exit value
        xmlNodeAtt = _node.Attributes.GetNamedItem("exit");
        if (xmlNodeAtt != null)
            exitValue = xmlNodeAtt.Value;

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
            // Skipable?
            if (isNodeSkipable(xmlNode) == false)
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

                // Unknown (exclude known standard strings)
                else
                    Debug.LogWarning("Unknown XML node with a text (" + xmlNode.Name + ")");
            }

            // Next sibling
            xmlNode = xmlNode.NextSibling;
        }
        while (xmlNode != null);

        // Create text
        text = new DialogueText(textID, nextTextID, exitValue, choiceList, partList);

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
        string enabledFunc = "";
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

        // Get attribute: enabled_func
        xmlNodeAtt = _node.Attributes.GetNamedItem("enabled_func");
        if (xmlNodeAtt != null)
            enabledFunc = xmlNodeAtt.Value;

        // Get text
        text = _node.InnerText;
        if (text == null || text.Length == 0)
        {
            Debug.LogError("Choices need text!");
            return null;
        }

        // Create choice
        choice = new Choice(choiceID, text, nextTextID, exitValue, enabledFunc);

        return choice;
    }
}