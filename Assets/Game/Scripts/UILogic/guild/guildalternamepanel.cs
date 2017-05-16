using UnityEngine;
using System.Collections;
using PureMVC.Interfaces;
using System;

public class guildalternamepanel : BasePanel
{
    public UIButton off_btn;
    public UIInput inputnewname;
    public UILabel hint;
    public UIButton sure_btn;
}
public class GuildAlterNameMediator : UIMediator<guildalternamepanel>
{
    private guildalternamepanel panel
    {
        get
        {
            return m_Panel as guildalternamepanel;
        }
    }
    private GuildBase mGuildBase;
    public static GuildAlterNameMediator guildalternameMediator;
    private string guildnewname = string.Empty;
    PlayerInfo playerinfo = PlayerMediator.playerInfo;
    public GuildAlterNameMediator() : base("guildalternamepanel")
    {
        m_isprop = false;
        RegistPanelCall(NotificationID.GuildAlterName_Show, OpenPanel);
        RegistPanelCall(NotificationID.GuildAlterName_Hide, ClosePanel);
    }

    protected override void OnShow(INotification notification)
    {
        if (guildalternameMediator == null)
        {
            guildalternameMediator = Facade.RetrieveMediator("GuildAlterNameMediator") as GuildAlterNameMediator;
        }
        mGuildBase = GuildBaseConfig.GetGuildBase(1);
        panel.hint.gameObject.SetActive(true);
        panel.inputnewname.characterLimit = mGuildBase.nameLenMax;
        UILabel needLabel = panel.sure_btn.transform.FindChild("Label").GetComponent<UILabel>();
        needLabel.text = mGuildBase.changeNameDiamond.ToString();
    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.off_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.sure_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.inputnewname.gameObject).onClick = OnClick;
        //EventDelegate.Add(panel.inputnewname.onChange, IntroValueChanged);

    }

    private void OnClick(GameObject go)
    {
        if (go==panel.off_btn.gameObject)
        {
            Facade.SendNotification(NotificationID.GuildAlterName_Hide);
        }
        else if (go==panel.sure_btn.gameObject)
        {
            guildnewname = panel.inputnewname.value;

            foreach (GuildInfo info in GuildBaseConfig.mGuildList)
            {
                if (info.guildName == guildnewname)
                {
                    GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err25"));
                    return;
                }
            }
            if (guildnewname.Length < mGuildBase.nameLenMin)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err22"));
                return;
            }
            int needDiamond = mGuildBase.changeNameDiamond;
            if (playerinfo.diamond< needDiamond)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_31"));
            }
    
            ServerCustom.instance.SendClientMethods(GuildProxy.OnClientChangeName, guildnewname);
            Facade.SendNotification(NotificationID.GuildAlterName_Hide);
        }
        else if (go == panel.inputnewname.gameObject)
        {
            panel.hint.gameObject.SetActive(false);
        }
    }

    void IntroValueChanged()
    {
        int maxleng = mGuildBase.nameLenMax;
        if (panel.inputnewname.value.Length > maxleng)
        {
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_31"));
            guildnewname = panel.inputnewname.value;
            panel.inputnewname.value = guildnewname.Substring(0, maxleng);
        }
    }
}
