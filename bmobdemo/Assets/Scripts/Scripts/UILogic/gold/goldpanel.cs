using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using PureMVC.Patterns;
using PureMVC.Interfaces;

public enum GoldType
{
    Power = 0,
    Euro = 1,
    Diamond = 2,
}
public class goldpanel : BasePanel
{
    public UIButton Mailbtn;
    public UIButton setBtn;
    public UILabel diamondtext;
    public UILabel powertext;
    public UILabel euroText;
    public UIButton addPowerbtn;
    public UIButton adddiamondbtn;
    public UIButton addEuroBtn;
}
public class GoldMediator : UIMediator<goldpanel>
{
    private CommonInfo maxPower;
    public static GoldMediator goldMediator;
    private goldpanel panel
    {
        get
        {
            return m_Panel as goldpanel;
        }
    } 
    public GoldMediator() : base("goldpanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.Gold_Show, OpenPanel);
        RegistPanelCall(NotificationID.Gold_Hide, ClosePanel);
    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.setBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.Mailbtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.addPowerbtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.adddiamondbtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.addEuroBtn.gameObject).onClick = OnClick;
    }
    private void OnClick(GameObject go)
    {
        if (go == m_Panel.Mailbtn.gameObject)
        {
            MailProxy.Instance.GetMailList();
        }
        else if (go == m_Panel.setBtn.gameObject)
        {

        }
        else if (go == m_Panel.Mailbtn.gameObject)
        {
            MailProxy.Instance.GetMailList();
        }
        if (go == panel.addPowerbtn.gameObject)
        {
            Facade.SendNotification(NotificationID.Power_Show, GoldType.Power);
        }
        else if (go == panel.adddiamondbtn.gameObject)
        {
            if (GUIManager.HasView("storepanel"))
                return;
            Facade.SendNotification(NotificationID.Store_Show);
        }
        else if (go == panel.addEuroBtn.gameObject)
        {
            Facade.SendNotification(NotificationID.Power_Show, GoldType.Euro);
        }
    }
    protected override void OnShow(INotification notification)
    {
        if (goldMediator == null)
        {
            goldMediator = Facade.RetrieveMediator("GoldMediator") as GoldMediator;
        }
        maxPower = CommonConfig.GetCommonInfo(1);
        panel.diamondtext.text = PlayerMediator.playerInfo.diamond.ToString();
        panel.powertext.text = UtilTools.StringBuilder(PlayerMediator.playerInfo.bodyPower, "/", maxPower.value);
        panel.euroText.text = PlayerMediator.playerInfo.euro.ToString();       
    }

    public void GoldChangeCall(string name)
    {
        switch (name)
        {
            case "euro":
                panel.euroText.text = PlayerMediator.playerInfo.euro.ToString();
                break;
            case "diamond":
                panel.diamondtext.text = PlayerMediator.playerInfo.diamond.ToString();
                break;
            case "bodyPower":
                panel.powertext.text = UtilTools.StringBuilder(PlayerMediator.playerInfo.bodyPower, "/", maxPower.value);
                break;
        }
    }
    protected override void OnDestroy()
    {
        goldMediator = null;
        base.OnDestroy();
    }
}
