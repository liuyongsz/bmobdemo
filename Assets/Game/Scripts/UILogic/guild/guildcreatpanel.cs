using UnityEngine;
using System.Collections;
using PureMVC.Interfaces;
using System.Collections.Generic;

public class guildcreatpanel : BasePanel
{
    public UITexture emblem;         //会徽
    public UIInput inputname;        //公会名称
    public UIInput inputintro;       //公会简介
    public UIButton offcreate_btn;
    public UIButton sure_btn;
    public UIButton gameicon;
    public UIButton palyericon;

}
public class GuildCreatMediator : UIMediator<guildcreatpanel>
{
    private guildcreatpanel panel
    {
        get
        {
            return m_Panel as guildcreatpanel;
        }
    }

    public static GuildCreatMediator guildcreatMediator;
    private GuildBase mGuildBase;

    public GuildCreatMediator() : base("guildcreatpanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.GuildCreat_Show, OpenPanel);
        RegistPanelCall(NotificationID.GuildCreat_Hide, ClosePanel);
    }
    private string guildname = string.Empty;
    private string guildintro = string.Empty;
    protected override void OnShow(INotification notification)
    {

        if (guildcreatMediator == null)
        {
            guildcreatMediator = Facade.RetrieveMediator("GuildCreatMediator") as GuildCreatMediator;
        }
        mGuildBase = GuildBaseConfig.GetGuildBase(1);
        panel.inputname.characterLimit = mGuildBase.nameLenMax;
        panel.inputintro.characterLimit = mGuildBase.introductionLen;

    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.offcreate_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.sure_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.gameicon.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.palyericon.gameObject).onClick = OnClick;
        EventDelegate.Add(panel.inputintro.onChange, IntroValueChanged);
        EventDelegate.Add(panel.inputname.onChange, NameValueChanged);

    }
    private void OnClick(GameObject go)
    {
        if (go == panel.offcreate_btn.gameObject)
        {
            Facade.SendNotification(NotificationID.GuildCreat_Hide);
        }
        else if (go == panel.sure_btn.gameObject)
        {
            guildname = panel.inputname.value;
            guildintro = panel.inputintro.value;

            int maxleng = mGuildBase.nameLenMax;
            int minleng = mGuildBase.nameLenMin;
            foreach (GuildInfo info in GuildBaseConfig.mGuildList)
            {
                if (info.guildName == panel.inputname.value)
                {
                    GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err25"));
                    return;
                }
            }
            if (panel.inputname.value.Length > maxleng)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err23"));
                return;
            }
            if (panel.inputname.value.Length < minleng)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err22"));
                return;
            }

            if(PlayerMediator.playerInfo.diamond< mGuildBase.createNeedDiamond)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err3"));
                return;
            }
            ServerCustom.instance.SendClientMethods(GuildProxy.OnClientCreateGuild,guildname, guildintro);
            
        }
        else if (go == panel.gameicon.gameObject)
        {
            //----------------------
        }
        else if (go == panel.palyericon.gameObject)
        {
            //----------------------
        }
    }
    /// <summary>
    /// 限制简介字数
    /// </summary>
    void IntroValueChanged()
    {
        int maxleng = GuildBaseConfig.GetGuildBase(1).noticeLen;
        if (panel.inputintro.value.Length >= maxleng)
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_31"));
    }
    /// <summary>
    /// 限制公会名字
    /// </summary>
    void NameValueChanged()
    {
        

        //if (panel.inputname.value.Length >= maxleng)
        //{
        //    GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err23"));
        //    return;            
        //}
        //if (panel.inputname.value.Length <=minleng)
        //{
        //    GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err22"));
        //    return;
        //}
    }
    protected override void OnDestroy()
    {
        EventDelegate.Remove(panel.inputintro.onChange, IntroValueChanged);
        EventDelegate.Remove(panel.inputintro.onChange, NameValueChanged);
        base.OnDestroy();
    }
}
