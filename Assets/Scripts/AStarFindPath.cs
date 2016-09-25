using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarFindPath : MonoBehaviour {
    //private GridScr Grid;

    public static AStarFindPath _astar;

    //起点坐标
    GridScr StarScr;
    //终点坐标
    GridScr EndScr;
    //开启列表和关闭列表
    List<GridScr> openSet = new List<GridScr>();
    HashSet<GridScr> closeSet = new HashSet<GridScr>();

    void Awake()
    {
        _astar = this;
    }

    public AStarFindPath(GridScr select01, GridScr select02)
    {
        StarScr = select01;
        EndScr = select02;

        FindingPath();
    }

    //寻路方法
    void FindingPath()
    {
        openSet.Add(StarScr);

        while (openSet.Count >0 )
        {
            GridScr curNode = openSet[0];

            for (int i = 0, max = openSet.Count; i < max; i++)
            {
                if (openSet[i].fCost <= curNode.fCost &&
                    openSet[i].hCost < curNode.hCost)
                {
                    curNode = openSet[i];
                }
            }

            openSet.Remove(curNode);
            closeSet.Add(curNode);

            //找到目标点
            if (curNode == EndScr)
            {
                generatePath(StarScr, EndScr);
                return;
            }

            //判断周围节点，选择一个最好的
            foreach (var item in GridController._gridcontroller.getNeibourHood(curNode))
            {
                //如果是墙，或者已经在关闭列表中
                if (item.ChessmanColor != 0 || closeSet.Contains(item))
                {
                    continue;
                }
                //计算当前相邻节点与开始节点的距离
                int newCost = curNode.gCost + getDistanceGrid (curNode, item);
                // 如果距离更小，或者原来不在开始列表中
                if (newCost < item.gCost || !openSet.Contains(item))
                {
                    // 更新与开始节点的距离
                    item.gCost = newCost;
                    // 更新与终点的距离
                    item.hCost = getDistanceGrid(item, EndScr);
                    // 更新父节点为当前选定的节点
                    item.parent = curNode;
                    // 如果节点是新加入的，将它加入打开列表中
                    if (!openSet.Contains(item))
                    {
                        openSet.Add(item);
                    }
                }
            }
        }
        generatePath(StarScr, null);
    }

    // 生成路径
    void generatePath(GridScr startNode, GridScr endNode)
    {
        List<GridScr> path = new List<GridScr>();
        if (endNode != null)
        {
            GridScr temp = endNode;
            while (temp != startNode)
            {
                path.Add(temp);
                temp = temp.parent;
            }
            // 反转路径
            path.Reverse();
        }

    }

    //获取两个节点之间的距离
    int getDistanceGrid(GridScr start,GridScr end)
    {
        int x = Mathf.Abs(start.Gridx - end.Gridx);
        int y = Mathf.Abs(start.Gridy + end.Gridy);
        //曼哈顿估价
        return x * 14 + y * 14;
    }

}

