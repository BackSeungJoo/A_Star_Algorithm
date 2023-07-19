using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObstacleMap : TileMap_Controller
{
    private const string OBSTACLE_TILEMAP_OBJ_NAME = "Obstacle_Tilemap";
    private GameObject[] castleObjs = default;  //!< 길찾기 알고리즘을 테스트 할 출발지와 목적지를 캐싱한 오브젝트 배열

    //! Awake 타임에 초기화할 내용을 재정의 한다.
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
        // { 출발지와 목적지를 설정해서 타일을 배치한다.
        castleObjs = new GameObject[2];
        Terrain_Controller[] passableTerrains = new Terrain_Controller[2];

        List<Terrain_Controller> searchTerrains = default;
        int searchIdx = 0;
        Terrain_Controller foundTile = default;

        // 출발지는 좌측에서 우측으로 y축을 서치해서 빈 지형을 받아온다.
        searchIdx = 0;
        foundTile = default;

        // loop : 출발지를 찾는 루프
        while(foundTile == null | foundTile == default)
        {
            // 위에서 아래로 서치한다.
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
        }       // end loop : 출발지를 찾는 루프
        passableTerrains[0] = foundTile;

        // 목적지는 우측에서 좌측으로 y축을 서치해서 빈 지형을 받아온다.
        searchIdx = mapController.MapcellSize.x - 1;
        foundTile = default;
        while(foundTile == null || foundTile == default)
        {
            // 아래에서 위로 서치한다.
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
        }       // end loop : 목적지를 찾는 루프
        passableTerrains[1] = foundTile;

        // } 출발지와 목적지를 설정해서 타일을 배치한다.

        // { 출발지와 목적지에 지물을 추가한다.
        GameObject changeTilePrefab = ResManager.Instance.obstaclePrefabs[RDefine.OBSTACLE_PREF_PLAIN_CASTLE];
        GameObject tempchangeTile = default;

        // loop : 출발지와 목적지를 인스턴스화 해서 캐싱하는 루프
        for(int i = 0; i < 2; i++)
        {
            tempchangeTile = Instantiate(changeTilePrefab, tileMap.transform);
            tempchangeTile.name = string.Format("{0}_{1}", changeTilePrefab.name, passableTerrains[i].TileIdx1D);

            tempchangeTile.SetLocalScale(passableTerrains[i].transform.localScale);
            tempchangeTile.SetLocalPos(passableTerrains[i].transform.localPosition);

            // 출발지와 목적지를 캐싱한다.
            castleObjs[i] = tempchangeTile;
            Add_Obstacle(tempchangeTile);

            tempchangeTile = default;

        }       // end loop : 출발지와 목적지를 인스턴스화 해서 캐싱하는 루프
        // } 출발지와 목적지에 지물을 추가한다.

        Update_SourDestToPathFinder();

    }       // DoStart ()

    //! 지물을 추가한다.
    public void Add_Obstacle(GameObject obstacle_)
    {
        allTileObjs.Add(obstacle_);
    }       // Add_Obstacle ()

    //! 패스 파인더에 출발지와 목적지를 설정한다
    public void Update_SourDestToPathFinder()
    {
        PathFinder.Instance.sourceObj = castleObjs[0];
        PathFinder.Instance.destinationObj = castleObjs[1];
    }       // Update_SourDestToPathFinder ()
}
