/*
 * Project:	Billy's Payback
 * File:	DynamicCode.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Represents a single dynamic code part
 */
public class DynamicCode
{
    // The code's ID
    private int m_codeID;
    public int CodeID
    {
        get { return m_codeID; }
        private set { m_codeID = value; }
    }

    // The class path
    private string m_classPath;
    public string ClassPath
    {
        get { return m_classPath; }
        private set { }
    }

    // The entry point
    private string m_entryPoint;
    public string EntryPoint
    {
        get { return m_entryPoint; }
        private set { }
    }

    // The code
    private string m_code;
    public string Code
    {
        get { return m_code; }
        private set { }
    }

    // Constructor
    public DynamicCode(int _id, string _classPath, string _entryPoint, string _code)
    {
        // Copy
        m_codeID = _id;
        m_classPath = _classPath;
        m_entryPoint = _entryPoint;
        m_code = _code;
    }
}
