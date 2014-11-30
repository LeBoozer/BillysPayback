/*
 * Project:	Billy's Payback
 * File:	KickOff.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * This script is being used to kick-off the game-state sequence.
 */
public class KickOff : MonoBehaviour
{
	// Will be called as soon as the script started
	void Start ()
	{
		// Set first game-state: into
		Game.Instance.GSM.setGameState<IntroState>();
	}
}
