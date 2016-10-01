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

    public ChessmanScr chessmanObj;
    public GridScr parent;

    //记录坐标
    public Vector3 pos;

    //设置网格对象
    public void SetGrid(int row, int col)
    {
        Gridx = row;
        Gridy = col;
        pos.x = row;
        pos.y = col;

        BianHao = row + col * GridController._gridcontroller.MaxColNum;
        this.gameObject.name = "Gird" + BianHao;

        SetGridSprite((row + col) % 2);
    }

    //处理鼠标点击时间
	void OnMouseDown()
	{
        if (this.chessmanObj != null)
        {
            GridController._gridcontroller.Selected01 = this;
        }
        if (this.chessmanObj == null &&
            GridController._gridcontroller.Selected01 != null)
        {
            GridController._gridcontroller.Selected02 = this;
            if (GridController._gridcontroller.Selected01 != null &&
                GridController._gridcontroller.Selected02 != null)
            {
                GridController._gridcontroller.ChessmanMoveTo();
            }
        }
	}

    public void RemoveChessman()
    {
        this.chessmanObj = null;
    }

    public void changeParentGrid(GridScr scr)
    {
        this.chessmanObj = scr.chessmanObj;
        chessmanObj.transform.parent = this.transform;
    }

	//设置图片对象
	void SetGridSprite(int spr)
	{
        Sprite sprite = GridController._gridcontroller.GridSpr[spr];
		this.GetComponent<SpriteRenderer> ().sprite = sprite;
	}

}
