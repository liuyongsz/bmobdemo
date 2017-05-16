using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClothesLevelPanel : BabyMediator
{
    private Transform max;
    private UISprite levelBtn;
    private UISprite cancelBtn;
    private UISprite helpBtn;
    private UIGrid equipGrid;
    private UISlider levelSlider;
    private UILabel sliderNum;
    private UILabel level;
    private UILabel addValue;
    private UILabel lockValue;
    private UILabel clothesName;
    private UILabel money;
    private UISprite suo;
    private int currentExp;
    private int currentLevel;
    private int qualityEquipIndex = 0;
    private Dictionary<string, EquipItemInfo> equipList = new Dictionary<string, EquipItemInfo>();
    private Dictionary<string, EquipItemInfo> chooseEquipList = new Dictionary<string, EquipItemInfo>();
    public ClothesLevelPanel(GameObject go)
    {
        max = UtilTools.GetChild<Transform>(go.transform, "max");
        levelBtn = UtilTools.GetChild<UISprite>(go.transform, "levelBtn");
        cancelBtn = UtilTools.GetChild<UISprite>(go.transform, "cancelBtn");
        helpBtn = UtilTools.GetChild<UISprite>(go.transform, "helpBtn");
        suo = UtilTools.GetChild<UISprite>(go.transform, "Sprite");
        levelSlider = UtilTools.GetChild<UISlider>(go.transform, "levelSlider");
        sliderNum = UtilTools.GetChild<UILabel>(go.transform, "levelSlider/Label");
        level = UtilTools.GetChild<UILabel>(go.transform, "level");
        addValue = UtilTools.GetChild<UILabel>(go.transform, "addValue");
        lockValue = UtilTools.GetChild<UILabel>(go.transform, "lockValue");
        clothesName = UtilTools.GetChild<UILabel>(go.transform, "clothesName");
        money = UtilTools.GetChild<UILabel>(go.transform, "money");
        equipGrid = UtilTools.GetChild<UIGrid>(go.transform, "ScrollView/equipGrid");
        equipGrid.enabled = true;
        equipGrid.BindCustomCallBack(UpdateEquipItem);
        equipGrid.StartCustom();
        UIEventListener.Get(levelBtn.gameObject).onClick = OnClickEx;
        UIEventListener.Get(cancelBtn.gameObject).onClick = OnClickEx;
        UIEventListener.Get(helpBtn.gameObject).onClick = OnClickEx;
    }
    void OnClickEx(GameObject go)
    {
        if (go == levelBtn.gameObject)
        {
            if (qualityEquipIndex > 0)
            {
                GUIManager.SetPromptInfoChoose(TextManager.GetUIString("UICreate1"), TextManager.GetUIString("UIclothes10"), UseCost);
                return;
            }
            UseCost();
        }
        else if (go == cancelBtn.gameObject)
        {
            chooseEquipList.Clear();
            equipGrid.UpdateCustomView();
            currentExp = 0;
            currentExp += babyMediator.clothesInfo.exp;
            currentLevel = 0;
            qualityEquipIndex = 0;
            money.text = "0";
            money.gameObject.SetActive(false);
            levelBtn.gameObject.SetActive(false);
            cancelBtn.gameObject.SetActive(false);
            UpdateInfo(babyMediator.clothesInfo.exp, babyMediator.clothesInfo.level);
        }
        else if (go == helpBtn.gameObject)
        {
            List<object> decsList = new List<object>();
            string text = TextManager.GetPropsString("state10007");
            decsList.Add(text);
            GUIManager.ShowHelpPanel(decsList);
        }
    }
    void UseCost()
    {
        List<object> List = new List<object>();
        foreach (EquipItemInfo item in chooseEquipList.Values)
        {
            Dictionary<string, object> list = new Dictionary<string, object>();
            list.Add("itemID", int.Parse(item.itemID));
            list.Add("number", item.star);
            List.Add(list);
        }
        ServerCustom.instance.SendClientMethods("onClientClothesLevel", babyMediator.clothesInfo.configID, List);
    }
    public override void SetChildPanel()
    {
        equipList.Clear();
        List<EquipItemInfo> List = EquipConfig.GetEquipDataListByPlayerID(0);
        for (int i = 0; i < List.Count; ++i)
        {
            equipList.Add(List[i].uuid, List[i]);
        }
        chooseEquipList.Clear();
        AddList();
        currentExp = 0;
        currentExp += babyMediator.clothesInfo.exp;
        currentLevel = 0;
        qualityEquipIndex = 0;
        money.text = "0";
        chooseEquipList.Clear();
        money.gameObject.SetActive(false);
        levelBtn.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(false);
        clothesName.text = TextManager.GetItemString(babyMediator.clothesInfo.configID.ToString());
        UpdateInfo(currentExp, babyMediator.clothesInfo.level);
    }

    /// <summary>
    /// 升级成功
    /// </summary>
    public void LevelSucess()
    {
        foreach (string key in chooseEquipList.Keys)
        {
            equipList.Remove(key);
        }
        currentExp = 0;
        currentExp += babyMediator.clothesInfo.exp;
        currentLevel = 0;
        qualityEquipIndex = 0;
        money.text = "0";
        AddList();
        chooseEquipList.Clear();
        money.gameObject.SetActive(false);
        levelBtn.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(false);
        UpdateInfo(currentExp, babyMediator.clothesInfo.level);
    }
    public void UpdateInfo(int infoExp, int infoLevel)
    {
        int needExp = 0;
        int currentxp = 0;
        string color = string.Empty;
        ClothesLevelInfo levelInfo = ClothesLevelConfig.GetClothesLevelInfoByID(babyMediator.clothesInfo.configID + infoLevel);
        ClothesSlevelInfo slevelInfo = ClothesSlevelConfig.GetClothesSlevelInfoByID(babyMediator.clothesInfo.configID + babyMediator.clothesInfo.star);
        if (infoLevel > babyMediator.clothesInfo.level)
        {
            color = string.Format("[00FF00]{0}[-]", infoLevel);
            level.text = UtilTools.StringBuilder(TextManager.GetUIString("UI2010"), "     ", color);
            color = string.Format("[00FF00]{0}[-]", levelInfo.addValue + slevelInfo.addValue);
            addValue.text = UtilTools.StringBuilder(TextManager.GetUIString(babyMediator.clothesInfo.addValue), "     ", color);

            if (babyMediator.clothesInfo.percentType == 0)
            {
                color = string.Format("[00FF00]{0}[-]", levelInfo.lockValue + slevelInfo.lockValue);
                lockValue.text = UtilTools.StringBuilder(TextManager.GetUIString(babyMediator.clothesInfo.lockAdd), "     ", color);
            }
            else
            {
                color = string.Format("[00FF00]{0}[-]", levelInfo.lockValue + slevelInfo.lockValue + "%");
                lockValue.text = UtilTools.StringBuilder(TextManager.GetUIString(babyMediator.clothesInfo.lockAdd), "     ", color);
            }
        }
        else
        {
            level.text = UtilTools.StringBuilder(TextManager.GetUIString("UI2010"), "     ", infoLevel);
            addValue.text = UtilTools.StringBuilder(TextManager.GetUIString(babyMediator.clothesInfo.addValue), "     ", levelInfo.addValue + slevelInfo.addValue);
            if (babyMediator.clothesInfo.percentType == 0)
                lockValue.text = UtilTools.StringBuilder(TextManager.GetUIString(babyMediator.clothesInfo.lockAdd), "     ", levelInfo.lockValue + slevelInfo.lockValue);
            else
                lockValue.text = UtilTools.StringBuilder(TextManager.GetUIString(babyMediator.clothesInfo.lockAdd), "     ", levelInfo.lockValue + slevelInfo.lockValue, "%");
        }
        suo.gameObject.SetActive(!(infoLevel >= babyMediator.clothesInfo.lockLevel));

        if (infoLevel >= babyMediator.clothesInfo.maxLevel)
        {
            levelSlider.value = 0;
            if (!max.gameObject.activeSelf)
                max.gameObject.SetActive(true);
            sliderNum.text = TextManager.GetSystemString("ui_system_clothes_2");
            return;
        }
        if (max.gameObject.activeSelf)
            max.gameObject.SetActive(false);
        if (infoLevel <= 1)
        {
            needExp = levelInfo.needExp;
            currentxp = infoExp;
        }
        else
        {
            ClothesLevelInfo lastInfo = ClothesLevelConfig.GetClothesLevelInfoByID(babyMediator.clothesInfo.configID + infoLevel - 1);
            needExp = levelInfo.needExp - lastInfo.needExp;
            currentxp = infoExp - lastInfo.needExp;
            lastInfo = null;
        }   
        levelSlider.value = currentxp * 1.0f / needExp;
        sliderNum.text = UtilTools.StringBuilder(currentxp, "/", needExp);
    }
    void UpdateEquipItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        EquipItemInfo info = item.oData as EquipItemInfo;
        item.onClick = ChooseEquip;
        UITexture icon = item.mScripts[0] as UITexture;
        UISprite gou = item.mScripts[1] as UISprite;
        UITexture star = item.mScripts[2] as UITexture;
        UISprite color = item.mScripts[3] as UISprite;
        gou.gameObject.SetActive(chooseEquipList.ContainsKey(info.uuid));
        color.spriteName = UtilTools.StringBuilder("color", info.star);
        LoadSprite.LoaderItem(icon, info.itemID.ToString());
        UtilTools.SetStar(info.star, star.GetComponentsInChildren<UISprite>());
    }
    void AddList()
    {
        List<object> list = new List<object>();
        foreach (EquipItemInfo item in equipList.Values)
        {
            if (item.star > 5)
                continue;
            list.Add(item);
        }
        list.Sort(CompareEquipItem);
        equipGrid.AddCustomDataList(list);
    }
    void ChooseEquip(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;      
        EquipItemInfo info = item.oData as EquipItemInfo;
        EquipUseInfo addInfo = UseEquipConfig.GetClothesByStar(info.star);
        UISprite isChoose = item.mScripts[1] as UISprite;
        if (addInfo == null)
        {
            Debug.LogError("not have info");
            return;
        }
        string nowLevel = UtilTools.StringBuilder(TextManager.GetUIString("UI2010"), "     ", babyMediator.clothesInfo.maxLevel);
        if ((babyMediator.clothesInfo.level >= babyMediator.clothesInfo.maxLevel || string.Equals(level.text, nowLevel))&& !isChoose.gameObject.activeSelf)
        {
            if (!max.gameObject.activeSelf)
                max.gameObject.SetActive(true);
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_clothes_2"));
            return;
        }
        if (max.gameObject.activeSelf)
            max.gameObject.SetActive(false);    
        if (isChoose.gameObject.activeSelf)
        {
            if (info.star >= 4)
                qualityEquipIndex--;
            isChoose.gameObject.SetActive(false);
            chooseEquipList.Remove(info.uuid);
            currentExp -= addInfo.exp;
            money.text = (UtilTools.IntParse(money.text) - addInfo.money).ToString();          
        }
        else
        {
            if (info.star >= 4)
                qualityEquipIndex++;
            isChoose.gameObject.SetActive(true);
            chooseEquipList.Add(info.uuid, info);
            currentExp += addInfo.exp;
            money.text = (UtilTools.IntParse(money.text) + addInfo.money).ToString();
        }
        CheckLevel();
        money.gameObject.SetActive(chooseEquipList.Count != 0);
        levelBtn.gameObject.SetActive(chooseEquipList.Count != 0);
        cancelBtn.gameObject.SetActive(chooseEquipList.Count != 0);
    }
    void CheckLevel()
    {
        ClothesLevelInfo info;
        for (int i = 1; i <= babyMediator.clothesInfo.maxLevel; ++i)
        {
            info = ClothesLevelConfig.GetClothesLevelInfoByID(babyMediator.clothesInfo.configID + i);
            if (currentExp < info.needExp)
            {
                currentLevel = i;
                break;
            }
            currentLevel = i;
        }
        UpdateInfo(currentExp, currentLevel);
    }
    /// <summary>
    /// 排序
    /// </summary>
    public int CompareEquipItem(object x, object y)
    {
        if (x == null)
        {
            if (y == null)
                return 0;
            else
                return -1;
        }
        else
        {
            if (y == null)
            {
                return 1;
            }
            else
            {
                EquipItemInfo a = x as EquipItemInfo;
                EquipItemInfo b = y as EquipItemInfo;
                return a.star.CompareTo(b.star);
            }
        }
    }

    /// <summary>
    /// 释放
    /// </summary>
    protected override void OnDestroy()
    {
        max = null;
        levelBtn = null;
        cancelBtn = null;
        helpBtn = null;
        equipGrid = null;
        levelSlider = null;
        sliderNum = null;
        level = null;
        addValue = null;
        lockValue = null;
        clothesName = null;
        money = null;
        suo = null;
    }
}
