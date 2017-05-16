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
        BabyMediator.babyInfo.liking = int.Parse(liking.ToString());
        BabyMediator.babyInfo.fullTime = int.Parse(fullTime.ToString());
        BabyMediator.babyInfo.closeTouch = int.Parse(closeTouch.ToString());
        BabyMediator.babyInfo.itemTouch = int.Parse(itemTouch.ToString());
        BabyMediator.babyInfo.likingTime = int.Parse(likingTime.ToString());
        BabyMediator.babyInfo.getRewardList.Clear();
        List<object> getList = list as List<object>;
        for (int i = 0; i < getList.Count; ++i)
        {
            BabyMediator.babyInfo.getRewardList.Add(int.Parse(getList[i].ToString()));
        }      
        if (ServerCustom.instance.fromOtherBtn)
            Facade.SendNotification(NotificationID.Team_Show);
    }
    public void onGetBabyItemInfo(List<object> list)
    {
        BabyMediator.babyInfo.clothesInfoList.Clear();
        ClothesInfo clothes;
        for (int i = 0; i < list.Count; ++i)
        {
            Dictionary<string, object> Info = list[i] as Dictionary<string, object>;
            int configID = int.Parse(Info["configID"].ToString());
            clothes = ClothesConfig.GetClothesByID(configID);
            if (clothes == null)
            {
                LogSystem.LogError(configID, "not in pei zhi!!!");
                continue;
            }
            clothes.isWear = int.Parse(Info["isWear"].ToString());
            clothes.level = int.Parse(Info["level"].ToString());
            clothes.star = int.Parse(Info["star"].ToString());
            clothes.exp = int.Parse(Info["exp"].ToString());
            if (BabyMediator.babyInfo.clothesInfoList.ContainsKey(clothes.configID))
                BabyMediator.babyInfo.clothesInfoList[clothes.configID] = clothes;
            else
                BabyMediator.babyInfo.clothesInfoList.Add(clothes.configID, clothes);
        }
        if (ServerCustom.instance.fromOtherBtn)
            Facade.SendNotification(NotificationID.Baby_Show, 1);
        else
            Facade.SendNotification(NotificationID.Baby_Show);
    }
    public void onBabyCallBack(object obj)
    {
        int index = UtilTools.IntParse(obj.ToString());
        switch (index)
        {
            case 4:
                GUIManager.SetPromptInfoChoose(TextManager.GetUIString("UICreate1"), TextManager.GetUIString("equip_error_6"), UseCost);
                return;
            case 6:
                BabyMediator.babyMediator.ChangeClothesSucess();
                break;
            case 8:
                BabyMediator.babyMediator.clothesInheritPanel.InheritSuceess();
                break;
            case 10:
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_clothes_12"));
                return;

        }
        GUIManager.SetPromptInfo(TextManager.GetSystemString(UtilTools.StringBuilder("ui_system_clothes_", index)), null);
    }
    public void onTouchSucess(object obj, object obj1)
    {
        int liking = UtilTools.IntParse(obj.ToString());
        int likingTime = UtilTools.IntParse(obj1.ToString());
        BabyMediator.babyMediator.touchPanel.TouchSucess(liking, likingTime);
    }
    void UseCost()
    {
        Facade.SendNotification(NotificationID.Power_Show, GoldType.Euro);
    }
    public void onLevelSucessCallBack(object exp, object level)
    {
        BabyMediator.babyMediator.clothesInfo.level = UtilTools.IntParse(level.ToString());
        BabyMediator.babyMediator.clothesInfo.exp = UtilTools.IntParse(exp.ToString());
        BabyMediator.babyMediator.clothesLevelPanel.LevelSucess();
        GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_clothes_9"), null);
    }
    public void onSlevelSucessCallBack(object star)
    {
        BabyMediator.babyMediator.clothesSlevelPanel.UpdateInfo(UtilTools.IntParse(star.ToString()));
        GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_12"), null);
    }

    public void onGetRewardSuceess(object obj)
    {
        BabyMediator.babyInfo.getRewardList.Add(int.Parse(obj.ToString()));
        if (BabyMediator.babyMediator != null)
            BabyMediator.babyMediator.GetMapRewardSucess(int.Parse(obj.ToString()));
    }
    public void onFullTimeOver(object n, object n1, object obj)
    {
        int isBegin = UtilTools.IntParse(n.ToString());
        BabyMediator.babyInfo.fullTime = UtilTools.IntParse(obj.ToString());
        BabyMediator.babyInfo.liking = UtilTools.IntParse(n1.ToString());
        if (isBegin == 0)
        {
            if (BabyMediator.babyMediator != null)
            {
                TimerManager.AddTimerRepeat("likingTime", 1, BabyMediator.babyMediator.UpdateLikingTime);
            }
        }
        else
        {
            if (BabyMediator.babyMediator != null)
            {
                if (TimerManager.IsHaveTimer("likingTime"))
                    TimerManager.Destroy("likingTime");
                if (!TimerManager.IsHaveTimer("happyTime"))
                {
                    TimeSpan timeSpan = (DateTime.Now - new DateTime(1970, 1, 1));
                    int time = (int)timeSpan.TotalSeconds;
                    BabyMediator.babyMediator.happyTime = 6 * 3600 - (time - BabyMediator.babyInfo.fullTime - 8 * 3600);
                    BabyMediator.babyMediator.desc = TextManager.GetUIString("UIclothes12");
                    TimerManager.AddTimerRepeat("happyTime", 1, BabyMediator.babyMediator.UpdateHappyTime);
                    BabyMediator.babyMediator.touchPanel.UpdateSlider();
                }
            }
        }
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


