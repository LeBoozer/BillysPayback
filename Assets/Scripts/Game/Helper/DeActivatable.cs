/*
 * Project:	Billy's Payback
 * File:	DeActivatable.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Blueprint for (de)-activatable objects
 */
public interface DeActivatable
{
    // Returns true if the object has been activated, otherwise false
    bool isActivated();

    // Will be called as soon as the object is being activated by an activator
    void onActivate();

    // Will be called as soon as the object is being deactivated by an activator
    void onDeactivate();
}