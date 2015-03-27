/*
 * Project:	Billy's Payback
 * File:	Boss.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System;



/**
 * Describe the inferface for all boss
 */
public interface Boss 
{
	// let the boss fight start
	void StartBossFight();
	
	// let the boss fight end
    void EndBossFight(Action _event);

    // let the boss fight break
    void BreakBossFight();

    void OnBreakBossFight(Action _event);
}
