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

    // Entry for integer values
    [System.Serializable]
    public class Integer
    {
        public string m_name;
        public int m_value;
        public bool m_additive;
    }

    // List with defined boolean values
    public Boolean[] m_boolean;

    // List with defined integer values
    public Integer[] m_integer;

    // Override: FSMAction::OnAction()
    override public void onAction()
    {
        // Local variables
        object tempObj = null;
        int tempInt = 0;

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

        // Set all integer values
        foreach (Integer i in m_integer)
        {
            // Check
            if (i.m_name == null || i.m_name.Length == 0)
            {
                Debug.LogError("Global variables need valid names!");
                continue;
            }

            // Additive?
            if (i.m_additive == true)
            {
                // Available?
                if (Game.Instance.ScriptEngine.IsGlobalVar(i.m_name) == true)
                {
                    tempObj = Game.Instance.ScriptEngine.GetGlobalVar(i.m_name);
                    if (tempObj != null && tempObj is System.Int32)
                        tempInt = (int)tempObj;
                }

                // Add
                tempInt += i.m_value;
            }
            else
                tempInt = i.m_value;

            // Add to script engine
            Game.Instance.ScriptEngine.AddGlobalVar(i.m_name, tempInt);
        }
    }
}
