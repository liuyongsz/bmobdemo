using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProxyInstance;

/// <summary>
/// πÀŒ ÷Æ’˘
/// </summary>
public class WorldBossProxy : Proxy<WorldBossProxy>
{

    public WorldBossProxy()
        : base(ProxyID.WorldBoss)
    {
        KBEngine.Event.registerOut(this, "onGetWorldBossInfo");
    }
    
    public void onGetWorldBossInfo(Dictionary<string,object> list)
    {
      
    }
}
