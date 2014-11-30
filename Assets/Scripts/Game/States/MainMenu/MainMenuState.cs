/*
 * Project:	Billy's Payback
 * File:	MainMenuState.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Represents the game-state: intro
 */
public class MainMenuState : GameState, GameStateController
{		
	// Overrides: GameState::onInit()
	public void onInit()
	{
	}
	
	// Overrides: GameState::onExit()
	public void onExit()
	{
	}
	
	// Overrides: GameState::onEnter()
	public void onEnter()
	{
	}
	
	// Overrides: GameState::onLeave()
	public void onLeave()
	{
	}
	
	// Overrides: GameState::onUpdate()
	public void onUpdate(float _d)
	{}
	
	// Overrides: GameState::onInit()
	public void onUpdateFixed(float _d)
	{
	}
	
	// Overrides: GameStateController::getController()
	public GameStateController getController()
	{
		return this;
	}
	
	// Overrides: GameStateController::isAdaptiveLevel()
	public bool isAdaptiveLevel()
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
		return "MainMenu";
	}
	
	// Overrides: GameStateController::onGui()
	public bool onGui(bool _done, float _deltaTime)
	{
		return true;
	}
}
