using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using PureMVC.Patterns;
using PureMVC.Interfaces;

public class EnterPanel : MonoBehaviour
{
    private UITexture matchBtn;
    private UITexture arenaBtn;
    private UITexture worldBossBtn;
    private UITexture unionBtn;
    private UISprite backBtn;

    void Awake()
    {
        matchBtn = UtilTools.GetChild<UITexture>(this.transform, "matchBtn");
        arenaBtn = UtilTools.GetChild<UITexture>(this.transform, "arenaBtn");
        worldBossBtn = UtilTools.GetChild<UITexture>(this.transform, "worldBossBtn");
        unionBtn = UtilTools.GetChild<UITexture>(this.transform, "unionBtn");
        backBtn = UtilTools.GetChild<UISprite>(this.transform, "backBtn");
    }
    void Start()
    {
        UIEventListener.Get(matchBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(arenaBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(worldBossBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(unionBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(backBtn.gameObject).onClick = OnClick;
    }
    void OnClick(GameObject go)
    {
        if (go == arenaBtn.gameObject)
        {
            Facade.Instance.SendNotification(NotificationID.Arena_Show);
            MonoBehaviour.Destroy(this.gameObject);
        }
        else if (go == backBtn.gameObject)
        {
            MonoBehaviour.Destroy(this.gameObject);
        }
        else if (go == worldBossBtn.gameObject)
        {
            Debug.Log(DateTime.Now.ToString("yyyy-M-d"));
            ServerCustom.instance.SendClientMethods(GuildProxy.OnClientAdviserList);
        }
    }
    void OnDestroy()
    {
        matchBtn = null;
        arenaBtn = null;
        worldBossBtn = null;
        unionBtn = null;
        backBtn = null;
    }
}
