/*
 * Project:	Billy's Payback
 * File:	TS_PlayOnAction.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * TODO
 */
public class TS_PlayOnAction : FSMAction
{
	// True to play the particle system on action
	public bool 					m_playOnAction = true;
	
	// List of particle systems which should be played on action
	public List<ParticleSystem> 	m_particleSystemList = new List<ParticleSystem>();
	
	// Override: FSMAction::OnAction()
	override public void onAction()
	{
		// Enable/disable
		foreach(ParticleSystem ps in m_particleSystemList)
		{
			if(ps != null)
			{
				// Play?
				if(m_playOnAction == true)
					ps.Play();
				else
					ps.Stop();
			}
		}
	}
}