/*
 * Project:	Billy's Payback
 * File:	GameState.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Represents the blueprint for a game-state
 */
public interface GameState : EditorObject
{
	// Will be called as soon as the game-state is being entered/activated
	void onEnter ();

	// Will be called as soon as the game-state is being left/deactivated
	void onLeave ();
	
	// Returns information regarding the game-state (null when no detailled information about the game-state is required!)
	GameStateController getController();
}