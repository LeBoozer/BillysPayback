/*
 * Project:	Billy's Payback
 * File:	LoadingScreen.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 *
 */
public class LoadingScreen : MonoBehaviour
{
	// Indicates whether a loading process is being executed currently
	private bool 				m_isLoading = false;
	
	// Indicates whether the loading screen is waiting for the controller
	private bool 				m_isWaitingForConroller = false;

	// The target game-state. Will be notified after the loading has been finished.
	private GameState 			m_targetGameState = null;
	
	// The target game-state-controller
	private GameStateController m_targetConroller = null;

	// (Re)-initializes the loading screen
	public void reinitialize(GameState _targetState)
	{
		// Reset
		m_isLoading = false;
	
		// Copy
		m_targetGameState = _targetState;
		m_targetConroller = _targetState.getController();
	}
	
	// Will be called as soon as the loading progress has been finished
	private void onDone()
	{
		// Custom loading screen?
		if(m_targetConroller.isCustomScreen() && m_isWaitingForConroller == true)
			return;
	
		// Reset
		m_isLoading = false;
		m_isWaitingForConroller = false;
	
		// Notify target
		m_targetGameState.onEnter();
	}

	// Will be called as soon as the Gui is about to be drawn
	void OnGUI ()
	{	
		// Loading?
		if(m_isLoading == true || m_isWaitingForConroller == true)
		{
			// Notify controller?
			if(m_targetConroller.isCustomScreen() == true)
			{
				m_isWaitingForConroller = !m_targetConroller.onGui(m_isWaitingForConroller, Time.deltaTime);
				if(m_isWaitingForConroller == false)
					onDone();
			}
			else
			{
				// Todo
				//GUI.DrawTexture (new Rect(0,0,Screen.width,Screen.height), loadingTexture, ScaleMode.StretchToFill);
			}
		}
	}
	
	// Will be called at a fixed rate
	void Update()
	{
		// Local variables
		bool loading = Application.isLoadingLevel;
	
		// Skip unwanted combinations
		if(m_isLoading == loading)
			return;
		
		// Loading finished?
		if(m_isLoading == true && loading == false)
		{
			m_isWaitingForConroller = true;
			onDone();
		}
		
		// Copy
		m_isLoading = loading;
	}
}