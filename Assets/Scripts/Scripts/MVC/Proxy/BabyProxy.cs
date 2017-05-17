using UnityEngine;
using System.Collections;
using PureMVC.Patterns;
using System.Collections.Generic;
using System;

public class BabyProxy : Proxy<BabyProxy>
{

    public BabyProxy()
        : base(ProxyID.Baby)
    {
        KBEngine.Event.registerOut(this, "onGetBabyInfo");
        KBEngine.Event.registerOut(this, "onGetBabyItemInfo"); 
        KBEngine.Event.registerOut(this, "onBabyCallBack");
        KBEngine.Event.registerOut(this, "onLevelSucessCallBack");
        KBEngine.Event.registerOut(this, "onSlevelSucessCallBack");
        KBEngine.Event.registerOut(this, "onTouchSucess");
        KBEngine.Event.registerOut(this, "onFullTimeOver");
        KBEngine.Event.registerOut(this, "onGetRewardSuceess");
    }

    public void onGetBabyInfo(object liking, object fullTime, object closeTouch, object itemTouch, object likingTime,object list)
    {
      
    }
    public void onGetBabyItemInfo(List<object> list)
    {
       
    }
    public void onBabyCallBack(object obj)
    {
       
    }
    public void onTouchSucess(object obj, object obj1)
    {

    }
    void UseCost()
    {
        Facade.SendNotification(NotificationID.Power_Show, GoldType.Euro);
    }
    public void onLevelSucessCallBack(object exp, object level)
    {
    
    }
    public void onSlevelSucessCallBack(object star)
    {
      
    }

    public void onGetRewardSuceess(object obj)
    {
      
    }
    public void onFullTimeOver(object n, object n1, object obj)
    {
       
    }
}

public class BabyInfo
{
    public int liking;
    public int fullTime;
    public int closeTouch;
    public int itemTouch;
    public int likingTime;
    public List<int> getRewardList = new List<int>();
    public Dictionary<int, ClothesInfo> clothesInfoList = new Dictionary<int, ClothesInfo>();
}


