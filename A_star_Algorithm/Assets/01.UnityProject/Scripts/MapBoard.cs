using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainUtils;

public class MapBoard : MonoBehaviour
{
    private const string TERRAIN_MAP_OBJ_NAME = "Terrain_Grid";

    public Vector2Int MapcellSize { get; private set; } = default;
    public Vector2 MapcellGap { get; private set; } = default;

    private Terrain_Map terrainMap = default;

    private void Awake()
    {
        // { 매니저 스크립트를 초기화한다.
        ResManager.Instance.Create();
        // } 매니저 스크립트를 초기화한다.

        // { 맵에 지형을 초기화하여 배치한다.
        terrainMap = gameObject.FindChildComponent<Terrain_Map>(TERRAIN_MAP_OBJ_NAME);
        terrainMap.InitAwake(this);
        MapcellSize = terrainMap.GetCellSize();
        MapcellGap = terrainMap.GetCellGap();
        // } 맵에 지형을 초기화하여 배치한다.
    }

    //! 타일 인덱스를 받아서 해당 타일을 리턴하는 함수
    public Terrain_Controller GetTerrain(int idx1D)
    {
        return terrainMap.GetTile(idx1D);
    }       // GetTerrain()

    //! 맵의 x 좌표를 받아서 해당 열의 타일을 리스트로 가져오는 함수
    public List<Terrain_Controller> GetTerrains_Colum(int xIdx2D)
    {
        return GetTerrains_Colum(xIdx2D, false);
    }
    //! 맵의 x 좌표를 받아서 해당 열의 타일을 리스트로 가져오는 함수, 오버로딩
    public List<Terrain_Controller> GetTerrains_Colum(int xIdx2D, bool isSortReverse)
    {
        List<Terrain_Controller> terrains = new List<Terrain_Controller>();
        Terrain_Controller tempTile = default;
        int tileIdx1D = 0;

        // loop : y열의 크기만큼 순회하는 루프
        for (int y = 0; y < MapcellSize.y; y++)
        {
            tileIdx1D = y * MapcellSize.x + xIdx2D;

            tempTile = terrainMap.GetTile(tileIdx1D);
            terrains.Add(tempTile);
        }

        if(terrains.IsValid())
        {
            if(isSortReverse) { terrains.Reverse(); }
            else { /*Do nothing*/}

            return terrains;
        }
        else { return default; }
    }        // GetTerrains_Colum ()

    //! 지형의 인덱스를 2D 좌표로 리턴하는 함수
    public Vector2Int GetTileIdx2d(int idx1D)
    {
        Vector2Int tileIdx2D = Vector2Int.zero;
        tileIdx2D.x = idx1D % MapcellSize.x;
        tileIdx2D.y = idx1D / MapcellSize.y;

        return tileIdx2D;
    }       // GetTileIdx2d ()

    //! 지형의 2D 좌표를 인덱스로 리턴하는 함수
    public int GetTileIdx1D(Vector2Int idx2D)
    {
        int tileIdx1D = (idx2D.y * MapcellSize.x) + idx2D.x;
        return tileIdx1D;
    }       // GetTileIdx1D ()

    //! 두 지형 사이의 타일 거리를 리턴하는 함수
    public Vector2Int GetDistance2D(GameObject targetTerrainObj, GameObject destTerrainObj)
    {
        Vector2 localDistance = destTerrainObj.transform.localPosition - targetTerrainObj.transform.localPosition;

        Vector2Int distance2D = Vector2Int.zero;
        distance2D.x = Mathf.RoundToInt(localDistance.x / MapcellGap.x);
        distance2D.y = Mathf.RoundToInt(localDistance.y / MapcellGap.y);

        // 절댓값 연산을 함
        distance2D = GFunc.Abs(distance2D);

        return distance2D;
    }       // GetDistance2D ()

    //! 2D 좌표를 기준으로 주변 4방향 타일의 인덱스를 리턴하는 함수
    public List<int> GetTileIdx2D_Around4ways(Vector2Int targetIdx2D)
    {
        List<int> idx1D_around4ways = new List<int>();
        List<Vector2Int> idx2D_around4ways = new List<Vector2Int>();
        idx2D_around4ways.Add(new Vector2Int(targetIdx2D.x - 1, targetIdx2D.y));
        idx2D_around4ways.Add(new Vector2Int(targetIdx2D.x + 1, targetIdx2D.y));
        idx2D_around4ways.Add(new Vector2Int(targetIdx2D.x, targetIdx2D.y - 1));
        idx2D_around4ways.Add(new Vector2Int(targetIdx2D.x, targetIdx2D.y + 1));

        foreach(var idx2D in idx2D_around4ways)
        {
            // 2D 좌표가 유효한지 검사한다.
            if(idx2D.x.IsInRange(0, MapcellSize.x) == false) { continue; }
            if(idx2D.y.IsInRange(0, MapcellSize.y) == false) { continue; }

            idx1D_around4ways.Add(GetTileIdx1D(idx2D));
        }

        return idx1D_around4ways;
    }
}      
