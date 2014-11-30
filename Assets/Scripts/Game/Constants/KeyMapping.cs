/*
 * Project:	Billy's Payback
 * File:	KeyMapping.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Holds all relevant key mappings from the input manager. If you change one of this value either in the input manager or in this mapping class, adjust the other value respectively!
 */
public abstract class KeyMapping
{
	/* Constant values */
	public static readonly string KEY_VALUE_HORIZONTAL		= "Horizontal";
	public static readonly string KEY_VALUE_VERTICAL 		= "Vertical";
	public static readonly string KEY_ACTION_MOVE_RIGHT 	= "Right";
	public static readonly string KEY_ACTION_MOVE_LEFT 		= "Left";
	public static readonly string KEY_ACTION_MOVE_UP 		= "Up";
	public static readonly string KEY_ACTION_MOVE_DOWN 		= "Down";
	public static readonly string KEY_ACTION_JUMP 			= "Jump";
	public static readonly string KEY_ACTION_SHOOT 			= "Shoot";
}