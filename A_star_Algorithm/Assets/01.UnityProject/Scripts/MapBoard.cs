using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainUtils;

public class MapBoard : MonoBehaviour
{
    private const string TERRAIN_MAP_OBJ_NAME = "Terrain_Grid";
    private const string OBSTACLE_MAP_OBJ_NAME = "Obstacle_Grid";

    public Vector2Int MapcellSize { get; private set; } = default;
    public Vector2 MapcellGap { get; private set; } = default;

    private Terrain_Map terrainMap = default;
    private ObstacleMap obstacleMap = default;

    private void Awake()
    {
        // { �Ŵ��� ��ũ��Ʈ�� �ʱ�ȭ�Ѵ�.
        ResManager.Instance.Create();

        // PathFinder �ʱ�ȭ
        PathFinder.Instance.Create();
        PathFinder.Instance.mapBoard = this;
        // } �Ŵ��� ��ũ��Ʈ�� �ʱ�ȭ�Ѵ�.

        // { �ʿ� ������ �ʱ�ȭ�Ͽ� ��ġ�Ѵ�.
        terrainMap = gameObject.FindChildComponent<Terrain_Map>(TERRAIN_MAP_OBJ_NAME);
        terrainMap.InitAwake(this);
        MapcellSize = terrainMap.GetCellSize();
        MapcellGap = terrainMap.GetCellGap();
        // } �ʿ� ������ �ʱ�ȭ�Ͽ� ��ġ�Ѵ�.

        // { �ʿ� ������ �ʱ�ȭ�Ͽ� ��ġ�Ѵ�.
        obstacleMap = gameObject.FindChildComponent<ObstacleMap>(OBSTACLE_MAP_OBJ_NAME);
        obstacleMap.InitAwake(this);
        // } �ʿ� ������ �ʱ�ȭ�Ͽ� ��ġ�Ѵ�.

    }

    //! Ÿ�� �ε����� �޾Ƽ� �ش� Ÿ���� �����ϴ� �Լ�
    public Terrain_Controller GetTerrain(int idx1D)
    {
        return terrainMap.GetTile(idx1D);
    }       // GetTerrain()

    //! ���� x ��ǥ�� �޾Ƽ� �ش� ���� Ÿ���� ����Ʈ�� �������� �Լ�
    public List<Terrain_Controller> GetTerrains_Colum(int xIdx2D)
    {
        return GetTerrains_Colum(xIdx2D, false);
    }
    //! ���� x ��ǥ�� �޾Ƽ� �ش� ���� Ÿ���� ����Ʈ�� �������� �Լ�, �����ε�
    public List<Terrain_Controller> GetTerrains_Colum(int xIdx2D, bool isSortReverse)
    {
        List<Terrain_Controller> terrains = new List<Terrain_Controller>();
        Terrain_Controller tempTile = default;
        int tileIdx1D = 0;

        // loop : y���� ũ�⸸ŭ ��ȸ�ϴ� ����
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

    //! ������ �ε����� 2D ��ǥ�� �����ϴ� �Լ�
    public Vector2Int GetTileIdx2d(int idx1D)
    {
        Vector2Int tileIdx2D = Vector2Int.zero;
        tileIdx2D.x = idx1D % MapcellSize.x;
        tileIdx2D.y = idx1D / MapcellSize.x; //�� x�� ������?

        return tileIdx2D;
    }       // GetTileIdx2d ()

    //! ������ 2D ��ǥ�� �ε����� �����ϴ� �Լ�
    public int GetTileIdx1D(Vector2Int idx2D)
    {
        int tileIdx1D = (idx2D.y * MapcellSize.x) + idx2D.x;
        return tileIdx1D;
    }       // GetTileIdx1D ()

    //! �� ���� ������ Ÿ�� �Ÿ��� �����ϴ� �Լ�
    public Vector2Int GetDistance2D(GameObject targetTerrainObj, GameObject destTerrainObj)
    {
        Vector2 localDistance = destTerrainObj.transform.localPosition - targetTerrainObj.transform.localPosition;

        Vector2Int distance2D = Vector2Int.zero;
        distance2D.x = Mathf.RoundToInt(localDistance.x / MapcellGap.x);
        distance2D.y = Mathf.RoundToInt(localDistance.y / MapcellGap.y);

        // ���� ������ ��
        distance2D = GFunc.Abs(distance2D);

        return distance2D;
    }       // GetDistance2D ()

    //! 2D ��ǥ�� �������� �ֺ� 4���� Ÿ���� �ε����� �����ϴ� �Լ�
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
            // 2D ��ǥ�� ��ȿ���� �˻��Ѵ�.
            if(idx2D.x.IsInRange(0, MapcellSize.x) == false) { continue; }
            if(idx2D.y.IsInRange(0, MapcellSize.y) == false) { continue; }

            idx1D_around4ways.Add(GetTileIdx1D(idx2D));
        }

        return idx1D_around4ways;
    }
}      
