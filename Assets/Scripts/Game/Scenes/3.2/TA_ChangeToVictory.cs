/*
 * Project:	Billy's Payback
 * File:	TA_ChangeToVictory.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * ToDo
 */
public class TA_ChangeToVictory : FSMAction
{
    // Override: FSMAction::OnAction()
    override public void onAction()
    {
        // Change game-state: victory
        Game.Instance.GSM.setGameState<VictoryState>();
    }
}
