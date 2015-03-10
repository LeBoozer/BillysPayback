/*
 * Project:	Billy's Payback
 * File:	TA_SetPlayerDiamonds.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Transition action which will set the count of the player's diamonds
 */
public class TA_SetPlayerDiamonds : FSMAction
{
    // True to add the amount, otherwise subtract
    public bool m_addAmount = true;

    // The amount of diamonds
    public int m_amount = 0;

    // Override: FSMAction::OnAction()
    override public void onAction()
    {
        // Add/subtract amount
        Game.Instance.PlayerData.CollectedDiamonds += (m_addAmount == true ? 1 : -1) * m_amount;
	}
}
