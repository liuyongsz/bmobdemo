using UnityEngine;
using System.Collections;
using PureMVC.Interfaces;
using System.Collections.Generic;
using AssetBundles;
using System;

public class battleendpanel: BasePanel
{
    public Transform defeated;
    public Transform victorypanel;

    public UILabel txtWinScore;
    public UILabel txtFailScore;

    public UISprite bg;
}
public class BattleEndMediator : UIMediator<battleendpanel>
{

    LevelRewardInfo levelinfo;
    TD_CloneLevel chapterinfo;

    private bool m_win;

    public static BattleEndMediator battleEndMediator;
    private battleendpanel panel
    {
        get
        {
            return m_Panel as battleendpanel;
        }
    }

    public BattleEndMediator() : base("battleendpanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.BattleEnd_Show, OpenPanel);
        RegistPanelCall(NotificationID.BattleEnd_Hide, ClosePanel);
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void AddComponentEvents()
    {

        UIEventListener.Get(m_Panel.bg.gameObject).onClick = OnClick_Back;
    }

    private void OnClick_Back(GameObject go)
    {
        CloneProxy.Instance.onClientLeaveClone();
        GameProxy.Instance.GotoMainCity();
    }

    private void OnClick(GameObject go)
    {
        
    }

    protected override void OnShow(INotification notification)
    {
        m_win = (bool)notification.Body;

        UpdateDisplay();
    }

    /// <summary>更新显示</summary>
    private void UpdateDisplay()
    {
        m_Panel.defeated.gameObject.SetActive(!m_win);
        m_Panel.victorypanel.gameObject.SetActive(m_win);

        if (m_win)
        {
            m_Panel.txtWinScore.text = MatchManager.LeftGoalNum + ":" + MatchManager.RightGoalNum;
        }
        else
        {
            m_Panel.txtFailScore.text = MatchManager.LeftGoalNum + ":" + MatchManager.RightGoalNum;
        }
    }

    void UpdateGoodsItem(UIGridItem item)
    {
      

    }
    private void SetStar()
    {
       
    }
   
}