using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class GridController : MonoBehaviour {
    public static GridController _gridcontroller;

    //棋子相关
    public GameObject ChessmanObj;
    public Sprite[] ChessmanSpr;
    //预览
    public GridScr[] NextGridScrTransform;
    public GameObject NextGrid;
    public GameObject NextParent;

    //最大行列数
    public int MaxRowNum = 9;
    public int MaxColNum = 9;

    //网格相关的
    public GameObject GridObj;
    public Sprite[] GridSpr;
    public GridScr[,] GridScrTransform;

    public GameObject GridParent;

    public int NewChessmanNum = 3;

    private int GridTotal;                  //总的棋子数（网格数）
    private int NowChessmanNum = 0;         //目前棋子数

    private List<GameObject> pathObj = new List<GameObject>();

    [HideInInspector]
    public GridScr Selected01;
    [HideInInspector]
    public GridScr Selected02;

    void Awake()
    {
        _gridcontroller = this;
    }

    /// <summary>
    /// 新建棋盘
    /// </summary>
    public void NewGridCreate()
	{
        GridScrTransform = new GridScr[MaxRowNum, MaxColNum];

        GridTotal = MaxRowNum * MaxColNum;

		for (int i = 0; i < MaxRowNum; i++) 
		{
			for (int j = 0; j < MaxColNum; j++) 
			{
                GridInstant(i, j, GridParent);
			}
		}
        //预览网格创建
        NextGridCreate();
        //预览棋子创建
        NextChessmanCreate();
        //随机位置创建棋子
        DropChessman();
	}

    //预览网格创建
    void NextGridCreate()
    {
        NextGridScrTransform = new GridScr[NewChessmanNum];
        for (int i = 0; i < NewChessmanNum; i++)
        {
            NextInstant(i, NextParent);
        }
    }

    //预览棋子的创建
    void NextChessmanCreate()
    {
        for (int i = 0; i < NewChessmanNum; i++)
        {
            //删除当前已有的
            if (NextGridScrTransform[i].chessmanObj != null)
                Destroy(NextGridScrTransform[i].chessmanObj);
            //随机对象
            int ram = Random.Range(0, ChessmanSpr.Length - 1) + 1;
            ChessmanInstant(ram, NextGridScrTransform[i]);
        }
    }

    //随机位置放置棋子
    public void DropChessman()
    {
        //有足够位置
        if (NowChessmanNum < GridTotal - NewChessmanNum)
        {
            for (int i = 0; i < NewChessmanNum; i++) 			//取决于游戏难度（每次产生数）
            {
                int weizhi = Random.Range(0, GridTotal);
                GridScr scr = GridScrTransform[weizhi % MaxRowNum, weizhi / MaxColNum];
                if (scr.chessmanObj == null) //网格中没有棋子
                {
                    int color = NextGridScrTransform[i].chessmanObj.GetChessmanColor();
                    //创建新棋子
                    ChessmanInstant(color, scr);
                    //创建新预览棋子
                    NextChessmanCreate();
                    Debug.Log("不重复" + i);
                }
                else
                {
                    i -= 1;                                                                     //不清楚这里会不会出错
                    Debug.Log("重复的" + i);
                }
            }
            NowChessmanNum += NewChessmanNum;
        }
        else
        {
            //无法创建时，游戏结束
            GameManager._gameManager.GameOver();
        }
    }

    //移动棋子
    public void ChessmanMoveTo()
    {
        //AStarFindPath._astar.FindingPath(Selected01, Selected02);
        //List<GridScr> _path = AStarFindPath._astar.path;
        GameObject obj = Selected01.chessmanObj.gameObject;
        Vector3 sele01 = GetGridVector(Selected01);
        Vector3 sele02 = GetGridVector(Selected02);

        obj.transform.DOMove(sele02, 0.1f);
        Selected02.changeParentGrid(Selected01);
        Selected01.RemoveChessman();

        //将选择的网格置为空
        Selected01 = null;
        Selected02 = null;

        //判断是否可以消除
        if (!wipeBall(Selected01))
        {
            //不能消除
            //创建新棋子
            DropChessman();
            NextChessmanCreate();
        }
    }


    bool wipeBall(GridScr scr)
    {
        int x = scr.Gridx;
        int y = scr.Gridy;

        int color = scr.chessmanObj.GetChessmanColor();

        //思路就是当一个棋子移动时，判断它八个方向相同颜色的棋子，相同时就消除
        bool[] jieshu = new bool[MaxRowNum];
        int[] tongseshu = new int[MaxRowNum];
        for (int i = 0; i < MaxRowNum; i++)
        {
            jieshu[i] = false;
            tongseshu[i] = 0;
        }

        //从scr向八方向开始辐射
        //寻找4次，因为大于4的情况已经判断过了（在上回合）
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < MaxRowNum; j++)
            {
                if (j == 4||jieshu[j])
                    continue;
                int tx = x + (j / 3 - 1) * i;
                int ty = y + (j % 3 - 1) * i;
                if (tx>=0&&tx<9&&ty>=0&&ty<9)
                {
                    if(color == GridScrTransform[tx,ty].chessmanObj.GetChessmanColor())
						tongseshu[j]++;
					else
						jieshu[j] = true;
                }
                else
                {
                    jieshu[j] = true;
                }
            }
        }

        //将同一条线上的同色数相加
        int zongshu = 0;
        for (int i = 0; i < 4; i++)
        {
            int temi = tongseshu[i] + tongseshu[MaxRowNum - 1 - i];
            if (temi >= 4)
            {
                //那么可以消除
                zongshu += temi;
                //像两个方向延伸消除
                for (int j = 0; j < tongseshu[i]; j++)
                {
                    int tx = x+(i/3-1)*j;
					int ty = y+(i%3-1)*j;
					GridScrTransform[tx,ty].chessmanObj.SetChessmanColor(0);
                }
                for (int j = 1; j <= tongseshu[MaxRowNum - 1 - i]; j++)
                {
                    int tx = x + ((8 - i) / 3 - 1) * j;
                    int ty = y + ((8 - i) % 3 - 1) * j;
                    GridScrTransform[tx, ty].chessmanObj.SetChessmanColor(0);
                }
            }
        }
        zongshu ++;
        if (zongshu >4)
        {
            //加分处理
            GridScrTransform[x, y].chessmanObj.SetChessmanColor(0);
            NowChessmanNum -= zongshu;

            GameManager._gameManager.SetScore();
            return true;
        }
        else
        {
            return false;
        }

    }

    //获得网格的三维向量
    Vector3 GetGridVector(GridScr scr)
    {
        Vector3 vec = new Vector3();
        vec.x = scr.Gridx;
        vec.y = scr.Gridy;
        return vec;
    }

    void GridInstant(int row, int col, GameObject parent)
    {
        //实例化网格
        GameObject obj = Instantiate(GridObj);
        obj.transform.parent = parent.transform;
        obj.transform.localPosition = new Vector2(row, col);
        //添加到二维网格
        GridScr scr = obj.GetComponent<GridScr>();
        GridScrTransform[row, col] = scr;

        scr.SetGrid(row, col);
    }

    void ChessmanInstant(int color,GridScr scr)
    {
        //实例化棋子
        GameObject obj = Instantiate(ChessmanObj);

        scr.chessmanObj.SetChessman(color, obj);
        obj.transform.parent = scr.gameObject.transform;
        obj.transform.localPosition = new Vector2(0, 0);
    }

    void NextInstant(int row,GameObject parent)
    {
        GameObject obj = Instantiate(GridController._gridcontroller.NextGrid);
        obj.transform.parent = parent.transform;
        obj.transform.localPosition = new Vector2(row, 0);

        GridScr scr = obj.GetComponent<GridScr>();
        NextGridScrTransform[row] = scr;
    }

}











