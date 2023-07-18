using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMap : TileMap_Controller
{
    private const string OBSTACLE_TILEMAP_OBJ_NAME = "Obstacle_Tilemap";
    private GameObject[] castleObjs = default;  //!< ��ã�� �˰����� �׽�Ʈ �� ������� �������� ĳ���� ������Ʈ �迭

    //! Awake Ÿ�ӿ� �ʱ�ȭ�� ������ ������ �Ѵ�.
    public override void InitAwake(MapBoard mapController_)
    {
        this.tileMapObjName = OBSTACLE_TILEMAP_OBJ_NAME;
        base.InitAwake(mapController_);
    }       // InitAwake ()

    private void Start()
    {
        StartCoroutine(DelayStart(0f));
    }

    private IEnumerator DelayStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        DoStart();
    }       // DelayStart ()

    private void DoStart()
    {
        // { ������� �������� �����ؼ� Ÿ���� ��ġ�Ѵ�.
        castleObjs = new GameObject[2];
        Terrain_Controller[] passableTerrains = new Terrain_Controller[2];

        List<Terrain_Controller> searchTerrains = default;
        int searchIdx = 0;
        Terrain_Controller foundTile = default;

        // ������� �������� �������� y���� ��ġ�ؼ� �� ������ �޾ƿ´�.
        searchIdx = 0;
        foundTile = default;

        // loop : ������� ã�� ����
        while(foundTile == null | foundTile == default)
        {
            // ������ �Ʒ��� ��ġ�Ѵ�.
            searchTerrains = mapController.GetTerrains_Colum(searchIdx, true);
            foreach(var searchTerrain in searchTerrains)
            {
                if (searchTerrain.IsPassable)
                {
                    foundTile = searchTerrain;
                    break;
                } 
                else { /*Do nothing*/ }
            }

            if (foundTile != null || foundTile != default) { break; }
            if (mapController.MapcellSize.x - 1 <= searchIdx) { break; }
            searchIdx++;
        }       // end loop : ������� ã�� ����
        passableTerrains[0] = foundTile;

        // �������� �������� �������� y���� ��ġ�ؼ� �� ������ �޾ƿ´�.
        searchIdx = mapController.MapcellSize.x - 1;
        foundTile = default;
        while(foundTile == null || foundTile == default)
        {
            // �Ʒ����� ���� ��ġ�Ѵ�.
            searchTerrains = mapController.GetTerrains_Colum(searchIdx);
            foreach(var searchTerrain in searchTerrains)
            {
                if(searchTerrain.IsPassable)
                {
                    foundTile = searchTerrain;
                    break;
                }
                else { /*Do nothing*/}
            }

            if(foundTile != null || foundTile != default) { break; }
            if (searchIdx <= 0) { break; }
            searchIdx--;
        }       // end loop : �������� ã�� ����
        passableTerrains[1] = foundTile;

        // } ������� �������� �����ؼ� Ÿ���� ��ġ�Ѵ�.

        // { ������� �������� ������ �߰��Ѵ�.
        // GameObject changeTilePrefab = ResManager.Instance.Obs
        // } ������� �������� ������ �߰��Ѵ�.

    }       // DoStart ()

    //! ������ �߰��Ѵ�.
    public void Add_Obstacle(GameObject obstacle_)
    {
        allTileObjs.Add(obstacle_);
    }       // Add_Obstacle ()
}
