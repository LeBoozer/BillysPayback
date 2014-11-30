/*
 * Project:	Billy's Payback
 * File:	IntroState.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Represents the game-state: intro
 */
public class IntroState : GameState, GameStateController
{	
	// Indicates whether the waiting time has been passed
	private bool m_timeIsUp = false;

	// Remaining time in seconds
	private float m_remainingTime = GameConfig.INTRO_SHOW_TIME_SEC;

	// Overrides: GameState::onInit()
	public void onInit()
	{
		Debug.Log("Intro::onInit()");
	}
	
	// Overrides: GameState::onExit()
	public void onExit()
	{
		Debug.Log("Intro::onExit()");
	}
	
	// Overrides: GameState::onEnter()
	public void onEnter()
	{
		Debug.Log("Intro::onEnter()");
	}
	
	// Overrides: GameState::onLeave()
	public void onLeave()
	{
		Debug.Log("Intro::onLeave()");
	}
	
	// Overrides: GameState::onUpdate()
	public void onUpdate(float _d)
	{
		// Update remaining time
		if(m_timeIsUp == false)
		{
			// Subtract elapsed time
			m_remainingTime -= _d;
			
			// Done?
			if(m_remainingTime <= 0.0f)
			{				
				// Switch game-state: main-menu
               	Game.Instance.GSM.setGameState<MainMenuState>();

                // Set flag
                m_timeIsUp = true;
            }
        }
	}
	
	// Overrides: GameState::onInit()
	public void onUpdateFixed(float _d)
	{
	}
	
	// Overrides: GameStateController::getController()
	public GameStateController getController()
	{
		return this;
	}
	
	// Overrides: GameStateController::isAdditiveLevel()
	public bool isAdditiveLevel()
	{
		return false;
	}
	
	// Overrides: GameStateController::isCustomScreen()
	public bool isCustomScreen()
	{
		return false;
	}
	
	// Overrides: GameStateController::getTargetLevelName()
	public string getTargetLevelName()
	{
		return "Intro";
	}
	
	// Overrides: GameStateController::onGui()
	public bool onGui(bool _done, float _deltaTime)
	{
		return true;
	}
}
