using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;
using System;

public class buyitempanel : BasePanel
{
    public UISprite buyBtn;
    public UISprite cancelBtn;
    public UISprite minBtn;
    public UISprite maxBtn;
    public UISprite subtractBtn;
    public UISprite addBtn;
    public UISprite itemcolor;
    public UISprite moneyType;
    public UILabel itemName;
    public UILabel itemdesc;
    public UILabel buyPrcie;
    public UITexture itemIcon;
    public UILabel changeNum;
}
public class BuyItemMediator : UIMediator<buyitempanel>
{
    public buyitempanel panel
    {
        get
        {
            return m_Panel as buyitempanel;
        }
    }
    public delegate void BuyItem(ShopItemInfo info,int num);
    public BuyItem buyItem;
    private string itemID = string.Empty;
    private ShopItemInfo shopItemInfo;

    public BuyItemMediator() : base("buyitempanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.BuyItem_Show, OpenPanel);
        RegistPanelCall(NotificationID.BuyItem_Hide, ClosePanel);
    }

    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        ShowItemInfo showItemInfo = (notification.Body as List<object>)[0] as ShowItemInfo;
        shopItemInfo = (notification.Body as List<object>)[1] as ShopItemInfo;
        buyItem = showItemInfo.buyItem;
        panel.changeNum.text = "1";
        if (shopItemInfo.moneyType == 3)
        {
            itemID = shopItemInfo.itemID;
            panel.buyPrcie.text = shopItemInfo.itemPrice.ToString();
        }       
        else
        {
            panel.buyPrcie.text = (shopItemInfo.itemPrice * (shopItemInfo.disCount * 1.0f / 100)).ToString();
            itemID = shopItemInfo.itemID.Substring(0, shopItemInfo.itemID.Length - 1);
        }           
        UtilTools.SetMoneySprite(shopItemInfo.moneyType, panel.moneyType);
        ItemInfo info = ItemManager.GetItemInfo(itemID);
        LoadSprite.LoaderItem(panel.itemIcon, itemID, false);
        panel.itemcolor.spriteName = UtilTools.StringBuilder("color", info.color);
        panel.itemName.text = TextManager.GetItemString(itemID);
        panel.itemdesc.text = TextManager.GetPropsString("description_" + itemID);
    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.cancelBtn.gameObject).onClick = OnClick; 
        UIEventListener.Get(panel.buyBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.minBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.maxBtn.gameObject).onClick = OnClick;
        LongClickEvent.Get(panel.subtractBtn.gameObject).onPress = OnPress;
        LongClickEvent.Get(panel.addBtn.gameObject).onPress = OnPress;
        LongClickEvent.Get(panel.subtractBtn.gameObject).duration = 3;
        LongClickEvent.Get(panel.addBtn.gameObject).duration = 3;
    }

    void OnPress(GameObject go, bool isPressed)
    {
        if (!isPressed)
            LongClickEvent.Get(go).time = 0;
        int number = 0;
        int amount = 0;
        if (go == panel.addBtn.gameObject)
        {
            amount = int.Parse(panel.changeNum.text);
            if (shopItemInfo.moneyType == 3)
            {
                number = Mathf.FloorToInt(PlayerMediator.playerInfo.guildDonate / shopItemInfo.itemPrice);
                if (number >= shopItemInfo.limitTime)
                    number = shopItemInfo.limitTime;
                if (amount >= number)
                    return;
                amount++;
                panel.changeNum.text = amount.ToString();
                panel.buyPrcie.text = (shopItemInfo.itemPrice * amount).ToString();
                return;
            }

            if (shopItemInfo.moneyType == 0)
                number = Mathf.FloorToInt(PlayerMediator.playerInfo.diamond / (shopItemInfo.itemPrice * shopItemInfo.disCount / 100));
            else if (shopItemInfo.moneyType == 1)
                number = Mathf.FloorToInt(PlayerMediator.playerInfo.euro / (shopItemInfo.itemPrice * shopItemInfo.disCount / 100));
            else
                number = Mathf.FloorToInt(PlayerMediator.playerInfo.blackMoney / (shopItemInfo.itemPrice * shopItemInfo.disCount / 100));

            if (ItemManager.GetShopItemInfo(shopItemInfo.itemID).limitTime != 0 && number > ItemManager.GetShopItemInfo(shopItemInfo.itemID).limitTime)
                number = ItemManager.GetShopItemInfo(shopItemInfo.itemID).limitTime;
            if (amount >= number)
                return;
            amount++;
            panel.changeNum.text = amount.ToString();
            panel.buyPrcie.text = (shopItemInfo.itemPrice * amount * (shopItemInfo.disCount * 1.0f / 100)).ToString();
        }
        else
        {
            amount = int.Parse(panel.changeNum.text);
            if (amount <= 1)
            {
                return;
            }
            amount--;
            panel.changeNum.text = amount.ToString();
            panel.buyPrcie.text = (shopItemInfo.itemPrice * amount * (shopItemInfo.disCount * 1.0f / 100)).ToString();
        }
    }
    void OnClick(GameObject go)
    {
        if (go == panel.buyBtn.gameObject)
        {
            buyItem(shopItemInfo, UtilTools.IntParse(panel.changeNum.text));
            ClosePanel(null);

        }
        else if (go == panel.cancelBtn.gameObject)
        {
            ClosePanel(null);
        }
        else if (go == panel.minBtn.gameObject)
        {
            panel.changeNum.text = "1";
            if (shopItemInfo.moneyType == 3)
                panel.buyPrcie.text = shopItemInfo.itemPrice.ToString();
            else
                panel.buyPrcie.text = (shopItemInfo.itemPrice * (shopItemInfo.disCount * 1.0f / 100)).ToString();
        }
        else if (go == panel.maxBtn.gameObject)
        {
            int number = 0;
            if (shopItemInfo.moneyType == 3)
            {
                number = Mathf.FloorToInt(PlayerMediator.playerInfo.guildDonate / shopItemInfo.itemPrice);
                if (number >= shopItemInfo.limitTime)
                    number = shopItemInfo.limitTime;
                panel.changeNum.text = number.ToString();
                panel.buyPrcie.text = (shopItemInfo.itemPrice * number).ToString();
                return;
            }
            if (shopItemInfo.moneyType == 0)
                 number = Mathf.FloorToInt(PlayerMediator.playerInfo.diamond / (shopItemInfo.itemPrice * shopItemInfo.disCount / 100));              
            else if (shopItemInfo.moneyType == 1)
                number = Mathf.FloorToInt(PlayerMediator.playerInfo.euro / (shopItemInfo.itemPrice * shopItemInfo.disCount / 100));               
            else
                number = Mathf.FloorToInt(PlayerMediator.playerInfo.blackMoney / (shopItemInfo.itemPrice * shopItemInfo.disCount / 100));

            if (ItemManager.GetShopItemInfo(shopItemInfo.itemID).limitTime != 0 && number > ItemManager.GetShopItemInfo(shopItemInfo.itemID).limitTime)
                number = ItemManager.GetShopItemInfo(shopItemInfo.itemID).limitTime;
            panel.changeNum.text = number.ToString();
            panel.buyPrcie.text = (shopItemInfo.itemPrice * number * (shopItemInfo.disCount * 1.0f / 100)).ToString();                  
        }     
    }
    /// <summary>
    /// 释放
    /// </summary>
    protected override void OnDestroy()
    {
        panel.buyBtn = null;
        panel.cancelBtn = null;
        panel.minBtn = null;
        panel.maxBtn = null;
        panel.subtractBtn = null;
        panel.addBtn = null;
        panel.itemcolor = null;
        panel.itemName = null;
        panel.moneyType = null;
        panel.itemdesc = null;
        panel.buyPrcie = null;
        panel.itemIcon = null;
        panel.changeNum = null;
        base.OnDestroy();
    }
}