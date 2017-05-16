using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using PureMVC.Patterns;
using PureMVC.Interfaces;

public class mainpanel : BasePanel
{
    public UIButton Lvbtn;
    public UIButton babyBtn;
    public UIButton Storebtn;
    public UIButton Cardbtn;
    public UIButton Friendbtn;
	public UIButton Chatbtn;
    public UIButton Bagbtn;
    public UIButton BallTeambtn;
    public UIButton equipBtn;
    public UIButton helpBtn;
    public UIButton formationBtn;
    public UIButton rankBtn;
    public UIButton guildBtn;
    public UILabel playername;
    public UILabel fightValue;
    public UISprite expSlider;
    public UITexture activety_4;
    public UITexture headicon;
    public UILabel Level;
    public UILabel Vip;
}
public class MainMediator : UIMediator<mainpanel>
{
    private mainpanel panel
    {
        get
        {
            return m_Panel as mainpanel;
        }
    }
    public static MainMediator mainMediator;
    public MainMediator() : base("mainpanel")
    {
        m_isprop = false;
        RegistPanelCall(NotificationID.Show_Main, OpenPanel);
        RegistPanelCall(NotificationID.Fight_Change, ChangeFightValue);
        RegistPanelCall(NotificationID.Vip_Change, ChangeVipValue);
        RegistPanelCall(NotificationID.Level_Change, ChangeLevelValue);
        RegistPanelCall(NotificationID.Hide_Main, ClosePanel);
    }   

    protected override void AddComponentEvents()
    {   
        UIEventListener.Get(m_Panel.Lvbtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.Storebtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.headicon.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.Cardbtn.gameObject).onClick = OnClick;
		UIEventListener.Get(m_Panel.Friendbtn.gameObject).onClick = OnClick;
		UIEventListener.Get(m_Panel.Chatbtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.Bagbtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.BallTeambtn.gameObject).onClick = OnClick; 
        UIEventListener.Get(m_Panel.equipBtn.gameObject).onClick = OnClick; 
        UIEventListener.Get(m_Panel.helpBtn.gameObject).onClick = OnClick; 
        UIEventListener.Get(m_Panel.babyBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.formationBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.activety_4.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.rankBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.guildBtn.gameObject).onClick = OnClick;
    }
    void ChangeFightValue(INotification notification)
    {
        panel.fightValue.text = PlayerMediator.playerInfo.fightValue.ToString();
    }
    void ChangeVipValue(INotification notification)
    {
        panel.Vip.text = "VIP" + PlayerMediator.playerInfo.vipLevel.ToString();
    }
    void ChangeLevelValue(INotification notification)
    {
        panel.Level.text = PlayerMediator.playerInfo.level.ToString();
    }
    private void OnClick(GameObject go)
    {
        if (go == m_Panel.Lvbtn.gameObject)
        {
            //Facade.SendNotification(NotificationID.BallerClip_Show);
            Facade.SendNotification(NotificationID.Gold_Hide);
            CloneProxy.Instance.onClientReqEnterClone();
            ClosePanel(null);
        }
        else if (go == m_Panel.headicon.gameObject)
        {
            Facade.SendNotification(NotificationID.Role_Show);
        }
        else if (go == m_Panel.Storebtn.gameObject)
        {
            if (!GameShopMediator.firstOpenUI)
            {
                ServerCustom.instance.SendClientMethods("onClientGetShopItemInfo");
                return;
            }
            Facade.SendNotification(NotificationID.GameShop_Show);
        }
        else if (go == m_Panel.Cardbtn.gameObject)
        {
            Facade.SendNotification(NotificationID.Card_Show);
        }
        else if (go == m_Panel.Friendbtn.gameObject)
        {
            Facade.SendNotification(NotificationID.Friend_Show);
        }
        else if (go == m_Panel.Chatbtn.gameObject)
        {
            Facade.SendNotification(NotificationID.Chat_Show);
        }
        else if (go == m_Panel.Bagbtn.gameObject)
        {
            Facade.SendNotification(NotificationID.Bag_Show);
            //PanelManager.OpenPanel("TestPanel3");
        }
        else if (go == m_Panel.BallTeambtn.gameObject)
        {
            ServerCustom.instance.fromOtherBtn = true;
            ServerCustom.instance.SendClientMethods("onClientGetBabyInfo");
        }
        else if (go == m_Panel.equipBtn.gameObject)
        {
            Facade.SendNotification(NotificationID.EquipMain_Show);
        }
        else if (go == m_Panel.helpBtn.gameObject)
        {
            //Facade.SendNotification(NotificationID.Baby_Show);
            Facade.SendNotification(NotificationID.CheckPoint_Show);
        }
        else if (go == m_Panel.babyBtn.gameObject)
        {
            ServerCustom.instance.fromOtherBtn = false;
            ServerCustom.instance.SendClientMethods("onClientGetBabyInfo");
            ServerCustom.instance.SendClientMethods("onClientGetBabyItemInfo");
        }
        else if (go == m_Panel.formationBtn.gameObject)
        {
            Facade.SendNotification(NotificationID.TeamFormine_Show);
        }
        else if (go == panel.activety_4.gameObject)
        {
            ResourceManager.Instance.LoadPrefab("enterpanel", (name, obj) =>
            {
                if (obj == null)
                    return;
                UtilTools.SetParentWithPosition(obj.transform, UICamera.mainCamera.transform, Vector3.zero, Vector3.one);
            });
        }
        else if (go == panel.rankBtn.gameObject)
        {
            ServerCustom.instance.SendClientMethods("onClientGetFightValueRank", 0);
        }
        else if (go == panel.guildBtn.gameObject)
        {
            if (PlayerMediator.playerInfo.guildDBID <= 0)
                Facade.SendNotification(NotificationID.GuildList_Show);
            else
            {
                Facade.SendNotification(NotificationID.GuildMain_Show);
                ServerCustom.instance.SendClientMethods(GuildProxy.OnClientGetGuildInfo);
            }
        }
    }
    protected override void OnShow(INotification notification)
    {
        if (mainMediator == null)
        {
            mainMediator = Facade.RetrieveMediator("MainMediator") as MainMediator;
        }        
        ServerCustom.instance.SendClientMethods("onClientGetAllCardInfo");
        ServerCustom.instance.SendClientMethods("onClientGetAllPieces");
        ServerCustom.instance.SendClientMethods("onClientGetItemList");
        ServerCustom.instance.SendClientMethods("onClientGetFriendList");
        LoadSprite.LoaderHead(panel.headicon, "jueshetouxiang1", false); 
        panel.Level.text = PlayerMediator.playerInfo.level.ToString();
        panel.playername.text = PlayerMediator.playerInfo.name;
        panel.fightValue.text = PlayerMediator.playerInfo.fightValue.ToString();
        panel.Vip.text = "VIP" + PlayerMediator.playerInfo.vipLevel.ToString();

        Facade.SendNotification(NotificationID.BALLOPER_CLOSE);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    } 
}
