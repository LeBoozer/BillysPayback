/*
 * Project:	Billy's Payback
 * File:	DynamicScript.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Represents a single dynamic script
 */
public class DynamicScript
{
    // The script's name
    private string m_scriptName;
    public string ScriptName
    {
        get { return m_scriptName; }
        private set { m_scriptName = value; }
    }

    // The script's result parameter (name)
    private string m_resultName;
    public string ResultName
    {
        get { return m_resultName; }
        private set { m_resultName = value; }
    }

    // The script's result parameter (type, name)
    private string m_resultTypeName;
    public string ResultTypeName
    {
        get { return m_resultTypeName; }
        private set { m_resultTypeName = value; }
    }

    // The code
    private string m_code;
    public string Code
    {
        get { return m_code; }
        private set { }
    }

    // Constructor
    public DynamicScript(string _name, string _resultName, string _resultType, string _code)
    {
        // Copy
        m_scriptName = _name;
        m_resultName = _resultName;
        m_resultTypeName = _resultType;
        m_code = _code;
    }
}
