using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : GSingleton<PathFinder>
{
    #region 지형 탐색을 위한 변수
    public GameObject sourceObj = default;
    public GameObject destinationObj = default;
    public MapBoard mapBoard = default;
    #endregion      // 지형 탐색을 위한 변수

    #region A star 알고리즘으로 최단거리를 찾기 위한 변수
    private List<AStarNode> aStarResultPath = default;
    private List<AStarNode> aStarOpenPath = default;
    private List<AStarNode> aStarClosePath = default;
    #endregion      // A star 알고리즘으로 최단거리를 찾기 위한 변수

    //! 출발지와 목적지 정보로 길을 찾는 함수
    public void FindPath_Astar()
    {
        StartCoroutine(DelayFindPath_Astar(1.0f));
    }       // FindPath_Astar()

    //! 탐색 알고리즘에 딜레이를 건다.
    private IEnumerator DelayFindPath_Astar(float delay_)
    {
        // A star 알고리즘을 사용하기 위해서 패스 리스트를 초기화한다.
        aStarOpenPath = new List<AStarNode>();
        aStarClosePath = new List<AStarNode>();
        aStarResultPath = new List<AStarNode>();

        Terrain_Controller targetTerrain = default;

        // 출발지의 인덱스를 구해서, 출발지 노드를 찾아온다.
        string[] sourceObjNameParts = sourceObj.name.Split('_');
        int sourceIdx1D = -1;
        int.TryParse(sourceObjNameParts[sourceObjNameParts.Length - 1], out sourceIdx1D);
        targetTerrain = mapBoard.GetTerrain(sourceIdx1D);

        // 찾아온 출발지 노드를 open 리스트에 추가
        AStarNode targetNode = new AStarNode(targetTerrain, destinationObj);
        Add_AstarOpenList(targetNode);

        int loopIdx = 0;
        bool isFoundDestination = false;
        bool isNowayToGo = false;

        //while (loopIdx < 10)
        while(isFoundDestination == false && isNowayToGo == false)
        {
            // { Open 리스트를 순회해서 가장 코스트가 낮은 노드를 선택한다.
            AStarNode minCostNode = default;

            // loop : 가장 코스트가 낮은 노드를 선택한다.
            foreach(var terrainNode in aStarOpenPath)
            {
                // if : 가장 작은 코스트의 노드가 비어 있는 경우
                if(minCostNode == default)
                {
                    minCostNode = terrainNode;
                }

                // else : 가장 작은 코스트의 노드가 캐싱되어 있는 경우
                else
                {
                    // terrainNode 가 더 작은 코스트를 가지는 경우
                    // minCostNode 를 업데이트 한다.
                    if (terrainNode.AstarF < minCostNode.AstarF)
                    {
                        minCostNode = terrainNode;
                    }
                    else { continue; }
                }
            }
            // } Open 리스트를 순회해서 가장 코스트가 낮은 노드를 선택한다.

            //Debug.LogFormat("Terrain.TileIdx1D : {0}", minCostNode.Terrain.TileIdx1D);

            Debug.Log("minCostNode : " + $"{minCostNode == null}");

            minCostNode.ShowCost_Astar();
            minCostNode.Terrain.SetTileActiveColor(RDefine.TileStatusColor.SEARCH);

            // 선택한 노드가 목적지에 도달했는지 확인한다.
            bool isArriveDest = mapBoard.GetDistance2D(minCostNode.Terrain.gameObject, destinationObj).Equals(Vector2Int.zero);

            // if : 선택한 노드가 목적지에 도착한 경우
            if (isArriveDest)
            {
                // { 목적지에 도착했다면 aStarResultPath 리스트를 설정한다.
                AStarNode resultNode = minCostNode;
                bool isSet_aStarResultPathOk = false;

                // loop : 이전 노드를 찾지 못할 때 까지 순회하는 루프
                while(isSet_aStarResultPathOk == false)
                {
                    aStarResultPath.Add(resultNode);
                    if(resultNode.aStarPrevNode == default || resultNode.aStarPrevNode == null)
                    {
                        isSet_aStarResultPathOk = true;
                        break;
                    }
                    else { /*Do nothing*/ }

                    resultNode = resultNode.aStarPrevNode;
                }
                // } 목적지에 도착했다면 aStarResultPath 리스트를 설정한다.

                // Open list와 close list를 정리한다.
                aStarOpenPath.Clear();
                aStarClosePath.Clear();
                isFoundDestination = true;
                break;
            }

            // else : 선택한 노드가 목적지에 도착하지 못한 경우
            else
            {
                // { 도착하지 않았다면 현재 타일을 기준으로 4 방향 노드를 찾아온다.
                List<int> nextSearchIdx1Ds = mapBoard.GetTileIdx2D_Around4ways(minCostNode.Terrain.TileIdx2D);

                // 찾아온 노드 중에서 이동 가능한 노드를 Open List에 추가한다.
                AStarNode nextNode = default;

                // loop : 이동 가능한 노드를 open List에 추가하는 루프
                foreach (var nextIdx1D in nextSearchIdx1Ds)
                {
                    nextNode = new AStarNode(mapBoard.GetTerrain(nextIdx1D), destinationObj);

                    if (nextNode.Terrain.IsPassable == false) { continue; }

                    Add_AstarOpenList(nextNode, minCostNode);
                }
                // } 도착하지 않았다면 현재 타일을 기준으로 4 방향 노드를 찾아온다.

                // 탐색이 끝난 노드는 Close list 에 추가하고, open list에서 제거한다.
                // 이 때, Open list가 비어있다면 더 이상 탐색할 수 있는 길이 존재하지 않는 것이다.
                aStarClosePath.Add(minCostNode);
                aStarOpenPath.Remove(minCostNode);

                // if : 목적지에 도착하지 못했는데 더 이상 탐색할 수 있는 길이 없는 경우
                if(aStarOpenPath.IsValid() == false)
                {
                    GFunc.LogWarning("[Warning] There are no more tiles to explore.");
                    isNowayToGo = true;
                }

                foreach(var tempNode in aStarOpenPath)
                {
                    GFunc.Log($"Idx : {tempNode.Terrain.TileIdx1D}," + $"Cost : {tempNode.AstarF}");
                }
            }

            loopIdx++;
            yield return new WaitForSeconds(delay_);
        }       // loop : A star 알고리즘으로 길을 찾는 메인 루프

    }       // DelayFondPath_Astar()

    //! 비용을 설정한 노드를 open 리스트에 추가하는 함수
    private void Add_AstarOpenList(AStarNode targetTerrain_, AStarNode prevNode = default)
    {
        // Open 리스트에 추가하기 전에 알고리즘 비용을 설정한다.
        Update_AstarCostToTerrain(targetTerrain_, prevNode);

        AStarNode closeNode = aStarClosePath.FindNode(targetTerrain_);

        // if : close List에 이미 탐색이 끝난 좌표의 노드가 존재하는 경우
        if(closeNode != default && closeNode != null)
        {
            // 이미 탐색이 끝난 좌표의 노드가 존재하는 경우에는 OpenList에 추가하지 않는다.
            /*Do nothing*/
        }

        // else : 아직 탐색이 끝나지 않은 노드인 경우
        else
        {
            AStarNode openedNode = aStarOpenPath.FindNode(targetTerrain_);

            // if : Open List에 현재 추가할 노드와 같은 좌표의 노드가 존재하는 경우
            if(openedNode != default && openedNode != null)
            {
                // 타겟 노드의 코스트가 더 작은 경우에는 OpenList에서 노드를 교체한다.
                // 타겟 노드의 코스트가 더 큰 경우에는 OpenList에 추가하지 않는다.
                if(targetTerrain_.AstarF < openedNode.AstarF) 
                {
                    aStarOpenPath.Remove(openedNode);
                    aStarOpenPath.Add(targetTerrain_);
                }
                else { /*Do nothing*/ }
            }

            // else : Open List에 현재 추가할 노드와 같은 좌표의 노드가 없는 경우
            else
            {
                aStarOpenPath.Add(targetTerrain_);
            }
        }
    }       // Add_AstarOpenList

    //! Target 지형 정보와 Destination 지형 정보로 Distance와 Heuristic을 설정하는 함수
    private void Update_AstarCostToTerrain(AStarNode targetNode, AStarNode prevNode)
    {
        // { Target 지형에서 Destination 까지의 2D 타일 거리를 계산하는 로직
        Vector2Int distance2D = mapBoard.GetDistance2D(targetNode.Terrain.gameObject, destinationObj);
        int totalDistance2D = distance2D.x + distance2D.y;

        // Heruistic은 직선거리로 고정한다.
        Vector2 localDistance = destinationObj.transform.localPosition - targetNode.Terrain.transform.localPosition;
        float heuristic = Mathf.Abs(localDistance.magnitude);
        // } Target 지형에서 Destination 까지의 2D 타일 거리를 계산하는 로직

        // { 이전 노드가 존재하는 경우, 이전 노드의 코스트를 추가해서 연산한다.
        if(prevNode == default || prevNode == null) { /*Do nothing*/ }
        else
        {
            // + 1은 보정치. 맨해튼 알고리즘
            totalDistance2D = Mathf.RoundToInt(prevNode.AstarG + 1.0f);
        }
        targetNode.UpdateCost_Astar(totalDistance2D, heuristic, prevNode);
        // } 이전 노드가 존재하는 경우, 이전 노드의 코스트를 추가해서 연산한다.
    }
}       // class PathFinder
