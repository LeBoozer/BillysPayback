/*
 * Project:	Billy's Payback
 * File:	Layer.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Copy of the layer used within the unity editor. 
 * This is needed to reference tags within the code without using any hardcoding mechanisms!
 */
public abstract class Layer
{
	public static readonly string LAYER_COLLECTABLE 		= "Collectable";
	public static readonly string LAYER_ENEMY 				= "Enemy";
	public static readonly string LAYER_PLAYER 				= "Player";
	public static readonly string LAYER_PROJECTILE_PLAYER	= "ProjectilePlayer";
	public static readonly string LAYER_PROJECTILE_ENEMY	= "ProjectileEnemy";
	public static readonly string LAYER_ENVIROMENT 			= "Enviroment";
    public static readonly string LAYER_ENEMY_BLOCKER       = "EnemyBlocker";
	
	// Converts the layer name to the corresponding layer ID
	public static int getLayerIDByName(string _layerName)
	{
		return LayerMask.NameToLayer(_layerName);
	}
	
	// Converts the layer ID to the coressponding layer name
	public static string getLayerNameByID(int _id)
	{
		return LayerMask.LayerToName(_id);
	}
}