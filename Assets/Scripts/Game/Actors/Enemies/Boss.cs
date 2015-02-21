using UnityEngine;
using System.Collections;
using System;




public interface Boss 
{
	// let the boss fight start
	void StartBossFight();
	
	// let the boss fight end
    void EndBossFight(Action _event);

    // let the boss fight break
    void BreakBossFight();
}
