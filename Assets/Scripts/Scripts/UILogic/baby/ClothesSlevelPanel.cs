using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ClothesSlevelPanel : BabyMediator
{
    private Transform slevelInfo;
    private Transform needCost;
    private Transform jianTou;
    private Transform max;
    private UISprite oldItem;
    private UITexture oldStar;
    private UISprite newItem;
    private UITexture newStar;
    private UILabel firstValue;
    private UILabel clothesName;
    private UILabel firstAdd;
    private UILabel secondValue;
    private UILabel secondAdd;
    private UISprite suo;
    private UISprite suo1;
    private UISprite sLevelBtn;
    private List<ConSuleItem> ConSuleList = new List<ConSuleItem>();
    public ClothesSlevelPanel(GameObject go)
    {
        max = UtilTools.GetChild<Transform>(go.transform, "slevelInfo/max");
        slevelInfo = UtilTools.GetChild<Transform>(go.transform, "slevelInfo");
        needCost = UtilTools.GetChild<Transform>(go.transform, "needCost");
        jianTou = UtilTools.GetChild<Transform>(go.transform, "Sprite");
        oldItem = UtilTools.GetChild<UISprite>(go.transform, "oldItem");
        newItem = UtilTools.GetChild<UISprite>(go.transform, "newItem");
        oldStar = UtilTools.GetChild<UITexture>(go.transform, "oldItem/star");
        newStar = UtilTools.GetChild<UITexture>(go.transform, "newItem/star");
        firstValue = UtilTools.GetChild<UILabel>(go.transform, "slevelInfo/firstValue");
        firstAdd = UtilTools.GetChild<UILabel>(go.transform, "slevelInfo/firstAdd");
        secondValue = UtilTools.GetChild<UILabel>(go.transform, "slevelInfo/secondValue");
        secondAdd = UtilTools.GetChild<UILabel>(go.transform, "slevelInfo/secondAdd");
        clothesName = UtilTools.GetChild<UILabel>(go.transform, "clothesName");
        suo = UtilTools.GetChild<UISprite>(go.transform, "slevelInfo/suo");
        suo1 = UtilTools.GetChild<UISprite>(go.transform, "needCost/suo");
        sLevelBtn = UtilTools.GetChild<UISprite>(go.transform, "needCost/sLevelBtn");
        ConSuleList = UtilTools.SetConSumeItemList(3, go.transform.FindChild("needCost"));
        UIEventListener.Get(sLevelBtn.gameObject).onClick = OnClickEx;
    }

    public override void SetChildPanel()
    {
        clothesName.text = TextManager.GetItemString(babyMediator.clothesInfo.configID.ToString());
        oldItem.spriteName = UtilTools.StringBuilder("color" + babyMediator.clothesInfo.star);
        newItem.spriteName = UtilTools.StringBuilder("color" + babyMediator.clothesInfo.star);
        oldItem.transform.FindChild("level/Label").GetComponent<UILabel>().text = babyMediator.clothesInfo.level.ToString();
        newItem.transform.FindChild("level/Label").GetComponent<UILabel>().text = babyMediator.clothesInfo.level.ToString();     
        LoadSprite.LoaderItem(oldItem.transform.FindChild("icon").GetComponent<UITexture>(), babyMediator.clothesInfo.configID.ToString(), false);
        LoadSprite.LoaderItem(newItem.transform.FindChild("icon").GetComponent<UITexture>(), babyMediator.clothesInfo.configID.ToString(), false);
        suo.gameObject.SetActive(babyMediator.clothesInfo.level < babyMediator.clothesInfo.lockLevel);
        suo1.gameObject.SetActive(babyMediator.clothesInfo.level < babyMediator.clothesInfo.lockLevel);
        UpdateInfo(babyMediator.clothesInfo.star);
    }
    public void UpdateInfo(int star)
    {
        babyMediator.clothesInfo.star = star;
        babyMediator.panel.leftGrid.UpdateCustomData(babyMediator.clothesInfo);
        UtilTools.SetStar(babyMediator.clothesInfo.star, oldItem.transform.FindChild("star").GetComponentsInChildren<UISprite>(), babyMediator.clothesInfo.maxstar);
        bool isMax = babyMediator.clothesInfo.star < babyMediator.clothesInfo.maxstar;
        firstAdd.gameObject.gameObject.SetActive(isMax);
        secondAdd.gameObject.gameObject.SetActive(isMax);
        needCost.gameObject.gameObject.SetActive(isMax);
        max.gameObject.gameObject.SetActive(!isMax);
        newItem.gameObject.SetActive(isMax);
        jianTou.gameObject.SetActive(isMax);
        ClothesLevelInfo levelInfo = ClothesLevelConfig.GetClothesLevelInfoByID(babyMediator.clothesInfo.configID + babyMediator.clothesInfo.level);
        ClothesSlevelInfo slevel = ClothesSlevelConfig.GetClothesSlevelInfoByID(babyMediator.clothesInfo.configID + babyMediator.clothesInfo.star);
        if (babyMediator.clothesInfo.star >= babyMediator.clothesInfo.maxstar)
        {
            oldItem.transform.localPosition = new Vector2(127, 94);
            slevelInfo.localPosition = new Vector2(0, -122);
            oldItem.spriteName = UtilTools.StringBuilder("color" + babyMediator.clothesInfo.star);
            firstValue.text = UtilTools.StringBuilder(TextManager.GetUIString(babyMediator.clothesInfo.addValue), "     ", levelInfo.addValue + slevel.addValue);
            if (babyMediator.clothesInfo.percentType == 0)
                secondValue.text = UtilTools.StringBuilder(TextManager.GetUIString(babyMediator.clothesInfo.lockAdd), "     ", levelInfo.lockValue + slevel.lockValue);
            else
                secondValue.text = UtilTools.StringBuilder(TextManager.GetUIString(babyMediator.clothesInfo.lockAdd), "     ", levelInfo.lockValue + slevel.lockValue, "%");
            return;
        }
        firstAdd.color = Color.green;
        secondAdd.color = Color.green;
        oldItem.transform.localPosition = new Vector2(-32, 127);
        slevelInfo.localPosition = Vector2.zero;
        newItem.spriteName = UtilTools.StringBuilder("color" + (babyMediator.clothesInfo.star + 1));
        UtilTools.SetStar(babyMediator.clothesInfo.star + 1, newItem.transform.FindChild("star").GetComponentsInChildren<UISprite>(), babyMediator.clothesInfo.maxstar);
        for (int i = 0; i < ConSuleList.Count; ++i)
        {
            string itemID = slevel.cost.Split(';')[i].Split(',')[0].ToString();
            int count = UtilTools.IntParse(slevel.cost.Split(';')[i].Split(',')[1]);
            ConSuleList[i].name.text = TextManager.GetItemString(itemID);
            int itemCount = ItemManager.GetBagItemCount(itemID);
            if (itemCount >= count)
                ConSuleList[i].count.color = Color.white;
            else
                ConSuleList[i].count.color = Color.red;
            ConSuleList[i].count.text = UtilTools.StringBuilder(itemCount, "/", count);
            ItemInfo item = ItemManager.GetItemInfo(itemID);
            if (item == null)
                continue;
            ConSuleList[i].color.spriteName = UtilTools.StringBuilder("color" + item.color);
            LoadSprite.LoaderItem(ConSuleList[i].Icon, itemID);
        }
        firstValue.text = UtilTools.StringBuilder(TextManager.GetUIString(babyMediator.clothesInfo.addValue), "     ", levelInfo.addValue + slevel.addValue);
        if (babyMediator.clothesInfo.percentType == 0)
            secondValue.text = UtilTools.StringBuilder(TextManager.GetUIString(babyMediator.clothesInfo.lockAdd), "     ", levelInfo.lockValue + slevel.lockValue);
        else
            secondValue.text = UtilTools.StringBuilder(TextManager.GetUIString(babyMediator.clothesInfo.lockAdd), "     ", levelInfo.lockValue + slevel.lockValue, "%");
        slevel = ClothesSlevelConfig.GetClothesSlevelInfoByID(babyMediator.clothesInfo.configID + babyMediator.clothesInfo.star + 1);
        firstAdd.text = (levelInfo.addValue + slevel.addValue).ToString();
        if (babyMediator.clothesInfo.percentType == 0)
            secondAdd.text = (levelInfo.lockValue + slevel.lockValue).ToString();
        else
            secondAdd.text = UtilTools.StringBuilder(levelInfo.lockValue + slevel.lockValue, "%");
    }

    void OnClickEx(GameObject go)
    {
        if (go == sLevelBtn.gameObject)
        {
            ServerCustom.instance.SendClientMethods("onClientClothesSlevel", babyMediator.clothesInfo.configID);
        }
    }
    /// <summary>
    /// 释放
    /// </summary>
    protected override void OnDestroy()
    {
        if (ConSuleList.Count > 0)
        {
            for (int i = 0; i < ConSuleList.Count; ++i)
            {
                ConSuleList[i].trans = null;
                ConSuleList[i].Icon = null;
                ConSuleList[i].name = null;
                ConSuleList[i].color = null;
                ConSuleList[i].count = null;
            }
            ConSuleList.Clear();
        }
        slevelInfo = null;
        needCost = null;
        jianTou = null;
        max = null;
        oldItem = null;
        oldStar = null;
        newItem = null;
        newStar = null;
        firstValue = null;
        clothesName = null;
        firstAdd = null;
        secondValue = null;
        secondAdd = null;
        suo = null;
        suo1 = null;
        sLevelBtn = null;
    }
}
