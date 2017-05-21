using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KBEngine;
using ProxyInstance;

public enum CardType
{
    Free = 1,
    Diamond = 2,
    Ten = 3,
    First = 4,
}
public class CardProxy : Proxy<CardProxy>
{   
    public CardProxy()
        : base(ProxyID.Card)
    {
        KBEngine.Event.registerOut(this, "set_euroFreeTimes");
        KBEngine.Event.registerOut(this, "set_euroLastTime");
        KBEngine.Event.registerOut(this, "set_diamondFreeTimes");
        KBEngine.Event.registerOut(this, "set_diamondLastTime");
        KBEngine.Event.registerOut(this, "set_tenFirstCall");
        KBEngine.Event.registerOut(this, "lotteryResult");
    }

    public void set_euroFreeTimes(KBEngine.Entity entity, object old)
    {
        old = entity.getDefinedProperty("euroFreeTimes");
        CardMediator.cardInfo.euroFreeTimes = int.Parse(old.ToString());
        CardPanelCall(CardType.Free);
    }
    public void set_euroLastTime(KBEngine.Entity entity, object old)
    {
        old = entity.getDefinedProperty("euroLastTime");
        CardMediator.cardInfo.euroLastTime = int.Parse(old.ToString());
        CardPanelCall(CardType.Free);
    }
    public void set_diamondFreeTimes(KBEngine.Entity entity, object old)
    {
        old = entity.getDefinedProperty("diamondFreeTimes");
        CardMediator.cardInfo.diamondFreeTimes = int.Parse(old.ToString());
        CardPanelCall(CardType.Diamond);
    }
    public void set_diamondLastTime(KBEngine.Entity entity, object old)
    {
        old = entity.getDefinedProperty("diamondLastTime");
        CardMediator.cardInfo.diamondLastTime = int.Parse(old.ToString());
        CardPanelCall(CardType.Diamond);
    }
    public void set_tenFirstCall(KBEngine.Entity entity, object old)
    {
        old = entity.getDefinedProperty("tenFirstCall");
        CardMediator.cardInfo.tenFirstCall = int.Parse(old.ToString());
    }
    public void lotteryResult(object old, object data)
    {
        int index = UtilTools.IntParse(old.ToString());
        if (index == 3)
        {
            GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_11"), null);
            return;
        }
        List<object> list = new List<object>();
        for (int i = 0; i < data.ToString().Split(',').Length; ++i)
        {
            list.Add(data.ToString().Split(',')[i]);          
        }
        ItemMediator.panelType = PanelType.Reward;
        Facade.SendNotification(NotificationID.ItemInfo_Show, list);
    }
    public void CardPanelCall(CardType type)
    {
        if (GUIManager.HasView("cardpanel"))
        {
            CardMediator.cardMediator.UpdateView(type);
        }       
    }
}
