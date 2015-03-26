/*
 * Project:	Billy's Payback
 * File:	CP_Boss.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * special check point for the boss fight
 */
public class CP_Boss : CheckPoint 
{
    public List<GameObject> m_boss = new List<GameObject>();

    // Use this for initialization
    new void Awake()
    {
        // constructor of the checkpoints
        base.Awake();

        // 
        if (m_boss == null)
            return;
        Vector3[] startPositionOfBoss = new Vector3[m_boss.Count];
        for (int i = 0; i < m_boss.Count; ++i)
            if (m_boss[i] != null)
                startPositionOfBoss[i] = m_boss[i].transform.position;
        m_checkPointAction = () =>
        {
            // for each game object in the list
            for (int i = 0; i < m_boss.Count; ++i)
            {
                // get the current game object
                GameObject boss = m_boss[i];

                // isnt a game object?
                if (boss == null)
                    continue;

                // seek the boss-script
                Boss script = null;
                Component[] con = boss.GetComponents(typeof(Boss));
                if (con != null && con.Length != 0)
                {
                    foreach (Component com in con)
                    {
                        if (com is Boss)    
                        {
                            script = com as Boss;
                            break;
                        }
                    }
                }

                // let break the boss fight
                if(script != null)
                    script.BreakBossFight();

                // reset position of boss
                boss.transform.position = startPositionOfBoss[i];
            }

        };

    }
}
