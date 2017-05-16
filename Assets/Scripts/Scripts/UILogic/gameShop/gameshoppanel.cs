using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;
using System;

public class gameshoppanel : BasePanel
{
    public UIGrid shopGrid;
    public UITexture sportBtn;
    public UITexture blackBtn;
    public UITexture guildBtn; 
    public UISprite backBtn;
    public UISprite ShopBtn;
    public UIToggle everydayBtn;
    public UIToggle gemBtn;
    public UIToggle itemBtn;
    public Transform blackTab;
    public UISprite gearRotate;
    public UISprite moneySpri;
    public UILabel biaoTi;
    public UILabel moneyLabel;
    public Transform scrollView;
    public EnhanceScrollView enhanceScrollView;
}
public class GameShopMediator : UIMediator<gameshoppanel>
{
    public gameshoppanel panel
    {
        get
        {
            return m_Panel as gameshoppanel;
        }
    }
    private Vector3 downVec = Vector3.zero;
    private Vector3 upVec = Vector3.zero;
    private int direction = 0;
    private int chooseType = 1;
    public static GameShopMediator gameShopMediator;
    public static bool firstOpenUI = false;
    public static Dictionary<int, ShopItemInfo> guildShopList = new Dictionary<int, ShopItemInfo>();
    private int buyIndex = 0;
    private ShopItemInfo currentInfo;


    public GameShopMediator() : base("gameshoppanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.GameShop_Show, OpenPanel);
        RegistPanelCall(NotificationID.GameShop_Hide, ClosePanel);
    }
    /// <summary>
    /// 界面显示前调用
    /// </summary>
    protected override void OnStart(INotification notification)
    {
        if (gameShopMediator == null)
        {
            gameShopMediator = Facade.RetrieveMediator("GameShopMediator") as GameShopMediator;
        }
        panel.enhanceScrollView.sourceCamera = UICamera.mainCamera;
        panel.enhanceScrollView.gearRotate = panel.gearRotate.transform;
        panel.enhanceScrollView.callBack = OnClick;
        chooseType = 1;
        panel.shopGrid.enabled = true;
        panel.shopGrid.BindCustomCallBack(UpdateShopGrid);
        panel.shopGrid.StartCustom();

    }
    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        Facade.SendNotification(NotificationID.Gold_Hide);
        if (notification.Body != null)
        {
            int index = (int)notification.Body;
            switch (index)
            {
                case 1:
                    UtilTools.SetMoneySprite(2, panel.moneySpri);
                    panel.enhanceScrollView.startCenterIndex = 1;
                    panel.biaoTi.text = TextManager.GetUIString("UIshop2");
                    panel.shopGrid.AddCustomDataList(AddGridList(2, 0));
                    return;
            }
        }
        UtilTools.SetMoneySprite(0, panel.moneySpri);
        panel.biaoTi.text = TextManager.GetUIString("UIshop1");
        panel.moneyLabel.text = PlayerMediator.playerInfo.diamond.ToString();
        panel.shopGrid.AddCustomDataList(AddGridList(1, 1));
    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.everydayBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.gemBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.itemBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.backBtn.gameObject).onClick = OnClick;
    }


    void OnClick(GameObject go)
    {
        if (go == panel.sportBtn.gameObject)
        {
            if (chooseType == 1)
                return;
            chooseType = 1;
            panel.biaoTi.text = TextManager.GetUIString("UIshop1");
            panel.shopGrid.AddCustomDataList(AddGridList(1, 1));
            panel.scrollView.transform.localPosition = Vector3.zero;
            panel.blackTab.gameObject.SetActive(true);
            panel.everydayBtn.value = true;
            UtilTools.SetMoneySprite(0, panel.moneySpri);
            panel.moneyLabel.text = PlayerMediator.playerInfo.diamond.ToString();

        }
        else if (go == panel.blackBtn.gameObject)
        {
            if (chooseType == 2)
                return;
            chooseType = 2;
            panel.biaoTi.text = TextManager.GetUIString("UIshop2");
            panel.shopGrid.AddCustomDataList(AddGridList(2, 0));
            panel.scrollView.transform.localPosition = new Vector3(0, 78, 0);
            panel.blackTab.gameObject.SetActive(false);
            UtilTools.SetMoneySprite(2, panel.moneySpri);
            panel.moneyLabel.text = PlayerMediator.playerInfo.blackMoney.ToString();


        }
        else if (go == panel.guildBtn.gameObject)
        {
            if (chooseType == 3)
                return;
            chooseType = 3;
            panel.biaoTi.text = TextManager.GetUIString("UIshop11");
            List<object> objList = new List<object>();
            foreach (ShopItemInfo item in guildShopList.Values)
                objList.Add(item);
            panel.shopGrid.AddCustomDataList(objList);
            panel.scrollView.transform.localPosition = new Vector3(0, 78, 0);
            panel.blackTab.gameObject.SetActive(false);
            UtilTools.SetMoneySprite(3, panel.moneySpri);
            panel.moneyLabel.text = PlayerMediator.playerInfo.guildDonate.ToString();
        }
        else if (go == panel.everydayBtn.gameObject)
        {
            panel.shopGrid.AddCustomDataList(AddGridList(1, 1));
            UtilTools.SetMoneySprite(0, panel.moneySpri);
            panel.moneyLabel.text = PlayerMediator.playerInfo.diamond.ToString();
        }
        else if (go == panel.gemBtn.gameObject)
        {
            panel.shopGrid.AddCustomDataList(AddGridList(1, 2));
            UtilTools.SetMoneySprite(0, panel.moneySpri);
            panel.moneyLabel.text = PlayerMediator.playerInfo.diamond.ToString();
        }
        else if (go == panel.itemBtn.gameObject)
        {
            panel.shopGrid.AddCustomDataList(AddGridList(1, 3));
            UtilTools.SetMoneySprite(1, panel.moneySpri);
            panel.moneyLabel.text = PlayerMediator.playerInfo.euro.ToString();
        }
        else if (go == panel.backBtn.gameObject)
        {
            ClosePanel(null);
        }
    }


    void UpdateShopGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        item.gameObject.SetActive(false);
        item.gameObject.SetActive(true);
        ShopItemInfo info = item.oData as ShopItemInfo;
        UISprite money = item.mScripts[0] as UISprite;
        UILabel price = item.mScripts[1] as UILabel;
        UISprite color = item.mScripts[2] as UISprite;
        UITexture icon = item.mScripts[3] as UITexture;
        UILabel limit = item.mScripts[4] as UILabel;
        UILabel Name = item.mScripts[5] as UILabel;
        UISprite recommend = item.mScripts[6] as UISprite;
        UISprite discount = item.mScripts[7] as UISprite;
        UILabel discountNum = item.mScripts[8] as UILabel;
        UISprite islimit = item.mScripts[9] as UISprite;
        item.onClick = OnClickItem;
        recommend.gameObject.SetActive(chooseType != 3);
        discount.gameObject.SetActive(chooseType != 3);
        islimit.gameObject.SetActive(chooseType != 3);       
        ItemInfo data;
        if (chooseType == 3)
        {
            data = ItemManager.GetItemInfo(info.itemID);
            LoadSprite.LoaderItem(icon, info.itemID);
            UtilTools.SetMoneySprite(3, money);
            price.text = info.itemPrice.ToString();
            Name.text = TextManager.GetItemString(info.itemID);
            limit.gameObject.SetActive(info.limitTime != 0);
            limit.text = info.limitTime.ToString();
            islimit.gameObject.SetActive(info.limitTime == 0);
            color.spriteName = UtilTools.StringBuilder("color", data.color);
            return;
        }
        data = ItemManager.GetItemInfo(info.itemID.Substring(0, info.itemID.Length - 1));
        color.spriteName = UtilTools.StringBuilder("color", data.color);
        price.text = (info.itemPrice * (info.disCount * 1.0f / 100)).ToString();
        Name.text = TextManager.GetItemString(data.itemID);
        UtilTools.SetTextColor(Name, data.color);
        LoadSprite.LoaderItem(icon, data.itemID);
        UtilTools.SetMoneySprite(info.moneyType, money);
        limit.gameObject.SetActive(info.limitTime != 0);
        islimit.gameObject.SetActive(false);
        if (info.isLimit != 0)
        {
            limit.text = info.limitTime.ToString();
            islimit.gameObject.SetActive(info.limitTime == 0);
        }
        recommend.gameObject.SetActive(info.recommend == 1);
        discount.gameObject.SetActive(info.disCount != 100);
        if (discount.gameObject.activeSelf)
            discountNum.text = string.Format(TextManager.GetUIString("UIshop4"), info.disCount / 10);
    }
    public void ShoppingSucess()
    {
        if (currentInfo.moneyType == 3)
        {
            currentInfo.limitTime -= buyIndex;
        }
        else
        {
            if (currentInfo.isLimit == 1)
                currentInfo.limitTime -= buyIndex;
        }
        panel.shopGrid.UpdateCustomData(currentInfo);
        GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_shop_2"));
    }
    void OnClickItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        UISprite islimit = item.mScripts[9] as UISprite;
        if (islimit.gameObject.activeSelf)
        {
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_shop_1"));
            return;
        }
        currentInfo = item.oData as ShopItemInfo;
        if (currentInfo.moneyType == 0 && PlayerMediator.playerInfo.diamond < currentInfo.itemPrice)
        {
            GameShopProxy.Instance.onShopInfoCallBack(2);
            return;
        }
        if (currentInfo.moneyType == 1 && PlayerMediator.playerInfo.euro < currentInfo.itemPrice)
        {
            GameShopProxy.Instance.onShopInfoCallBack(1);
            return;
        }
        if (currentInfo.moneyType == 2 && PlayerMediator.playerInfo.blackMoney < currentInfo.itemPrice)
        {
            GameShopProxy.Instance.onShopInfoCallBack(3);
            return;
        }
        if (currentInfo.moneyType == 3 && PlayerMediator.playerInfo.guildDonate < currentInfo.itemPrice)
        {
            GameShopProxy.Instance.onShopInfoCallBack(5);
            return;
        }
        ShowItemInfo showItemInfo = new ShowItemInfo();
        showItemInfo.buyItem = BuyItem;
        List<object> list = new List<object>();
        list.Add(showItemInfo);
        list.Add(currentInfo);
        Facade.SendNotification(NotificationID.BuyItem_Show, list);
    }

    void BuyItem(ShopItemInfo info, int num)
    {       
        ServerCustom.instance.SendClientMethods("onClientShopping", long.Parse(info.itemID), num);
        buyIndex = num;
    }
    List<object> AddGridList(int shopType, int tabType)
    {
        List<object> objList = new List<object>();
        foreach (ShopItemInfo item in ItemManager.shopList.Values)
        {
            if (item.itemType == shopType && item.tabType == tabType)
                objList.Add(item);
        }
        return objList;
    }

    /// <summary>
    /// 释放
    /// </summary>
    protected override void OnDestroy()
    {
        Facade.SendNotification(NotificationID.Gold_Show);
        panel.shopGrid = null;
        panel.sportBtn = null;
        panel.blackBtn = null;
        panel.guildBtn = null;
        panel.backBtn = null;
        panel.ShopBtn = null;
        panel.everydayBtn = null;
        panel.gemBtn = null;
        panel.itemBtn = null;
        panel.blackTab = null;
        panel.gearRotate = null;
        panel.moneySpri = null;
        panel.biaoTi = null;
        panel.moneyLabel = null;
        panel.scrollView = null;
        panel.enhanceScrollView = null;
        base.OnDestroy();
    }
}