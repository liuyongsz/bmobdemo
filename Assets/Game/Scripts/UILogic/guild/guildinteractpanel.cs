using UnityEngine;
using System.Collections;
using PureMVC.Interfaces;
using System.Collections.Generic;

public class guildinteractpanel : BasePanel
{
    public UIButton off_btn;
    public UITexture playerhead;
    public UILabel playername;
    public UILabel duty;
    public UILabel weekdonate;
    public UILabel alldonate;
    public UIButton addfriendBtn;
    public UIButton deletfriendBtn;
    public UIButton chatBtn;
    public UIButton lookBtn;
    public UIButton appointBtn;
    public UIButton expelBtn;
    public UIButton transferBtn;
    public Transform appoint;
    public Transform funcGrid;
    public UIGrid powerGrid;

}
public class GuildInteractMediator : UIMediator<guildinteractpanel>
{
    public guildinteractpanel panel
    {
        get
        {
            return m_Panel as guildinteractpanel;
        }
    }
    public static GuildInteractMediator guildinteractMediator;

    public GuildMemberInfo mInfo = null;
    private bool appointPower = false;
    public GuildInteractMediator() : base("guildinteractpanel")
    {
        m_isprop = true;
        setDepth = 7;
        RegistPanelCall(NotificationID.GuildInteract_Show, OpenPanel);
        RegistPanelCall(NotificationID.GuildInteract_Hide, ClosePanel);
    }
    protected override void OnShow(INotification notification)
    {
        if (guildinteractMediator == null)
        {
            guildinteractMediator = Facade.RetrieveMediator("GuildInteractMediator") as GuildInteractMediator;
        }

        mInfo = notification.Body as GuildMemberInfo;

        Init();
    }
    protected override void AddComponentEvents()
    {
        base.AddComponentEvents();
        for(int i = 0; i<panel.funcGrid.childCount; i++)
        {
            Transform child = panel.funcGrid.GetChild(i);
            UIEventListener.Get(child.gameObject).onClick = OnClick;
        }
        UIEventListener.Get(panel.off_btn.gameObject).onClick = OnClick;

    }
    private void Init()
    {
        InitGrid();
        SetPlayerInfo();
        SetFuns();
        SetPowerGridDate();
    }
    private void InitGrid()
    {
        panel.powerGrid.enabled = true;
        panel.powerGrid.BindCustomCallBack(UpdatePowerGridItem);
        panel.powerGrid.StartCustom();
    }
    private void  SetPlayerInfo()
    {
        panel.duty.text = TextManager.GetUIString("UIGuildPower"+mInfo.power);
        panel.playername.text = mInfo.playerName;
        panel.weekdonate.text = mInfo.weekDonate.ToString();
        panel.alldonate.text = mInfo.sumDonate.ToString();

    }
    private void SetFuns()
    {

        bool isFriend = UtilTools.isMyFriend(mInfo.id);
        panel.addfriendBtn.gameObject.SetActive(!isFriend);
        panel.deletfriendBtn.gameObject.SetActive(isFriend);

        appointPower = GuildMainMediator.guildmainMediator.IsPowerEnough("5");
        panel.appointBtn.gameObject.SetActive(appointPower);

        bool isFire = GuildMainMediator.guildmainMediator.IsPowerEnough("11");
        panel.expelBtn.gameObject.SetActive(isFire);

        panel.transferBtn.gameObject.SetActive(PlayerMediator.playerInfo.guildPower == 5);


        panel.funcGrid.GetComponent<NGUIGrid>().Reposition();
    }


    public void SetPowerGridDate()
    {
        panel.duty.text = TextManager.GetUIString("UIGuildPower" + mInfo.power);
        List<object> listObj = new List<object>();

        PowerInfo info;
        for(int i=4; i>0; i--)
        {
            info = new PowerInfo();
            info.power = i;
            info.count = GetGuildPowerNum(i);
            info.totalNum = GuildOfficialConfig.GetGuildOfficial(i).num;
            listObj.Add(info);
        }

        panel.powerGrid.AddCustomDataList(listObj);

    }
    private void UpdatePowerGridItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        PowerInfo info = item.oData as PowerInfo;
        item.onClick = OnClickItem;
        UILabel name = item.mScripts[0] as UILabel;
        UILabel count = item.mScripts[1] as UILabel;
        name.text = TextManager.GetUIString("UIGuildPower" + info.power);
        count.text = string.Format("({0}/{1})", info.count, info.totalNum);
    }
    private void OnClick(GameObject go)
    {
        switch(go.transform.name)
        {
            case "off_btn":
                Facade.SendNotification(NotificationID.GuildInteract_Hide);
                break;
            case "addfriendBtn":
                GUIManager.SetJumpText(TextManager.GetUIString("UIGuild31"));
                ServerCustom.instance.SendClientMethods("onClientApplyAddFriend", mInfo.id);
                break;
            case "deletfriendBtn":
                GUIManager.SetJumpText(TextManager.GetUIString("UIGuild31"));
                ServerCustom.instance.SendClientMethods("onClientDelFriend", mInfo.id);
                break;
            case "chatBtn":
                List<object> list = new List<object>();
                list.Add(mInfo.id);
                list.Add(mInfo.playerName);
                Facade.SendNotification(NotificationID.Chat_Show, list);
                break;
            case "lookBtn":
                UtilTools.RoleShowInfo(mInfo.id);
                break;
            case "appointBtn":
                bool active = panel.appoint.gameObject.activeSelf;
               panel.appoint.gameObject.SetActive(!active);
                break;
            case "expelBtn":
                if(mInfo.power>=4)
                {
                    GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err27"));
                    return;
                }
                string title = TextManager.GetUIString("UI_Guildoffice22");
                string content = TextManager.GetUIString("UI_Guildoffice23");
                GUIManager.SetPromptInfoChoose(title, content, SureExpel);
                break;
            case "transferBtn":
                string title1 = TextManager.GetUIString("UI_GuildPower1");
                string content1 = TextManager.GetUIString("UI_Guildoffice24");
                GUIManager.SetPromptInfoChoose(title1, content1, SureTransferPower);
                break;
        }
    }
    //确定开除
    private void SureExpel()
    {
        ServerCustom.instance.SendClientMethods(GuildProxy.OnClientKickOut, mInfo.id);
    }
    //转让公会 
    private void SureTransferPower()
    {
        ServerCustom.instance.SendClientMethods(GuildProxy.OnClientGuildTransfer, mInfo.id);
    }
    private void OnClickItem(UIGridItem item )
    {
        PowerInfo info = item.oData as PowerInfo;
        if(info.count == info.totalNum)
        {
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err21"));
            return;
        }
        if(PlayerMediator.playerInfo.guildPower<= info.power)
        {
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err11"));
            return;
        }
        ServerCustom.instance.SendClientMethods(GuildProxy.OnClientAppointPower,mInfo.id,info.power);
    }
    //获取职位数量
    private int GetGuildPowerNum(int power)
    {
        int num = 0;

        for(int i=0; i<GuildMainMediator.mMyGuild.memberList.Count; i++)
        {
            GuildMemberInfo info = GuildMainMediator.mMyGuild.memberList[i];
            if (info.power == power)
                num++;
        }

        return num;
    }
    class PowerInfo
    {
        public int power;
        public int count;
        public int totalNum;
    }
}
