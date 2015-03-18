/*
 * Project:	Billy's Payback
 * File:	TA_ChangeEnemy.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * The transition action will set attributes to defined enemies
 */
public class TA_ChangeEnemy : FSMAction
{
    // Represents an enemy attribute: bool
    [System.Serializable]
    public class BooleanAttribute
    {
        public bool m_use = false;
        public bool m_value = false;
    }

    // Represents an enemy object
    [System.Serializable]
    public class EnemyObject
    {
        public Enemy m_enemy = null;
        public BooleanAttribute m_canMove = new BooleanAttribute();
    }

    // List with enemy objects
    public EnemyObject[] m_enemies = null;

    // Override: FSMAction::OnAction()
    override public void onAction()
    {
        // Loop through all defines enemy objects
        foreach(EnemyObject obj in m_enemies)
        {
            // Validate
            if (obj.m_enemy == null)
                continue;

            // Can move attribute
            if (obj.m_canMove.m_use == true)
                obj.m_enemy.m_allowToMove = obj.m_canMove.m_value;
        }
    }
}
