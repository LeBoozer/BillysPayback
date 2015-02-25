/*
 * Project:	Billy's Payback
 * File:	ScaleRandom.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Scales the attched game-object randomly
 */
public class ScaleRandom : MonoBehaviour
{
    // The min. scaler for the x-axis
    public float m_minScalerX = 1.0f;

    // The max. scaler for the x-axis
    public float m_maxScalerX = 1.0f;

    // The min. scaler for the y-axis
    public float m_minScalerY = 1.0f;

    // The max. scaler for the y-axis
    public float m_maxScalerY = 1.0f;

    // The min. scaler for the z-axis
    public float m_minScalerZ = 1.0f;

    // The max. scaler for the z-axis
    public float m_maxScalerZ = 1.0f;

    // Use this for initialization
    void Start()
    {
        // Local variables
        float x = transform.localScale.x * Random.RandomRange(m_minScalerX, m_maxScalerX);
        float y = transform.localScale.y * Random.RandomRange(m_minScalerY, m_maxScalerY);
        float z = transform.localScale.z * Random.RandomRange(m_minScalerZ, m_maxScalerZ);

        // Set new scale
        transform.localScale = new Vector3(x, y, z);
    }
}
