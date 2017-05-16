using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;
using System;

public class babypanel : BasePanel
{
    public Transform mModelParent;
    public Transform slevelPanel;
    public Transform clothesPanel;
    public Transform levelPanel;
    public Transform inheritPanel;
    public Transform touchPanel;
    public Transform mapPanel;
    public Transform tab;
    public Transform mapInfo;
    public UITexture mModelTexture;
    public UITexture mohu;
    public UIScrollView scrollView;
    public UIGrid ClothesGrid;
    public UIGrid leftGrid;
    public UIGrid mapGrid;
    public UIScrollView rightView;
    public UIScrollView leftView;
    public UIScrollView mapScrollView;
    public UIToggle head;
    public UIToggle clothes;
    public UIToggle skirt;
    public UIToggle shoe;
    public UIToggle adorn;
    public UIToggle changeBtn;
    public UIToggle levelBtn;
    public UIToggle sLevelBtn;
    public UIToggle inheritBtn;
    public UIToggle mapBtn;
    public UILabel lockLevel;
    public UILabel techAdd;
    public UILabel healthAdd;
    public UISprite wearBtn;
    public UISprite babyState;
    public UISprite closeMapInfoBtn;
    public UIButton backBtn;
    public UISprite propertiesBtn;
}
public enum ClothesType
{
    Head = 1,
    Clothes = 2,
    Skirt = 3,
    Shoe = 4,
    Adorn = 5,
}

public enum TabType
{
    Change = 1,
    Level = 2,
    Slevel = 3,
    Inherit = 4,
    Touch = 5,
    Map = 6,
    MapInfo = 7,
}
public class BabyMediator : UIMediator<babypanel>
{

    public babypanel panel
    {
        get
        {
            return m_Panel as babypanel;
        }
    }
    private UIToggle currentTog;
    public string desc;
    public TabType tabType;
    public ClothesType currentType;
    public static BabyInfo babyInfo = new BabyInfo();
    public ClothesInfo clothesInfo;
    public ClothesInfo isWearClothesInfo;
    public TouchPanel touchPanel;
    public ClothesSlevelPanel clothesSlevelPanel;
    public ClothesInheritPanel clothesInheritPanel;
    public ClothesLevelPanel clothesLevelPanel;
    public static BabyMediator babyMediator;
    private List<Transform> transList = new List<Transform>();
    private ModelUIManager.UIModelInfo mUModelInfo = null;
    private CommonInfo cdInfo;
    private CommonInfo likingInfo;
    public int happyTime;
    private bool fromOtherBtn = false;
    private GameObject cloneMapItem;
    private List<ConSuleItem> ConSuleList = new List<ConSuleItem>();
    public BabyMediator() : base("babypanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.Baby_Show, OpenPanel);
        RegistPanelCall(NotificationID.Baby_Hide, ClosePanel);
    }
    /// <summary>
    /// 界面显示前调用
    /// </summary>
    protected override void OnStart(INotification notification)
    {
        if (babyMediator == null)
        {
            babyMediator = Facade.RetrieveMediator("BabyMediator") as BabyMediator;
        }
        transList.Add(panel.clothesPanel);
        transList.Add(panel.levelPanel);
        transList.Add(panel.slevelPanel);
        transList.Add(panel.touchPanel);
        transList.Add(panel.inheritPanel);
        panel.leftGrid.enabled = true;
        panel.leftGrid.BindCustomCallBack(UpdateLeftClothesGrid);
        panel.leftGrid.StartCustom();

        panel.mapGrid.enabled = true;
        panel.mapGrid.BindCustomCallBack(UpdateMapGrid);
        panel.mapGrid.StartCustom();

        panel.ClothesGrid.enabled = true;
        panel.ClothesGrid.BindCustomCallBack(UpdateClothesGrid);
        panel.ClothesGrid.StartCustom();
        tabType = TabType.Change;
        currentType = ClothesType.Head;     
        
    }

    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        LoadSprite.LoaderBGTexture(panel.mohu.material, "baby", false);             
        cdInfo = CommonConfig.GetCommonInfo(4);
        likingInfo = CommonConfig.GetCommonInfo(5);
        string modelID = "baby";
        ModelUIManager.CreateModel(modelID, (ModelUIManager.UIModelInfo o) =>
        {
            mUModelInfo = o;
            mUModelInfo.oModel.SetActive(true);
            ModelUIManager.ChangeShader(mUModelInfo.oModel, "Diffuse", string.Empty, "Effect");         
            mUModelInfo.oModel.transform.FindChild("sakura_body").gameObject.AddComponent<MeshCollider>();            
            panel.mModelTexture.mainTexture = mUModelInfo.rtexture;
            AddSpinWithMouse();
        });
        currentTog = panel.changeBtn;
        SetChildPanel();
        panel.lockLevel.text = string.Format(TextManager.GetUIString("UILockLevel"), clothesInfo.lockLevel);
        if (notification.Body != null)
        {
            fromOtherBtn = true;
            OnClick(panel.babyState.gameObject);
        }
    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.head.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.clothes.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.skirt.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.shoe.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.adorn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.changeBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.levelBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.sLevelBtn.gameObject).onClick = OnClick; 
        UIEventListener.Get(panel.inheritBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.wearBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.backBtn.gameObject).onClick = OnClick; 
        UIEventListener.Get(panel.babyState.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.mModelParent.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.mapBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.closeMapInfoBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.propertiesBtn.gameObject).onClick = OnClick;
    }
    void HiddenUI(Transform trans)
    {
        panel.tab.gameObject.SetActive(trans != panel.inheritPanel);
        for (int i = 0; i < transList.Count; ++i)
        {
            transList[i].gameObject.SetActive(trans == transList[i]);
        }
    }
    public void UpdateGridByType(ClothesType type)
    {
        List<object> list = new List<object>();
        foreach (ClothesInfo item in babyInfo.clothesInfoList.Values)
        {
            if (item.type != (int)type)
                continue;
            if (item.isWear == 1)
                isWearClothesInfo = item;
            list.Add(item);
        }
        list.Sort(CompareItem);
        if (list.Count > 0)
            clothesInfo = list[0] as ClothesInfo;
        if (tabType == TabType.Change)
        {
            panel.wearBtn.gameObject.SetActive(false);
            panel.ClothesGrid.ClearCustomGrid();
            panel.ClothesGrid.AddCustomDataList(list);
        }
        else
        {
            panel.leftGrid.ClearCustomGrid();
            panel.leftGrid.AddCustomDataList(list);
            if (panel.levelPanel.gameObject.activeSelf)
                clothesLevelPanel.SetChildPanel();
            else if (panel.slevelPanel.gameObject.activeSelf)
                clothesSlevelPanel.SetChildPanel();
            else
                clothesInheritPanel.SetChildPanel();
        }
    }

    /// <summary>
    /// 界面数据刷新
    /// </summary>
    public virtual void SetChildPanel()
    {
        TimeSpan timeSpan = (DateTime.Now - new DateTime(1970, 1, 1));
        int time = (int)timeSpan.TotalSeconds;
        happyTime = 6 * 3600 - (time - babyInfo.fullTime - 8 * 3600);
        if (happyTime > 6 * 60 * 60 || happyTime <= 0)
            TimerManager.AddTimerRepeat("likingTime", 1, UpdateLikingTime);
        else
        {
            desc = TextManager.GetUIString("UIclothes12");
            TimerManager.AddTimerRepeat("happyTime", 1, UpdateHappyTime);
        }
        UpdateLiking();
        UpdateGridByType(currentType);
    }
    public void UpdateHappyTime()
    {
        happyTime--;        
        if (touchPanel != null && touchPanel.descTime != null)
        {         
            touchPanel.descTime.text = string.Format(desc, UtilTools.formatDuring(happyTime));
        }           
    }
    /// <summary>
    /// 好感度刷新
    /// </summary>
    public void UpdateLiking()
    {
        BabyStar info = BabyLikingConfig.GetBabyStarByLiking(babyInfo.liking);
        panel.babyState.spriteName = UtilTools.StringBuilder("state", info.state);
        panel.techAdd.text = UtilTools.StringBuilder(TextManager.GetUIString("tech"), "  +", info.addvalue.Split(',')[0], "%");
        panel.healthAdd.text = UtilTools.StringBuilder(TextManager.GetUIString("health"), "  +", info.addvalue.Split(',')[1], "%");
        if (!TimerManager.IsHaveTimer("likingTime"))
            return;   
        if (info == null)
        {
            LogSystem.LogError(babyInfo.liking, "not in pei zhi!!!");
            return;
        }
        if (info.state == 1)
            desc = TextManager.GetUIString("UIclothes8");
        else
            desc = UtilTools.StringBuilder(TextManager.GetUIString("UIclothes9"), TextManager.GetUIString("UIclothesState" + info.state));      
    }
    public void UpdateLikingTime()
    {       
        if (babyInfo.likingTime == 0)
        {
            babyInfo.liking -= likingInfo.value;
            if (babyInfo.liking < 0)
                babyInfo.liking = 0;
            if (touchPanel != null && touchPanel.descTime != null)
                touchPanel.descTime.text = string.Empty;
            TimerManager.Destroy("likingTime");
            return;
        }
        babyInfo.likingTime--;

        if (babyInfo.likingTime % (cdInfo.value * 60) == 0)
        {
            babyInfo.liking -= likingInfo.value;
            if (babyMediator.touchPanel != null)
                babyMediator.touchPanel.UpdateSlider();
        }
        if (touchPanel != null && touchPanel.descTime != null)
            touchPanel.descTime.text = string.Format(desc, UtilTools.formatDuring(babyInfo.likingTime));
    }
    /// <summary>
    /// 刷新主界面时装数据
    /// </summary>
    void UpdateClothesGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        ClothesInfo info = item.oData as ClothesInfo;
        UITexture icon = item.mScripts[0] as UITexture;
        UILabel name = item.mScripts[1] as UILabel;
        UITexture star = item.mScripts[2] as UITexture;
        UILabel level = item.mScripts[3] as UILabel;
        UILabel addValue = item.mScripts[4] as UILabel;
        UILabel lockValue = item.mScripts[5] as UILabel;
        UISprite isWear = item.mScripts[6] as UISprite;
        UISprite suo = item.mScripts[7] as UISprite;
        UISprite color = item.mScripts[8] as UISprite;
        UISprite select = item.mScripts[9] as UISprite;
        select.gameObject.SetActive(clothesInfo == info);
        item.onClick = ChooseClothes;
        LoadSprite.LoaderItem(icon, info.configID.ToString());
        isWear.gameObject.SetActive(info.isWear == 1);
        name.text = TextManager.GetItemString(info.configID.ToString());
        level.text = "Lv " + info.level;
        color.spriteName = UtilTools.StringBuilder("color", info.star);
        UtilTools.SetStar(info.star, star.GetComponentsInChildren<UISprite>(), info.maxstar);
        ClothesLevelInfo levelInfo = ClothesLevelConfig.GetClothesLevelInfoByID(info.configID + info.level);
        ClothesSlevelInfo slevelInfo = ClothesSlevelConfig.GetClothesSlevelInfoByID(info.configID + info.star);
        if (levelInfo == null)
        {
            LogSystem.LogError(info.level, "not in pei zhi!!!");
            return;
        }
        addValue.text = UtilTools.StringBuilder(TextManager.GetUIString(info.addValue), "     ", levelInfo.addValue + slevelInfo.addValue);
        suo.gameObject.SetActive(info.level < info.lockLevel);
        if (info.level >= info.lockLevel)
            lockValue.color = Color.white;
        else
            lockValue.color = new Color(0.44f, 0.47f, 0.5f);
        if (info.percentType == 0)
            lockValue.text = UtilTools.StringBuilder(TextManager.GetUIString(info.lockAdd), "     ", levelInfo.lockValue + slevelInfo.lockValue);
        else
            lockValue.text = UtilTools.StringBuilder(TextManager.GetUIString(info.lockAdd), "     ", levelInfo.lockValue + slevelInfo.lockValue, "%");
    }

    /// <summary>
    /// 刷新左边时装数据
    /// </summary>
    void UpdateLeftClothesGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        ClothesInfo info = item.oData as ClothesInfo;
        UITexture icon = item.mScripts[0] as UITexture;
        UISprite isWear = item.mScripts[1] as UISprite;
        UITexture star = item.mScripts[2] as UITexture;
        UISprite isLevel= item.mScripts[3] as UISprite;
        UILabel level = item.mScripts[4] as UILabel;
        UISprite select = item.mScripts[5] as UISprite;
        select.gameObject.SetActive(clothesInfo == info);
        item.onClick = ChooseClothes;
        isLevel.gameObject.SetActive(tabType == TabType.Slevel);
        level.text = info.level.ToString();
        isWear.gameObject.SetActive(info.isWear == 1);
        LoadSprite.LoaderItem(icon, info.configID.ToString());
        item.GetComponent<UISprite>().spriteName = UtilTools.StringBuilder("color", info.star);
        UtilTools.SetStar(info.star, star.GetComponentsInChildren<UISprite>(), info.maxstar);
    }
    /// <summary>
    /// 领取套装奖励
    /// </summary>
    void GetMapReward(GameObject go)
    {
        ClothesMapInfo info = (NGUITools.FindInParents<UIGridItem>(go).oData) as ClothesMapInfo;
        if (info.haveNum < 5)
        {
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_clothes_14"));
            return;
        }
        ServerCustom.instance.SendClientMethods("onClientGetMapReward", info.mapID);        
    }
    /// <summary>
    /// 激活套装
    /// </summary>
    public void GetMapRewardSucess(int mapID)
    {
        GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_clothes_15"));
        panel.mapGrid.UpdateCustomData(ClothesMapConfig.configList[mapID] as object);
    }
    /// <summary>
    /// 刷新图鉴数据
    /// </summary>
    void UpdateMapGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        ClothesMapInfo info = item.oData as ClothesMapInfo;
        UILabel Name = item.mScripts[0] as UILabel;
        UITexture icon = item.mScripts[1] as UITexture;
        UISlider slider = item.mScripts[2] as UISlider;
        UILabel sliderLabel = item.mScripts[3] as UILabel;
        UILabel value = item.mScripts[4] as UILabel;
        UILabel value1 = item.mScripts[5] as UILabel;
        UISprite item1 = item.mScripts[6] as UISprite;
        UISprite item2 = item.mScripts[7] as UISprite;
        UISprite getRewardBtn = item.mScripts[8] as UISprite;
        UISprite isGet = item.mScripts[9] as UISprite;
        item.onClick = SeeClothesMapInfo;
        UIEventListener.Get(getRewardBtn.gameObject).onClick = GetMapReward;
        Name.text = TextManager.GetItemString(info.suitName);
        LoadSprite.LoaderItem(icon, info.spriteName, false);
        int index = 0;
        for (int i = 0; i < 5; ++i)
        {
            int itemId = info.mapID * 10000 + i * 1000;
            if (haveClothes(itemId))
                index++;
        }
        info.haveNum = index;
        if(HaveGetReward(info.mapID))
        {
            getRewardBtn.transform.FindChild("Label").GetComponent<UILabel>().text = TextManager.GetUIString("UIclothes17");
            getRewardBtn.color = Color.black;
            value.color = Color.green;
            value1.color = Color.green;
        }
        else
        {
            getRewardBtn.transform.FindChild("Label").GetComponent<UILabel>().text = TextManager.GetUIString("UI1126");
            getRewardBtn.color = Color.white;
            value.color = Color.white;
            value1.color = Color.white;
        }
        getRewardBtn.GetComponent<BoxCollider>().enabled = !HaveGetReward(info.mapID);
        isGet.gameObject.SetActive(!(info.haveNum == 5));
        
        slider.value = index * 1.0f / 5;
        sliderLabel.text = index + "/5";
        value.text = UtilTools.StringBuilder(TextManager.GetUIString(info.addValue.Split(';')[0].Split(',')[0]), " +", info.addValue.Split(';')[0].Split(',')[1]);
        value1.text = UtilTools.StringBuilder(TextManager.GetUIString(info.addValue.Split(';')[1].Split(',')[0]), " +", info.addValue.Split(';')[1].Split(',')[1]);
        string itemID = string.Empty;
        int num = 0;
        ItemInfo itemInfo;
        item2.gameObject.SetActive(info.reward.Split(';').Length >= 2);
        for (int i = 0; i < info.reward.Split(';').Length; ++i)
        {
            itemID = info.reward.Split(';')[i].Split(',')[0];
            num = UtilTools.IntParse(info.reward.Split(';')[i].Split(',')[1]);
            itemInfo = ItemManager.GetItemInfo(itemID);
            if (i == 0)
            {
                item1.spriteName = "color" + itemInfo.color;
                LoadSprite.LoaderItem(item1.transform.FindChild("icon").GetComponent<UITexture>(), itemID, false);
                item1.transform.FindChild("num").GetComponent<UILabel>().text = num.ToString();
            }
            else
            {
                item2.spriteName = "color" + itemInfo.color;
                LoadSprite.LoaderItem(item2.transform.FindChild("icon").GetComponent<UITexture>(), itemID, false);
                item2.transform.FindChild("num").GetComponent<UILabel>().text = num.ToString();
            }
        }      
        
    }

    void SeeClothesMapInfo(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        tabType = TabType.MapInfo;
        ClothesMapInfo info = item.oData as ClothesMapInfo;
        if (cloneMapItem != null)
            MonoBehaviour.Destroy(cloneMapItem);
        cloneMapItem = GameObject.Instantiate(item.gameObject);
        panel.mapScrollView.gameObject.SetActive(false);
        panel.mapInfo.gameObject.SetActive(true);
        panel.propertiesBtn.gameObject.SetActive(false);
        UtilTools.SetParentWithPosition(cloneMapItem.transform, panel.mapInfo.transform, new Vector3(-451, -36, 0), Vector3.one);
        cloneMapItem.GetComponent<UIGridItem>().mScripts[8].gameObject.SetActive(false);
        if (ConSuleList.Count < 1)
        {
            ConSuleList = UtilTools.SetConSumeItemList(5, panel.mapInfo.transform);
        }
        ClothesInfo clothesInfo;
        for (int i = 0; i < ConSuleList.Count; ++i)
        {
            int itemId = info.mapID * 10000 + i * 1000;
            clothesInfo = ClothesConfig.GetClothesByID(itemId);
            LoadSprite.LoaderItem(ConSuleList[i].Icon, itemId.ToString(), false);
            ConSuleList[i].color.spriteName = "color" + clothesInfo.initstar;
            ConSuleList[i].name.text = TextManager.GetItemString(itemId.ToString());
        }
    }
    /// <summary>
    /// 选择时装
    /// </summary>
    void ChooseClothes(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        if (clothesInfo == item.oData as ClothesInfo)
            return;
        if (tabType == TabType.Level || tabType == TabType.Slevel)
        {
            UIGridItem gridItem = panel.leftGrid.GetCustomGridItem(clothesInfo);
            if (gridItem != null)
                (gridItem.mScripts[5] as UISprite).gameObject.SetActive(false);
            (item.mScripts[5] as UISprite).gameObject.SetActive(true);
        }
        else
        {
            UIGridItem gridItem = panel.ClothesGrid.GetCustomGridItem(clothesInfo);
            if (gridItem != null)
                (gridItem.mScripts[9] as UISprite).gameObject.SetActive(false);
            (item.mScripts[9] as UISprite).gameObject.SetActive(true);
        }        
        clothesInfo = item.oData as ClothesInfo;
       
        if (panel.clothesPanel.gameObject.activeSelf)
            panel.wearBtn.gameObject.SetActive(clothesInfo.isWear != 1);
        else if (panel.levelPanel.gameObject.activeSelf)
            clothesLevelPanel.SetChildPanel();
        else if (panel.slevelPanel.gameObject.activeSelf)
            clothesSlevelPanel.SetChildPanel();
        else if (panel.inheritPanel.gameObject.activeSelf)
            clothesInheritPanel.SetChildPanel();
    }



    public void OnClick(GameObject go)
    {
        //  头部
        if (go == panel.head.gameObject)
        {
            currentType = ClothesType.Head;
            UpdateGridByType(currentType);
        }
        //  上衣
        else if (go == panel.clothes.gameObject)
        {
            currentType = ClothesType.Clothes;
            UpdateGridByType(currentType);
        }
        //  裙子
        else if (go == panel.skirt.gameObject)
        {
            currentType = ClothesType.Skirt;
            UpdateGridByType(currentType);
        }
        //  鞋子
        else if (go == panel.shoe.gameObject)
        {
            currentType = ClothesType.Shoe;
            UpdateGridByType(currentType);
        }
        //  装饰
        else if (go == panel.adorn.gameObject)
        {
            currentType = ClothesType.Adorn;
            UpdateGridByType(currentType);
        }
        //  穿戴
        else if (go == panel.wearBtn.gameObject)
        {
            if (clothesInfo == null)
                return;
            ServerCustom.instance.SendClientMethods("onClientChangeClothes", clothesInfo.configID);
        }
        //  换装
        else if (go == panel.changeBtn.gameObject)
        {
            currentTog = panel.changeBtn;
            tabType = TabType.Change;          
            HiddenUI(panel.clothesPanel);
            if (!panel.rightView.gameObject.activeSelf)
                panel.rightView.gameObject.SetActive(true);
            panel.tab.localPosition = new Vector3(-170.5f, -211, 0);
            panel.leftView.gameObject.SetActive(false);
            panel.rightView.gameObject.SetActive(true);
            panel.mModelParent.gameObject.SetActive(true);
            panel.wearBtn.gameObject.SetActive(false);
            SetChildPanel();
        }
        //  强化
        else if (go == panel.levelBtn.gameObject)
        {
            currentTog = panel.levelBtn;
            tabType = TabType.Level;
            HiddenUI(panel.levelPanel);
            panel.tab.localPosition = new Vector3(-560, -211, 0);
            panel.rightView.gameObject.SetActive(false);
            panel.leftView.gameObject.SetActive(true);
            panel.mModelParent.gameObject.SetActive(false);
            if (clothesLevelPanel == null)
                clothesLevelPanel = new ClothesLevelPanel(panel.levelPanel.gameObject);
            babyMediator.UpdateGridByType(babyMediator.currentType);
        }
        //  进阶
        else if (go == panel.sLevelBtn.gameObject)
        {
            currentTog = panel.sLevelBtn;
            tabType = TabType.Slevel;
            HiddenUI(panel.slevelPanel);
            panel.tab.localPosition = new Vector3(-560, -211, 0);
            panel.rightView.gameObject.SetActive(false);
            panel.leftView.gameObject.SetActive(true);
            panel.mModelParent.gameObject.SetActive(false);
            if (clothesSlevelPanel == null)
                clothesSlevelPanel = new ClothesSlevelPanel(panel.slevelPanel.gameObject);
            babyMediator.UpdateGridByType(babyMediator.currentType);
        }
        //  传承
        else if (go == panel.inheritBtn.gameObject)
        {
            currentTog = panel.inheritBtn;
            tabType = TabType.Inherit;
            HiddenUI(panel.inheritPanel);
            panel.tab.localPosition = new Vector3(-170.5f, -211, 0);
            panel.rightView.gameObject.SetActive(false);
            panel.leftView.gameObject.SetActive(false);
            panel.mModelParent.gameObject.SetActive(true);
            if (clothesInheritPanel == null)
                clothesInheritPanel = new ClothesInheritPanel(panel.inheritPanel.gameObject);
            babyMediator.UpdateGridByType(babyMediator.currentType);
        }
        else if (go == panel.babyState.gameObject)
        {
            tabType = TabType.Touch;
            UITexture mohu = panel.touchPanel.FindChild("Texture").GetComponent<UITexture>();
            if (mohu.mainTexture == null)
            {
                LoadSprite.LoaderBGTexture(mohu.material, "baby", false);             
            }
            panel.rightView.gameObject.SetActive(false);
            HiddenUI(panel.touchPanel);
            if (touchPanel == null)
                touchPanel = new TouchPanel(panel.touchPanel.gameObject);
            touchPanel.SetChildPanel();
        }
        else if (go == panel.backBtn.gameObject)
        {
            if (tabType == TabType.Touch)
            {
                if (!babyMediator.fromOtherBtn)
                {
                    tabType = TabType.Change;
                    HiddenUI(panel.clothesPanel);
                    if (!panel.rightView.gameObject.activeSelf)
                        panel.rightView.gameObject.SetActive(true);
                    panel.changeBtn.value = true;
                    panel.babyState.gameObject.SetActive(true);
                    panel.tab.localPosition = new Vector3(-170.5f, -211, 0);
                    panel.leftView.gameObject.SetActive(false);
                    panel.rightView.gameObject.SetActive(true);
                    panel.mModelParent.gameObject.SetActive(true);
                    panel.wearBtn.gameObject.SetActive(false);
                    SetChildPanel();
                }
                else
                    ClosePanel(null);
            }
            else
            {
                ClosePanel(null);
            }
        }
        else if (go == panel.mapBtn.gameObject)
        {
            currentTog.value = true;
            tabType = TabType.Map;
            panel.mapPanel.gameObject.SetActive(true);
            List<object> list = new List<object>();
            foreach (ClothesMapInfo item in ClothesMapConfig.configList.Values)
            {
                list.Add(item);
            }
            panel.mapGrid.AddCustomDataList(list);
        }
        else if (go == panel.mModelParent.gameObject)
        {
            if (!panel.touchPanel.gameObject.activeSelf)
                return;
            int maxLiking = UtilTools.IntParse(BabyLikingConfig.GetBabyStarByStar(5).extent.Split('-')[1]);
            if (babyInfo.liking >= maxLiking)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_clothes_12"));
                return;
            }
            if (babyMediator.touchPanel.chooseTouchItem.itemID == "102034")
            {
                if (babyInfo.closeTouch < 1)
                {
                    GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_clothes_13"));
                    return;
                }
            }
            Vector3 worldV = Vector3.zero;
            Vector3 dianV = Input.mousePosition;
            dianV.x += 430;

            worldV = UICamera.mainCamera.ScreenToWorldPoint(dianV);
            Vector3 uiCamera = UICamera.mainCamera.transform.position;
            uiCamera.z -= 500;
            Vector3 camera = mUModelInfo.camera.transform.position;
            camera.z -= 500;

            Vector3 temp = worldV - uiCamera;
            Ray ray = new Ray(mUModelInfo.camera.transform.position, temp.normalized);
            Debug.DrawRay(camera, temp, Color.red, 1000000);//
            RaycastHit hit;

            // 射线的碰撞检测  
            if (Physics.Raycast(ray, out hit, 1 << 8))
            {
                Debug.Log(hit.collider.name);
            }
            if (hit.collider != null)
                touchPanel.TouchBaby(hit.collider.name);
        }
        else if (go == panel.closeMapInfoBtn.gameObject)
        {
            if (tabType == TabType.Map)
            {
                panel.mapGrid.ClearCustomGrid();
                panel.mapPanel.gameObject.SetActive(false);
                if (panel.clothesPanel.gameObject.activeSelf)
                {
                    tabType = TabType.Change;
                }                 
                else if (panel.levelPanel.gameObject.activeSelf)
                    tabType = TabType.Level;
                else if (panel.slevelPanel.gameObject.activeSelf)
                    tabType = TabType.Slevel;
                else if (panel.inheritPanel.gameObject.activeSelf)
                    tabType = TabType.Inherit;
            }
            else if (tabType == TabType.MapInfo)
            {
                panel.propertiesBtn.gameObject.SetActive(true);
                panel.mapInfo.gameObject.SetActive(false);
                panel.mapScrollView.gameObject.SetActive(true);
                tabType = TabType.Map;
            }
        }
        else if (go == panel.propertiesBtn.gameObject)
        {
            PromptInfo promptInfo = new PromptInfo();
            promptInfo.type = PromptType.Properties;
            PropertiesInfo infos;
            infos = new PropertiesInfo();
            infos.propertiesName = "shoot";
            promptInfo.propertiesInfoList.Add("shoot", infos);
            infos = new PropertiesInfo();
            infos.propertiesName = "controll";
            promptInfo.propertiesInfoList.Add("controll", infos);
            infos = new PropertiesInfo();
            infos.propertiesName = "passBall";
            promptInfo.propertiesInfoList.Add("passBall", infos);
            infos = new PropertiesInfo();
            infos.propertiesName = "defend";
            promptInfo.propertiesInfoList.Add("defend", infos);
            infos = new PropertiesInfo();
            infos.propertiesName = "reel";
            promptInfo.propertiesInfoList.Add("reel", infos);
            infos = new PropertiesInfo();
            infos.propertiesName = "trick";
            promptInfo.propertiesInfoList.Add("trick", infos);
            infos = new PropertiesInfo();        
            infos.propertiesName = "keep";
            promptInfo.propertiesInfoList.Add("keep", infos);
            infos = new PropertiesInfo();
            infos.propertiesName = "steal";
            promptInfo.propertiesInfoList.Add("steal", infos);               
            infos = new PropertiesInfo();
            infos.propertiesName = "tech";
            promptInfo.propertiesInfoList.Add("tech", infos);
            infos = new PropertiesInfo();
            infos.propertiesName = "health";
            promptInfo.propertiesInfoList.Add("health", infos);
            for (int i = 0; i < babyInfo.getRewardList.Count; ++i)
            {
                ClothesMapInfo info = ClothesMapConfig.GetClothesSlevelInfoByID(babyInfo.getRewardList[i]);
                for (int j = 0; j < info.addValue.Split(';').Length; ++j)
                {
                    infos = new PropertiesInfo();
                    infos.propertiesName = info.addValue.Split(';')[j].Split(',')[0];
                    infos.propertiesValue = UtilTools.IntParse(info.addValue.Split(';')[j].Split(',')[1]);
                    if (promptInfo.propertiesInfoList.ContainsKey(infos.propertiesName))
                        promptInfo.propertiesInfoList[infos.propertiesName].propertiesValue += infos.propertiesValue;
                    else
                        promptInfo.propertiesInfoList.Add(infos.propertiesName, infos);
                }                         
            }         
            Facade.SendNotification(NotificationID.Prompt_Show, promptInfo);
        }
    }
    
    public void ChangeClothesSucess()
    {
        clothesInfo.isWear = 1;
        isWearClothesInfo.isWear = 0;
        panel.wearBtn.gameObject.SetActive(false);
        UpdateGridByType(currentType); 
    }
    bool haveClothes(int configID)
    {
        if (babyInfo.clothesInfoList.ContainsKey(configID))
            return true;
        return false;
    }
    bool HaveGetReward(int mapID)
    {
        for (int i = 0; i < BabyMediator.babyInfo.getRewardList.Count; ++i)
        {
            if (babyInfo.getRewardList[i] == mapID)
                return true;         
        }
        return false;
    }
    /// <summary>
    /// 排序
    /// </summary>
    public int CompareItem(object x, object y)
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
                ClothesInfo a = x as ClothesInfo;
                ClothesInfo b = y as ClothesInfo;
                if (a.isWear > b.isWear)
                    return -1;
                else if (a.isWear < b.isWear)
                    return 1;
                else
                    return a.worthValue.CompareTo(b.worthValue);
            }
        }
    }
    
    /// <summary>
    /// 增加鼠标控制界面模型旋转脚本
    /// </summary>
    private void AddSpinWithMouse()
    {
        if (mUModelInfo == null || mUModelInfo.oModel == null)
        {
            return;
        }
        SpinWithMouse swMouse = panel.mModelParent.GetComponent<SpinWithMouse>();
        if (swMouse == null)
        {
            swMouse = panel.mModelParent.gameObject.AddComponent<SpinWithMouse>();
        }
        if (swMouse != null)
        {
            swMouse.target = mUModelInfo.oModel.transform;
        }
    }

    /// <summary>
    /// 释放
    /// </summary>
    protected override void OnDestroy()
    {
        ModelUIManager.ClearModels();
        TimerManager.Destroy("happyTime");
        TimerManager.Destroy("likingTime");
        for (int i = 0; i < transList.Count; ++i)
        {
            transList[i] = null;
        }
        if(clothesLevelPanel!=null)
        {
            clothesLevelPanel.OnDestroy();
            clothesLevelPanel = null;
        }
        if (clothesSlevelPanel != null)
        {
            clothesSlevelPanel.OnDestroy();
            clothesSlevelPanel = null;
        }
        if (clothesInheritPanel != null)
        {
            clothesInheritPanel.OnDestroy();
            clothesInheritPanel = null;
        }
        if (touchPanel != null)
        {
            touchPanel.OnDestroy();
            touchPanel = null;
        }
        for (int i = 0; i < transList.Count; ++i)
        {
            transList[i] = null;
        }
        transList.Clear();
        clothesInfo = null;
        babyMediator = null;
        fromOtherBtn = false;
        currentTog = null;
        panel.mModelParent = null;
        if (cloneMapItem != null)
            MonoBehaviour.Destroy(cloneMapItem);
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
        panel.mModelParent = null;
        panel.slevelPanel = null;
        panel.clothesPanel = null;
        panel.levelPanel = null;
        panel.inheritPanel = null;
        panel.touchPanel = null;
        panel.mapPanel = null;
        panel.tab = null;
        panel.mapInfo = null;
        panel.mModelTexture = null;
        panel.mohu = null;
        panel.scrollView = null;
        panel.ClothesGrid = null;
        panel.leftGrid = null;
        panel.mapGrid = null;
        panel.rightView = null;
        panel.leftView = null;
        panel.mapScrollView = null;
        panel.head = null;
        panel.clothes = null;
        panel.skirt = null;
        panel.shoe = null;
        panel.adorn = null;
        panel.changeBtn = null;
        panel.levelBtn = null;
        panel.sLevelBtn = null;
        panel.inheritBtn = null;
        panel.mapBtn = null;
        panel.lockLevel = null;
        panel.techAdd = null;
        panel.healthAdd = null;
        panel.wearBtn = null;
        panel.babyState = null;
        panel.closeMapInfoBtn = null;
        panel.backBtn = null;
        panel.propertiesBtn = null;
        base.OnDestroy();
    }
}