using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode
{
    public Terrain_Controller Terrain { get; private set; }
    public GameObject DestinationObj { get; private set; }

    // A star algorithm
    public float AstarF { get; private set; } = float.MaxValue;
    public float AstarG { get; private set; } = float.MaxValue;
    public float AstarH { get; private set; } = float.MaxValue;

    public AStarNode aStarPrevNode { get; private set; } = default;
    public AStarNode(Terrain_Controller terrain_, GameObject destinationObj_)
    {
        Terrain = terrain_;
        DestinationObj = destinationObj_;
    }       // AstarNode()

    //! Astar 알고리즘에 사용할 비용을 설정한다.
    public void UpdateCost_Astar(float gCost, float heuristic, AStarNode prevNode)
    {
        float aStarF = gCost + heuristic;

        if(aStarF < AstarF)
        {
            AstarG = gCost;
            AstarH = heuristic;
            AstarF = aStarF;

            aStarPrevNode = prevNode;
        }       // if: 새로 계산한 비용이 더 작은 경우에만 업데이트 한다.
        else { /* Do nothing */ }
    }       // UpdateCost_Astar()


    //! 설정한 비용을 출력한다.
    public void ShowCost_Astar()
    {
        GFunc.Log($"TileIdx1D: {Terrain.TileIdx1D}," + $"F : {AstarF}, G : {AstarG}, H : {AstarH}");
    }       // showCost_Astar()

}
