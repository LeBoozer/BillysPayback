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
    // Constant values
    private static readonly string RUN_TYPE_ID_DEFAULT  = "default";
    private static readonly string RUN_TYPE_ID_AWAKE    = "awake";

    // Enumeration with all valid types for a script's run type
    public enum RunType
    {
        // Script will be executed if needed
        RUN_DEFAULT,

        // Script will be executed on awake
        RUN_AWAKE
    };

    // The script's name
    private string m_scriptName;
    public string ScriptName
    {
        get { return m_scriptName; }
        private set { m_scriptName = value; }
    }

    // The script's run type
    private RunType m_scriptRunType = RunType.RUN_DEFAULT;
    public RunType ScriptRunType
    {
        get { return m_scriptRunType; }
        set { m_scriptRunType = value; }
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
    public DynamicScript(string _name, string _runType, string _resultName, string _resultType, string _code)
    {
        // Copy
        m_scriptName = _name;
        m_resultName = _resultName;
        m_resultTypeName = _resultType;
        m_code = _code;

        // Determine run type
        if (_runType == null || _runType.Length == 0 || _runType.Equals(RUN_TYPE_ID_DEFAULT) == true)
            m_scriptRunType = RunType.RUN_DEFAULT;
        else if (_runType.Equals(RUN_TYPE_ID_AWAKE) == true)
            m_scriptRunType = RunType.RUN_AWAKE;
        else
            Debug.LogWarning("Script (" + m_scriptName + ") has unspported run-type: " + _runType);
    }
}
