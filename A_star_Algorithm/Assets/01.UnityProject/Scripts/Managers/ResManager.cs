using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Prefabs를 전부 가져옴. Resources.LoadAll 부분이 중요함.
public class ResManager : GSingleton<ResManager>
{
    private const string TERRAIN_PREF_PATH = "Prefabs";

    public Dictionary<string, GameObject> terrainPrefabs = default;

    protected override void Init()
    {
        base.Init();

        terrainPrefabs = new Dictionary<string, GameObject>();
        terrainPrefabs.AddObjs(Resources.LoadAll<GameObject>(TERRAIN_PREF_PATH));
    }
}
