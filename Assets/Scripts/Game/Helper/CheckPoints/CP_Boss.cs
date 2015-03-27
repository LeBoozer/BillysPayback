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
        SA_SaveObjectTransform.Entry[] bossData = new SA_SaveObjectTransform.Entry[m_boss.Count];
        for (int i = 0; i < m_boss.Count; ++i)
            if (m_boss[i] != null)
            {
                bossData[i] = new SA_SaveObjectTransform.Entry();
                bossData[i].m_transform = new SA_SaveObjectTransform.StoredTransforms();
                bossData[i].m_transform.m_localPosition = m_boss[i].transform.localPosition;
                bossData[i].m_transform.m_localRotation = m_boss[i].transform.localRotation;
                bossData[i].m_transform.m_localScale    = m_boss[i].transform.localScale;
            }
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
                if (script != null)
                    script.BreakBossFight();

                // reset position of boss
                boss.transform.localPosition = bossData[i].m_transform.m_localPosition;
                boss.transform.localRotation = bossData[i].m_transform.m_localRotation;
                boss.transform.localScale    = bossData[i].m_transform.m_localScale;
            }

        };

    }
}
