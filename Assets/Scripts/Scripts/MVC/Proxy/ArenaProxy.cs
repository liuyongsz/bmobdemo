using PureMVC.Patterns;
using System;
using System.Collections.Generic;
using UnityEngine;


public class ArenaProxy : Proxy<ArenaProxy>
{
    public ArenaProxy()
        : base(ProxyID.Arena)
    {
        KBEngine.Event.registerOut(this, "onGetArenaRankValue"); 
        KBEngine.Event.registerOut(this, "onGetThreeArenaValue");
        KBEngine.Event.registerOut(this, "onArenaCallBack");
        KBEngine.Event.registerOut(this, "onGetRecord");
        KBEngine.Event.registerOut(this, "onGetUpdateCD");
    }
    public void onGetUpdateCD(object v)
    {
        ArenaMediator.cdTime = int.Parse(v.ToString());
        if (ArenaMediator.arenaMediator == null)
            return;
        ArenaMediator.arenaMediator.UpdateArenacd();
    }
    public void onGetThreeArenaValue(List<object> list)
    {
        ArenaMediator.arenaInfoList.Clear();
        ArenaInfo arenaInfo;
        for (int i = 0; i < list.Count; ++i)
        {
            Dictionary<string, object> info = list[i] as Dictionary<string, object>;
            arenaInfo = new ArenaInfo();
            arenaInfo.dbid = UtilTools.IntParse(info["dbid"].ToString());
            arenaInfo.ranking = UtilTools.IntParse(info["rank"].ToString());
            arenaInfo.fightValue = UtilTools.IntParse(info["fightValue"].ToString());
            arenaInfo.formation = UtilTools.IntParse(info["formation"].ToString());
            arenaInfo.camp = UtilTools.IntParse(info["camp"].ToString());
            arenaInfo.playerName = info["name"].ToString();
            arenaInfo.club = info["club"].ToString();
            ArenaMediator.arenaInfoList.Add(arenaInfo.dbid, arenaInfo);
        }       
        if (ArenaMediator.arenaMediator == null)
            return;
        ArenaMediator.arenaMediator.GetArenaRank();
    }
    public void onGetArenaRankValue(List<object> list, object obj)
    {
        List<object> listObj = new List<object>();
        RankMediator.ArenaRankList.Clear();
        ArenaInfo arenaInfo;
        for (int i = 0; i < list.Count; ++i)
        {
            Dictionary<string, object> info = list[i] as Dictionary<string, object>;
            arenaInfo = new ArenaInfo();
            arenaInfo.dbid = UtilTools.IntParse(info["dbid"].ToString());
            arenaInfo.ranking = UtilTools.IntParse(info["rank"].ToString());
            if (arenaInfo.ranking > 100)
                break;
            arenaInfo.fightValue = UtilTools.IntParse(info["fightValue"].ToString());
            arenaInfo.formation = UtilTools.IntParse(info["formation"].ToString());
            arenaInfo.camp = UtilTools.IntParse(info["camp"].ToString());
            arenaInfo.playerName = info["name"].ToString();
            arenaInfo.club = info["club"].ToString();
            RankMediator.ArenaRankList.Add(arenaInfo.dbid, arenaInfo);
            listObj.Add(arenaInfo);
        }
        RankMediator.arenaPage = 17;
        if (RankMediator.rankMediator != null)
        {
            RankMediator.rankMediator.GetArenaRank(listObj);
            return;
        }         
        Facade.SendNotification(NotificationID.Rank_Show, listObj);
    }
    public void onGetRecord(List<object> list)
    {
        if (ArenaMediator.arenaMediator != null)
            ArenaMediator.arenaMediator.GetMyRecord(list);
    }
    public void onArenaCallBack(object obj)
    {
        int index = UtilTools.IntParse(obj.ToString());
        if (index == 1)
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_rank_3"));
        else
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_shop_2"));
    }
}

public class ArenaInfo
{
    public int dbid;
    public int ranking;
    public string club;
    public string playerName;
    public int fightValue;
    public int formation;
    public int camp;
}
