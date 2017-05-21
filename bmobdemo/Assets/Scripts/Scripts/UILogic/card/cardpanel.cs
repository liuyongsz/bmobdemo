using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PureMVC.Interfaces;

public class DrawCardInfo
{
    public int euroLastTime;
    public int euroFreeTimes;
    public int diamondFreeTimes;
    public int diamondLastTime;
    public int tenFirstCall;
}
/// <summary>
/// 实例化控件
/// </summary>
public class cardpanel : BasePanel
{
    public Transform lotter;
    public Transform reward;
    public Transform getballer;
    public UIGrid rewardGrid;
    public UIGrid PieceGrid;
    public UIButton freeBtn;
    public UIButton purpleBtn;
    public UIButton orangeBtn;
    public UISprite changeBtn;
    public UISprite seeBtn;
    public UIButton backBtn;
    public UILabel freeTimes;
    public UILabel orangeBtnText;
    public UILabel orangedesc;
    public UILabel freeCount;
    public UILabel purpleCount;
    public UILabel purpleTimes;
    public UILabel orangeCount;
    public UIToggle allBaller;
    public UIToggle forward;
    public UIToggle middle;
    public UIToggle back;
    public UIToggle door;
    public UIToggle allItem;
    public UIToggle baller;
    public UIToggle equip;
    public UIToggle material;
    public UITexture beijing;
}
public class CardMediator : UIMediator<cardpanel>
{
    public static DrawCardInfo cardInfo = Instance.Get<DrawCardInfo>();

    private LotteryInfo freeInfo;

    private LotteryInfo tenInfo;
    private LotteryInfo diamondInfo;

    public static CardMediator cardMediator;

    private int euroLastTime;

    private int diamondLastTime;

    private enum BackType
    {
        Main,
        Reward,
        Baller,
    }
    private int tabType = 0;
    private BackType backType = BackType.Main;
    /// <summary>
    /// 注册界面逻辑
    /// </summary>
    public CardMediator() : base("cardpanel")
    {
        m_isprop = false;
        m_ismain = false;
        RegistPanelCall(NotificationID.Card_Show, OpenPanel);
        RegistPanelCall(NotificationID.Card_Hide, ClosePanel);
    }
    /// <summary>
    /// 绑定点击事件
    /// </summary>
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(m_Panel.freeBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.purpleBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.orangeBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.changeBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.seeBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.backBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.allBaller.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.forward.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.middle.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.back.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.door.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.allItem.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.baller.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.equip.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.material.gameObject).onClick = OnClick;
    }
    /// <summary>
    /// 界面显示时初始化数据
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        LoadSprite.LoaderImage(m_Panel.beijing, "bg/qiuchang", false); 
        backType = BackType.Main;
        m_Panel.PieceGrid.enabled = true;
        m_Panel.PieceGrid.BindCustomCallBack(UpdatePieceGrid);
        m_Panel.PieceGrid.StartCustom();

        m_Panel.rewardGrid.enabled = true;
        m_Panel.rewardGrid.BindCustomCallBack(UpdateRewardGrid);
        m_Panel.rewardGrid.StartCustom();
        freeInfo = LotteryConfig.GetLotteryInfoByType(1);
        diamondInfo = LotteryConfig.GetLotteryInfoByType(2);
        tenInfo = LotteryConfig.GetLotteryInfoByType(3);
        m_Panel.orangedesc.text = TextManager.GetUIString("UICard3");
        m_Panel.orangeCount.text = tenInfo.capitalValue.ToString();
        if (cardMediator == null)
            cardMediator = Facade.RetrieveMediator("CardMediator") as CardMediator;
        m_Panel.freeTimes.text = string.Format(TextManager.GetUIString("UICard7"), freeInfo.freeCount - cardInfo.euroFreeTimes);
        UpdateView(CardType.First);       
    }
    /// <summary>
    /// 点击事件
    /// </summary>
    void OnClick(GameObject go)
    {
        if (go == m_Panel.backBtn.gameObject)
        {
            if (backType == BackType.Main)
            {
                ClosePanel(null);
            }
            else if (backType == BackType.Reward)
            {
                m_Panel.reward.gameObject.SetActive(false);
                m_Panel.lotter.gameObject.SetActive(true);
                backType = BackType.Main;
            }
            else
            {
                m_Panel.getballer.gameObject.SetActive(false);
                m_Panel.lotter.gameObject.SetActive(true);
                backType = BackType.Main;
            }
        }
        else if (go == m_Panel.freeBtn.gameObject)
        {
            if (PlayerMediator.playerInfo.euro < freeInfo.capitalValue)
            {
                GUIManager.PromptBuyEuro();
                return;
            }
            ServerCustom.instance.SendClientMethods("onClientLottery", (Byte)1);
        }
        else if (go == m_Panel.purpleBtn.gameObject)
        {
            if (PlayerMediator.playerInfo.diamond < diamondInfo.capitalValue)
            {
                GUIManager.PromptBuyDiamod(TextManager.GetUIString("UIStore1"));
                return;
            }
            ServerCustom.instance.SendClientMethods("onClientLottery", (Byte)2);
        }
        else if (go == m_Panel.orangeBtn.gameObject)
        {
            if (PlayerMediator.playerInfo.diamond < tenInfo.capitalValue)
            {
                GUIManager.PromptBuyDiamod(TextManager.GetUIString("UIStore1"));
                return;
            }
            ServerCustom.instance.SendClientMethods("onClientLottery", (Byte)3);
        }
        else if (go == m_Panel.seeBtn.gameObject)
        {
            tabType = 6;
            backType = BackType.Reward;
            m_Panel.reward.gameObject.SetActive(true);
            m_Panel.lotter.gameObject.SetActive(false);
            m_Panel.allItem.value = true;
            List<object> listObj = new List<object>();
            foreach (RewardInfo info in LotterRewardManager.configList.Values)
            {
                listObj.Add(info);
            }
            m_Panel.rewardGrid.AddCustomDataList(listObj);

        }
        else if (go == m_Panel.changeBtn.gameObject)
        {
            tabType = 1;
            backType = BackType.Baller;
            m_Panel.getballer.gameObject.SetActive(true);
            m_Panel.lotter.gameObject.SetActive(false);
            m_Panel.allBaller.value = true;
            List<object> listObj = new List<object>();
            foreach (RewardInfo info in PieceSwitchConfig.configList.Values)
            {
                listObj.Add(info);
            }
            m_Panel.PieceGrid.AddCustomDataList(listObj);
        }
        else if (go == m_Panel.allBaller.gameObject)
        {
            if (tabType == 1)
                return;
            tabType = 1;
            List<object> listObj = new List<object>();
            foreach (RewardInfo info in PieceSwitchConfig.configList.Values)
            {
                listObj.Add(info);
            }
            m_Panel.PieceGrid.AddCustomDataList(listObj);
        }
        else if (go == m_Panel.forward.gameObject)
        {
            if (tabType == 2)
                return;
            tabType = 2;
            AddBallerGrid("1");
        }
        else if (go == m_Panel.middle.gameObject)
        {
            if (tabType == 3)
                return;
            tabType = 3;
            AddBallerGrid("2");
        }
        else if (go == m_Panel.back.gameObject)
        {
            if (tabType == 4)
                return;
            tabType = 4;
            AddBallerGrid("3");
        }
        else if (go == m_Panel.door.gameObject)
        {
            if (tabType == 5)
                return;
            tabType = 5;
            AddBallerGrid("4");
        }
        else if (go == m_Panel.allItem.gameObject)
        {
            if (tabType == 6)
                return;
            tabType = 6;
            List<object> listObj = new List<object>();
            foreach (RewardInfo info in LotterRewardManager.configList.Values)
            {
                listObj.Add(info);
            }
            m_Panel.rewardGrid.AddCustomDataList(listObj);
        }
        else if (go == m_Panel.baller.gameObject)
        {
            if (tabType == 7)
                return;
            tabType = 7;
            List<object> listObj = new List<object>();
            foreach (RewardInfo info in LotterRewardManager.configList.Values)
            {
                if (info.itemType != 7)
                {
                    continue;
                }
                listObj.Add(info);
            }
            m_Panel.rewardGrid.AddCustomDataList(listObj);
        }
        else if (go == m_Panel.equip.gameObject)
        {
            if (tabType == 8)
                return;
            tabType = 8;
            List<object> listObj = new List<object>();
            foreach (RewardInfo info in LotterRewardManager.configList.Values)
            {
                if (info.itemType != 1)
                {
                    continue;
                }
                listObj.Add(info);
            }
            if (listObj.Count == 0)
            {
                m_Panel.rewardGrid.ClearCustomGrid();
                return;
            }
            m_Panel.rewardGrid.AddCustomDataList(listObj);
        }
        else if (go == m_Panel.material.gameObject)
        {
            if (tabType == 9)
                return;
            tabType = 9;
            List<object> listObj = new List<object>();
            foreach (RewardInfo info in LotterRewardManager.configList.Values)
            {
                if (info.itemType != 2)
                {
                    continue;
                }
                listObj.Add(info);
            }
            if (listObj.Count == 0)
            {
                m_Panel.rewardGrid.ClearCustomGrid();
                return;
            }
            m_Panel.rewardGrid.AddCustomDataList(listObj);
        }
    }
    void AddBallerGrid(string index)
    {

    }
    /// <summary>
    /// GridItem点击事件
    /// </summary>  
    void OnClickItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        RewardInfo rewardInfo = item.oData as RewardInfo;
        if (rewardInfo.itemType != 7)
        {
            ShowItemInfo showItemInfo = new ShowItemInfo();
            ItemMediator.panelType = PanelType.Info;
            showItemInfo.sellItem = null;
            showItemInfo.useItem = null;
            List<object> list = new List<object>();
            list.Add(showItemInfo);
            list.Add(rewardInfo);
            Facade.SendNotification(NotificationID.ItemInfo_Show, list);
        }
        else if (rewardInfo.itemType == 7)
        {
            Facade.SendNotification(NotificationID.Team_Show, rewardInfo.itemID);
        }
    }
    void UpdatePieceGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        RewardInfo rewardInfo = item.oData as RewardInfo;
        UISprite color = item.mScripts[0] as UISprite;
        UITexture head = item.mScripts[1] as UITexture;
        UILabel Name = item.mScripts[2] as UILabel;
        UISlider slider= item.mScripts[3] as UISlider;
        UISprite btn = item.mScripts[4] as UISprite;
        UILabel Num = item.mScripts[5] as UILabel;
        UISprite hetong = item.mScripts[6] as UISprite;
        slider.value = 0;
        ItemInfo info = ItemManager.GetItemInfo(rewardInfo.itemID);
        color.spriteName = UtilTools.StringBuilder("color" + info.color);
        Name.text = TextManager.GetItemString(rewardInfo.itemID);
        if (info.color == 5)
            hetong.spriteName = "jinhetong";
        else
            hetong.spriteName = "zihetong";

        LoadSprite.LoaderItem(head, rewardInfo.itemID, false);
    }
    void UpdateRewardGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        RewardInfo rewardInfo = item.oData as RewardInfo;
        UISprite color = item.mScripts[0] as UISprite;
        UITexture head = item.mScripts[1] as UITexture;
        UILabel Name = item.mScripts[2] as UILabel;
        UISprite stars = item.mScripts[3] as UISprite;
        item.onClick = OnClickItem;
        Name.text = TextManager.GetItemString(rewardInfo.itemID);
        UISprite[] star = stars.GetComponentsInChildren<UISprite>();
        stars.gameObject.SetActive(rewardInfo.itemType == 7);
        if (rewardInfo.itemType != 7)
        {
            LoadSprite.LoaderItem(head, rewardInfo.itemID, false);
            ItemInfo info = ItemManager.GetItemInfo(rewardInfo.itemID);
            color.spriteName = UtilTools.StringBuilder("color" + info.color);
        }

    }
    /// <summary>
    /// 更新界面数据
    /// </summary>
    public void UpdateView(CardType type)
    {
        TimeSpan timeSpan = (DateTime.Now - new DateTime(1970, 1, 1));
        int time = (int)timeSpan.TotalSeconds;
        if (type == CardType.Free)
        {
            SetFreeTime(time);
        }
        else if (type == CardType.Diamond)
        {
            SetDiamondTime(time);
        }
        else if (type == CardType.First)
        {
            SetFreeTime(time);
            SetDiamondTime(time);
        }   
    }
    /// <summary>
    /// 刷新美元抽数据
    /// </summary>
    void SetFreeTime(int time)
    {
        euroLastTime = time - cardInfo.euroLastTime - 8 * 3600;
        if (euroLastTime < freeInfo.freeCD)
        {
            m_Panel.freeCount.text = freeInfo.capitalValue.ToString();
            euroLastTime = freeInfo.freeCD - euroLastTime;
            UpdateEuroCardCD();
            TimerManager.Destroy("UpdateEuroCardCD");
            TimerManager.AddTimerRepeat("UpdateEuroCardCD", 1, UpdateEuroCardCD);
        }
        else
        {
            m_Panel.freeTimes.text = string.Format(TextManager.GetUIString("UICard7"), freeInfo.freeCount - cardInfo.euroFreeTimes);
            m_Panel.freeCount.text = TextManager.GetUIString("UICard6");
        }
            
    }

    /// <summary>
    /// 刷新钻石抽数据
    /// </summary>
    void SetDiamondTime(int time)
    {
        diamondLastTime = time - cardInfo.diamondLastTime - 8 * 3600;
        m_Panel.purpleCount.text = diamondInfo.capitalValue.ToString();
        if (diamondLastTime > diamondInfo.freeCD)
        {
            m_Panel.purpleTimes.text = TextManager.GetUIString("UICard6");
        }
        else
        {
            diamondLastTime = diamondInfo.freeCD - diamondLastTime;
            UpdateDiamondCardCD();
            TimerManager.Destroy("UpdateDiamondCardCD");
            TimerManager.AddTimerRepeat("UpdateDiamondCardCD", 1, UpdateDiamondCardCD);
        }
    }
    void UpdateEuroCardCD()
    {
        if (euroLastTime >= freeInfo.freeCD)
        {
            euroLastTime = freeInfo.freeCD;
        }
        m_Panel.freeTimes.text = string.Format(TextManager.GetUIString("UICard8"), UtilTools.TimeFormat(--euroLastTime));
        if (euroLastTime <= 0)
        {
            euroLastTime = 0;
            m_Panel.freeTimes.text = string.Format(TextManager.GetUIString("UICard7"), freeInfo.freeCount - cardInfo.euroFreeTimes);
            TimerManager.Destroy("UpdateDiamondCardCD");
        }
    }
    /// <summary>
    /// 钻石抽CD倒计时
    /// </summary>
    void UpdateDiamondCardCD()
    {
        if (diamondLastTime >= diamondInfo.freeCD)
        {
            diamondLastTime = diamondInfo.freeCD;
        }
        m_Panel.purpleTimes.text = string.Format(TextManager.GetUIString("UICard8"), UtilTools.TimeFormat(--diamondLastTime));
        if (diamondLastTime <= 0)
        {
            diamondLastTime = 0;
            m_Panel.purpleTimes.text = TextManager.GetUIString("UICard4");
            TimerManager.Destroy("UpdateDiamondCardCD");
        }
    }
    /// <summary>
    /// 界面关闭时调用
    /// </summary>
    protected override void OnDestroy()
    {
        TimerManager.Destroy("UpdateEuroCardCD");
        TimerManager.Destroy("UpdateDiamondCardCD");
        base.OnDestroy();
    }
}
