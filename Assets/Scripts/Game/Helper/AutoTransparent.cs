/*
 * Project:	Billy's Payback
 * File:	AutoTransparent.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * ToDo
 */
public class AutoTransparent : MonoBehaviour 
{
    // True if the game-object is transparent
    private bool m_isTransparent = false;

    // The old shader
    private Shader m_oldShader = null;

    // The old color (if available)
    private Color m_oldColor = Color.white;

    // The transparency (if available)
    private float m_transparency = 1.0f;

    // The target transparency
    private float m_targetTransparency = 0.50f;

    // The fall-off (returns to 100% in x.y seconds)
    private float m_fallOff = 0.1f;

    // Override: MonoBehaviour::Awake()
    void Awake()
    {
        // Got a material?
        if (GetComponent<Renderer>().material != null)
        {
            // Save the current shader
            m_oldShader = GetComponent<Renderer>().material.shader;

            // Set standard transparence shader
            GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");

            // Material has color property?
            if (GetComponent<Renderer>().material.HasProperty("_Color") == true)
            {
                m_oldColor = GetComponent<Renderer>().material.color;
                m_transparency = m_oldColor.a;
            }
        }
	}

    // Override: MonoBehaviour::OnDestroy()
    void OnDestroy()
    {
        // Reset shader
        if (m_oldShader == null)
            return;
        GetComponent<Renderer>().material.shader = m_oldShader;
        GetComponent<Renderer>().material.color = m_oldColor;
    }

    // Override: MonoBehaviour::Update()
    void Update()
    {
        // Local variables
        Color currentColor;

        // Remove component?
        if(m_isTransparent == false && m_transparency >= 1.0f)
        {
            Destroy(this);
            return;
        }

        // Fading-out?
        if(m_isTransparent == true)
        {
            if (m_transparency > m_targetTransparency)
                m_transparency -= ((1.0f - m_targetTransparency) * Time.deltaTime) / m_fallOff;
        }
        else
        {
            m_transparency += ((1.0f - m_targetTransparency) * Time.deltaTime) / m_fallOff;
        }

        // Set transparency
        currentColor = GetComponent<Renderer>().material.color;
        currentColor.a = m_transparency;
        GetComponent<Renderer>().material.color = currentColor;

	    // Clear flag
        m_isTransparent = false;
	}

    // Notifies the game-object, that it should stay transparent
    public void makeTransparent()
    {
        // Set flag
        m_isTransparent = true;
    }
}
