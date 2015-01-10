/*
 * Project:	Billy's Payback
 * File:	Hitable.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * TODO
 */
public abstract class Hitable : MonoBehaviour
{
    /**
     * TODO
     */
    public abstract void onHit(Hitable _source);
}
