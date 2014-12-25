/*
 * Project:	Billy's Payback
 * File:	Diamond.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * ToDo
 */
public class Diamond : MonoBehaviour
{
    // Override: MonoBehaviour::Awake()
    void Awake()
    {
        transform.Rotate(Vector3.up * Random.value * 100 * GameConfig.DIAMONG_ROTATION_SPEED);
    }

    // Override: MonoBehaviour::Update()
    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * GameConfig.DIAMONG_ROTATION_SPEED);
    }
}
