using UnityEngine;
using System.Collections;
using PureMVC.Interfaces;
using System;

public class guildspeedpanel : BasePanel
{
    public UIButton sub_btn;
    public UIButton add_btn;
    public UIButton off_btn;
    public UIButton sure_btn;

    public UILabel hour;
    public UILabel needmoney;
    public UILabel timeLable;
    public UILabel buildname;

}
public class GuildSpeedMediator : UIMediator<guildspeedpanel>
{
    private guildspeedpanel panel
    {
        get
        {
            return m_Panel as guildspeedpanel;
        }
    }
    private int mMinTime = 1;
    private int mSpeedTime = 1;
    private GuildBuildInfo mGuildBuildInfo;
    private string mTimerUpgradeKey = "speedUpgradeTimerKey";
    public static GuildSpeedMediator guildspeedMediator;
    private GuildBase mGuildBase = null;
    public GuildSpeedMediator() : base("guildspeedpanel")
    {
        m_isprop = true;

        RegistPanelCall(NotificationID.GuildSpeed_Hide, ClosePanel);
        RegistPanelCall(NotificationID.GuildSpeed_Show, OpenPanel);
    }
    protected override void OnShow(INotification notification)
    {
        if (guildspeedMediator == null)
        {
            guildspeedMediator = Facade.RetrieveMediator("GuildSpeedMediator") as GuildSpeedMediator;
        }

        mGuildBuildInfo = notification.Body as GuildBuildInfo;
        mGuildBase = GuildBaseConfig.GetGuildBase(1);
        panel.buildname.text = TextManager.GetUIString("UIGuildBuild"+ mGuildBuildInfo.id)  ;
        SetUpgradeTime();
        SetSpeedInfo();
    }

    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.off_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.sure_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.sub_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.add_btn.gameObject).onClick = OnClick;
    }

    private void SetUpgradeTime()
    {
        TimeSpan timeSpan = (DateTime.Now - new DateTime(1970, 1, 1));
        long time = (long)timeSpan.TotalSeconds - 8 * 60 * 60;

        long leftTime = mGuildBuildInfo.endTimes - time;

        TimerManager.Destroy(mTimerUpgradeKey);
        TimerManager.AddTimerRepeat(mTimerUpgradeKey, 1, delegate (object[] obj)
        {
            if (leftTime <= 0)
            {
                leftTime = 0;
                TimerManager.Destroy(mTimerUpgradeKey);
                panel.timeLable.text = "";
            }
            else
            {
                leftTime--;
                panel.timeLable.text = UtilTools.formatDuring(leftTime);
            }

        });


    }
    private void OnClick(GameObject go)
    {
        switch (go.transform.name)
        {
            case "off_btn":
                Facade.SendNotification(NotificationID.GuildSpeed_Hide);
                break;
            case "sure_btn":
                int totalFunds = GuildMainMediator.mMyGuild.guildFunds;
                int needFunds = mSpeedTime * mGuildBase.speedTimeFunds;
                if(needFunds>totalFunds)
                {
                    GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_guild_err20"), null);
                    return;
                }

                ServerCustom.instance.SendClientMethods(GuildProxy.OnClientBuildSpeed, mGuildBuildInfo.id,mSpeedTime);

                Facade.SendNotification(NotificationID.GuildSpeed_Hide);

                break;
            case "sub_btn":
                if (mSpeedTime > 1)
                {
                    mSpeedTime--;
                    SetSpeedInfo();
                }
                break;
            case "add_btn":
                mSpeedTime++;
                SetSpeedInfo();
                break;
        }
    }
    private void SetSpeedInfo()
    {
        panel.hour.text = mSpeedTime.ToString();
        panel.needmoney.text = (mSpeedTime * mGuildBase.speedTimeFunds).ToString();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TimerManager.Destroy(mTimerUpgradeKey);
        mSpeedTime = 1;
    }
}