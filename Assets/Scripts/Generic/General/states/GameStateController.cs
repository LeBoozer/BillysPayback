/*
 * Project:	Billy's Payback
 * File:	GameStateController.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 *
 */
public interface GameStateController
{
	// Returns whether the level can be added to the current level.
	// This requires that all game-objects are grouped into one root game-object with the name: _Root_[levelname].
	bool isAdditiveLevel();
	
	// Returns whether the loading screen uses a custom GUI or a pre-build one
	bool isCustomScreen();

	// Returns the name of the target level which should be loaded.
	// Return null or an empty string to indicate that no level data needs to be loaded by the application.
	string getTargetLevelName();
	
	// Will be called for GUI events (is only being called if "isCustomScreen" returns true)
	// _done: True if the loading screen can be removed
	// Returns true if the controller confirms that the loading screen has been removed
	bool onGui(bool _done, float _deltaTime);
}