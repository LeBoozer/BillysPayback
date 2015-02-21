/*
 * Project:	Billy's Payback
 * File:	RotateRandomY.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Rotates the game-object randomly about the y-axis
 */
public class RotateRandomY : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        transform.Rotate(Vector3.up * Random.value * 100 * GameConfig.DIAMONG_ROTATION_SPEED);
	}
}