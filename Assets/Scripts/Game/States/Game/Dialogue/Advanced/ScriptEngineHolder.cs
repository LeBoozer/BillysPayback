﻿/*
 * Project:	Billy's Payback
 * File:	DynamicScript.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System;

/*
 * Represents a single dynamic script
 */
public class ScriptEngineHolder : MonoBehaviour
{
    // The script engine instance
    private Jurassic.ScriptEngine m_scriptEngine = new Jurassic.ScriptEngine();

    // Constructor
    public ScriptEngineHolder()
    {
        // Initialize
        initialize();
    }

    // Executes a script (parameters are not supported yet!)
    // Returns the script result (parameter _result)
    // Returns true on success
    public bool executeScript<_T>(DynamicScript _script, ref _T _result)
        where _T : new()
    {
        // Local variables
        Type resultParamType = null;
        _T defaultValueResultParam;
        bool hasResult = false;

        // Result required?
        if (_script.ResultName != null && _script.ResultName.Length > 0 && _script.ResultTypeName != null && _script.ResultTypeName.Length > 0)
        {
            // Get type for the result param
            resultParamType = Type.GetType(_script.ResultTypeName);
            if (resultParamType == null)
                return false;
            if (resultParamType.IsValueType == false || Nullable.GetUnderlyingType(resultParamType) != null)
                return false;

            // Get default parameter for the result type
            defaultValueResultParam = new _T();
            if (defaultValueResultParam == null)
                return false;

            // Set global parameter for the result
            m_scriptEngine.SetGlobalValue(_script.ResultName, defaultValueResultParam);

            // Set flag
            hasResult = true;
        }

        // Execute script
        m_scriptEngine.Execute(_script.Code);

        // Get result
        if (hasResult)
            _result = m_scriptEngine.GetGlobalValue<_T>(_script.ResultName);

        return true;
    }

    // Executes a script (parameters are not supported yet!)
    // Returns the script result (parameter _result)
    // Returns true on success
    public bool executeScript(DynamicScript _script, ref object _result)
    {
        // Local variables
        Type resultParamType = null;
        object defaultValueResultParam = null;
        bool hasResult = false;

        // Result required?
        if (_script.ResultName != null && _script.ResultName.Length > 0 && _script.ResultTypeName != null && _script.ResultTypeName.Length > 0)
        {
            // Get type for the result param
            resultParamType = Type.GetType(_script.ResultTypeName);
            if (resultParamType == null)
                return false;
            if (resultParamType.IsValueType == false || Nullable.GetUnderlyingType(resultParamType) != null)
                return false;

            // Get default parameter for the result type
            defaultValueResultParam = Activator.CreateInstance(resultParamType);
            if (defaultValueResultParam == null)
                return false;

            // Set global parameter for the result
            m_scriptEngine.SetGlobalValue(_script.ResultName, defaultValueResultParam);

            // Set flag
            hasResult = true;
        }

        // Execute script
        m_scriptEngine.Execute(_script.Code);

        // Get result
        if (hasResult)
            _result = m_scriptEngine.GetGlobalValue(_script.ResultName);

        return true;
    }

    // Initializes the script engine with default values
    private void initialize()
    {
        // Allow interop operations
        m_scriptEngine.EnableExposedClrTypes = true;

        // Add globals
        m_scriptEngine.SetGlobalValue("Game", typeof(Game));

        // Helper functions for system logging
        m_scriptEngine.SetGlobalFunction("DebugLog", new System.Action<string>(DebugLog));
        m_scriptEngine.SetGlobalFunction("DebugWarning", new System.Action<string>(DebugWarning));
        m_scriptEngine.SetGlobalFunction("DebugError", new System.Action<string>(DebugError));

        // Helper functions for global variable support
        m_scriptEngine.SetGlobalFunction("AddGlobalVar", new System.Action<string, object>(AddGlobalVar));
        m_scriptEngine.SetGlobalFunction("GetGlobalVar", new System.Func<string, object>(GetGlobalVar));
        m_scriptEngine.SetGlobalFunction("IsGlobalVar", new System.Func<string, System.Boolean>(IsGlobalVar));

        // Add custom functions (player data)
        m_scriptEngine.SetGlobalFunction("Player_GetNumLifePoints", new System.Func<int>(Player_GetNumLifePoints));
        m_scriptEngine.SetGlobalFunction("Player_GetNumLifes", new System.Func<int>(Player_GetNumLifes));
        m_scriptEngine.SetGlobalFunction("Player_GetCharacteristicAttention", new System.Func<int>(Player_GetCharacteristicAttention));
        m_scriptEngine.SetGlobalFunction("Player_GetCharacteristicAutonomous", new System.Func<int>(Player_GetCharacteristicAutonomous));
        m_scriptEngine.SetGlobalFunction("Player_GetCharacteristicChallenge", new System.Func<int>(Player_GetCharacteristicChallenge));
        m_scriptEngine.SetGlobalFunction("Player_GetCharacteristicCollecting", new System.Func<int>(Player_GetCharacteristicCollecting));
        m_scriptEngine.SetGlobalFunction("Player_GetCharacteristicCompassion", new System.Func<int>(Player_GetCharacteristicCompassion));
        m_scriptEngine.SetGlobalFunction("Player_GetCharacteristicLightness", new System.Func<int>(Player_GetCharacteristicLightness));
        m_scriptEngine.SetGlobalFunction("Player_GetCharacteristicPatience", new System.Func<int>(Player_GetCharacteristicPatience));

        m_scriptEngine.SetGlobalFunction("Player_AddCharacteristicCompassion", new System.Action<int>(Player_AddCharacteristicCompassion));
    }

    #region Helper
    #region Log
    public void DebugLog(string _entry)
    {
        // Write to log
        Debug.Log(_entry);
    }
    public void DebugWarning(string _entry)
    {
        // Write to log
        Debug.LogWarning(_entry);
    }
    public void DebugError(string _entry)
    {
        // Write to log
        Debug.LogError(_entry);
    }
    #endregion Log
    #region Global variables
    public void AddGlobalVar(string _name, object _data)
    {
        // Add data
        m_scriptEngine.SetGlobalValue(_name, _data);
    }
    public object GetGlobalVar(string _name)
    {
        // Get data
        return m_scriptEngine.GetGlobalValue(_name);
    }
    public System.Boolean IsGlobalVar(string _name)
    {
        // Get data
        return m_scriptEngine.HasGlobalValue(_name);
    }
    #endregion Global variables
    #endregion Helper

    #region Player data
    public int Player_GetNumLifePoints()
    {
        return Game.Instance.PlayerData.LifePoints; 
    }
    public int Player_GetNumLifes()
    {
        return Game.Instance.PlayerData.LifePoints;
    }
    #region Characteristics
    public int Player_GetCharacteristicAttention()
    {
        return Game.Instance.PlayerData.getPlayerCharacteristics(PlayerData.PlayerType.PT_ATTENTION);
    }
    public int Player_GetCharacteristicAutonomous()
    {
        return Game.Instance.PlayerData.getPlayerCharacteristics(PlayerData.PlayerType.PT_AUTONOMOUS);
    }
    public int Player_GetCharacteristicChallenge()
    {
        return Game.Instance.PlayerData.getPlayerCharacteristics(PlayerData.PlayerType.PT_CHALLENGE);
    }
    public int Player_GetCharacteristicCollecting()
    {
        return Game.Instance.PlayerData.getPlayerCharacteristics(PlayerData.PlayerType.PT_COLLECTING);
    }
    public int Player_GetCharacteristicCompassion()
    {
        return Game.Instance.PlayerData.getPlayerCharacteristics(PlayerData.PlayerType.PT_COMPASSION);
    }
    public int Player_GetCharacteristicLightness()
    {
        return Game.Instance.PlayerData.getPlayerCharacteristics(PlayerData.PlayerType.PT_LIGHTNESS);
    }
    public int Player_GetCharacteristicPatience()
    {
        return Game.Instance.PlayerData.getPlayerCharacteristics(PlayerData.PlayerType.PT_PATIENCE);
    }

    public void Player_AddCharacteristicCompassion(int _d)
    {
        Game.Instance.PlayerData.addPlayerCharacteristics(PlayerData.PlayerType.PT_COMPASSION, (short)_d);
    }
    #endregion Characteristics
    #endregion Player data
}
