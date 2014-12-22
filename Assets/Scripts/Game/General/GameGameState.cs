/*
 * Project:	Billy's Payback
 * File:	GameGameState.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Represents the game-state: game
 */
public class GameGameState : GameState, GameStateController
{
#region (GameState methods)
    // Overrides: GameState::onInit()
    public void onInit()
    {
        Debug.Log("Game::onInit()");
    }

    // Overrides: GameState::onExit()
    public void onExit()
    {
        Debug.Log("Game::onExit()");
    }

    // Overrides: GameState::onEnter()
    public void onEnter()
    {
        Debug.Log("Game::onEnter()");
    }

    // Overrides: GameState::onLeave()
    public void onLeave()
    {
        Debug.Log("Game::onLeave()");
    }

    // Overrides: GameState::onUpdate()
    public void onUpdate(float _d)
    {
    }

    // Overrides: GameState::onInit()
    public void onUpdateFixed(float _d)
    {
    }
#endregion

#region (GameStateController methods)
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
        return "1. Analyse";
    }

    // Overrides: GameStateController::onGui()
    public bool onGui(bool _done, float _deltaTime)
    {
        return true;
    }
#endregion
}
