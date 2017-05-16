using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;

public enum RankType
{
    Fight,
    Official,
    Level,
    Money,
    Guild,
    Baller,
    My,
}
public class rankpanel : BasePanel
{
    public UISprite backBtn;
    public UISprite closeBtn;
    public UITexture beijing;
    public UISprite leftBtn;
    public UISprite rightBtn;
    public UISprite selfBtn;
    public UISprite myBtn;
    public UISprite myArenaBtn;
    public UISprite arenaRightBtn;
    public UISprite arenaLeftBtn;
    public UIToggle fightBtn;
    public UIToggle officerBtn;
    public UIToggle levelBtn;
    public UIToggle moneyBtn;
    public UIToggle guildBtn;
    public UIToggle ballerBtn;
    public UIGrid rankGrid;
    public UIGrid arenaGrid;
    public Transform tab;
    public Transform otherTab;
    public Transform fightTitle;
    public Transform officailTitle; 
    public Transform moneyTitle;
    public Transform levelTitle;
    public Transform ballerTitle;
    public Transform guildTitle;
    public Transform arenaRank;
    public UILabel pageLabel;
    public UILabel arenaPage;
}
public class RankMediator : UIMediator<rankpanel>
{
    public rankpanel panel
    {
        get
        {
            return m_Panel as rankpanel;
        }
    }
    private PlayerInfo choosePlayerInfo;
    private GuildInfo guildInfo;
    private ArenaInfo arenaInfo;
    private int currentPage = 1;
    public static bool firstOpenUI = false;
    private RankType rankType;
    public static int allPage = 0;
    public static int arenaPage = 0;
    private bool isTab = true;
    public static RankMediator rankMediator;
    public static Dictionary<int, PlayerInfo> fightRankList = new Dictionary<int, PlayerInfo>();
    public static Dictionary<int, PlayerInfo> levelRankList = new Dictionary<int, PlayerInfo>();
    public static Dictionary<int, PlayerInfo> moneyRankList = new Dictionary<int, PlayerInfo>();
    public static Dictionary<int, PlayerInfo> officialRankList = new Dictionary<int, PlayerInfo>();
    public static Dictionary<int, PlayerInfo> ballerRankList = new Dictionary<int, PlayerInfo>();
    public static Dictionary<int, GuildInfo> guildRankList = new Dictionary<int, GuildInfo>();
    public static Dictionary<int, ArenaInfo> ArenaRankList = new Dictionary<int, ArenaInfo>();
    public RankMediator() : base("rankpanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.Rank_Show, OpenPanel);
        RegistPanelCall(NotificationID.Rank_Hide, ClosePanel);
    }
    /// <summary>
    /// 界面显示前调用
    /// </summary>
    protected override void OnStart(INotification notification)
    {
        if (rankMediator == null)
        {
            rankMediator = Facade.RetrieveMediator("RankMediator") as RankMediator;
        }
        isTab = true;
        rankType = RankType.Fight;
        currentPage = 1;
        panel.rankGrid.enabled = true;
        panel.rankGrid.BindCustomCallBack(UpdateRankGrid);
        panel.rankGrid.StartCustom();
        panel.arenaGrid.enabled = true;
        panel.arenaGrid.BindCustomCallBack(UpdateArenaRankGrid);
        panel.arenaGrid.StartCustom();
    }
    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        if(notification.Body!=null)
        {
            arenaInfo = null;
            List<object> listObj = notification.Body as List<object>;
            panel.arenaRank.gameObject.SetActive(true);
            panel.arenaGrid.AddCustomDataList(listObj);
            string text = string.Format("[00FF00]{0}[-]", currentPage);
            panel.arenaPage.text = text + "/" + arenaPage.ToString();
            return;
        }
        LoadSprite.LoaderBGTexture(panel.beijing.material, "qiuchang", false);
        UpdateRankList();      
    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.backBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.leftBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.rightBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.selfBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.fightBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.officerBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.levelBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.moneyBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.guildBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.ballerBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.myBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.myArenaBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.closeBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.arenaLeftBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.arenaRightBtn.gameObject).onClick = OnClick;
    }
    void OnClick(GameObject go)
    {
        if (go == panel.backBtn.gameObject || go == panel.closeBtn.gameObject)
        {
            ClosePanel(null);
        }
        else if (go == panel.leftBtn.gameObject)
        {
            if (currentPage == 1)
                return;
            currentPage--;
            GetRankList();
        }
        else if (go == panel.rightBtn.gameObject)
        {
            if (currentPage == allPage)
                return;
            currentPage++;
            GetRankList();
        }
        else if (go == panel.selfBtn.gameObject)
        {
            if (isTab)
            {
                TweenPosition.Begin(panel.tab.gameObject, 0.1f, new Vector3(0, 231, 0));
                TweenPosition.Begin(panel.otherTab.gameObject, 0.1f, new Vector3(0, 262, 0));
                isTab = false;
            }
            else
            {
                TweenPosition.Begin(panel.otherTab.gameObject, 0.1f, Vector3.zero);
                TweenPosition.Begin(panel.tab.gameObject, 0.1f, new Vector3(0, -15, 0));
                isTab = true;
            }
        }
        else if (go == panel.fightBtn.gameObject)
        {
            if (rankType == RankType.Fight)
                return;
            rankType = RankType.Fight;
            currentPage = 1;
            GetRankList();
        }
        else if (go == panel.levelBtn.gameObject)
        {
            if (rankType == RankType.Level)
                return;
            rankType = RankType.Level;
            currentPage = 1;
            GetRankList();
        }
        else if (go == panel.moneyBtn.gameObject)
        {
            if (rankType == RankType.Money)
                return;
            rankType = RankType.Money;
            currentPage = 1;
            GetRankList();
        }
        else if (go == panel.officerBtn.gameObject)
        {
            if (rankType == RankType.Official)
                return;
            rankType = RankType.Official;
            currentPage = 1;
            GetRankList();
        }
        else if (go == panel.ballerBtn.gameObject)
        {
            if (rankType == RankType.Baller)
                return;
            rankType = RankType.Baller;
            currentPage = 1;
            GetRankList();
        }
        else if (go == panel.guildBtn.gameObject)
        {
            if (rankType == RankType.Guild)
                return;
            rankType = RankType.Guild;
            currentPage = 1;
            GetRankList();
        }
        else if (go == panel.myBtn.gameObject)
        {
            if (CheckSelfInRankList(PlayerMediator.playerInfo.roleId))
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_rank_1"));
                return;
            }
            ServerCustom.instance.SendClientMethods("onClientGetMySelfValueRank", PlayerMediator.playerInfo.roleId, "fightValue");
        }

        else if (go == panel.myArenaBtn.gameObject)
        {           
            if (ArenaRankList.ContainsKey(PlayerMediator.playerInfo.roleId) && PlayerMediator.playerInfo.name == ArenaRankList[PlayerMediator.playerInfo.roleId].playerName)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_rank_1"));
                return;
            }
            if (PlayerMediator.playerInfo.myRank > 100)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_rank_4"));
                return;
            }             
            int page = Mathf.CeilToInt(PlayerMediator.playerInfo.myRank * 1.0f / 6);       
            currentPage = page;
            ServerCustom.instance.SendClientMethods("onClientGetArenaRank", page - 1);
        }
        else if (go == panel.arenaLeftBtn.gameObject)
        {
            if (currentPage == 1)
                return;
            currentPage--;
            ServerCustom.instance.SendClientMethods("onClientGetArenaRank", currentPage - 1);
        }
        else if (go == panel.arenaRightBtn.gameObject)
        {
            if (currentPage == 17)
                return;
            currentPage++;
            ServerCustom.instance.SendClientMethods("onClientGetArenaRank", currentPage - 1);
        }
    }
    void ChangeRankTitle()
    {
        panel.fightTitle.gameObject.SetActive(rankType == RankType.Fight);
        panel.officailTitle.gameObject.SetActive(rankType == RankType.Official);
        panel.levelTitle.gameObject.SetActive(rankType == RankType.Level);
        panel.ballerTitle.gameObject.SetActive(rankType == RankType.Baller);
        panel.moneyTitle.gameObject.SetActive(rankType == RankType.Money);
        panel.guildTitle.gameObject.SetActive(rankType == RankType.Guild);
        panel.myBtn.gameObject.SetActive(rankType != RankType.Baller && rankType != RankType.Guild);     
    }
    void GetRankList()
    {
        switch (rankType)
        {
            case RankType.Fight:
                ServerCustom.instance.SendClientMethods("onClientGetFightValueRank", currentPage - 1);
                break;
            case RankType.Level:
                ServerCustom.instance.SendClientMethods("onClientGetLevelValueRank", currentPage - 1);
                break;
            case RankType.Money:
                ServerCustom.instance.SendClientMethods("onClientGetMoneyValueRank", currentPage - 1);
                break;
            case RankType.Official:
                ServerCustom.instance.SendClientMethods("onClientGetOfficialValueRank", currentPage - 1);
                break;
            case RankType.Baller:
                ServerCustom.instance.SendClientMethods("onClientGetBallerValueRank", currentPage - 1);
                break;
            case RankType.Guild:
                ServerCustom.instance.SendClientMethods("onClientGetGuildValueRank", currentPage - 1);
                break;
        }
        ChangeRankTitle();     
    }

    public void GetArenaRank(List<object> listObj)
    {
        arenaInfo = null;
        panel.arenaGrid.AddCustomDataList(listObj);
        string text = string.Format("[00FF00]{0}[-]", currentPage);
        panel.arenaPage.text = text + "/" + arenaPage.ToString();
    }
    bool CheckSelfInRankList(int dbid)
    {
        switch (rankType)
        {
            case RankType.Fight:
                if (fightRankList.ContainsKey(dbid))
                    return true;
                return false;
            case RankType.Level:
                if (levelRankList.ContainsKey(dbid))
                    return true;
                return false;
            case RankType.Money:
                if (moneyRankList.ContainsKey(dbid))
                    return true;
                return false;
            case RankType.Official:
                if (officialRankList.ContainsKey(dbid))
                    return true;
                return false;
            case RankType.Baller:
                if (ballerRankList.ContainsKey(dbid))
                    return true;
                return false;           
        }
        return false;
    }
    public void UpdateRankList()
    {
        List<object> list = new List<object>();
        switch (rankType)
        {
            case RankType.Fight:
                foreach (PlayerInfo item in fightRankList.Values)
                    list.Add(item);                             
                break;
            case RankType.Level:
                foreach (PlayerInfo item in levelRankList.Values)
                    list.Add(item);              
                break;
            case RankType.Money:
                foreach (PlayerInfo item in moneyRankList.Values)
                    list.Add(item);
                break;
            case RankType.Official:
                foreach (PlayerInfo item in officialRankList.Values)
                    list.Add(item);
                break;
            case RankType.Baller:
                foreach (PlayerInfo item in ballerRankList.Values)
                    list.Add(item);
                break;
            case RankType.Guild:
                foreach (GuildInfo item in guildRankList.Values)
                    list.Add(item);
                break;
        }
        guildInfo = null;
        choosePlayerInfo = null;
        panel.rankGrid.ClearCustomGrid();
        panel.rankGrid.AddCustomDataList(list);
        string text = string.Format("[00FF00]{0}[-]", currentPage);
        panel.pageLabel.text = text + "/" + allPage.ToString();
    }
    public void FindMySelfRank(int page)
    {
        currentPage = page;
        string text = string.Format("[00FF00]{0}[-]", currentPage);
        panel.pageLabel.text = text + "/" + allPage.ToString();
    }

    public void UpdateArenaRankGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        ArenaInfo info = item.oData as ArenaInfo;
        UILabel ranking = item.mScripts[0] as UILabel;
        UITexture head = item.mScripts[1] as UITexture;
        UILabel name = item.mScripts[2] as UILabel;
        UILabel formation = item.mScripts[3] as UILabel;
        UILabel fight = item.mScripts[4] as UILabel;
        UISprite select = item.mScripts[5] as UISprite;
        UITexture clubHead = item.mScripts[6] as UITexture;
        UILabel clubName = item.mScripts[7] as UILabel;
        UILabel dimaondLabel = item.mScripts[8] as UILabel;
        UILabel blackLabel = item.mScripts[9] as UILabel;
        UISprite rankSprite = item.mScripts[10] as UISprite;
        item.onClick = OnClickArenaItem;
        LoadSprite.LoaderHead(head, "jueshetouxiang1", false);
        LoadSprite.LoaderHead(clubHead, "jueshetouxiang1", false);
        select.gameObject.SetActive(info.dbid == PlayerMediator.playerInfo.roleId && info.playerName == PlayerMediator.playerInfo.name);
        if (select.gameObject.activeSelf)
            arenaInfo = info;
        clubName.text = info.club;
        rankSprite.gameObject.SetActive(info.ranking <= 3);
        ranking.gameObject.SetActive(info.ranking > 3);
        if (info.ranking <= 3)
            rankSprite.spriteName = info.ranking.ToString();
        else
            ranking.text = info.ranking.ToString();
        name.text = info.playerName;
        formation.text = info.formation.ToString();
        fight.text = info.fightValue.ToString();
        ArenaReward reward = ArenaConfig.GetArenaRewardByRank(info.ranking);
        if (reward == null)
        {
            dimaondLabel.text = "0";
            blackLabel.text = "0";
        }
        else
        {
            dimaondLabel.text = reward.arenaReward.Split(';')[1].ToString();
            blackLabel.text = reward.arenaReward.Split(';')[0].ToString();
        }
    }
    public void UpdateRankGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        PlayerInfo info = item.oData as PlayerInfo;
        UISprite rankSprite = item.mScripts[0] as UISprite;
        UILabel ranking = item.mScripts[1] as UILabel;
        UITexture head = item.mScripts[2] as UITexture;
        UILabel name = item.mScripts[3] as UILabel;
        UILabel formation = item.mScripts[4] as UILabel;
        UILabel level = item.mScripts[5] as UILabel;
        UILabel fightValue = item.mScripts[6] as UILabel;
        UISprite select = item.mScripts[7] as UISprite;
        UILabel master = item.mScripts[8] as UILabel;
        UILabel ballerLevel = item.mScripts[9] as UILabel;
        UILabel ballerFight = item.mScripts[10] as UILabel;
        UILabel guildName = item.mScripts[11] as UILabel;
        UILabel guildLevel = item.mScripts[12] as UILabel;
        UILabel reputation = item.mScripts[13] as UILabel;
        LoadSprite.LoaderHead(head, "jueshetouxiang1", false);
        if (rankType == RankType.Guild)
            item.onClick = OnClickGuildItem;
        else
            item.onClick = OnClickItem;
        fightValue.mIntConvert = rankType == RankType.Money;
        fightValue.gameObject.SetActive(rankType != RankType.Baller && rankType != RankType.Guild);
        level.gameObject.SetActive(rankType != RankType.Baller && rankType != RankType.Guild);
        master.gameObject.SetActive(rankType == RankType.Baller);
        ballerFight.gameObject.SetActive(rankType == RankType.Baller);
        ballerLevel.gameObject.SetActive(rankType == RankType.Baller);
        guildName.gameObject.SetActive(info == null);
        guildLevel.gameObject.SetActive(info == null);
        reputation.gameObject.SetActive(info == null);
        if (info == null)
        {
            GuildInfo guildInfos = item.oData as GuildInfo;
            if (select.gameObject.activeSelf)
                guildInfo = guildInfos;
            guildInfos.ranking += (currentPage - 1) * 7;
            rankSprite.gameObject.SetActive(guildInfos.ranking <= 3);
            ranking.gameObject.SetActive(guildInfos.ranking > 3);
            if (guildInfos.ranking <= 3)
                rankSprite.spriteName = guildInfos.ranking.ToString();
            else
                ranking.text = guildInfos.ranking.ToString();
            name.text = guildInfos.guildName;
            formation.text = guildInfos.camp.ToString();
            guildName.text = guildInfos.leader;
            guildLevel.text = guildInfos.level.ToString();
            reputation.text = guildInfos.reputation.ToString();
            return;
        }
        select.gameObject.SetActive(info.roleId == PlayerMediator.playerInfo.roleId);
        if (select.gameObject.activeSelf)
            choosePlayerInfo = info;
        info.ranking += (currentPage - 1) * 7;
        rankSprite.gameObject.SetActive(info.ranking <= 3);
        ranking.gameObject.SetActive(info.ranking > 3);
        if (info.ranking <= 3)
            rankSprite.spriteName = info.ranking.ToString();
        else
            ranking.text = info.ranking.ToString();
        if (rankType == RankType.Fight)
        {
            name.text = info.name;
            level.text = info.level.ToString();
            formation.text = info.formation.ToString();
            fightValue.text = info.fightValue.ToString();
        }
        else if (rankType == RankType.Level)
        {
            name.text = info.name;
            formation.text = info.formation.ToString();
            fightValue.text = info.level.ToString();
            if (info.guildName == string.Empty)
                level.text = TextManager.GetUIString("UIRank11");
            else
                level.text = info.guildName;
        }

        else if (rankType == RankType.Money)
        {
            name.text = info.name;
            formation.text = info.formation.ToString();
            fightValue.text = info.euro.ToString();
            if (info.guildName == string.Empty)
                level.text = TextManager.GetUIString("UIRank11");
            else
                level.text = info.guildName;
        }
        else if (rankType == RankType.Official)
        {
            name.text = info.name;
            formation.text = info.formation.ToString();
            fightValue.text = info.achievements.ToString();
            level.text = info.officalPosition.ToString();
        }
        else if (rankType == RankType.Baller)
        {
            name.text = TextManager.GetItemString(info.cardId);
            formation.text = info.formation.ToString();
            master.text = info.masterName.ToString();
            ballerLevel.text = info.level.ToString();
            ballerFight.text = info.fightValue.ToString();
        }
    }
    void OnClickItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        if (choosePlayerInfo != null)
            panel.rankGrid.GetCustomGridItem(choosePlayerInfo).mScripts[7].gameObject.SetActive(false);
        item.mScripts[7].gameObject.SetActive(true);
        choosePlayerInfo = item.oData as PlayerInfo;
        if (choosePlayerInfo == null)
            return;
        if (choosePlayerInfo.roleId == PlayerMediator.playerInfo.roleId)
            Facade.SendNotification(NotificationID.Role_Show);
        else
            ServerCustom.instance.SendClientMethods("onClientGetPlayerInfo", choosePlayerInfo.roleId);
    }
    void OnClickGuildItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        if (guildInfo != null)
            panel.rankGrid.GetCustomGridItem(guildInfo).mScripts[7].gameObject.SetActive(false);
        item.mScripts[7].gameObject.SetActive(true);
        guildInfo = item.oData as GuildInfo;
        if (guildInfo == null)
            return;
      
    }

    void OnClickArenaItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        if (arenaInfo != null)
            panel.arenaGrid.GetCustomGridItem(arenaInfo).mScripts[5].gameObject.SetActive(false);
        item.mScripts[5].gameObject.SetActive(true);
        arenaInfo = item.oData as ArenaInfo;
        if (arenaInfo == null)
            return;

        if (arenaInfo.playerName == PlayerMediator.playerInfo.name)
        {
            Facade.SendNotification(NotificationID.Role_Show);
            return;
        }
        ServerCustom.instance.SendClientMethods("onClientGetArenaPlayerInfo", arenaInfo.ranking);
    }
    /// <summary>
    /// 释放
    /// </summary>
    protected override void OnDestroy()
    {
        arenaInfo = null;
        rankMediator = null;
        firstOpenUI = false;
        base.OnDestroy();
    }
}
