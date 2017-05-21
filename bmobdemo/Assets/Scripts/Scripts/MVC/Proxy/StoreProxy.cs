using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProxyInstance;
/// <summary>
/// ил╣Й
/// </summary>
public class StoreProxy : Proxy<StoreProxy>
{

    public string userId;
    public string userPwd;

    public StoreProxy()
        : base(ProxyID.Store)
    {
        KBEngine.Event.registerOut(this, "onBuyDiamondSucess");
    }

    public void onBuyDiamondSucess(object obj)
    {
        GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_shop_2")); 
    }
}
