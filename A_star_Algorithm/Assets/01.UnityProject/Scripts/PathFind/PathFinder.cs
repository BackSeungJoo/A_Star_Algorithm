using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : GSingleton<PathFinder>
{
    #region ���� Ž���� ���� ����
    public GameObject sourceObj = default;
    public GameObject destinationObj = default;
    public MapBoard mapBoard = default;
    #endregion      // ���� Ž���� ���� ����

    #region A star �˰������� �ִܰŸ��� ã�� ���� ����
    private List<AStarNode> aStarResultPath = default;
    private List<AStarNode> aStarOpenPath = default;
    private List<AStarNode> aStarClosePath = default;
    #endregion      // A star �˰������� �ִܰŸ��� ã�� ���� ����

    //! ������� ������ ������ ���� ã�� �Լ�
    public void FindPath_Astar()
    {
        StartCoroutine(DelayFindPath_Astar(1.0f));
    }       // FindPath_Astar()

    //! Ž�� �˰��� �����̸� �Ǵ�.
    private IEnumerator DelayFindPath_Astar(float delay_)
    {
        // A star �˰����� ����ϱ� ���ؼ� �н� ����Ʈ�� �ʱ�ȭ�Ѵ�.
        aStarOpenPath = new List<AStarNode>();
        aStarClosePath = new List<AStarNode>();
        aStarResultPath = new List<AStarNode>();

        Terrain_Controller targetTerrain = default;

        // ������� �ε����� ���ؼ�, ����� ��带 ã�ƿ´�.
        string[] sourceObjNameParts = sourceObj.name.Split('_');
        int sourceIdx1D = -1;
        int.TryParse(sourceObjNameParts[sourceObjNameParts.Length - 1], out sourceIdx1D);
        targetTerrain = mapBoard.GetTerrain(sourceIdx1D);

        // ã�ƿ� ����� ��带 open ����Ʈ�� �߰�
        AStarNode targetNode = new AStarNode(targetTerrain, destinationObj);
        Add_AstarOpenList(targetNode);

        int loopIdx = 0;
        bool isFoundDestination = false;
        bool isNowayToGo = false;

        //while (loopIdx < 10)
        while(isFoundDestination == false && isNowayToGo == false)
        {
            // { Open ����Ʈ�� ��ȸ�ؼ� ���� �ڽ�Ʈ�� ���� ��带 �����Ѵ�.
            AStarNode minCostNode = default;

            // loop : ���� �ڽ�Ʈ�� ���� ��带 �����Ѵ�.
            foreach(var terrainNode in aStarOpenPath)
            {
                // if : ���� ���� �ڽ�Ʈ�� ��尡 ��� �ִ� ���
                if(minCostNode == default)
                {
                    minCostNode = terrainNode;
                }

                // else : ���� ���� �ڽ�Ʈ�� ��尡 ĳ�̵Ǿ� �ִ� ���
                else
                {
                    // terrainNode �� �� ���� �ڽ�Ʈ�� ������ ���
                    // minCostNode �� ������Ʈ �Ѵ�.
                    if (terrainNode.AstarF < minCostNode.AstarF)
                    {
                        minCostNode = terrainNode;
                    }
                    else { continue; }
                }
            }
            // } Open ����Ʈ�� ��ȸ�ؼ� ���� �ڽ�Ʈ�� ���� ��带 �����Ѵ�.

            //Debug.LogFormat("Terrain.TileIdx1D : {0}", minCostNode.Terrain.TileIdx1D);

            Debug.Log("minCostNode : " + $"{minCostNode == null}");

            minCostNode.ShowCost_Astar();
            minCostNode.Terrain.SetTileActiveColor(RDefine.TileStatusColor.SEARCH);

            // ������ ��尡 �������� �����ߴ��� Ȯ���Ѵ�.
            bool isArriveDest = mapBoard.GetDistance2D(minCostNode.Terrain.gameObject, destinationObj).Equals(Vector2Int.zero);

            // if : ������ ��尡 �������� ������ ���
            if (isArriveDest)
            {
                // { �������� �����ߴٸ� aStarResultPath ����Ʈ�� �����Ѵ�.
                AStarNode resultNode = minCostNode;
                bool isSet_aStarResultPathOk = false;

                // loop : ���� ��带 ã�� ���� �� ���� ��ȸ�ϴ� ����
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
                // } �������� �����ߴٸ� aStarResultPath ����Ʈ�� �����Ѵ�.

                // Open list�� close list�� �����Ѵ�.
                aStarOpenPath.Clear();
                aStarClosePath.Clear();
                isFoundDestination = true;
                break;
            }

            // else : ������ ��尡 �������� �������� ���� ���
            else
            {
                // { �������� �ʾҴٸ� ���� Ÿ���� �������� 4 ���� ��带 ã�ƿ´�.
                List<int> nextSearchIdx1Ds = mapBoard.GetTileIdx2D_Around4ways(minCostNode.Terrain.TileIdx2D);

                // ã�ƿ� ��� �߿��� �̵� ������ ��带 Open List�� �߰��Ѵ�.
                AStarNode nextNode = default;

                // loop : �̵� ������ ��带 open List�� �߰��ϴ� ����
                foreach (var nextIdx1D in nextSearchIdx1Ds)
                {
                    nextNode = new AStarNode(mapBoard.GetTerrain(nextIdx1D), destinationObj);

                    if (nextNode.Terrain.IsPassable == false) { continue; }

                    Add_AstarOpenList(nextNode, minCostNode);
                }
                // } �������� �ʾҴٸ� ���� Ÿ���� �������� 4 ���� ��带 ã�ƿ´�.

                // Ž���� ���� ���� Close list �� �߰��ϰ�, open list���� �����Ѵ�.
                // �� ��, Open list�� ����ִٸ� �� �̻� Ž���� �� �ִ� ���� �������� �ʴ� ���̴�.
                aStarClosePath.Add(minCostNode);
                aStarOpenPath.Remove(minCostNode);

                // if : �������� �������� ���ߴµ� �� �̻� Ž���� �� �ִ� ���� ���� ���
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
        }       // loop : A star �˰������� ���� ã�� ���� ����

    }       // DelayFondPath_Astar()

    //! ����� ������ ��带 open ����Ʈ�� �߰��ϴ� �Լ�
    private void Add_AstarOpenList(AStarNode targetTerrain_, AStarNode prevNode = default)
    {
        // Open ����Ʈ�� �߰��ϱ� ���� �˰��� ����� �����Ѵ�.
        Update_AstarCostToTerrain(targetTerrain_, prevNode);

        AStarNode closeNode = aStarClosePath.FindNode(targetTerrain_);

        // if : close List�� �̹� Ž���� ���� ��ǥ�� ��尡 �����ϴ� ���
        if(closeNode != default && closeNode != null)
        {
            // �̹� Ž���� ���� ��ǥ�� ��尡 �����ϴ� ��쿡�� OpenList�� �߰����� �ʴ´�.
            /*Do nothing*/
        }

        // else : ���� Ž���� ������ ���� ����� ���
        else
        {
            AStarNode openedNode = aStarOpenPath.FindNode(targetTerrain_);

            // if : Open List�� ���� �߰��� ���� ���� ��ǥ�� ��尡 �����ϴ� ���
            if(openedNode != default && openedNode != null)
            {
                // Ÿ�� ����� �ڽ�Ʈ�� �� ���� ��쿡�� OpenList���� ��带 ��ü�Ѵ�.
                // Ÿ�� ����� �ڽ�Ʈ�� �� ū ��쿡�� OpenList�� �߰����� �ʴ´�.
                if(targetTerrain_.AstarF < openedNode.AstarF) 
                {
                    aStarOpenPath.Remove(openedNode);
                    aStarOpenPath.Add(targetTerrain_);
                }
                else { /*Do nothing*/ }
            }

            // else : Open List�� ���� �߰��� ���� ���� ��ǥ�� ��尡 ���� ���
            else
            {
                aStarOpenPath.Add(targetTerrain_);
            }
        }
    }       // Add_AstarOpenList

    //! Target ���� ������ Destination ���� ������ Distance�� Heuristic�� �����ϴ� �Լ�
    private void Update_AstarCostToTerrain(AStarNode targetNode, AStarNode prevNode)
    {
        // { Target �������� Destination ������ 2D Ÿ�� �Ÿ��� ����ϴ� ����
        Vector2Int distance2D = mapBoard.GetDistance2D(targetNode.Terrain.gameObject, destinationObj);
        int totalDistance2D = distance2D.x + distance2D.y;

        // Heruistic�� �����Ÿ��� �����Ѵ�.
        Vector2 localDistance = destinationObj.transform.localPosition - targetNode.Terrain.transform.localPosition;
        float heuristic = Mathf.Abs(localDistance.magnitude);
        // } Target �������� Destination ������ 2D Ÿ�� �Ÿ��� ����ϴ� ����

        // { ���� ��尡 �����ϴ� ���, ���� ����� �ڽ�Ʈ�� �߰��ؼ� �����Ѵ�.
        if(prevNode == default || prevNode == null) { /*Do nothing*/ }
        else
        {
            // + 1�� ����ġ. ����ư �˰���
            totalDistance2D = Mathf.RoundToInt(prevNode.AstarG + 1.0f);
        }
        targetNode.UpdateCost_Astar(totalDistance2D, heuristic, prevNode);
        // } ���� ��尡 �����ϴ� ���, ���� ����� �ڽ�Ʈ�� �߰��ؼ� �����Ѵ�.
    }
}       // class PathFinder
