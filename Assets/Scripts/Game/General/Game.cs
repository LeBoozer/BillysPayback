/*
 * Project:	Billy's Payback
 * File:	Game.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Defines the game manager class. This is the main class of the whole game.
 * The game manager contains various information about the player and his current progress
 * and also manages the game-states.
 */
public class Game
{
	// The singleton instance
	private static Game			m_singletonInstance;
	
	// The game object
	private GameObject			m_singletonGameObject;

	// The GSM
	private GameStateMaschine 	m_gsm;
	
	// The player data
	private PlayerData			m_playerData;

    // The player
    private Player              m_player;

    // The script engine instance
    private ScriptEngineHolder  m_scriptEngine;

	// Protected constructor
	protected Game()
	{}
	
	// Returns the GSM
	public GameStateMaschine GSM
	{
		get { return m_gsm; }
		private set{}
	}
	
	// Returns the player data
	public PlayerData PlayerData
	{
		get { return m_playerData; }
		private set {}
	}

    // Returns/sets the player instance
    public Player Player
    {
        get { return m_player; }
        set { m_player = value; }
    }

    // Returns the script engine instance
    public ScriptEngineHolder ScriptEngine
    {
        get { return m_scriptEngine; }
        private set { m_scriptEngine = value; }
    }

	// Returns the singleton instance
	public static Game Instance
	{
		get
		{
			// Local variables
			GameObject obj = null;
			
			// Instance already available?
			if(m_singletonInstance != null)
				return m_singletonInstance;
			
			// Create singleton instance
			m_singletonInstance = new Game();
			
			// Try to find game-object
			obj = GameObject.Find(typeof(Game).ToString());
			if(obj == null)
			{
				// Create game-object and mark it as persistent!
				obj = new GameObject(typeof(Game).ToString());
				Object.DontDestroyOnLoad(obj);
			}
			m_singletonInstance.m_singletonGameObject = obj;
			
			// Add components
			m_singletonInstance.m_gsm 			= m_singletonInstance.m_singletonGameObject.AddComponent<GameStateMaschine>();
			m_singletonInstance.m_playerData 	= m_singletonInstance.m_singletonGameObject.AddComponent<PlayerData>();
			m_singletonInstance.m_scriptEngine  = m_singletonInstance.m_singletonGameObject.AddComponent<ScriptEngineHolder>();

            // Add console game object
            obj = (GameObject)GameObject.Instantiate(Resources.Load("Helper/Console"));
            if (obj != null)
            {
                obj.AddComponent<ConsoleCommands>();
                obj.transform.parent = m_singletonInstance.m_singletonGameObject.transform;
            }

			return m_singletonInstance;		
		}
	}

	// Adds a new component
	public static _K addComponent<_K>()
		where _K : UnityEngine.Component
	{
		// Local variables
		_K comp = null;
		
		// Section manager already available?
		comp = Instance.m_singletonGameObject.GetComponent<_K>();
		if(comp != null)
			return comp;
		
		// Add section manager
		return Instance.m_singletonGameObject.AddComponent<_K>();
	}
}