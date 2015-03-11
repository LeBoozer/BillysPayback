/*
 * Project:	Billy's Payback
 * File:	TA_SaveCharacteristics.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/**
 * Save the Characteristics in a txt - data
 */
public class TA_SaveCharacteristics : FSMAction
{

    /**
     * Save the characteristics of the player for the evaluation
     */
    public override void onAction()
    {
        //
        string path = "Characteristics.txt";

        List<string> lines = new List<string>();

        PlayerData data = Game.Instance.PlayerData;

        lines.Add("Challenge\t"     + data.getPlayerCharacteristics(PlayerData.PlayerType.PT_CHALLENGE));
        lines.Add("Attention\t"     + data.getPlayerCharacteristics(PlayerData.PlayerType.PT_ATTENTION));
        lines.Add("Autonomous\t"    + data.getPlayerCharacteristics(PlayerData.PlayerType.PT_AUTONOMOUS));
        lines.Add("Collecting\t"    + data.getPlayerCharacteristics(PlayerData.PlayerType.PT_COLLECTING));
        lines.Add("Compassion\t"    + data.getPlayerCharacteristics(PlayerData.PlayerType.PT_COMPASSION));
        lines.Add("Lightness\t"     + data.getPlayerCharacteristics(PlayerData.PlayerType.PT_LIGHTNESS));
        lines.Add("Patience\t"      + data.getPlayerCharacteristics(PlayerData.PlayerType.PT_PATIENCE));

        File.WriteAllLines(path, lines.ToArray());
    }
}
