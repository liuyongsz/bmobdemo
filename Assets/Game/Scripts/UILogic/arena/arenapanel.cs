using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;
using System;


public class arenapanel : BasePanel
{
    public UIGrid arenaGrid;
    public UIGrid recordGrid;
    public UILabel myRank;
    public UILabel myfight;
    public UILabel arenaMoney;
    public UILabel dimaondLabel;
    public UILabel blackLabel;
    public UILabel ArenaBtnLabel;
    public UILabel todayLastTimes;
    public UISprite updateArenaBtn;
    public UISprite rankBtn;
    public UISprite backBtn;
    public UISprite changeFormationBtn;
    public UISprite swtichShopBtn;
    public UISprite findMyRecordBtn;
    public UISprite buyBtn;
    public UISprite helpBtn;
    public UISprite closeBtn;
    public Transform record;
}

public class ArenaMediator : UIMediator<arenapanel>
{
    public arenapanel panel
    {
        get
        {
            return m_Panel as arenapanel;
        }
    }
    public static ArenaInfo arenaInfo;
    public static ArenaMediator arenaMediator;
    private bool isUpdate = true;
    public static int cdTime = 0;
    public static Dictionary<int, ArenaInfo> arenaInfoList = new Dictionary<int, ArenaInfo>();
    public ArenaMediator() : base("arenapanel")
    {
        m_isprop = false;
        RegistPanelCall(NotificationID.Arena_Show, OpenPanel);
        RegistPanelCall(NotificationID.Arena_Hide, ClosePanel);
    }

    /// <summary>
    /// 界面显示前调用
    /// </summary>
    protected override void OnStart(INotification notification)
    {
        if (arenaMediator == null)
            arenaMediator = Facade.RetrieveMediator("ArenaMediator") as ArenaMediator;
        panel.recordGrid.enabled = true;
        panel.recordGrid.BindCustomCallBack(UpdateMyRecordGrid);
        panel.recordGrid.StartCustom();
        panel.arenaGrid.enabled = true;
        panel.arenaGrid.BindCustomCallBack(UpdateArenaRankGrid);
        panel.arenaGrid.StartCustom();

    }
    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        ServerCustom.instance.SendClientMethods("onClientGetUpdateCD");    
        GetArenaRank();
        UpdateMyInfo(ArenaConfig.GetArenaRewardByRank(PlayerMediator.playerInfo.myRank));      
    }

    void UpdateMyInfo(ArenaReward rewardInfo)
    {
        if (rewardInfo == null)
        {
            panel.dimaondLabel.text = "0";
            panel.blackLabel.text = "0";
        }
        else
        {
            string[] reward = rewardInfo.arenaReward.Split(';');
            panel.blackLabel.text = reward[0].ToString();
            panel.dimaondLabel.text = reward[1].ToString();
        }
        panel.buyBtn.gameObject.SetActive(PlayerMediator.playerInfo.arenaTimes == 0);
        panel.myRank.text = PlayerMediator.playerInfo.myRank.ToString();
        panel.myfight.text = PlayerMediator.playerInfo.fightValue.ToString();
        panel.arenaMoney.text = PlayerMediator.playerInfo.blackMoney.ToString();
    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.updateArenaBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.rankBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.backBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.changeFormationBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.swtichShopBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.buyBtn.gameObject).onClick = OnClick; 
        UIEventListener.Get(panel.helpBtn.gameObject).onClick = OnClick; 
        UIEventListener.Get(panel.closeBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.findMyRecordBtn.gameObject).onClick = OnClick;
    }
    void OnClick(GameObject go)
    {
        if (go == panel.updateArenaBtn.gameObject)
        {
            if (!isUpdate)
                return;
            ServerCustom.instance.SendClientMethods("onClientUpdateArenaRank");
            isUpdate = false;          
        }
        else if (go == panel.rankBtn.gameObject)
        {
            ServerCustom.instance.SendClientMethods("onClientGetArenaRank", 0);
        }
        else if (go == panel.changeFormationBtn.gameObject)
        {
            Facade.SendNotification(NotificationID.TeamFormine_Show);
        }
        else if (go == panel.swtichShopBtn.gameObject)
        {
            Facade.SendNotification(NotificationID.GameShop_Show, 1);
        }
        else if (go == panel.buyBtn.gameObject)
        {
            GUIManager.SetPromptInfoChoose(TextManager.GetUIString("UIArena2"),TextManager.GetUIString("UIArena1"), BuyArenaTimes);
        }
        else if (go.name == "pkBtn")
        {
            if (PlayerMediator.playerInfo.arenaTimes == 0)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_rank_2"));
                return;
            }
            UIGridItem item = NGUITools.FindInParents<UIGridItem>(go);
            if (item == null || item.mScripts == null || item.oData == null)
                return;
            arenaInfo = item.oData as ArenaInfo;
            GUIManager.SetPlayersFormation(arenaInfo);
        }
        else if (go == panel.helpBtn.gameObject)
        {
            List<object> decsList = new List<object>();
            for (int i = 10012; i <= 10015; ++i)
            {
                string Name = "state" + i.ToString();
                string text = TextManager.GetPropsString(Name);
                decsList.Add(text);
            }
            GUIManager.ShowHelpPanel(decsList);
        }
        else if (go == panel.findMyRecordBtn.gameObject)
        {
            ServerCustom.instance.SendClientMethods("onClientGetRecord");         
        }
        else if (go == panel.backBtn.gameObject)
        {
            ClosePanel(null);
        }
        else if (go == panel.closeBtn.gameObject)
        {
            panel.recordGrid.ClearCustomGrid();
            panel.record.gameObject.SetActive(false);
        }
    }
    public void GetMyRecord(List<object> list)
    {
        if (list.Count < 1)
        {
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_arena_1"));
            return;
        }
        List<object> listObj = new List<object>();
        for (int i = 0; i < list.Count; ++i)
        {
            Dictionary<string, object> info = list[i] as Dictionary<string, object>;
            string text = UtilTools.StringBuilder(i, ";", info["type"].ToString(), ";", info["isWin"].ToString(), ";", info["enemyName"].ToString());
            listObj.Add(text);
        }
        panel.record.gameObject.SetActive(true);
        panel.recordGrid.AddCustomDataList(listObj);
    }
    void UpdateMyRecordGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        string text = item.oData as string;
        UILabel record = item.mScripts[0] as UILabel;
        UILabel desc = item.mScripts[1] as UILabel;
        record.text = text.Split(';')[1];
        desc.text = "测试" + string.Format(TextManager.GetUIString("UIArena3"), text.Split(';')[3]);
    }
    public void BuyArenaTimesSucess()
    {
        panel.buyBtn.gameObject.SetActive(PlayerMediator.playerInfo.arenaTimes == 0);
        panel.todayLastTimes.text = PlayerMediator.playerInfo.arenaTimes.ToString();
    }
    void BuyArenaTimes()
    {
        ServerCustom.instance.SendClientMethods("onClientBuyArenaTimes");      
    }

    public void UpdateArenacd()
    {
        if (cdTime < 10)
        {
            panel.updateArenaBtn.color = Color.black;
            panel.ArenaBtnLabel.text = (10 - cdTime).ToString();
        }
        TimerManager.AddTimerRepeat("isUpdate", 1, CanChangeArena);
    }
    void CanChangeArena()
    {
        cdTime++;
        if (cdTime == 10)
        {         
            isUpdate = true;
            TimerManager.Destroy("isUpdate");
            if (arenaMediator == null)
                return;
            panel.updateArenaBtn.color = Color.white;
            panel.ArenaBtnLabel.text = TextManager.GetUIString("UIRank16");
            return;
        }
        panel.ArenaBtnLabel.text = (10 - cdTime).ToString();
    }
    void UpdateArenaRankGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        ArenaInfo info = item.oData as ArenaInfo;
        UITexture icon = item.mScripts[0] as UITexture;
        UILabel name= item.mScripts[1] as UILabel;
        UILabel rank = item.mScripts[2] as UILabel;
        UILabel fightValue = item.mScripts[3] as UILabel;
        UISprite pkBtn = item.mScripts[4] as UISprite;
        UIEventListener.Get(pkBtn.gameObject).onClick = OnClick;
     
        name.text = info.playerName;
        rank.text = info.ranking.ToString();
        fightValue.text = info.fightValue.ToString();
    }
    public void GetArenaRank()
    {
        panel.todayLastTimes.text = PlayerMediator.playerInfo.arenaTimes.ToString();
        List<object> list = new List<object>();
        foreach (ArenaInfo item in arenaInfoList.Values)
            list.Add(item);
        list.Sort(CompareItem);
        panel.arenaGrid.AddCustomDataList(list);
    }


    /// <summary>
    /// 排序
    /// </summary>
    public int CompareItem(object x, object y)
    {
        if (x == null)
        {
            if (y == null)
                return 0;
            else
                return -1;
        }
        else
        {
            if (y == null)
            {
                return 1;
            }
            else
            {
                ArenaInfo a = x as ArenaInfo;
                ArenaInfo b = y as ArenaInfo;
                return a.ranking.CompareTo(b.ranking);
            }
        }
    }

    /// <summary>
    /// 释放
    /// </summary>
    protected override void OnDestroy()
    {
        arenaInfo = null;
        arenaMediator = null;
        base.OnDestroy();
    }
}
