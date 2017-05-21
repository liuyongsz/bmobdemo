using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using PureMVC.Patterns;
using PureMVC.Interfaces;

public class powerpanel : BasePanel
{
    public UISprite closeBtn;
    public UILabel today;
    public UILabel Text;
    public UILabel maxTime;
    public UIButton exchangeOnce;
    public UIButton exchangeFive;
    public UISprite dollar;
    public UISprite power;
}
public class PowerMediator : UIMediator<powerpanel>
{
    public static PowerMediator powerMediator;

    public GoldType shopType;

    private powerpanel panel
    {
        get
        {
            return m_Panel as powerpanel;
        }
    }
  
    public PowerMediator() : base("powerpanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.Power_Show, OpenPanel);
        RegistPanelCall(NotificationID.Power_Hide, ClosePanel);
    }
    protected override void OnShow(INotification notification)
    {
        if (powerMediator == null)
        {
            powerMediator = Facade.RetrieveMediator("PowerMediator") as PowerMediator;
        }
        GoldType type = (GoldType)notification.Body;
        this.shopType = type;
        switch (type)
        {
            case GoldType.Power:
                panel.maxTime.text = TextManager.GetString("UIPower3");
                panel.Text.text = TextManager.GetUIString("UI1047");               
                panel.dollar.gameObject.SetActive(false);
                panel.power.gameObject.SetActive(true);
                break;
            case GoldType.Euro:
                panel.maxTime.text = TextManager.GetString("UIPower2");
                panel.Text.text = TextManager.GetUIString("UI1046");            
                panel.power.gameObject.SetActive(false);
                panel.dollar.gameObject.SetActive(true);
                break;
        }
        UpdateTimes();
    }
    public void UpdateTimes()
    {
        switch (shopType)
        {
            case GoldType.Power:                
                panel.today.text = string.Format(TextManager.GetString("UIPower"), 10 - PlayerMediator.playerInfo.PowerTimes);
                break;
            case GoldType.Euro:
                panel.today.text = string.Format(TextManager.GetString("UIPower"), 5 - PlayerMediator.playerInfo.euroBuyTimes);
                break;
        }
    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.closeBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.exchangeOnce.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.exchangeFive.gameObject).onClick = OnClick;
    }

    void OnClick(GameObject go)
    {
        if (go == panel.closeBtn.gameObject)
        {
            ClosePanel(null);
        }
        else if (go == panel.exchangeOnce.gameObject)
        {
            ServerCustom.instance.SendClientMethods("onClientBuyPower",(int)this.shopType, 1);
        }
        else if (go == panel.exchangeFive.gameObject)
        {
            ServerCustom.instance.SendClientMethods("onClientBuyPower", (int)this.shopType, 5);
        }
    }
    protected override void OnDestroy()
    {
        powerMediator = null;
        base.OnDestroy();
    }
}