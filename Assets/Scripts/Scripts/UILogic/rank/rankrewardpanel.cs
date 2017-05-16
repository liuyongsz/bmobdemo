using UnityEngine;
using System.Collections;
using PureMVC.Interfaces;

public enum RankRewardType
{
    Preview,//预览
    Settlement,//结算
}

public class rankrewardpanel : BasePanel
{

	
}
public class RankRewardMediator : UIMediator<rankrewardpanel>
{
    public rankrewardpanel panel
    {
        get
        {
            return m_Panel as rankrewardpanel;
        }
    }

    public RankRewardMediator() : base("rankrewardpanel")
    {
        m_isprop = false;
        RegistPanelCall(NotificationID.RankReward_Show, OpenPanel);
        RegistPanelCall(NotificationID.RankReward_Hide, ClosePanel);
    }
    /// <summary>
    /// 界面显示
    /// </summary>
    /// <param name="notification"></param>
    protected override void OnShow(INotification notification)
    {

    }
    /// <summary>
    /// 界面关闭时调用，释放内存
    /// </summary>
    protected override void OnDestroy()
    {

    }
    protected override void AddComponentEvents()
    {

    }
}
