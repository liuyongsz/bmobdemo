using System;
using PureMVC.Interfaces;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using AssetBundles;
using UnityEngine.Events;

public class UpdateResourcesMediator : UIMediator<ui_update_resources>
{
    public static ui_update_resources panel;

    [HideInInspector]
    public string WarnMsg;
    [HideInInspector]
    public string SpeedMsg;

    public UpdateResourcesMediator() : base("ui_update_resources")
    {
        RegistPanelCall(NotificationID.UpdateResources_Open, OpenPanel);
        RegistPanelCall(NotificationID.UpdateResources_Close, ClosePanel);

        RegistPanelCall(NotificationID.UPDATE_MESSAGE,  OnRec_Info);
        RegistPanelCall(NotificationID.UPDATE_EXTRACT,  OnRec_Extrack);
        RegistPanelCall(NotificationID.UPDATE_DOWNLOAD, OnRec_Download);
        RegistPanelCall(NotificationID.UPDATE_PROGRESS, OnRec_Progress);

       
    }

    protected override void ClosePanel(INotification notification)
    {
        if(null != panel)
        {
            GameObject.Destroy(panel.gameObject);
            panel = null;
        }
    }

    protected override void OnShow(INotification notification)
    {
        SetDisplay();
    }

    /// <summary>更新消息</summary>
    private void OnRec_Info(INotification msg)
    {
        WarnMsg = msg.Body as string;

        SetDisplay();
    }

    /// <summary>更新解包</summary>
    private void OnRec_Extrack(INotification msg)
    {
        SpeedMsg = msg.Body as string;
    }

    /// <summary>更新下载</summary>
    private void OnRec_Download(INotification msg)
    {

    }

    /// <summary>更新进度</summary>
    private void OnRec_Progress(INotification msg)
    {
        SpeedMsg = SpeedMsg = msg.Body as string;

        SetDisplay();
    }

    private void SetDisplay()
    {
        if (null == panel) return;

        panel.SpeedMsg = SpeedMsg;
        panel.WarnMsg = WarnMsg;
    }
}