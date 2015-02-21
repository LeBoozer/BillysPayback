/*
 * Project:	Billy's Payback
 * File:	TA_SetGlobalVariable.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * The transition will set defined global vars
 */
public class TA_SetGlobalVariable : FSMAction
{
    // Entry for boolean values
    [System.Serializable]
    public class Boolean
    {
        public string m_name;
        public bool   m_value; 
    }

    // List with defined boolean values
    public Boolean[] m_boolean;

    // Override: FSMAction::OnAction()
    override public void onAction()
    {
        // Set all boolean values
        foreach(Boolean b in m_boolean)
        {
            // Check
            if(b.m_name == null || b.m_name.Length == 0)
            {
                Debug.LogError("Global variables need valid names!");
                continue;
            }

            // Add to script engine
            Game.Instance.ScriptEngine.AddGlobalVar(b.m_name, b.m_value);
        }
    }
}
