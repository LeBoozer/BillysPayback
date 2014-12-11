/*
 * Project:	Billy's Payback
 * File:	GameConfig.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Contains values for the game configuration
 */
public class GameConfig
{
	// Constant values
	public static readonly float 	INTRO_SHOW_TIME_SEC = 3.0f;
	
	public static readonly string 	LEVEL_ADDITIVE_ROOT_GO_NAME_RULE = "_Root_x";
	
	// Billy related values
	public static readonly int 		BILLY_LIFE_POINT 				= 5;
	public static readonly float 	BILLY_MAX_SPEED 				= 10.0f;
	public static readonly float 	BILLY_FLYING_FACTOR				= 0.25f;
	public static readonly float 	BILLY_JUMP_START_SPEED 			= 5.5f;
	public static readonly float 	BILLY_JUMP_START_SPEED_ENEMY 	= 5.0f;
}
