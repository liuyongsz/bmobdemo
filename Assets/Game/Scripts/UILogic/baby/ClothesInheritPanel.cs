using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClothesInheritPanel : BabyMediator
{
    private Transform inheritInfo;
    private Transform isChoose;
    private Transform chooseClothes;
    private Transform heritIsWear;
    private Transform beHeritIsWear;
    private UISprite heritBtn;
    private UILabel expAdd;
    private UIGrid clothesGrid;
    private UISprite heritAdd;
    private UISprite beHeritAdd;
    private UISprite heritColor;
    private UISprite beHeritColor;
    private UISprite select;
    private UILabel heritLevel;
    private UITexture heritStar;
    private UITexture heritIcon;
    private UITexture beHeritStar;
    private UITexture beHeritIcon;
    private UILabel beHeritLevel;
    private UILabel level;
    private UILabel levelAdd;
    private UILabel firstValue;
    private UILabel firstAdd;
    private UILabel secondValue;
    private UILabel secondAdd;
    private UILabel money;
    private UISprite inheritBtn;
    private UISprite closeBtn;
    private UISprite suo;
    private List<ConSuleItem> ConSuleList = new List<ConSuleItem>();
    private ClothesInfo heritInfo;
    private ClothesInfo beHeritInfo;
    private ClothesInherit clothesInheritInfo;
    private int chooseIndex = 0;
    private int currentExp;
    public ClothesInheritPanel(GameObject go)
    {
        clothesGrid = UtilTools.GetChild<UIGrid>(go.transform, "chooseClothes/ScrollView/Grid");
        inheritInfo = UtilTools.GetChild<Transform>(go.transform, "Info");
        heritIsWear = UtilTools.GetChild<Transform>(go.transform, "left/iswear");
        beHeritIsWear = UtilTools.GetChild<Transform>(go.transform, "right/iswear");
        chooseClothes = UtilTools.GetChild<Transform>(go.transform, "chooseClothes");
        closeBtn = UtilTools.GetChild<UISprite>(go.transform, "chooseClothes/biaoti/closeBtn");
        isChoose = UtilTools.GetChild<Transform>(go.transform, "isChoose");
        expAdd = UtilTools.GetChild<UILabel>(go.transform, "expAdd");
        heritLevel = UtilTools.GetChild<UILabel>(go.transform, "left/level");
        heritStar = UtilTools.GetChild<UITexture>(go.transform, "left/star");
        beHeritStar = UtilTools.GetChild<UITexture>(go.transform, "right/star");
        heritIcon = UtilTools.GetChild<UITexture>(go.transform, "left/icon");
        beHeritIcon = UtilTools.GetChild<UITexture>(go.transform, "right/icon");
        heritAdd = UtilTools.GetChild<UISprite>(go.transform, "left/Sprite");
        beHeritAdd = UtilTools.GetChild<UISprite>(go.transform, "right/Sprite");
        beHeritLevel = UtilTools.GetChild<UILabel>(go.transform, "right/level");
        heritColor = UtilTools.GetChild<UISprite>(go.transform, "left");
        beHeritColor = UtilTools.GetChild<UISprite>(go.transform, "right");
        suo = UtilTools.GetChild<UISprite>(go.transform, "Info/suo");
        select = UtilTools.GetChild<UISprite>(go.transform, "select");
        level = UtilTools.GetChild<UILabel>(go.transform, "Info/level");
        levelAdd = UtilTools.GetChild<UILabel>(go.transform, "Info/levelAdd");
        firstValue = UtilTools.GetChild<UILabel>(go.transform, "Info/firstValue");
        firstAdd = UtilTools.GetChild<UILabel>(go.transform, "Info/firstAdd");
        secondValue = UtilTools.GetChild<UILabel>(go.transform, "Info/secondValue");
        secondAdd = UtilTools.GetChild<UILabel>(go.transform, "Info/secondAdd");
        money = UtilTools.GetChild<UILabel>(go.transform, "Info/money");
        inheritBtn = UtilTools.GetChild<UISprite>(go.transform, "Info/Btn");
        ConSuleList = UtilTools.SetConSumeItemList(1, go.transform);
        UIEventListener.Get(heritColor.gameObject).onClick = OnClickEx;
        UIEventListener.Get(beHeritColor.gameObject).onClick = OnClickEx;
        UIEventListener.Get(inheritBtn.gameObject).onClick = OnClickEx; 
        UIEventListener.Get(ConSuleList[0].trans.gameObject).onClick = OnClickEx;
        UIEventListener.Get(closeBtn.gameObject).onClick = OnClickEx;
        clothesGrid.enabled = true;
        clothesGrid.BindCustomCallBack(UpdateClothesGrid);
        clothesGrid.StartCustom();
    }

    public override void SetChildPanel()
    {
        Init();
        clothesInheritInfo = ClothesInheritConfig.GetClothesInheritInfoByID(2);
        string itemID = clothesInheritInfo.cost;      
        ConSuleList[0].name.text = TextManager.GetItemString(itemID);
        int itemCount = ItemManager.GetBagItemCount(itemID);
        if (itemCount >= 1)
            ConSuleList[0].count.color = Color.white;
        else
            ConSuleList[0].count.color = Color.red;
        ConSuleList[0].count.text = UtilTools.StringBuilder(itemCount, "/", 1);
        ItemInfo item = ItemManager.GetItemInfo(itemID);
        if (item == null)
            return;
        ConSuleList[0].color.spriteName = UtilTools.StringBuilder("color" + item.color);
        LoadSprite.LoaderItem(ConSuleList[0].Icon, itemID);
        expAdd.text = string.Format(TextManager.GetUIString("UIclothes3"), clothesInheritInfo.percent);
        select.gameObject.SetActive(false);
    }
    void Init()
    {
        heritInfo = null;
        beHeritInfo = null;
        money.text = "0";
        heritColor.spriteName = "color1";
        beHeritColor.spriteName = "color1";
        heritIcon.mainTexture = null;
        beHeritIcon.mainTexture = null;
        heritAdd.gameObject.SetActive(true);
        beHeritAdd.gameObject.SetActive(true);        
        heritIsWear.gameObject.SetActive(false);
        beHeritIsWear.gameObject.SetActive(false);
        heritLevel.gameObject.SetActive(false);
        heritStar.gameObject.SetActive(false);
        beHeritStar.gameObject.SetActive(false);
        beHeritLevel.gameObject.SetActive(false);
        isChoose.gameObject.SetActive(true);
        inheritInfo.gameObject.SetActive(false);
    }
    void UpdateInfo()
    {
        currentExp = 0;
        int currentLevel = 0;
        if (heritInfo == null || beHeritInfo == null)
            return;
        isChoose.gameObject.SetActive(false);
        inheritInfo.gameObject.SetActive(true);
        level.text = UtilTools.StringBuilder(TextManager.GetUIString("UI2010"), "     ", beHeritInfo.level);
        ClothesLevelInfo levelInfo = ClothesLevelConfig.GetClothesLevelInfoByID(beHeritInfo.configID + beHeritInfo.level);
        firstValue.text = UtilTools.StringBuilder(TextManager.GetUIString(beHeritInfo.addValue), "     ", levelInfo.addValue);
        if (beHeritInfo.percentType == 0)
            secondValue.text = UtilTools.StringBuilder(TextManager.GetUIString(beHeritInfo.lockAdd), "     ", levelInfo.lockValue);
        else
            secondValue.text = UtilTools.StringBuilder(TextManager.GetUIString(beHeritInfo.lockAdd), "     ", levelInfo.lockValue, "%");
        currentExp = (int)(heritInfo.exp * (clothesInheritInfo.percent * 1.0f / 100));
        money.text = (currentExp * 10).ToString();
        currentExp += beHeritInfo.exp;
        currentLevel = beHeritInfo.level;
        while (currentExp >= levelInfo.needExp)
        {
            if (currentExp < levelInfo.needExp)
                break;
            currentLevel++;
            levelInfo = ClothesLevelConfig.GetClothesLevelInfoByID(beHeritInfo.configID + currentLevel);
            if (currentLevel >= beHeritInfo.maxLevel)
                break;
        }
        suo.gameObject.SetActive(!(currentLevel > beHeritInfo.lockLevel));
        levelAdd.color = Color.green;
        firstAdd.color = Color.green;
        if (currentLevel > beHeritInfo.lockLevel)
            secondAdd.color = Color.green;
        else
            secondAdd.color = Color.white;
        levelAdd.text = currentLevel.ToString();
        firstAdd.text = levelInfo.addValue.ToString();
        if (beHeritInfo.percentType == 0)
            secondAdd.text = levelInfo.lockValue.ToString();
        else
            secondAdd.text = UtilTools.StringBuilder(levelInfo.lockValue, "%");
    }

    public void InheritSuceess()
    {
        heritInfo.level = 1;
        heritInfo.exp = 0;
        beHeritInfo.level = UtilTools.IntParse(levelAdd.text);
        if (beHeritInfo.level == beHeritInfo.maxLevel)
            beHeritInfo.exp = ClothesLevelConfig.GetClothesLevelInfoByID(beHeritInfo.configID + beHeritInfo.level - 1).needExp;
        else
            beHeritInfo.exp = currentExp;
        Init();
        if (!select.gameObject.activeSelf)
            return;
        string itemID = clothesInheritInfo.cost;
        int itemCount = ItemManager.GetBagItemCount(itemID) - 1;
        if (itemCount >= 1)
            ConSuleList[0].count.color = Color.white;
        else
            ConSuleList[0].count.color = Color.red;
        ConSuleList[0].count.text = UtilTools.StringBuilder(itemCount, "/", 1);
    }
    void OnClickEx(GameObject go)
    {
        if (go == heritColor.gameObject)
        {
            chooseIndex = 0;           
            if (beHeritInfo != null)
                AddChooseClothesList(beHeritInfo.level, true);
            else
                AddChooseClothesList(1, true);          
        }
        else if (go == beHeritColor.gameObject)
        {
            chooseIndex = 1;            
            if (heritInfo != null)
                AddChooseClothesList(heritInfo.level, false);
            else
                AddChooseClothesList(20, false);         
        }
        else if (go == ConSuleList[0].trans.gameObject)
        {
            if (heritInfo == null || beHeritInfo == null)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_clothes_11"));
                return;
            }
            else if (ConSuleList[0].count.color == Color.red)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_clothes_5"));
                return;
            }
            if (select.gameObject.activeSelf)
            {
                select.gameObject.SetActive(false);
                clothesInheritInfo = ClothesInheritConfig.GetClothesInheritInfoByID(2);
            }
            else
            {
                select.gameObject.SetActive(true);
                clothesInheritInfo = ClothesInheritConfig.GetClothesInheritInfoByID(1);
            }
            expAdd.text = string.Format(TextManager.GetUIString("UIclothes3"), clothesInheritInfo.percent);
            UpdateInfo();
        }
        else if (go == inheritBtn.gameObject)
        {
            if (!select.gameObject.activeSelf && ItemManager.GetBagItemCount(clothesInheritInfo.cost) > 0)
            {
                GUIManager.SetPromptInfoChoose(TextManager.GetUIString("UICreate1"), TextManager.GetSystemString("ui_system_2"), UseCost);
                return;
            }
            if(currentExp> ClothesLevelConfig.GetClothesLevelInfoByID(beHeritInfo.configID + beHeritInfo.maxLevel-1).needExp)
            {
                GUIManager.SetPromptInfoChoose(TextManager.GetUIString("UICreate1"), TextManager.GetUIString("UIclothes11"), ContinueInherit);
                return;
            }              
            ServerCustom.instance.SendClientMethods("onClientInheritClothes", heritInfo.configID, beHeritInfo.configID, clothesInheritInfo.id);
        }
        else if (go == closeBtn.gameObject)
        {
            chooseClothes.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 确定不使用道具
    /// </summary>
    void UseCost()
    {
        if (currentExp > ClothesLevelConfig.GetClothesLevelInfoByID(beHeritInfo.configID + beHeritInfo.maxLevel - 1).needExp)
        {
            GUIManager.SetPromptInfoChoose(TextManager.GetUIString("UICreate1"), TextManager.GetUIString("UIclothes11"), ContinueInherit);
            return;
        }
        ServerCustom.instance.SendClientMethods("onClientInheritClothes", heritInfo.configID, beHeritInfo.configID, clothesInheritInfo.id);
    }
    void ContinueInherit()
    {
        ServerCustom.instance.SendClientMethods("onClientInheritClothes", heritInfo.configID, beHeritInfo.configID, clothesInheritInfo.id);
    }
    void UpdateClothesGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        ClothesInfo info = item.oData as ClothesInfo;
        UITexture icon = item.mScripts[0] as UITexture;
        UISprite isWear = item.mScripts[1] as UISprite;
        UITexture star = item.mScripts[2] as UITexture;
        UISprite color = item.mScripts[6] as UISprite;
        UILabel level = item.mScripts[4] as UILabel;
        UILabel Name = item.mScripts[7] as UILabel;
        item.onClick = ChooseClothes;
        Name.text = TextManager.GetItemString(info.configID.ToString());
        UtilTools.SetTextColor(Name, info.star);
        level.text = info.level.ToString();
        isWear.gameObject.SetActive(info.isWear == 1);
        color.spriteName = UtilTools.StringBuilder("color", info.star);
        LoadSprite.LoaderItem(icon, info.configID.ToString());
        UtilTools.SetStar(info.star, star.GetComponentsInChildren<UISprite>(), info.maxstar);
    }
    void ChooseClothes(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        ClothesInfo info = item.oData as ClothesInfo;
        if (chooseIndex == 0)
        {
            heritInfo = info;
            heritLevel.gameObject.SetActive(true);
            heritStar.gameObject.SetActive(true);
            heritAdd.gameObject.SetActive(false);
            heritLevel.text = heritInfo.level.ToString();
            heritIsWear.gameObject.SetActive(info.isWear == 1);
            heritColor.spriteName = UtilTools.StringBuilder("color", info.star);
            LoadSprite.LoaderItem(heritIcon, info.configID.ToString());
            UtilTools.SetStar(info.star, heritStar.GetComponentsInChildren<UISprite>(), info.maxstar);
        }           
        else
        {
            beHeritInfo = info;
            beHeritLevel.gameObject.SetActive(true);
            beHeritStar.gameObject.SetActive(true);
            beHeritAdd.gameObject.SetActive(false);
            beHeritLevel.text = beHeritInfo.level.ToString();
            beHeritIsWear.gameObject.SetActive(info.isWear == 1);
            beHeritColor.spriteName = UtilTools.StringBuilder("color", info.star);
            LoadSprite.LoaderItem(beHeritIcon, info.configID.ToString());
            UtilTools.SetStar(info.star, beHeritStar.GetComponentsInChildren<UISprite>(), info.maxstar);
        }          
        chooseClothes.gameObject.SetActive(false);
        UpdateInfo();
    }

    void AddChooseClothesList(int level, bool isHerit)
    {
        List<object> list = new List<object>();
        foreach (ClothesInfo item in babyInfo.clothesInfoList.Values)
        {
            if ((heritInfo != null && item.configID == heritInfo.configID) || (beHeritInfo != null && item.configID == beHeritInfo.configID))
                continue;
            if (isHerit)
            {
                if (item.level <= level)
                    continue;
            }
            else
            {
                if (item.level >= level)
                    continue;
            }
            list.Add(item);
        }
        if (list.Count < 1)
        {
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_clothes_10"));
            clothesGrid.ClearCustomGrid();
            return;
        }
        chooseClothes.gameObject.SetActive(true);
        clothesGrid.AddCustomDataList(list);
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
          inheritInfo = null;
        isChoose = null;
        chooseClothes = null;
        heritIsWear = null;
        beHeritIsWear = null;
        heritBtn = null;
        expAdd = null;
        clothesGrid = null;
        heritAdd = null;
        beHeritAdd = null;
        heritColor = null;
        beHeritColor = null;
        select = null;
        heritLevel = null;
        heritStar = null;
        heritIcon = null;
        beHeritStar = null;
        beHeritIcon = null;
        beHeritLevel = null;
        level = null;
        levelAdd = null;
        firstValue = null;
        firstAdd = null;
        secondValue = null;
        secondAdd = null;
        money = null;
        inheritBtn = null;
        closeBtn = null;
        suo = null;
    }
}
