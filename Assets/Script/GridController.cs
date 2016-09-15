using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridController : MonoBehaviour {
    public static GridController _gridcontroller;

    //预览
    public GameObject[] NextGridTransform;
    public GameObject[] NextChessmanTransform;
    public GameObject NextGridObj;
    public GameObject NextParent;

    //最大行列数
    public int MaxRowNum = 9;
    public int MaxColNum = 9;

    //网格相关的
    public GameObject GridObj;
    public Sprite[] GridSpr;
    [HideInInspector]
    public GridScr GridScr;
    public GameObject[,] GridTransform;
    public GridScr[,] GridScrTransform;
    //public List<GameObject> ChessmanTransform;

    public GameObject GridParent;

    public int NewChessmanNum = 3;

    //棋子相关的
    //public GameObject ChessmanObj;
    public GameObject[] ChessmanObjGrop;

    private int GridTotal;                  //总的棋子数（网格数）
    private int NowChessmanNum = 0;         //目前棋子数

    private List<GameObject> pathObj = new List<GameObject>();

    [HideInInspector]
    public GridScr Selected01;
    [HideInInspector]
    public GridScr Selected02;
    [HideInInspector]
    public AStarFindPath asr;
    Route_pt[] result = null;

    private int[] ramNum; 



    void Awake()
    {
        _gridcontroller = this;
    }

    /// <summary>
    /// 新建棋盘
    /// </summary>
    public void NewGridCreate()
	{
		GridTransform = new GameObject[MaxRowNum , MaxColNum];
        GridScrTransform = new GridScr[MaxRowNum, MaxColNum];
        //ChessmanTransform = new List<GameObject>();

        GridTotal = MaxRowNum * MaxColNum;

		for (int i = 0; i < MaxRowNum; i++) 
		{
			for (int j = 0; j < MaxColNum; j++) 
			{
				NewCellCreate (i, j);
			}
		}
        //预览网格创建
        NextGridGreate();
        //预览棋子创建
        NextChessmanCreate();
        //随机位置创建棋子
        DropChessman();
	}

    //新建棋盘网格
    void NewCellCreate(int row, int col)
    {
        //实例化网格
        GameObject obj = Instantiate(GridObj);
        obj.transform.parent = GridParent.transform;
        obj.transform.localPosition = new Vector2(row, col);

        //设置脚本
        GridScr Scr = obj.GetComponent<GridScr>();
        Scr.SetGridObj(row, col, 0);

        //添加到二维网格
        GridTransform[row, col] = obj;
        GridScrTransform[row, col] = Scr;

        //设置国际象棋棋盘效果
        Scr.SetGridSprite((row + col) % 2);
    }

    //预览网格创建
    void NextGridGreate()
    {
        NextGridTransform = new GameObject[NewChessmanNum];
        NextChessmanTransform = new GameObject[NewChessmanNum];

        for (int i = 0; i < NewChessmanNum; i++)
        {
            //实例化预览网格
            GameObject obj = Instantiate(NextGridObj);
            obj.transform.parent = NextParent.transform;
            obj.transform.localPosition = new Vector2(i, 0);

            NextGridTransform[i] = obj;
            //GridScr Scr = obj.GetComponent<GridScr>();
            //Scr.SetGridSprite(i % 2);
            //Scr.SetGridObj(i, 0, 0);
        }
    }

    //预览棋子的创建
    void NextChessmanCreate()
    {
        ramNum = new int[NewChessmanNum];
        for (int i = 0; i < NewChessmanNum; i++)
        {
            //删除当前已有的
            if (NextChessmanTransform[i] != null)
            {
                Destroy(NextChessmanTransform[i]);
            }
            //随机对象
            int ram = Random.Range(0, ChessmanObjGrop.Length - 1);
            ramNum[i] = ram;

            //ChessmanObj = ChessmanObjGrop[ram];
            //实例化棋子
            GameObject obj = Instantiate(ChessmanObjGrop[ram]);
            obj.transform.parent = NextGridTransform[i].transform;
            obj.transform.localPosition = new Vector2(0, 0);
            GridScr Scr = NextGridTransform[i].GetComponent<GridScr>();
            Scr.SetChessmanColor(ram + 1);

            //ChessmanScr ChessmanScr = obj.GetComponent<ChessmanScr>();
            NextChessmanTransform[i] = obj;
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
                if (GridScrTransform[ weizhi % MaxRowNum , weizhi / MaxColNum ].ChessmanColor == 0) //网格中没有棋子
                {
                    //创建新棋子
                    NewChessmanCreate(i, weizhi);
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

    //创建新棋子
    void NewChessmanCreate(int i, int weizhi)
    {
        //创建新棋子根据预览棋子，因此需要获取预览棋子的对象
        //实例化网格，网格位置
        GameObject obj = Instantiate(ChessmanObjGrop[ramNum[i]]);
        obj.transform.parent = GridTransform[ weizhi % MaxRowNum , weizhi / MaxColNum ].transform;
        obj.transform.localPosition = new Vector2(0, 0);

        //脚本设置
        GridScr scr = GridScrTransform[weizhi % MaxRowNum, weizhi / MaxColNum];
        //ChessmanScr Che = chessman.GetComponent<ChessmanScr>();

        //获取棋子颜色
        scr.SetChessmanColor(ramNum[i] + 1);
        //将网格设置为非空
        //ChessmanTransform[weizhi] = obj;
    }

    //移动棋子
    public void ChessmanMoveTo()
    {
        AStarFindPath._astar.FindingPath(Selected01, Selected02);
        Debug.Log(Selected01.Gridx + "," + Selected01.Gridy);
        Debug.Log(Selected02.Gridx + "," + Selected02.Gridy);

        for (int i = 0; i < pathObj.Count - 1; i++)
        {
            //移动物体
            Debug.Log("移动游戏物体");
        }

    }

    //获取周围的网格
    public List<GridScr> getNeibourHood(GridScr scr)
    {
        List<GridScr> list = new List<GridScr>();
        for (int i = -1; i < 1; i++)
        {
            for (int j = -1; j < 1; j++)
            {
                //是自己则跳过
                if (i == 0 && j == 0)
                {
                    continue;
                }
                int x = GridScr.Gridx + i;
                int y = GridScr.Gridy + j;
                //判断是否过界，未过界则记录在列表中
                if (x<MaxRowNum && x >= 0 && y < MaxColNum && y >= 0)
                {
                    list.Add(GridScrTransform[x, y]);
                }
            }
        }
        return list;
    }

    //更新路径
    public void updatePath(List<GridScr> lines)
    {
        int curListSize = pathObj.Count;
        for (int i = 0, max = lines.Count; i < max; i++) 
        {
            if (i < curListSize) 
            {
                pathObj [i].transform.position = lines [i].pos;
                pathObj [i].SetActive (true);
            } 
            else 
            {
                //GameObject obj = GameObject.Instantiate (Node, lines [i].pos, Quaternion.identity) as GameObject;
                //obj.transform.SetParent (PathRange.transform);
                //pathObj.Add (obj);
            }
        }
        for (int i = lines.Count; i < curListSize; i++) {
            pathObj [i].SetActive (false);
        }
    }
}











