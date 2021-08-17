using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SmallEnemyScriptableObject", menuName = "ScriptableObjects/SmallEnemyScriptableObject", order = 1)]
public class SmallEnemyScriptableObject : ScriptableObject
{
    public GameObject[] deathExplosions;
    public Color[] powerupColors;

    public GameObject[] powerups;

    [ColorUsage(true, true)]
    public Color[] powerupColorsHDR;

    public Material[] outlines;

    public Material[] seeThroughMats;

    public GameObject healthBarPrefab;

    public GameObject delayedDeathPrefab;

    public GameObject hitSparkPrefab;

    [GradientUsage(true)]
    public Gradient[] hitSparkGradients;
}
