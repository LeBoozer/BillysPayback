/*
 * Project:	Billy's Payback
 * File:	CP_Boss.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */

using UnityEngine;
using System.Collections;

/*
 * special check point for the boss fight
 */
public class CP_Boss : CheckPoint 
{
    public GameObject m_boss = null;

    // Use this for initialization
    new void Start()
    {
        // constructor of the checkpoints
        base.Start();

        // 
        if (m_boss == null)
            return;

        Vector3 startPositionOfBoss = m_boss.transform.position;
        Boss script = null;
        Component[] con = m_boss.GetComponents(typeof(Boss));
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

        m_checkPointAction = () =>
        {
            // let break the boss fight
            script.BreakBossFight();

            // reset position of boss
            m_boss.transform.position = startPositionOfBoss;

        };

    }
}
