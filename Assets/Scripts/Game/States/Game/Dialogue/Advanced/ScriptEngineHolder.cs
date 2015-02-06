/*
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
    }

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
    #endregion Characteristics
    #endregion Player data
}
