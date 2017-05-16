using System;
using PureMVC.Interfaces;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using AssetBundles;
using UnityEngine.Events;
using LuaFramework;

public class ui_update_resources : BasePanel
{
    public UILabel txtWarn;
    public UILabel txtSpeed;
    public UIProgressBar slider_progress;

    [HideInInspector]
    public string WarnMsg;
    [HideInInspector]
    public string SpeedMsg;
    public void Start()
    {
        //RectTransform tran = transform as RectTransform;
        //transform.localPosition = Vector3.zero;
        //transform.localRotation = Quaternion.Euler(0, 0, 0);
        //tran.offsetMin = Vector2.zero;
        //tran.offsetMax = Vector2.zero;
        //tran.localScale = Vector3.one;

        UpdateResourcesMediator.panel = this;
    }

    public void Update()
    {
        if (txtWarn == null) return;

        txtWarn.text = WarnMsg;
        txtSpeed.text = SpeedMsg;
        slider_progress.value = ThreadManager.Instance.LoadingProgress;
    }
}