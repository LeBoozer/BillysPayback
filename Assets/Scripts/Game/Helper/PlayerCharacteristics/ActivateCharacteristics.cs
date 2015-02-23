/*
 * Project:	Billy's Payback
 * File:	ActivateCharacteristics.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Activate all game objects with the characteristics
 */
public class ActivateCharacteristics : MonoBehaviour
{
    public short          m_threshold = 1;

    // Use this for initialization
    void Start()
    {
        // fetch player data 
        PlayerData data = Game.Instance.PlayerData;
        
        // create pattern list 
        List<string> patternList = new List<string>();

        // player characteristic challenge
        if (data.getPlayerCharacteristics(PlayerData.PlayerType.PT_CHALLENGE) >= m_threshold)
            patternList.Add("Challenge");
        else if (data.getPlayerCharacteristics(PlayerData.PlayerType.PT_CHALLENGE) <= -m_threshold)
            patternList.Add("NoChallenge");

        // player characteristic attention
        if (data.getPlayerCharacteristics(PlayerData.PlayerType.PT_ATTENTION) >= m_threshold)
            patternList.Add("Attention");
        else if (data.getPlayerCharacteristics(PlayerData.PlayerType.PT_ATTENTION) <= -m_threshold)
            patternList.Add("NoAttention");

        // player characteristic collector
        if (data.getPlayerCharacteristics(PlayerData.PlayerType.PT_COLLECTING) >= m_threshold)
            patternList.Add("Collector");
        else if (data.getPlayerCharacteristics(PlayerData.PlayerType.PT_COLLECTING) <= -m_threshold)
            patternList.Add("NoCollector");

        // player characteristic ambition
        if (data.getPlayerCharacteristics(PlayerData.PlayerType.PT_LIGHTNESS) >= m_threshold)
            patternList.Add("Ambition");
        else if (data.getPlayerCharacteristics(PlayerData.PlayerType.PT_LIGHTNESS) <= -m_threshold)
            patternList.Add("NoAmbition");

        // init needed variables 
        GameObject currentObject;
        string currentObjectName, patter;
        Queue<GameObject> queue = new Queue<GameObject>();
        bool match;

        // drop current to queue
        queue.Enqueue(this.gameObject);

        // for alle game object in the tree
        while (queue.Count != 0)
        {
            // fetch current game object
            currentObject = queue.Dequeue();
            currentObjectName = currentObject.name;

            // try to match the pattern to the current game object
            match = false;
            for (int i = patternList.Count; --i >= 0 && !match; )
            { 
                patter = patternList[i];
                if (currentObjectName.Contains(patter) && !currentObjectName.Contains("No" + patter))
                    match = true;
            }

            // if the current game object match to someone pattern
            if (match)
            {
                currentObject.SetActive(true);
                continue;
            }
            // else
            // drop all child to queue
            for (int i = currentObject.transform.childCount; --i >= 0; )
                queue.Enqueue(currentObject.transform.GetChild(i).gameObject);

        }




    }
}
