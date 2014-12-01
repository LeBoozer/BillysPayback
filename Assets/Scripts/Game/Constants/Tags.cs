/*
 * Project:	Billy's Payback
 * File:	Tags.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Copy of the tags used within the unity editor. 
 * This is needed to reference tags within the code without using any hardcoding mechanisms!
 */
public abstract class Tags
{
	public static readonly string TAG_UNTAGGED 			= "Untagged";
	public static readonly string TAG_RESPAWN 			= "Respawn";
	public static readonly string TAG_FINISH 			= "Finish";
	public static readonly string TAG_EDITOR_ONLY		= "EditorOnly";
	public static readonly string TAG_MAIN_CAMERA		= "MainCamera";
	public static readonly string TAG_PLAYER 			= "Player";
	public static readonly string TAG_GAME_CHARACTER 	= "GameCharacter";
	public static readonly string TAG_COMPANION			= "Companion";
	public static readonly string TAG_ENEMY				= "Enemy";
	public static readonly string TAG_ALLY				= "Ally";
	public static readonly string TAG_LOADING_SCREEN 	= "LoadingScreen";
	public static readonly string TAG_DIAMOND 			= "Diamond";
	public static readonly string TAG_KIWANO_POWER_UP 	= "KiwanoPowerUp";
}