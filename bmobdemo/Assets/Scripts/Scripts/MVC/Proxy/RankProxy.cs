using PureMVC.Patterns;
using System.Collections.Generic;
using UnityEngine;

public class RankProxy : Proxy<RankProxy>
{
    public RankProxy()
        : base(ProxyID.Rank)
    {
        KBEngine.Event.registerOut(this, "onGetFightValue");
        KBEngine.Event.registerOut(this, "onGetLevelValue");
        KBEngine.Event.registerOut(this, "onGetMoneyValue");
        KBEngine.Event.registerOut(this, "onGetMySelfRank");
        KBEngine.Event.registerOut(this, "onGetOfficalValue");
        KBEngine.Event.registerOut(this, "onGetBallerValue");
        KBEngine.Event.registerOut(this, "onGetPlayerInfo");
        KBEngine.Event.registerOut(this, "onGetGuildValue");
    }
    public void onGetPlayerInfo(Dictionary<string, object> info)
    {
        PlayerInfo playerInfo = new PlayerInfo();
        playerInfo.roleId = UtilTools.IntParse(info["dbid"].ToString());
        playerInfo.fightValue = UtilTools.IntParse(info["fightValue"].ToString());
        playerInfo.camp = UtilTools.IntParse(info["camp"].ToString());
        playerInfo.level = UtilTools.IntParse(info["level"].ToString());
        playerInfo.vipLevel = UtilTools.IntParse(info["vipLevel"].ToString());
        playerInfo.club = info["club"].ToString();
        playerInfo.name = info["playerName"].ToString();
        playerInfo.guildName = info["guildName"].ToString();
        playerInfo.slogan = info["slogan"].ToString();
        playerInfo.officalPosition = UtilTools.IntParse(info["offical"].ToString());       
        Facade.SendNotification(NotificationID.Role_Show, playerInfo);
    }
    public void onGetFightValue(List<object> list, object obj)
    {
        RankMediator.fightRankList.Clear();
        PlayerInfo playerInfo;
        for (int i = 0; i < list.Count; ++i)
        {
            Dictionary<string, object> info = list[i] as Dictionary<string, object>;
            playerInfo = new PlayerInfo();
            playerInfo.ranking = i + 1;
            playerInfo.roleId = UtilTools.IntParse(info["dbid"].ToString());
            playerInfo.name = info["name"].ToString();
            playerInfo.formation = UtilTools.IntParse(info["camp"].ToString());
            playerInfo.level = UtilTools.IntParse(info["level"].ToString());
            playerInfo.fightValue = UtilTools.IntParse(info["fightValue"].ToString());
            RankMediator.fightRankList.Add(playerInfo.roleId, playerInfo);
        }
        RankMediator.allPage = Mathf.CeilToInt(UtilTools.IntParse(obj.ToString()) * 1.0f / 7);
        if (!RankMediator.firstOpenUI)
        {
            RankMediator.firstOpenUI = true;
            Facade.SendNotification(NotificationID.Rank_Show);
            return;
        }
        RankMediator.rankMediator.UpdateRankList();
    }
    public void onGetLevelValue(List<object> list, object obj)
    {
        RankMediator.levelRankList.Clear();
        PlayerInfo playerInfo;
        for (int i = 0; i < list.Count; ++i)
        {
            Dictionary<string, object> info = list[i] as Dictionary<string, object>;
            playerInfo = new PlayerInfo();
            playerInfo.ranking = i + 1;
            playerInfo.roleId = UtilTools.IntParse(info["dbid"].ToString());
            playerInfo.name = info["name"].ToString();
            playerInfo.formation = UtilTools.IntParse(info["camp"].ToString());
            playerInfo.level = UtilTools.IntParse(info["level"].ToString());
            playerInfo.guildName = info["guildName"].ToString();
            RankMediator.levelRankList.Add(playerInfo.roleId, playerInfo);
        }
        RankMediator.allPage = Mathf.CeilToInt(UtilTools.IntParse(obj.ToString()) * 1.0f / 7);
        RankMediator.rankMediator.UpdateRankList();
    }

    public void onGetMoneyValue(List<object> list, object obj)
    {
        RankMediator.moneyRankList.Clear();
        PlayerInfo playerInfo;
        for (int i = 0; i < list.Count; ++i)
        {
            Dictionary<string, object> info = list[i] as Dictionary<string, object>;
            playerInfo = new PlayerInfo();
            playerInfo.ranking = i + 1;
            playerInfo.roleId = UtilTools.IntParse(info["dbid"].ToString());
            playerInfo.name = info["name"].ToString();
            playerInfo.formation = UtilTools.IntParse(info["camp"].ToString());
            playerInfo.euro = UtilTools.IntParse(info["euro"].ToString());
            playerInfo.guildName = info["guildName"].ToString();
            RankMediator.moneyRankList.Add(playerInfo.roleId, playerInfo);
        }
        RankMediator.allPage = Mathf.CeilToInt(UtilTools.IntParse(obj.ToString()) * 1.0f / 7);
        RankMediator.rankMediator.UpdateRankList();
    }

    public void onGetOfficalValue(List<object> list, object obj)
    {
        RankMediator.officialRankList.Clear();
        PlayerInfo playerInfo;
        for (int i = 0; i < list.Count; ++i)
        {
            Dictionary<string, object> info = list[i] as Dictionary<string, object>;
            playerInfo = new PlayerInfo();
            playerInfo.ranking = i + 1;
            playerInfo.roleId = UtilTools.IntParse(info["dbid"].ToString());
            playerInfo.name = info["name"].ToString();
            playerInfo.formation = UtilTools.IntParse(info["camp"].ToString());
            playerInfo.officalPosition = UtilTools.IntParse(info["officalPosition"].ToString());
            playerInfo.achievements = UtilTools.IntParse(info["achievements"].ToString());
            RankMediator.officialRankList.Add(playerInfo.roleId, playerInfo);
        }
        RankMediator.allPage = Mathf.CeilToInt(UtilTools.IntParse(obj.ToString()) * 1.0f / 7);
        RankMediator.rankMediator.UpdateRankList();
    }

    public void onGetBallerValue(List<object> list, object obj)
    {
        RankMediator.ballerRankList.Clear();
        PlayerInfo playerInfo;
        for (int i = 0; i < list.Count; ++i)
        {
            Dictionary<string, object> info = list[i] as Dictionary<string, object>;
            playerInfo = new PlayerInfo();
            playerInfo.ranking = i + 1;
            playerInfo.roleId = UtilTools.IntParse(info["dbid"].ToString());
            playerInfo.masterName = info["name"].ToString();
            playerInfo.formation = UtilTools.IntParse(info["camp"].ToString());
            playerInfo.level = UtilTools.IntParse(info["level"].ToString());
            playerInfo.cardId = info["cardConfigID"].ToString();
            playerInfo.fightValue = UtilTools.IntParse(info["cardFightValue"].ToString());
            RankMediator.ballerRankList.Add(playerInfo.roleId, playerInfo);
        }
        RankMediator.allPage = Mathf.CeilToInt(UtilTools.IntParse(obj.ToString()) * 1.0f / 7);
        RankMediator.rankMediator.UpdateRankList();
    }

    public void onGetGuildValue(List<object> list, object obj)
    {
        RankMediator.guildRankList.Clear();
        GuildInfo guildInfo;
        for (int i = 0; i < list.Count; ++i)
        {
            Dictionary<string, object> info = list[i] as Dictionary<string, object>;
            guildInfo = new GuildInfo();
            guildInfo.ranking = i + 1;
            guildInfo.id = UtilTools.IntParse(info["dbid"].ToString());
            guildInfo.guildName = info["guildName"].ToString();
            guildInfo.camp = UtilTools.IntParse(info["camp"].ToString());
            guildInfo.level = UtilTools.IntParse(info["level"].ToString());
            guildInfo.leader = info["leader"].ToString();
            guildInfo.reputation = UtilTools.IntParse(info["reputation"].ToString());
            guildInfo.guildFunds = UtilTools.IntParse(info["guildFunds"].ToString());       
            RankMediator.guildRankList.Add(guildInfo.id, guildInfo);
        }
        if (RankMediator.rankMediator!=null)
        {
            RankMediator.allPage = Mathf.CeilToInt(UtilTools.IntParse(obj.ToString()) * 1.0f / 7);
            RankMediator.rankMediator.UpdateRankList();
        }
        if (GuildTacticMediator.guildtacticMediator != null)
            GuildTacticMediator.guildtacticMediator.SetGuildListGrid();
    }
    public void onGetMySelfRank(object obj)
    {
        int currentPage = Mathf.CeilToInt(UtilTools.IntParse(obj.ToString()) * 1.0f / 7);
        RankMediator.rankMediator.FindMySelfRank(currentPage);      
    }
}
