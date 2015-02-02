﻿/*
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
	public static readonly float 	INTRO_SHOW_TIME_SEC                         = 3.0f;
	
	public static readonly string 	LEVEL_ADDITIVE_ROOT_GO_NAME_RULE            = "_Root_x";
	
	// Billy related values
	public static readonly int 		BILLY_LIFE_POINT 				            = 4;
	public static readonly int		BILLY_LIFE_NUMBER				            = 5;
	public static readonly float 	BILLY_MAX_SPEED 				            = 10.0f;
	public static readonly float 	BILLY_FLYING_FACTOR				            = 0.25f;
	public static readonly float 	BILLY_JUMP_MINIMAL_HEIGHT 			        = 1f;
	public static readonly float 	BILLY_JUMP_MAXIMAL_HEIGHT 			        = 4f;
	public static readonly float 	BILLY_JUMP_HEIGHT_FROM_ENEMY 	            = 2f;
    public static readonly float    BILLY_MINIMAL_KEYPRESS_TIME_FOR_JUMPING     = 0.1f;
    public static readonly float    BILLY_MAXIMAL_KEYPRESS_TIME_FOR_JUMPING     = 0.5f;

    // Antonio related values
    public static readonly float    ANTONIO_DISTANCE_MEAN_VALUE                 = 6f;
    public static readonly float    ANTONIO_DISTANCE_VARIANCE                   = 3f;

    // Diamond related values
    public static readonly float    DIAMONG_ROTATION_SPEED                      = 55.0f;

    // Egg related values
    public static readonly float    EGG_BENEATH_FACTOR                          = 0.65f;

    // Destroyable block related values
    public static readonly float    DESTROYABLE_BOX_DEFAULT_LIFE_TIME_SEC       = 5.0f;

    // Enemy related values
    public static readonly float    ENEMY_PLAYER_ABOVE_FACTOR                   = 0.5f;
    public static readonly float    ENEMY_MAX_SPEED                             = 3f;
    public static readonly float    ENEMY_JUMP_HEIGHT                           = 0.5f;
}
