/*
 * Project:	Billy's Payback
 * File:	GameStateMachine.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 * Manages the game-states
 */
public class GameStateMaschine : MonoBehaviour
{
	// The current game state
	private GameState			m_currentGameState = null;
	
	// Stack of paused-game states
	private Stack<GameState>	m_gameStateStack = new Stack<GameState>();	
	
	// Loading screen for state changes
	private LoadingScreen		m_loadingScreen = null;
	
	// Clears the stack
	private void _clearStack()
	{
		// Terminate all states within the stack
		foreach(GameState s in m_gameStateStack)
		{
			// Notify game-state
			s.onExit();
		}
		m_gameStateStack.Clear();
	}
	
#region (Undo additive level-loading)
	// Undos the additive level loading for the specified game-state (if required)
	private void _undoAdditiveLevelLoading(GameState _state)
	{
		// Local variables
		GameStateController stateCtrl = null;
		GameObject rootObjectLevel = null;
		string rootObjectLevelName = "";
	
		// Check parameter
		if(_state == null)
			return;
		stateCtrl = _state.getController();
		
		// Retrieve root game-object for the additive level
		rootObjectLevelName = GameConfig.LEVEL_ADDITIVE_ROOT_GO_NAME_RULE.Replace("x", stateCtrl.getTargetLevelName());
		rootObjectLevel = GameObject.Find(rootObjectLevelName);
		
		// Destroy root game-object
		if(rootObjectLevel != null)
			GameObject.Destroy(rootObjectLevel);
	}
#endregion
	
#region (Setup loading-screen)
	// Setups the loading screen for the specified game-state (if required!)
	// Returns true on success, otherwise false
	private bool _setupLoadingScreen(GameState _target, bool _additive)
	{
		// Local parameters
		GameObject obj = null;
		GameStateController stateCtrl = null;
	
		// Check parameter
		if(_target == null)
			return false;
			
		// Loading screen required?
		stateCtrl = _target.getController();
		if(stateCtrl == null || stateCtrl.getTargetLevelName() == null || stateCtrl.getTargetLevelName().Length == 0)
		{
			_target.onEnter();
			return true;
		}
		
		// Additive required?
		if(stateCtrl.isAdditiveLevel() != _additive)
			return false;
			
		// Loading screen already available?
		if(m_loadingScreen == null)
		{
			// Acquire game-object
			obj = GameObject.FindGameObjectWithTag(Tags.TAG_LOADING_SCREEN);
			if(obj == null)
			{
				// Create game-object
				obj = new GameObject(typeof(LoadingScreen).ToString());
				obj.tag = Tags.TAG_LOADING_SCREEN;
				Object.DontDestroyOnLoad(obj);
			}
			
			// Create loading screen
			m_loadingScreen = Game.addComponent<LoadingScreen>();
		}
		
		// (Re)-initialize loading screen
		m_loadingScreen.reinitialize(_target);
		
		// Start loading
		if(_additive == true)
			Application.LoadLevelAdditive(stateCtrl.getTargetLevelName());
		else
			Application.LoadLevel(stateCtrl.getTargetLevelName());
			
		return true;
	}
#endregion
	
	// Returns or updates the currently active game-state
	public GameState ActiveState
	{
		get { return m_currentGameState; }
		private set {}
	}
	
#region (Set new game-state)
	// Sets a new game-state by replacing and terminated the currently active one. All stacked ones will be deleted also
	public void setGameState<_T>()
		where _T : GameState, new()
	{	
		// Check parameter
		if(m_currentGameState != null && typeof(_T).Equals(m_currentGameState.GetType()) == true)
			return;
		
		// Terminate current state
		if(m_currentGameState != null)
		{
			m_currentGameState.onLeave();
			m_currentGameState.onExit();
		}
		
		// Clear stack
		_clearStack();
		
		// Create new game-state
		m_currentGameState = new _T();
		m_currentGameState.onInit();
		
		// Setup loading screen (if required)
		_setupLoadingScreen(m_currentGameState, false);
	}
#endregion
	
#region (Stack operations for game-states)
	// Changes the game-state. Instead of replacing and terminating the currently active state, the current state will be pushed onto the stack for later use
	// Returns true on success, otherwise false
	public bool pushGameState<_T>()
		where _T : GameState, new()
	{
		// Check parameter
		if(m_currentGameState != null && typeof(_T).Equals(m_currentGameState.GetType()) == true)
			return true;
			
		// Push current state
		if(m_currentGameState != null)
		{
			// Notify state
			m_currentGameState.onLeave();
			m_gameStateStack.Push(m_currentGameState);
		}
		
		// Create next game-state
		m_currentGameState = new _T();
		m_currentGameState.onInit();
		
		// Setup loading screen (if required)
		return _setupLoadingScreen(m_currentGameState, true);		
	}
	
	// Tries to pop the currently active game-state by replacing this one with the first stack element
	// Returns true on success, otherwise false
	public bool popGameState()
	{
		// Empty stack?
		if(m_gameStateStack.Count == 0)
			return false;
			
		// Terminate current state
		if(m_currentGameState != null)
		{
			m_currentGameState.onLeave();
			_undoAdditiveLevelLoading(m_currentGameState);
			m_currentGameState.onExit();
		}
		
		// Pop first element
		m_currentGameState = m_gameStateStack.Pop();
		m_currentGameState.onEnter();
	
		return true;
	}
#endregion
	
	// Will be called once a frame
	void Update ()
	{
		// Update current state
		if (m_currentGameState != null)
			m_currentGameState.onUpdate (Time.deltaTime);
	}

	// Will be called at a fixed rate
	void FixedUpdate()
	{
		// Update current state
		if (m_currentGameState != null)
				m_currentGameState.onUpdateFixed (Time.deltaTime);
	}
}
