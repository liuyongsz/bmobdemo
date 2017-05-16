using UnityEngine;
using System.Collections.Generic;
using PureMVC.Interfaces;

public class worldbosspanel : BasePanel
{
    public UILabel todayBossName;
    public UILabel todayBossGuild;
    public UILabel todayBossState;
    public UISprite previewBossBtn;
    public UISprite previeRewardBtn;
}
public class WorldBossMediator : UIMediator<worldbosspanel>
{
    public static AdviserInfo adviserInfo;
    public worldbosspanel panel
    {
        get
        {
            return m_Panel as worldbosspanel;
        }
    }

    public WorldBossMediator() : base("worldbosspanel")
    {
        m_isprop = false;
        RegistPanelCall(NotificationID.WorldBoss_Show, OpenPanel);
        RegistPanelCall(NotificationID.WorldBoss_Hide, ClosePanel);
    }

    /// <summary>
    /// 界面显示
    /// </summary>
    /// <param name="notification"></param>
    protected override void OnShow(INotification notification)
    {
        adviserInfo = GetAdviserInfoByConfigID(1);
        if (adviserInfo == null)
            return;
       
    }

    /// <summary>
    /// 界面关闭时调用，释放内存
    /// </summary>
    protected override void OnDestroy()
    {

    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.previewBossBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.previeRewardBtn.gameObject).onClick = OnClick;
    }
    private void OnClick(GameObject go)
    {
        if (go == panel.previewBossBtn.gameObject)
        {

        }
        else if (go == panel.previeRewardBtn.gameObject)
        {
            
        }
    }

    public static AdviserInfo GetAdviserInfoByConfigID(int id)
    {
        foreach (AdviserInfo item in GuildCounselorConfig.mAdviserDict.Values)
        {
            if (item.configID == id)
                return item;
        }
        return null;
    }
}
