using UnityEngine;
using System.Collections.Generic;
using System;

public class TouchPanel : BabyMediator
{
    private UIGrid touchGrid;
    private UILabel liking;
    public UILabel descTime;
    private UISlider likingSlider;
    private UITexture stars;
    private UISprite helpBtn;
    public ItemInfo chooseTouchItem;
    public TouchPanel(GameObject go)
    {
        touchGrid = UtilTools.GetChild<UIGrid>(go.transform, "ScrollView/touchGrid");
        liking = UtilTools.GetChild<UILabel>(go.transform, "likingSlider/Label");
        likingSlider = UtilTools.GetChild<UISlider>(go.transform, "likingSlider");
        stars = UtilTools.GetChild<UITexture>(go.transform, "star");
        descTime = UtilTools.GetChild<UILabel>(go.transform, "desc");
        helpBtn = UtilTools.GetChild<UISprite>(go.transform, "helpBtn");
        UIEventListener.Get(helpBtn.gameObject).onClick = OnClickEx;
        touchGrid.enabled = true;
        touchGrid.BindCustomCallBack(UpdateTouchItem);
        touchGrid.StartCustom();
    }
    void OnClickEx(GameObject go)
    {
        List<object> decsList = new List<object>();
        for (int i = 10008; i <= 10011; ++i)
        {
            string Name = "state" + i.ToString();
            string text = TextManager.GetPropsString(Name);
            decsList.Add(text);
        }
        GUIManager.ShowHelpPanel(decsList);
    }
    public override void SetChildPanel()
    {
        babyMediator.panel.babyState.gameObject.SetActive(false);
        UpdateSlider();
        List<object> list = new List<object>();
        foreach (ItemInfo item in ItemManager.itemList.Values)
        {
            if (item.itemType == 7)
                list.Add(item);
        }
        chooseTouchItem = list[0] as ItemInfo;
        touchGrid.AddCustomDataList(list);
    }
    public void UpdateSlider()
    {
        int star = BabyLikingConfig.GetBabyStarByLiking(babyInfo.liking).id; 
        UtilTools.SetStar(star, stars.GetComponentsInChildren<UISprite>());
        int maxLiking = UtilTools.IntParse(BabyLikingConfig.GetBabyStarByStar(star).extent.Split('-')[1]);
        likingSlider.value = babyInfo.liking / maxLiking;
        liking.text = UtilTools.StringBuilder(babyInfo.liking, "/", maxLiking);
        if (TimerManager.IsHaveTimer("happyTime"))
        {
            descTime.text = string.Format(babyMediator.desc, UtilTools.formatDuring(babyMediator.happyTime));
            return;
        }
        if (star == 1)
            desc = TextManager.GetUIString("UIclothes8");
        else
            desc = UtilTools.StringBuilder(TextManager.GetUIString("UIclothes9"), TextManager.GetUIString("UIclothesState" + star));
        if (babyInfo.likingTime < 1)
            descTime.text = string.Empty;
        else
            descTime.text = string.Format(babyMediator.desc, UtilTools.formatDuring(babyInfo.likingTime));
    }
    void UpdateTouchItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        ItemInfo info = item.oData as ItemInfo;
        UILabel Name = item.mScripts[0] as UILabel;
        UISprite color = item.mScripts[1] as UISprite;
        UITexture icon = item.mScripts[2] as UITexture;
        UILabel amount = item.mScripts[3] as UILabel;
        UISprite select = item.mScripts[4] as UISprite;
        item.onClick = ChooseTouchItem;
        LoadSprite.LoaderItem(icon, info.itemID);
        select.gameObject.SetActive(info == chooseTouchItem);
        color.spriteName = UtilTools.StringBuilder("color", info.color);
        if (item.miCurIndex == 0)
        {
            amount.text = UtilTools.StringBuilder(TextManager.GetUIString("UIclothes18"), babyInfo.closeTouch);
        }
        else
            amount.text = ItemManager.GetBagItemCount(info.itemID).ToString();
        Name.text = TextManager.GetItemString(info.itemID);
        UtilTools.SetTextColor(Name, info.color);
    }
    void ChooseTouchItem(UIGridItem item)
    {
        if (chooseTouchItem == item.oData as ItemInfo)
            return;
        (item.mScripts[4] as UISprite).gameObject.SetActive(true);
        UIGridItem gridItem = touchGrid.GetCustomGridItem(chooseTouchItem);
        if (gridItem != null)
            (gridItem.mScripts[4] as UISprite).gameObject.SetActive(false);
        chooseTouchItem = item.oData as ItemInfo;
    }
    public void TouchBaby(string bobyName)
    {
        if (chooseTouchItem.itemID == (touchGrid.GetCustomDataItem(0) as ItemInfo).itemID)
        {
            babyInfo.closeTouch--;
            touchGrid.UpdateCustomData(chooseTouchItem);
        }
        else
        {
            int count = UtilTools.IntParse((touchGrid.GetCustomGridItem(chooseTouchItem).mScripts[3] as UILabel).text);
            (touchGrid.GetCustomGridItem(chooseTouchItem).mScripts[3] as UILabel).text = (--count).ToString();
        }
        ServerCustom.instance.SendClientMethods("onClientTouchBabyInfo", UtilTools.IntParse(chooseTouchItem.itemID), 1, BabyLikingConfig.GetBabyStarByLiking(babyInfo.liking).id);
    }
    public void TouchSucess(int liking, int likingTime)
    {
        if (babyInfo.liking == 0)
        {
            TimerManager.AddTimerRepeat("likingTime", 1, babyMediator.UpdateLikingTime);
        }
        babyInfo.likingTime = likingTime;
        babyInfo.liking = liking;
        UpdateSlider();
    }
    /// <summary>
    /// 释放
    /// </summary>
    protected override void OnDestroy()
    {
        touchGrid = null;
        liking = null;
        descTime = null;
        likingSlider = null;
        stars = null;
        helpBtn = null;
    }
}
