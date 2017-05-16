using UnityEngine;
using System.Collections;
using PureMVC.Interfaces;
using System;

public class guildalterinfopanel : BasePanel
{
    public UIButton off_btn;
    public UITexture emblem;           //会徽
    public UILabel guildname;
    public UIButton alter_btn;
    public UIButton system_btn;
    public UIButton native_btn;
}
public class GuildAlterInfoMediator : UIMediator<guildalterinfopanel>
{
    private guildalterinfopanel panel
    {
        get
        {
            return m_Panel as guildalterinfopanel;
        }
    }
    public static GuildAlterInfoMediator guildalterinfoMediator;

    public GuildAlterInfoMediator() : base("guildalterinfopanel")
    {
        m_isprop = false;
        RegistPanelCall(NotificationID.GuildAlterInfo_Show, OpenPanel);
        RegistPanelCall(NotificationID.GuildAlterInfo_Hide, ClosePanel);
    }

    protected override void OnShow(INotification notification)
    {
        if (guildalterinfoMediator == null)
        {
            guildalterinfoMediator = Facade.RetrieveMediator("GuildAlterInfoMediator") as GuildAlterInfoMediator;
        }
        panel.guildname.text = GuildMainMediator.mMyGuild.name;
    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.off_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.alter_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.system_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.native_btn.gameObject).onClick = OnClick;

    }

    private void OnClick(GameObject go)
    {
        if (go == panel.off_btn.gameObject)
        {
            Facade.SendNotification(NotificationID.GuildAlterInfo_Hide);
        }
        else if (go == panel.alter_btn.gameObject) 
        {
            Facade.SendNotification(NotificationID.GuildAlterName_Show);
        }
        else if (go == panel.system_btn.gameObject)
        {

        }
        else if (go == panel.native_btn.gameObject)
        {

        }
    }
}
