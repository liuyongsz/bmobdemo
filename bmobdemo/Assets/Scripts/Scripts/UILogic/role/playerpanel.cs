using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PureMVC.Interfaces;
public class playerpanel : BasePanel
{
    public Transform changeHead;
    public UIButton backbtn;
    public UIButton changeheadbtn;
    public UIButton changeslogan;
    public UILabel playername;
    public UILabel ID;
    public UILabel VIP;
    public UILabel fightvalue;
    public UILabel levelvalue;
    public UILabel governmentvalue;
    public UILabel slogan;
    public UILabel serverLabel;
    public UIButton delFriendBtn;
    public UIButton addFriendBtn;
    public UIButton chatBtn;
    public UIButton mailBtn;
    public UIButton blackBtn;
    public UIButton CloseBtn;
    public Transform btn;
    public Transform otherBtn;
    public UIButton changeSloganBtnOK;
    public UIButton changeSloganCancel;
    public UIInput changeSloganText;
    public UIPanel switchSloganPanel;
    public UIButton changesloganbtn;

}

public class PlayerMediator : UIMediator<playerpanel>
{
    public playerpanel panel { get { return m_Panel as playerpanel; } }
    private PlayerInfo playerInfos;
    public static PlayerInfo playerInfo = new PlayerInfo();
    private FriendData friendData;
    private CommonInfo info;
    public static PlayerMediator playerMediator;
    public PlayerMediator() : base("playerpanel")
    {
        m_isprop = true;
        m_ismain = false;
        RegistPanelCall(NotificationID.Role_Show, OpenPanel);
        RegistPanelCall(NotificationID.Role_Hide, ClosePanel);
    }   
    protected override void AddComponentEvents()
    {
        if (null == m_Panel) return;
        UIEventListener.Get(m_Panel.backbtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.delFriendBtn.gameObject).onClick = OnClick;    
        UIEventListener.Get(m_Panel.blackBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.changeheadbtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.changesloganbtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.CloseBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.changeSloganBtnOK.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.changeSloganCancel.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.addFriendBtn.gameObject).onClick = OnClick; 
    }
    protected override void OnShow(INotification notification)
    {
        if (playerMediator == null)
        {
            playerMediator = Facade.RetrieveMediator("PlayerMediator") as PlayerMediator;
        }
        playerInfos = new PlayerInfo();    
        playerInfos = notification.Body as PlayerInfo;       
        if (playerInfos != null)
        {
            m_Panel.btn.gameObject.SetActive(false);
            m_Panel.otherBtn.gameObject.SetActive(true);
            m_Panel.ID.text = (playerInfos.roleId + 10000000).ToString();
            m_Panel.VIP.text = UtilTools.StringBuilder("VIP", playerInfos.vipLevel);
            m_Panel.fightvalue.text = playerInfos.fightValue.ToString();
            m_Panel.levelvalue.text = playerInfos.level.ToString();
            m_Panel.slogan.text = playerInfos.slogan.ToString();
            m_Panel.levelvalue.text = playerInfos.level.ToString();
            m_Panel.playername.text = playerInfos.name;
        }      
        else
        {
            m_Panel.ID.text = (playerInfo.roleId + 10000000).ToString();
            m_Panel.VIP.text = UtilTools.StringBuilder("VIP", playerInfo.vipLevel);
            m_Panel.fightvalue.text = playerInfo.fightValue.ToString();
            m_Panel.levelvalue.text = playerInfo.level.ToString();
            m_Panel.slogan.text = playerInfo.slogan.ToString();
            m_Panel.levelvalue.text = playerInfo.level.ToString();
            m_Panel.playername.text = playerInfo.name;
            m_Panel.btn.gameObject.SetActive(true);
            m_Panel.otherBtn.gameObject.SetActive(false);
            EventDelegate.Add(panel.changeSloganText.onChange, SloganValueChanged);
            for (int i = 0; i < ServerConfig.serverList.Count; i++)
            {
                if (PlayerPrefs.GetString("UserServer") == ServerConfig.serverList[i + 1].ip)
                {
                    m_Panel.serverLabel.text = "服务器" + ServerConfig.serverList[i + 1].Name;
                    break;
                }
            }
        }
        if(m_Panel.otherBtn.gameObject.activeSelf)
        {
            if(UtilTools.isMyFriend(playerInfos.roleId))
            {
                m_Panel.delFriendBtn.gameObject.SetActive(true);
                m_Panel.addFriendBtn.gameObject.SetActive(false);
            }
        }
    }

    void OnClick(GameObject go)
    {
        if (go == m_Panel.backbtn.gameObject)
        {
            ClosePanel(null);
        }
        else if (go == m_Panel.delFriendBtn.gameObject)
        {
            FriendMediator.friendList.Remove(friendData);
            FriendMediator.friendMediator.AddFriendGridList(FriendType.Friend);
            ServerCustom.instance.SendClientMethods("onClientDelFriend", friendData.dbid);
        }
        else if (go == m_Panel.blackBtn.gameObject)
        {
            if(null == FriendMediator.friendMediator)
            {
                CommonFun.Debug("FriendMediator.friendMediator is Null");
                return;
            }

            // 加入黑名单
            FriendMediator.friendList.Remove(friendData);
            FriendMediator.friendMediator.AddFriendGridList(FriendType.Friend);
            friendData.type = FriendType.Black;
            FriendMediator.blackFriendList.Add(friendData);
            ServerCustom.instance.SendClientMethods("onClientAddBlack", friendData.dbid);
        }
        else if (go == m_Panel.changeheadbtn.gameObject)
        {
            panel.changeHead.gameObject.SetActive(true);
        }
        else if (go == m_Panel.CloseBtn.gameObject)
        {
            panel.changeHead.gameObject.SetActive(false);
        }
        else if (go == m_Panel.changesloganbtn.gameObject)
        {
            info = CommonConfig.GetCommonInfo(2);
            panel.changeSloganText.characterLimit = info.value;
            panel.switchSloganPanel.gameObject.SetActive(true);
            panel.changeSloganText.label.text = TextManager.GetUIString("UISlogan");
            panel.changeSloganText.value = string.Empty;           
        }
        else if(go == m_Panel.addFriendBtn.gameObject )
        {
            ServerCustom.instance.SendClientMethods("onClientApplyAddFriend", playerInfos.roleId);
        }
        else if (go == m_Panel.changeSloganCancel.gameObject)
        {
            panel.switchSloganPanel.gameObject.SetActive(false);
        }
        else if (go == m_Panel.changeSloganBtnOK.gameObject)
        {
            string content = m_Panel.changeSloganText.value;
            if (content == string.Empty)
            {
                GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_30"), null);
                return;
            }
            ServerCustom.instance.SendClientMethods("onClientChangeSolgan",content);

            panel.switchSloganPanel.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 限制签名字数
    /// </summary>
    void SloganValueChanged()
    {
        if (panel.changeSloganText.value.Length >= info.value)
            GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_31"), null);
    }
    protected override void OnDestroy()
    {
        EventDelegate.Remove(panel.changeSloganText.onChange, SloganValueChanged);
        base.OnDestroy();     
    }
}
