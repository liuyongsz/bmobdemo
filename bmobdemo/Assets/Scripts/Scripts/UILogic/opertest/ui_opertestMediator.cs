using PureMVC.Interfaces;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using AssetBundles;
using System.Collections.Generic;

/// <summary>
/// 登录页面
/// </summary>
public class ui_opertestMediator : UIMediator<ui_opertest> {

    public ui_opertestMediator() : base("ui_opertest") {

        RegistPanelCall(NotificationID.OPERTEST_OPEN, base.OpenPanel);
        RegistPanelCall(NotificationID.OPERTEST_CLOSE, base.ClosePanel);
    }
  
    
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(m_Panel.btnPass.gameObject).onClick = OnClick_Pass;
        UIEventListener.Get(m_Panel.btnRndSel.gameObject).onClick = OnClick_RndSel;
        UIEventListener.Get(m_Panel.btnBallToSelPlayer.gameObject).onClick = OnClick_BallToSelPlayer;
        UIEventListener.Get(m_Panel.btnRndPass.gameObject).onClick = OnClick_RndPass;
        UIEventListener.Get(m_Panel.btnSel0.gameObject).onClick = OnClick_Sel0;
        UIEventListener.Get(m_Panel.btnMove.gameObject).onClick = OnClick_Move;
        UIEventListener.Get(m_Panel.btnMoveToMid.gameObject).onClick = OnClick_MoveToMid;

        UIEventListener.Get(m_Panel.btnMidToEnd.gameObject).onClick = OnClick_MidToEnd;
        UIEventListener.Get(m_Panel.btnEndTo.gameObject).onClick = OnClick_EndTo;
    }

    protected override void OnShow(INotification notification)
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
       
    }

    private void OnClick_MoveToMid(GameObject go)
    {
        GameObject player = GameObject.Find("Player_Team0");

        CommonFun.SetSelectedPlayer(player);
        CommonFun.BallToPlayerFoot();

        player = GameObject.Find("Player_Opponent2");
    }

    private void OnClick_MidToEnd(GameObject go)
    {
        GameObject player = GameObject.Find("Player_Team2");

    }

    private void OnClick_EndTo(GameObject go)
    {
        Facade.SendNotification(NotificationID.BALLOPER_OPEN);
    }

    private void OnClick_Sel0(GameObject go)
    {
        GameObject player = GameObject.Find("Player_TeamTest");

        CommonFun.SetSelectedPlayer(player);
        CommonFun.BallToPlayerFoot();
    }

    private void OnClick_RndSel(GameObject go)
    {
        CommonFun.RandomSelectedPlayer();
    }

    private void OnClick_Pass(GameObject go)
    {

    }

    private void OnClick_BallToSelPlayer(GameObject go)
    {
        CommonFun.BallToPlayerFoot();
    }

    private void OnClick_RndPass(GameObject go)
    {
        int count = CommonFun.Players.Count;

        int rndIndex = UnityEngine.Random.Range(0, count);

        GameObject player = CommonFun.Players[rndIndex];
    }

    private void OnClick_Move(GameObject go)
    {
        GameObject toTarget = GameObject.Find("test_target");

    }
}
