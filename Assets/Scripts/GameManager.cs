using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	public static GameManager _gameManager;

	void Awake(){
		_gameManager = this;
	}

	// Use this for initialization
	void Start () {
        NewGame();
	}
	
	// Update is called once per frame
	void Update () {

	}

    //开始新游戏
    public void NewGame()
    {
        //新建棋盘
        GridController._gridcontroller.NewGridCreate();

    }

    //储存游戏
    public void SaveGame()
    {

    }

    //载入游戏
    public void LoadGame()
    {

    }

    //结束游戏

    public void GameOver()
    {
        Debug.Log("游戏结束");
    }
}
