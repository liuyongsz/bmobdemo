using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KBEngine;

public class GameShopProxy : Proxy<GameShopProxy>
{

    public GameShopProxy()
        : base(ProxyID.GameShop)
    {        
        KBEngine.Event.registerOut(this, "onShopInfoCallBack");
        KBEngine.Event.registerOut(this, "onGetShopItemInfo");
        KBEngine.Event.registerOut(this, "onGetGuildShop");
        KBEngine.Event.registerOut(this, "onGetShopSucess");
    }

    public void onShopInfoCallBack(object obj)
    {
        int index = UtilTools.IntParse(obj.ToString());
        switch (index)
        {
            case 1:
                GUIManager.PromptBuyEuro();
                return;
            case 2:
                GUIManager.PromptBuyDiamod(TextManager.GetUIString("UIStore1"));
                return;
            case 3:
                GUIManager.SetPromptInfoChoose(TextManager.GetUIString("UICreate1"), TextManager.GetUIString("UIshop10"), BuyBlackMoney);
                return;
            case 5:
                GUIManager.SetPromptInfoChoose(TextManager.GetUIString("UICreate1"), TextManager.GetUIString("UIshop12"), BuyGuildMoney);
                return;
           
        }
    }
    public void onGetGuildShop(List<object> list)
    {
        ShopItemInfo info;
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, object> Info = list[i] as Dictionary<string, object>;
            info = new ShopItemInfo();
            info.itemID = Info["itemID"].ToString();
            info.limitTime = UtilTools.IntParse(Info["limitTimes"].ToString());
            info.itemPrice = UtilTools.IntParse(Info["price"].ToString());
            info.moneyType = 3;
            GameShopMediator.guildShopList.Add(UtilTools.IntParse(info.itemID), info);
        }
    }
    public void onGetShopItemInfo(List<object> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, object> Info = list[i] as Dictionary<string, object>;
            if (ItemManager.shopList.ContainsKey(Info["itemID"].ToString()))
            {
                ItemManager.shopList[Info["itemID"].ToString()].limitTime = UtilTools.IntParse(Info["limitTimes"].ToString());
            }   
        }
        GameShopMediator.firstOpenUI = true;
        Facade.SendNotification(NotificationID.GameShop_Show);
    }
    public void onGetShopSucess(object obj)
    {
        if (GameShopMediator.gameShopMediator == null)
            return;
        GameShopMediator.gameShopMediator.ShoppingSucess();
    }

    void BuyBlackMoney()
    {
       
    }
    void BuyGuildMoney()
    {

    }
}
