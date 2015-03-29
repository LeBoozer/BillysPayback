/*
 * Project:	Billy's Payback
 * File:	ConsoleCommands.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Handles console commands
 */
public class ConsoleCommands : MonoBehaviour
{
    // Override: MonoBehaviour::Awake()
    void Awake()
    {
        // Local variables
        var instance = ConsoleCommandsRepository.Instance;

        // Register commands
        instance.RegisterCommand("Light.Shadow", disableShadows);
        instance.RegisterCommand("PointLight.Shadow", disableShadowsPointLight);
        instance.RegisterCommand("Particles", disableParticleEffects);
    }

    // Disables all shadows
    public string disableShadows(params string[] _args)
    {
        // Local variables
        int param = 0;
        Light[] lights = null;

        // Check parameter
        if (_args == null || _args.Length < 1)
            return "";
        if (int.TryParse(_args[0], out param) == false)
            return "Invalid parameter!";

        // Get lights
        lights = GameObject.FindObjectsOfType<Light>();
        foreach (Light l in lights)
            l.shadows = param == 0 ? LightShadows.None : LightShadows.Soft;

        return "";
    }

    // Disables all shadows (point light)
    public string disableShadowsPointLight(params string[] _args)
    {
        // Local variables
        int param = 0;
        Light[] lights = null;

        // Check parameter
        if (_args == null || _args.Length < 1)
            return "";
        if (int.TryParse(_args[0], out param) == false)
            return "Invalid parameter!";

        // Get lights
        lights = GameObject.FindObjectsOfType<Light>();
        foreach (Light l in lights)
        {
            if (l.type != LightType.Point)
                continue;
            l.shadows = param == 0 ? LightShadows.None : LightShadows.Soft;
        }

        return "";
    }

    // Disables all particle systems
    public string disableParticleEffects(params string[] _args)
    {
        // Local variables
        int param = 0;
        ParticleRenderer[] renderer = null;
        ParticleEmitter[] emitter = null;

        // Check parameter
        if (_args == null || _args.Length < 1)
            return "";
        if (int.TryParse(_args[0], out param) == false)
            return "Invalid parameter!";

        // Get particle renderer
        renderer = GameObject.FindObjectsOfType<ParticleRenderer>();
        foreach (ParticleRenderer r in renderer)
            r.enabled = param == 0 ? false : true;

        // Get particle emitter
        emitter = GameObject.FindObjectsOfType<ParticleEmitter>();
        foreach (ParticleEmitter e in emitter)
            e.enabled = param == 0 ? false : true;

        return "";
    }
}
