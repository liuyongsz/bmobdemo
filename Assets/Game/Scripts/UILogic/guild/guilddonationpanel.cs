using UnityEngine;
using System.Collections;
using PureMVC.Interfaces;
using System;
using System.Collections.Generic;

public class guilddonationpanel : BasePanel
{
    public UIButton off_btn;
    public UIButton donation_btn;
    public UIGrid donationGrid;
    public UILabel myEuro_label;
    public UILabel gongxian_label;
    public UILabel guildmoney_label;
    public UIButton sub_btn;
    public UILabel count_label;
    public UISprite add_btn;
    public UILabel money_label;
    public UILabel nextLVneed;
    public UILabel reminderLabel;
}
public class GuildDonationMediator : UIMediator<guilddonationpanel>
{
    public guilddonationpanel panel
    {
        get
        {
            return m_Panel as guilddonationpanel;
        }
    }
    public static GuildDonationMediator guilddonationMediator;

    private GuildBuildInfo mBuildInfo = null;
    private GuildBase mGuildBase = null;
    public MyGuildInfo mMyGuildInfo = GuildMainMediator.mMyGuild;
    public int moneycount = 1;
    int contribution = PlayerMediator.playerInfo.guildDonate;

    public GuildDonationMediator() : base("guilddonationpanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.GuildDonation_Show, OpenPanel);
        RegistPanelCall(NotificationID.GuildDonation_Hide, ClosePanel);
    }

    protected override void OnShow(INotification notification)
    {
        if (guilddonationMediator == null)
        {
            guilddonationMediator = Facade.RetrieveMediator("GuildDonationMediator") as GuildDonationMediator;
        }

        mBuildInfo = notification.Body as GuildBuildInfo;
        mGuildBase = GuildBaseConfig.GetGuildBase(1);

        panel.donationGrid.enabled = true;
        panel.donationGrid.BindCustomCallBack(UpdateDonationGridItem);
        panel.donationGrid.StartCustom();

        if (GuildMainMediator.mMyGuild.memberList.Count <= 0)
            OnClientGetGuildMember();
        

        SetDonationInfo();
        SetDayDonateData();
    }
    //设置贡献列表
    public void SetDayDonateData()
    {

        if (GuildMainMediator.mMyGuild.memberList.Count <= 0)
            return;

        List<object> listObj = new List<object>();
        List<GuildMemberInfo> memberList = GuildMainMediator.mMyGuild.memberList;
        memberList.Sort(CompareDayDonate);
        for (int i = 0; i < memberList.Count; i++)
        {
            if (memberList[i].dayDonate > 0)
                listObj.Add(memberList[i]);
        }
        panel.reminderLabel.gameObject.SetActive(listObj.Count == 0);        
        panel.donationGrid.AddCustomDataList(listObj);
    }
    private void UpdateDonationGridItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        GuildMemberInfo info = item.oData as GuildMemberInfo;
        item.onClick = OnClickItem;
        UILabel name = item.mScripts[0] as UILabel;
        UILabel money = item.mScripts[1] as UILabel;
        name.text = info.playerName;
        money.text = info.dayDonate.ToString();          
    }

    private void OnClickItem(UIGridItem item)
    {
        GuildMemberInfo info = item.oData as GuildMemberInfo;
        UtilTools.RoleShowInfo(info.id);

    }


    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.off_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.donation_btn.gameObject).onClick = OnClick;
        LongClickEvent.Get(panel.sub_btn.gameObject).onPress = OnPress;
        LongClickEvent.Get(panel.add_btn.gameObject).onPress = OnPress;
        LongClickEvent.Get(panel.sub_btn.gameObject).duration = 4f;
        LongClickEvent.Get(panel.add_btn.gameObject).duration = 4f;
    }

    private void OnClick(GameObject go)
    {
        
        if (go==panel.off_btn.gameObject)
        {
            Facade.SendNotification(NotificationID.GuildDonation_Hide);
        }    
        else if (go == panel.donation_btn.gameObject)
        {
            int needEuro = moneycount * mGuildBase.euroPer;
          
            if (needEuro > PlayerMediator.playerInfo.euro)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err18"));
                return;
            }
            ServerCustom.instance.SendClientMethods(GuildProxy.OnClientDonate, moneycount);

            moneycount = 1;
            panel.count_label.text = moneycount.ToString();
            panel.money_label.text = moneycount * mGuildBase.euroPer + "";
        }
    }
    void OnPress(GameObject go, bool pressed)
    {
        if (go == panel.add_btn.gameObject)
        {
            LongClickEvent.Get(panel.add_btn.gameObject).time = 0;
            if (moneycount == 999)
                return;
            moneycount++;
            panel.count_label.text = moneycount.ToString();
            panel.money_label.text = moneycount * mGuildBase.euroPer + "";
        }
        else if(go == panel.sub_btn.gameObject)
        {
            LongClickEvent.Get(panel.sub_btn.gameObject).time = 0;
            if (moneycount <= 1)
                return;
            moneycount--;
            panel.count_label.text = moneycount.ToString();
            panel.money_label.text = moneycount * mGuildBase.euroPer + "";
        }
    }
    public void SetDonationInfo()
    {
       

        panel.myEuro_label.text = PlayerMediator.playerInfo.euro.ToString();
        panel.gongxian_label.text = PlayerMediator.playerInfo.guildDonate.ToString();
        panel.guildmoney_label.text = mMyGuildInfo.guildFunds.ToString();
    }
    //日贡献
    private int CompareDayDonate(GuildMemberInfo info1, GuildMemberInfo info2)
    {
        if (info1.dayDonate > info2.dayDonate)
            return -1;
        else if (info1.dayDonate < info2.dayDonate)
            return 1;
        else if (info1.onlineState < info2.onlineState)
            return -1;
        else if (info1.onlineState > info2.onlineState)
            return 1;
        else
            return 0;
    }


    //客户端请求自己公会成员列表
    private void OnClientGetGuildMember()
    {
        ServerCustom.instance.SendClientMethods(GuildProxy.OnClientGetGuildMember);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        moneycount = 1;
    }
}
