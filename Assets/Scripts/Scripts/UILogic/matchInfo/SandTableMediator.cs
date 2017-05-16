using PureMVC.Interfaces;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using AssetBundles;
using System.Collections.Generic;
using DG.Tweening;

public class ui_sandtable : BasePanel
{
    public UITexture imgBg;
}

/// <summary>
/// 比赛信息
/// </summary>
public class SandTableMediator: UIMediator<ui_sandtable> {

   

    public SandTableMediator() : base("ui_sandtable") {

        RegistPanelCall(NotificationID.SandTable_Show, base.OpenPanel);
        RegistPanelCall(NotificationID.SandTable_Hide, base.ClosePanel);
    }   
    
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(m_Panel.imgBg.gameObject).onClick = OnClick_CloseBtn;
    }

    protected override void OnShow(INotification notification)
    {
       
    }

    private void OnClick_CloseBtn(GameObject obj)
    {
        MatchManager.DirectRoundMatch();

        ClosePanel(null);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
