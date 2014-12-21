﻿/*
 * Project:	Billy's Payback
 * File:	DestroyableBox.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */

using UnityEngine;
using System.Collections;

/*
 * Script for the destroyable box
 */
public class DestroyableBox : MonoBehaviour 
{

	public 	float MAX_LIFE_TIME = 5; // in second
	public  float m_currentLifetime;
	private MeshRenderer 	m_mesh;
	private float 			m_lastFlipp;
	//private bool  m_stayPlayer = false;

	// Use this for initialization
	void Start () 
	{
		// init the lifetime of the broken box
		m_currentLifetime = 0;
		m_lastFlipp = Time.time;

		// get needed components
		m_mesh 		= GetComponentInChildren<MeshRenderer> ();

	}
	
	// Update is called once per frame
	void Update () 
	{
		// dont do anything if the life time under 0.01f 
		if (m_currentLifetime < 0.01f)
				return;

		// placeholder for future optical effects
		flashingEffect ();

		// alive?
		if (m_currentLifetime < MAX_LIFE_TIME)
				return;

		// destroy the gameobject
		// TO DO: create partical effekt
		Destroy (this.gameObject);
	}

	/**
	 * let disabled and enabled the mesh of the box
	 * depending on the remaining lifetime
	 */
	void flashingEffect ()
	{
		if (!m_mesh.enabled && Time.time - m_lastFlipp > 0.1f)
		{
			m_mesh.enabled = true;
			m_lastFlipp = Time.time;
		}
		else if(m_mesh.enabled && Time.time - m_lastFlipp > (MAX_LIFE_TIME - m_currentLifetime) )
		{
			m_mesh.enabled = false;
			m_lastFlipp = Time.time;
		}
	}

	/**
	 * processed the collision with the player
	 * must called with SendMessage in the Player-Class
	 */
	public void PlayerCollision (Object _obj)
	{
		// Transform from player
		GameObject player = _obj as GameObject;

		// useless message?
		if (player == null)
			return;

		// player about the box?
		if(player.transform.position.y > this.transform.position.y)
			m_currentLifetime += Time.deltaTime;
	}


}
