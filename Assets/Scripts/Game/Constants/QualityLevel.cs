/*
 * Project:	Billy's Payback
 * File:	QualityLevel.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Copy of the quality level layer used within the unity editor. 
 * This is needed to reference quality levels within the code without using any hardcoding mechanisms!
 */
public abstract class QualityLevel
{
    // Level names
    public static readonly string LEVEL_FASTEST     = "Fastest";
    public static readonly string LEVEL_FAST        = "Fast";
    public static readonly string LEVEL_SIMPLE      = "Simple";
    public static readonly string LEVEL_GOOD        = "Good";
    public static readonly string LEVEL_BEAUTIFUL   = "Beatiful";
    public static readonly string LEVEL_FANTASTIC   = "Fantastic";

    // Level indices
    public static readonly int LEVEL_INDEX_FASTEST      = 0;
    public static readonly int LEVEL_INDEX_FAST         = 1;
    public static readonly int LEVEL_INDEX_SIMPLE       = 2;
    public static readonly int LEVEL_INDEX_GOOD         = 3;
    public static readonly int LEVEL_INDEX_BEAUTIFUL    = 4;
    public static readonly int LEVEL_INDEX_FANTASTIC    = 5;

    // Returns the name of the current level
    public static string getCurrentLevel()
    {
        return QualitySettings.names[QualitySettings.GetQualityLevel()];
    }

    // Returns the index of the current level
    public static int getCurrentLevelIndex()
    {
        return QualitySettings.GetQualityLevel();
    }

    // Returns the number of quality levels
    public static int getLevelCount()
    {
        return QualitySettings.names.Length;
    }

    // Sets the current quality level by index
    public static void setQualityLevelByIndex(int _index)
    {
        if (_index >= getLevelCount())
            return;
        QualitySettings.SetQualityLevel(_index, true);
    }
}
