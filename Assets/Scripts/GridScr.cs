using UnityEngine;
using System.Collections;


/// <summary>
/// 挂在Grid游戏对象上，用来记录信息
/// </summary>
public class GridScr : MonoBehaviour {
	//网格信息
	public int Gridx;
	public int Gridy;
	public int BianHao;

    //用来寻路
    public int gCost;
    public int hCost;
    public int fCost
    {
        get { return gCost + hCost; }
    }

    public GameObject chessmanObj;

    public GridScr parent;

    //记录颜色
	public int ChessmanColor;                     //0代表空
    //记录坐标
    public Vector3 pos;
    /// <summary>
    /// 设置颜色
    /// </summary>
    /// <param name="type"></param>
	public void SetChessmanColor(int type)
    {
		ChessmanColor = type;
	}

    /// <summary>
    /// 获取颜色
    /// </summary>
    /// <returns></returns>
    public int GetChessmanColor()
    {
        return ChessmanColor;
    }

    /// <summary>
    /// 设置Grid对象，伪构造函数
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="type"></param>
	public void SetGridObj(int x,int y,int type)
	{
		Gridx = x;
		Gridy = y;
        pos.x = Gridx;
        pos.y = Gridy;
		ChessmanColor = type;

		BianHao = Gridx + Gridy * GridController._gridcontroller.MaxColNum;
		this.gameObject.name = "Gird" + BianHao;
	}

    //处理鼠标点击时间
	void OnMouseDown()
	{
        if (this.ChessmanColor > 0)
        {
            GridController._gridcontroller.Selected01 = this;
        }
        if (this.ChessmanColor == 0 &&
            GridController._gridcontroller.Selected01 != null)
        {
            GridController._gridcontroller.Selected02 = this;
            GridController._gridcontroller.ChessmanMoveTo();
        }
	}

	//设置图片对象
	public void SetGridSprite(int spr)
	{
        Sprite sprite = GridController._gridcontroller.GridSpr[spr];
		this.GetComponent<SpriteRenderer> ().sprite = sprite;
	}

}
