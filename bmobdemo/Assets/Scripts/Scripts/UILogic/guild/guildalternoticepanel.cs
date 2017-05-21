using UnityEngine;
using PureMVC.Interfaces;
using System.Collections;

public class guildalternoticepanel : BasePanel
{
    public UIButton off_btn;
    public UIInput inputintro;
    public UILabel intorhint;
    public UIInput inputnotice;
    public UILabel noticehint;
    public UIButton keep_btn;
}
public class GuildAlterNoticeMediator : UIMediator<guildalternoticepanel>
{
    private guildalternoticepanel panel
    {
        get
        {
            return m_Panel as guildalternoticepanel;
        }
    }
    public static GuildAlterNoticeMediator guildalternameMediator;
    private string newintro = string.Empty;
    private string newnotice = string.Empty;
    private GuildBase mGuildBase;

    PlayerInfo playerinfo = PlayerMediator.playerInfo;
    public GuildAlterNoticeMediator() : base("guildalternoticepanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.GuildAlterNotice_Show, OpenPanel);
        RegistPanelCall(NotificationID.GuildAlterNotice_Hide, ClosePanel);
    }

    protected override void OnShow(INotification notification)
    {
        if (guildalternameMediator == null)
        {
            guildalternameMediator = Facade.RetrieveMediator("GuildAlterNoticeMediator") as GuildAlterNoticeMediator;
        }

        mGuildBase = GuildBaseConfig.GetGuildBase(1);
        InitNoticeInro();

    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.off_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.keep_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.inputintro.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.inputnotice.gameObject).onClick = OnClick;
        EventDelegate.Add(panel.inputintro.onChange, IntroValueChanged);
        EventDelegate.Add(panel.inputnotice.onChange, IntroValueChanged);

    }
    private void InitNoticeInro()
    {

        panel.inputnotice.characterLimit = mGuildBase.noticeLen;
        panel.inputintro.characterLimit = mGuildBase.introductionLen;

        bool isNotice = string.IsNullOrEmpty(GuildMainMediator.mMyGuild.notice);
        panel.noticehint.gameObject.SetActive(isNotice);
        panel.inputnotice.value = GuildMainMediator.mMyGuild.notice;

        bool isIntro = string.IsNullOrEmpty(GuildMainMediator.mMyGuild.introduction);
        panel.intorhint.gameObject.SetActive(isIntro);
        panel.inputintro.value = GuildMainMediator.mMyGuild.introduction;


    }
    private void OnClick(GameObject go)
    {
        if (go == panel.off_btn.gameObject)
        {
            Facade.SendNotification(NotificationID.GuildAlterNotice_Hide);
        }
        else if (go == panel.keep_btn.gameObject)
        {

            newintro = panel.inputintro.value;
            newnotice = panel.inputnotice.value;
            int isChangeIntro = string.IsNullOrEmpty(newintro) ? 0 : 1;
            int isChangeNotice = string.IsNullOrEmpty(newnotice) ? 0 : 1;

            ServerCustom.instance.SendClientMethods(GuildProxy.OnClientChangeNotice, isChangeIntro, newintro, isChangeNotice, newnotice);
            Facade.SendNotification(NotificationID.GuildAlterNotice_Hide);
        }
        else if (go == panel.inputintro.gameObject)
        {
            panel.intorhint.gameObject.SetActive(false);
        }
        else if (go == panel.inputnotice.gameObject)
        {
            panel.noticehint.gameObject.SetActive(false);
        }
    }

    void IntroValueChanged()
    {
        int introMAX = GuildBaseConfig.GetGuildBase(1).introductionLen;
        int noticeMAX = GuildBaseConfig.GetGuildBase(1).noticeLen;
        if (panel.inputintro.value.Length >= introMAX||panel.inputnotice.value.Length>=noticeMAX)
            GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_31"), null);

    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    
    }
}