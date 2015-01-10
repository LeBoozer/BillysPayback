
/*
 * Project:	Billy's Payback
 * File:	HelperFunctions.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * TODO
 */
public static class HelperFunctions
{
    /**
     * Calculates the world scale for a given game-object
     */
    public static Vector3 getWorldScale(GameObject _source)
    {
        // Local variables
        Vector3 result = _source.transform.localScale;
        Transform parent = _source.transform.parent;

        // Move up to root
        while(parent != null)
        {
            result = Vector3.Scale(result, parent.localScale);
            parent = parent.parent;
        }

        return result;
    }
}
