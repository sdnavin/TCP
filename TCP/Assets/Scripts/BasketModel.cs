using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasketModel  {

    public GamePlayerData[] Gamedatas;

    public BasketModel(int gamenos)
    {
        Gamedatas = new GamePlayerData[gamenos];
    }
}


[System.Serializable]
public class PlayerData
{
    public string PlayerID;
    public string PlayerName;
    public Sprite PlayerDisplayPic;

}

[System.Serializable]
public class PillData
{
    public string PillName;
    public int PillCount;
    public Color[] PillColor;
}

[System.Serializable]
public class GamePlayerData
{
    public PlayerData[] PlayersData;
    public PillData[] AllPills;
}
