using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;
using System;

public class itempanel : BasePanel
{
    public Transform itemInfo;
    public Transform Info;
    public Transform batchSell;
    public Transform SellOne;
    public Transform getReward;
    public Transform ballerUpMentality;
    public Transform switchballer;
    public Transform chooseFriend;
    public Transform RewardItem;
    public Transform Mask;
    public Transform use;
    public UIButton CloseBtn;
}
public enum PanelType
{
    Info,
    Sell,
    Use,
    Reward,
    ChooseFriend,
    UpMentality,
    SwitchInherit,
}
public class ShowItemInfo
{      
    public ItemMediator.SellItem sellItem;
    public ItemMediator.UseItem useItem;
    public ItemMediator.OkSellOneBtn sellOne;
    public ItemMediator.OkUseOneBtn useOne;
    public BuyItemMediator.BuyItem buyItem;
}
public class ItemMediator : UIMediator<itempanel>
{
    private UIButton sellBtn;
    private UIButton useItemBtn;
    private UISprite UPMentalityBtn;
    private UIButton getRewardBtn;
    private UIButton cancelOneSellBtn;
    private UIButton okOneSellBtn;
    private UIButton okuse;
    private UIButton cancelUse;
    private UIButton minBtn;
    private UIButton maxBtn;
    private UIButton addBtn;
    private UIButton subtractBtn;
    private UITexture itemIcon;
    private UISprite itemcolor;
    private UIGrid ChooseGrid;
    private UIGrid FriendGrid;
    private UIToggle[] UPMentality;
    private UILabel[] mentalityDesc;
    private UILabel itemPrice;
    private UILabel itemDesc;
    private UILabel itemName;
    private UILabel itemNum;
    private UILabel haveNum;
    private UILabel changeNum;
    private UILabel sellPrcie;
 

    public delegate void SellItem(Item info);
    public SellItem sellItem;

    public delegate void UseItem(Item info);
    public UseItem useItem;


    public delegate void OkSellOneBtn(int count);
    public OkSellOneBtn SellOne;
    public delegate void OkUseOneBtn(Item info);
    public OkUseOneBtn UseOne;
    private int rewardIndex = 0;
    private List<object> rewardList = new List<object>();
    private ItemInfo item;
    private List<object> list = new List<object>();
    public static PanelType panelType = PanelType.Info;

    private Item info;
    private string itemID;
    /// <summary>
    /// 注册界面逻辑
    /// </summary>
    public ItemMediator() : base("itempanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.ItemInfo_Show, OpenPanel);
        RegistPanelCall(NotificationID.ItemInfo_Hide, ClosePanel);
    }

    /// <summary>
    /// 界面显示时调用
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        if (panelType == PanelType.Info || panelType == PanelType.Sell || panelType == PanelType.Use)
        {
            m_Panel.CloseBtn.transform.localPosition = new Vector3(323, 244, 0);
            m_Panel.itemInfo.gameObject.SetActive(true);
            itemIcon = m_Panel.transform.FindChild("itemInfo/itemIcon").GetComponent<UITexture>();
            itemcolor = m_Panel.transform.FindChild("itemInfo/itemcolor").GetComponent<UISprite>();
            itemName = m_Panel.transform.FindChild("itemInfo/itemName").GetComponent<UILabel>();
            ShowItemInfo showItemInfo = (notification.Body as List<object>)[0] as ShowItemInfo;
            itemID = (notification.Body as List<object>)[1] as string;
            string uuid = (notification.Body as List<object>)[2] as string;
            item = ItemManager.GetItemInfo(itemID);
            EquipItemInfo equip = EquipConfig.GetEquipDataByUUID(uuid);
            if (equip != null)
                itemcolor.spriteName = UtilTools.StringBuilder("color", equip.star);
            else
                itemcolor.spriteName = UtilTools.StringBuilder("color", item.color);
            itemName.text = TextManager.GetItemString(item.itemID);
            LoadSprite.LoaderItem(itemIcon, itemID, false);
            if (panelType == PanelType.Info)
            {
                m_Panel.SellOne.gameObject.SetActive(false);
                m_Panel.Info.gameObject.SetActive(true);
                sellBtn = m_Panel.transform.FindChild("itemInfo/Info/sellBtn").GetComponent<UIButton>();
                useItemBtn = m_Panel.transform.FindChild("itemInfo/Info/useItemBtn").GetComponent<UIButton>();
                itemDesc = m_Panel.transform.FindChild("itemInfo/Info/itemDesc").GetComponent<UILabel>();
                itemPrice = m_Panel.transform.FindChild("itemInfo/Info/itemPrice").GetComponent<UILabel>();
                itemNum = m_Panel.transform.FindChild("itemInfo/Info/itemNum").GetComponent<UILabel>();
                UIEventListener.Get(sellBtn.gameObject).onClick = OnClick;
                UIEventListener.Get(useItemBtn.gameObject).onClick = OnClick;
                if (item == null)
                {
                    return;
                }
                itemDesc.text = TextManager.GetPropsString("description_" + item.itemID);
                itemPrice.text = item.itemPrice.ToString();
                itemNum.text = ItemManager.GetBagItemCount(itemID.ToString()).ToString();
                sellItem = showItemInfo.sellItem;
                useItem = showItemInfo.useItem;
                if (sellItem == null && useItem == null)
                {
                    sellBtn.gameObject.SetActive(false);
                    useItemBtn.gameObject.SetActive(false);
                }
                else
                {
                    sellBtn.gameObject.SetActive(true);
                    useItemBtn.gameObject.SetActive(true);
                    sellItem = showItemInfo.sellItem;
                    useItem = showItemInfo.useItem;
                }
            }
            else if (panelType == PanelType.Sell)
            {
                m_Panel.Info.gameObject.SetActive(false);
                m_Panel.SellOne.gameObject.SetActive(true);
                minBtn = m_Panel.transform.FindChild("itemInfo/SellOne/minBtn").GetComponent<UIButton>();
                maxBtn = m_Panel.transform.FindChild("itemInfo/SellOne/maxBtn").GetComponent<UIButton>();
                addBtn = m_Panel.transform.FindChild("itemInfo/SellOne/addBtn").GetComponent<UIButton>();
                subtractBtn = m_Panel.transform.FindChild("itemInfo/SellOne/subtractBtn").GetComponent<UIButton>();
                cancelOneSellBtn = m_Panel.transform.FindChild("itemInfo/SellOne/cancelOneSellBtn").GetComponent<UIButton>();
                okOneSellBtn = m_Panel.transform.FindChild("itemInfo/SellOne/okOneSellBtn").GetComponent<UIButton>();
                haveNum = m_Panel.transform.FindChild("itemInfo/SellOne/haveNum").GetComponent<UILabel>();
                changeNum = m_Panel.transform.FindChild("itemInfo/SellOne/changeNum").GetComponent<UILabel>();
                sellPrcie = m_Panel.transform.FindChild("itemInfo/SellOne/sellPrcie").GetComponent<UILabel>();
                UIEventListener.Get(cancelOneSellBtn.gameObject).onClick = OnClick;
                LongClickEvent.Get(subtractBtn.gameObject).onPress = OnPress;
                LongClickEvent.Get(subtractBtn.gameObject).duration = 3;
                LongClickEvent.Get(addBtn.gameObject).onPress = OnPress;
                LongClickEvent.Get(addBtn.gameObject).duration = 3;
                UIEventListener.Get(minBtn.gameObject).onClick = OnClick;
                UIEventListener.Get(maxBtn.gameObject).onClick = OnClick;
                UIEventListener.Get(okOneSellBtn.gameObject).onClick = OnClick;
                subtractBtn.gameObject.SetActive(info.amount > 1);
                addBtn.gameObject.SetActive(info.amount > 1);
                maxBtn.gameObject.SetActive(info.amount > 1);
                minBtn.gameObject.SetActive(info.amount > 1);
                changeNum.gameObject.SetActive(info.amount > 1);
                changeNum.text = "1";
                sellPrcie.text = item.itemPrice.ToString();
                haveNum.text = info.amount.ToString();
                SellOne = showItemInfo.sellOne;
            }
            else if (panelType == PanelType.Use)
            {
                sellBtn = m_Panel.transform.FindChild("itemInfo/Info/sellBtn").GetComponent<UIButton>();
                useItemBtn = m_Panel.transform.FindChild("itemInfo/Info/useItemBtn").GetComponent<UIButton>();
                itemDesc = m_Panel.transform.FindChild("itemInfo/Info/itemDesc").GetComponent<UILabel>();
                itemPrice = m_Panel.transform.FindChild("itemInfo/Info/itemPrice").GetComponent<UILabel>();
                itemNum = m_Panel.transform.FindChild("itemInfo/Info/itemNum").GetComponent<UILabel>();
                sellBtn.gameObject.SetActive(false);
                useItemBtn.gameObject.SetActive(false);
                m_Panel.Info.gameObject.SetActive(true);
                m_Panel.use.gameObject.SetActive(true);
                if (item == null)
                {
                    return;
                }
                itemDesc.text = TextManager.GetPropsString("description_" + item.itemID);
                itemPrice.text = item.itemPrice.ToString();
                itemNum.text = info.amount.ToString();
                okuse = m_Panel.transform.FindChild("itemInfo/use/okuse").GetComponent<UIButton>();
                cancelUse = m_Panel.transform.FindChild("itemInfo/use/cancelUse").GetComponent<UIButton>();
                UIEventListener.Get(okuse.gameObject).onClick = OnClick;
                UIEventListener.Get(cancelUse.gameObject).onClick = OnClick;
                UseOne = showItemInfo.useOne;
            }
        }
        else if (panelType == PanelType.Reward)
        {
            rewardIndex = 0;
            m_Panel.Mask.GetComponent<BoxCollider>().enabled = false;
            UIEventListener.Get(m_Panel.Mask.gameObject).onClick = OnClick;
            m_Panel.CloseBtn.gameObject.SetActive(false);
            GameObject cell = m_Panel.RewardItem.gameObject;
            PoolManager.CreatePrefabPools(PoolManager.PoolKey.Prefab_RewardItem, cell, false);
            m_Panel.getReward.gameObject.SetActive(true);
            rewardList = notification.Body as List<object>;
            TimerManager.Destroy("rewardIndex");
            TimerManager.AddTimer("rewardIndex", 0.02f, CreateRewardItem);
            //getRewardBtn = m_Panel.transform.FindChild("getReward/getRewardBtn").GetComponent<UIButton>();
            //UIEventListener.Get(getRewardBtn.gameObject).onClick = OnClick;
        }
        else if (panelType == PanelType.ChooseFriend)
        {
            if (FriendMediator.friendList.Count < 1)
            {
                GUIManager.SetPromptInfo(TextManager.GetUIString("UI2048"), null);
                ClosePanel(null);
                return;
            }            
            m_Panel.CloseBtn.transform.localPosition = new Vector3(374, 229, 0);           
            m_Panel.chooseFriend.gameObject.SetActive(true);

            FriendGrid = UtilTools.GetChild<UIGrid>(m_Panel.transform, "chooseFriend/ScrollView/FriendGrid");
            FriendGrid.enabled = true;
            FriendGrid.BindCustomCallBack(UpdateFriendGrid);
            FriendGrid.StartCustom();
        }
        else if (panelType == PanelType.UpMentality)
        {
            m_Panel.CloseBtn.transform.localPosition = new Vector3(316, 277, 0);
            UPMentality = UtilTools.GetChilds<UIToggle>(m_Panel.transform, "ballerUpMentality/group");
            mentalityDesc = UtilTools.GetChilds<UILabel>(m_Panel.transform, "ballerUpMentality/group");
            UPMentalityBtn = m_Panel.transform.FindChild("ballerUpMentality/UPMentalityBtn").GetComponent<UISprite>();
            UIEventListener.Get(UPMentalityBtn.gameObject).onClick = OnClick;
            m_Panel.ballerUpMentality.gameObject.SetActive(true);
            TeamMediator.mentalityPanel.canClick = true;
            for (int i = 0; i < UPMentality.Length; ++i)
            {
                string desc = string.Empty;
                UPMentality[i].value = true;
                foreach (MentalityUpItem item in MentalityPanel.mentalityUpList.Values)
                {
                    if (UtilTools.IntParse(item.Name[0].ToString()) == i)
                    {
                        if (item.Value < 0)
                        {
                            desc += UtilTools.StringBuilder(TextManager.GetUIString(item.Name.Substring(1)), item.Value, "  ");
                        }
                        else
                        {
                            desc += UtilTools.StringBuilder(TextManager.GetUIString(item.Name.Substring(1)), " +", item.Value, "  ");
                        }
                        if (UPMentality[i].value)
                        {
                            UPMentality[i].value = item.Value > 0;
                        }
                    }
                }
                mentalityDesc[i].text = UtilTools.StringBuilder(string.Format(TextManager.GetUIString("UINumber"), i + 1), ": ", desc);
            }
        }
        else if (panelType == PanelType.SwitchInherit)
        {
            m_Panel.CloseBtn.transform.localPosition = new Vector3(378, 243, 0);
            m_Panel.switchballer.gameObject.SetActive(true);
            List<object> list = notification.Body as List<object>;
            ChooseGrid = UtilTools.GetChild<UIGrid>(m_Panel.transform, "switchballer/ScrollView/ChooseGrid");
            ChooseGrid.enabled = true;
            ChooseGrid.BindCustomCallBack(UpdateInheriter);
            ChooseGrid.StartCustom();
            ChooseGrid.AddCustomDataList(list);
        }
    }
    void CreateRewardItem()
    {
        if (rewardIndex == rewardList.Count - 1)
        {
            m_Panel.Mask.GetComponent<BoxCollider>().enabled = true;
            return;
        }
        string itemID = rewardList[rewardIndex] as string;
        ItemInfo info = ItemManager.GetItemInfo(itemID);
        GameObject go = GameObject.Instantiate(PoolManager.PopPrefab(PoolManager.PoolKey.Prefab_RewardItem).gameObject);
        go.transform.parent = m_Panel.getReward.transform;
        go.transform.localScale = Vector3.zero;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.GetComponent<UISprite>().spriteName = "color" + info.color;
        LoadSprite.LoaderItem(go.GetComponentInChildren<UITexture>(), itemID, false);
        go.gameObject.SetActive(true);
        TweenScale.Begin(go, 0.02f, Vector3.one);
        if (rewardIndex <= 4)
        {
            TweenPosition.Begin(go, 0.02f, new Vector3(-230 + rewardIndex * 130, 90, 0));
        }
        else
        {
            TweenPosition.Begin(go, 0.02f, new Vector3(-230 + (rewardIndex - 5) * 130, -35, 0));
        }
        rewardIndex++;
        TimerManager.AddTimer("rewardIndex", 0.2f, CreateRewardItem);
    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(m_Panel.CloseBtn.gameObject).onClick = OnClick;
    }
    void OnPress(GameObject go,bool isPress)
    {
        if (addBtn != null && go == addBtn.gameObject)
        {
            LongClickEvent.Get(addBtn.gameObject).time = 0;
            int amount = int.Parse(changeNum.text);
            if (int.Parse(changeNum.text) == info.amount)
            {
                return;
            }
            amount++;
            changeNum.text = amount.ToString();
            sellPrcie.text = (amount * item.itemPrice).ToString();
        }
        else if (subtractBtn != null && go == subtractBtn.gameObject)
        {
            LongClickEvent.Get(subtractBtn.gameObject).time = 0;
            int amount = int.Parse(changeNum.text);
            if (int.Parse(changeNum.text) == 1)
            {
                return;
            }
            amount--;
            changeNum.text = amount.ToString();
            sellPrcie.text = (amount * item.itemPrice).ToString();
        }
    }
    void OnClick(GameObject go)
    {
        if (sellBtn != null && go == sellBtn.gameObject && sellItem != null)
        {
            sellItem(info);
        }
        else if (useItemBtn != null && go == useItemBtn.gameObject && useItem != null)
        {
            useItem(info);
        }
        else if (okuse != null && go == okuse.gameObject)
        {
            UseOne(info);
        }
        else if (cancelUse != null && go == cancelUse.gameObject)
        {
            ClosePanel(null);
        }
        else if (okOneSellBtn != null && go == okOneSellBtn.gameObject && SellOne != null)
        {
            ClosePanel(null);
            SellOne(int.Parse(changeNum.text));
        }
        else if (minBtn != null && go == minBtn.gameObject)
        {
            changeNum.text = "1";
            sellPrcie.text = item.itemPrice.ToString();
        }
        else if (cancelOneSellBtn != null && go == cancelOneSellBtn.gameObject)
        {
            ClosePanel(null);
        }
        else if (go == m_Panel.CloseBtn.gameObject)
        {
            if (MentalityPanel.mentalityUpList.Count > 0)
            {
                MentalityPanel.mentalityUpList.Clear();
            }
            ClosePanel(null);
        }
        else if (maxBtn != null && go == maxBtn.gameObject)
        {
            changeNum.text = info.amount.ToString();
            sellPrcie.text = (info.amount * item.itemPrice).ToString();
        }
        else if (UPMentalityBtn != null && go == UPMentalityBtn.gameObject)
        {
            List<object> list = new List<object>();
            // 提升意识
            for (int i = 0; i < UPMentality.Length; ++i)
            {
                if (UPMentality[i].value)
                {
                    Dictionary<string, object> info = new Dictionary<string, object>();
                    info.Add("index", i);
                    list.Add(info);
                }
            }
            Dictionary<string, object> listDic = new Dictionary<string, object>();
            listDic.Add("values", list);
            ServerCustom.instance.SendClientMethods("ChooseUpMentalityInfo", TeamMediator.currentTeamBaller.id, listDic);
            ClosePanel(null);
        }
        else if (go == m_Panel.Mask.gameObject)
        {
            ClosePanel(null);
        }
       
    }
    void UpdateFriendGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        item.onClick = ChooseBaller;
        FriendData data = item.oData as FriendData;
        UILabel Name = item.mScripts[0] as UILabel;
        UITexture head = item.mScripts[1] as UITexture;
        Name.text = data.name;
    }
    void UpdateInheriter(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        item.onClick = ChooseBaller;
        TeamBaller baller = item.oData as TeamBaller;
        UILabel Name = item.mScripts[0] as UILabel;
        UISprite color = item.mScripts[1] as UISprite;
        UITexture head = item.mScripts[2] as UITexture;
        UISlider slider = item.mScripts[3] as UISlider;
        UITexture stars = item.mScripts[5] as UITexture;
        UILabel Level = item.mScripts[6] as UILabel;
        UILabel Num = item.mScripts[7] as UILabel;
        PlayerItem info = PlayerManager.GetPlayerItem(baller.level);
        slider.value = baller.exp * 1.0f / info.needExp;
        Num.text = UtilTools.StringBuilder(baller.exp, "/", info.needExp);
        Level.text = "Lv " + baller.level;
        Name.text = TextManager.GetItemString(baller.configId);
        color.spriteName = UtilTools.StringBuilder("color" + baller.star);
        LoadSprite.LoaderHead(head, UtilTools.StringBuilder("Card" + baller.configId), false);
        TD_Player player = Instance.Get<PlayerManager>().GetItem(int.Parse(baller.configId));
        UtilTools.SetStar(baller.star, stars.GetComponentsInChildren<UISprite>(), player.maxstar);
    }
    void ChooseBaller(UIGridItem item)
    {
        if (panelType == PanelType.SwitchInherit)
        {
            TeamBaller baller = item.oData as TeamBaller;
            TeamMediator.inheritPanel.CheckInheriter(baller.configId);
            Facade.SendNotification(NotificationID.ItemInfo_Hide);
        }
        else
        {

        }
    }
    /// <summary>
    /// 界面关闭时调用
    /// </summary>
    protected override void OnDestroy()
    {
        rewardList.Clear();
        base.OnDestroy();
    }
}