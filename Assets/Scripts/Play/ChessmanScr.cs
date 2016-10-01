using UnityEngine;
using System.Collections;

public class ChessmanScr : MonoBehaviour {
    int ChessmanColor;

    /// <summary>
    /// 设置棋子对象
    /// </summary>
    /// <param name="type"></param>
    public void SetChessman(int color, GameObject obj)
    {
        ChessmanColor = color;
        SetChessmanSprite(color - 1);
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
    /// 设置颜色
    /// </summary>
    /// <param name="color"></param>
    public void SetChessmanColor(int color)
    {
        if (color == 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            ChessmanColor = color;
            SetChessmanSprite(color - 1);
        }
    }

    //设置图片
    void SetChessmanSprite(int spr)
    {
        Sprite _sprite = GridController._gridcontroller.ChessmanSpr[spr];
        this.GetComponent<SpriteRenderer>().sprite = _sprite;
    }
}
