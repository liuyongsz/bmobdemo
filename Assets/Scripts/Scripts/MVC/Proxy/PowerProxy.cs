using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProxyInstance;

public class PowerProxy : Proxy<PowerProxy>
{
    public PowerProxy()
        : base(ProxyID.Power)
    {
        KBEngine.Event.registerOut(this, "onBodyPowerError");
    }


    void OnDestroy()
    {
        KBEngine.Event.deregisterOut(this);
    }
    public void onBodyPowerError(object obj)
    {
        int Index = int.Parse(obj.ToString());
        GUIManager.SetPromptInfo(TextManager.GetSystemString(UtilTools.StringBuilder("ui_system_" + (Index + 26))), null);
    }
}
