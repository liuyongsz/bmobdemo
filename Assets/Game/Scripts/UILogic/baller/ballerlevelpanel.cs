using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;
using System;
public class ballerlevelpanel : BasePanel
{
    public UIGrid levelBallerGrid;
    public UISprite closeLevelBtn;
    public UISprite Item102005;
    public UISprite Item102006;
    public UISprite Item102031;
    public UISprite Item102032;
    public UISprite select;
}

public class BallerLevelMediator : UIMediator<ballerlevelpanel>
{
    private ballerlevelpanel panel
    {
        get
        {
            return m_Panel as ballerlevelpanel;
        }
    }
    private bool severLevelCallSucess = true;
    public static BallerLevelMediator ballerLevelMediator;
    private TeamBaller chooseBaller;
    private string chooseItemID;
    private UILabel consule;

    /// <summary>
    /// 注册界面逻辑
    /// </summary>
    public BallerLevelMediator() : base("ballerlevelpanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.BallerLevel_Show, OpenPanel);
        RegistPanelCall(NotificationID.BallerLevel_Hide, ClosePanel);
    }

    /// <summary>
    /// 界面显示时调用
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        if (ballerLevelMediator == null)
        {
            ballerLevelMediator = Facade.RetrieveMediator("BallerLevelMediator") as BallerLevelMediator;
        }     
        List<object> ballerList = notification.Body as List<object>;
        if (ballerList.Count < 1)
            return;
        object obj = ballerList[0];
        ballerList.RemoveAt(0);
        ballerList.Sort(CompareBaller);
        ballerList.Insert(0,obj);
        chooseBaller = ballerList[0] as TeamBaller;
        panel.levelBallerGrid.enabled = true;
        panel.levelBallerGrid.BindCustomCallBack(UpdateLevelItemGrid);
        panel.levelBallerGrid.StartCustom();
        panel.levelBallerGrid.AddCustomDataList(ballerList);     
        LoadSprite.LoaderItem(panel.Item102005.transform.FindChild("head").GetComponent<UITexture>(), "102005", false);
        panel.Item102005.transform.FindChild("Label").GetComponent<UILabel>().text = ItemManager.GetBagItemCount("102005").ToString();
        panel.Item102005.spriteName = "color" + ItemManager.GetItemInfo("102005").color;
        LoadSprite.LoaderItem(panel.Item102006.transform.FindChild("head").GetComponent<UITexture>(), "102006", false);
        panel.Item102006.transform.FindChild("Label").GetComponent<UILabel>().text = ItemManager.GetBagItemCount("102006").ToString();
        panel.Item102006.spriteName = "color" + ItemManager.GetItemInfo("102006").color;
        LoadSprite.LoaderItem(panel.Item102031.transform.FindChild("head").GetComponent<UITexture>(), "102031", false);
        panel.Item102031.transform.FindChild("Label").GetComponent<UILabel>().text = ItemManager.GetBagItemCount("102031").ToString();
        panel.Item102031.spriteName = "color" + ItemManager.GetItemInfo("102031").color;
        LoadSprite.LoaderItem(panel.Item102032.transform.FindChild("head").GetComponent<UITexture>(), "102032", false);
        panel.Item102032.transform.FindChild("Label").GetComponent<UILabel>().text = ItemManager.GetBagItemCount("102032").ToString();
        panel.Item102032.spriteName = "color" + ItemManager.GetItemInfo("102032").color;
        LongClickEvent.Get(panel.Item102005.gameObject).onPress = OnPress;
        LongClickEvent.Get(panel.Item102005.gameObject).duration = 2.5f;
        LongClickEvent.Get(panel.Item102006.gameObject).onPress = OnPress;
        LongClickEvent.Get(panel.Item102006.gameObject).duration = 2.5f;
        LongClickEvent.Get(panel.Item102031.gameObject).onPress = OnPress;
        LongClickEvent.Get(panel.Item102031.gameObject).duration = 2.5f;
        LongClickEvent.Get(panel.Item102032.gameObject).onPress = OnPress;
        LongClickEvent.Get(panel.Item102032.gameObject).duration = 2.5f;
        UIEventListener.Get(panel.closeLevelBtn.gameObject).onClick = OnClick;        
    }
    void UpdateLevelItemGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        TeamBaller baller = item.oData as TeamBaller;
        item.onClick = ClickItem;
        UILabel Name = item.mScripts[0] as UILabel;
        UISprite color = item.mScripts[1] as UISprite;
        UITexture head = item.mScripts[2] as UITexture;
        UISlider slider = item.mScripts[3] as UISlider;
        UILabel sliderNum = item.mScripts[4] as UILabel;
        UILabel nowlevel = item.mScripts[5] as UILabel;
        UILabel nextlevel = item.mScripts[6] as UILabel;
        UITexture stars = item.mScripts[7] as UITexture;
        UISprite select = item.mScripts[8] as UISprite;
        select.gameObject.SetActive(baller == chooseBaller);
        Name.text = TextManager.GetItemString(baller.configId);
        color.spriteName = UtilTools.StringBuilder("color" + baller.star);
        LoadSprite.LoaderHead(head, UtilTools.StringBuilder("Card" + baller.configId), false);
        nowlevel.text = baller.level.ToString();
        nextlevel.text = (baller.level + 1).ToString();
        PlayerItem info = PlayerManager.GetPlayerItem(baller.level);
        TD_Player player = Instance.Get<PlayerManager>().GetItem(int.Parse(baller.configId));
        UtilTools.SetStar(baller.star, stars.GetComponentsInChildren<UISprite>(), player.maxstar);
        if (info != null)
        {
            slider.value = baller.exp * 1.0f / info.needExp;
            sliderNum.text = UtilTools.StringBuilder(baller.exp, "/", info.needExp);
        }
    }
    void OnClick(GameObject go)
    {
        if (go == panel.closeLevelBtn.gameObject)
        {
            ClosePanel(null);
        }
    }

    public void LevelSucess()
    {
        ConsumeItemInfo item = ItemManager.GetConsueItemInfo(chooseItemID);
        if (item == null)
            return;
        consule.text = (UtilTools.IntParse(consule.text) - 1).ToString();
        chooseBaller.exp += item.propValue;
        while (chooseBaller.level < PlayerManager.playerLevelList.Count)
        {
            PlayerItem info = PlayerManager.GetPlayerItem(chooseBaller.level);
            if (chooseBaller.exp >= info.needExp)
            {
                chooseBaller.level += 1;
                info = PlayerManager.GetPlayerItem(chooseBaller.level);
                chooseBaller.shoot += info.shoot;
                chooseBaller.trick += info.trick;
                chooseBaller.defend += info.shoot;
                chooseBaller.controll += info.control;
                chooseBaller.keep += info.keep;
                chooseBaller.steal += info.steal;
                chooseBaller.reel += info.reel;
                chooseBaller.tech += info.tech;
                chooseBaller.health += info.health;
                chooseBaller.passBall += info.pass;
            }
            else
                break;
        }
        UpdateTeamData();
        panel.levelBallerGrid.UpdateCustomData(chooseBaller);
        severLevelCallSucess = true;
    }

    public void LevelLimit(int level)
    {
        PlayerItem info;
        info = PlayerManager.GetPlayerItem(level);
        chooseBaller.exp = info.needExp;
        for (int i = chooseBaller.level; i < level; ++i)
        {
            info = PlayerManager.GetPlayerItem(chooseBaller.level);           
            chooseBaller.shoot += info.shoot;
            chooseBaller.trick += info.trick;
            chooseBaller.defend += info.def;
            chooseBaller.controll += info.control;
            chooseBaller.keep += info.keep;
            chooseBaller.steal += info.steal;
            chooseBaller.reel += info.reel;
            chooseBaller.tech += info.tech;
            chooseBaller.health += info.health;
            chooseBaller.passBall += info.pass;
        }
        chooseBaller.level = level;
        UpdateTeamData();
        panel.levelBallerGrid.UpdateCustomData(chooseBaller);      
        severLevelCallSucess = true;
        GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_35"), null);
    }

    void UpdateTeamData()
    {
        if (chooseBaller.id == TeamMediator.currentTeamBaller.id)
        {
            TeamMediator.currentTeamBaller = chooseBaller;
            TeamMediator.teamMediator.ShowBallerInfo();
            int needExp = PlayerManager.GetPlayerItem(chooseBaller.level).needExp;
            TeamMediator.teamMediator.panel.ExpSlider.transform.FindChild("Text").GetComponent<UILabel>().text = chooseBaller.exp + "/" + needExp;
            TeamMediator.teamMediator.panel.ExpSlider.value = chooseBaller.exp * 1.0f / needExp;
        }
        else
        {
            if (TeamMediator.teamList.ContainsKey(chooseBaller.configId))
            {
                TeamMediator.teamList[chooseBaller.configId] = chooseBaller;
            }
        }
    }
    void OnPress(GameObject go,bool isPress)
    {
        GoToPosition(go.name);
        if (chooseBaller.level == PlayerManager.playerLevelList.Count)
        {
            GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_6"), null);
            return;
        }
        if (chooseBaller.level >= PlayerMediator.playerInfo.level)
        {
            GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_26"), null);
            return;
        }
        consule = go.transform.FindChild("Label").GetComponent<UILabel>();        
        if (UtilTools.IntParse(consule.text)<1)
        {
            GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_7"), null);
            return;
        }
        if (severLevelCallSucess)
        {                        
            Item info = ItemManager.GetBagItemInfo(go.name.Replace("Item", ""));
            if (info == null)
            {
                return;
            }
            ServerCustom.instance.SendClientMethods("onClientLevelUp", chooseBaller.id, long.Parse(info.uuid), 1);
            chooseItemID = info.itemID;           
            //升级操作
            severLevelCallSucess = false;
        }           
    }
    void GoToPosition(string itemName)
    {
        switch (itemName)
        {
            case "Item102005":
                panel.select.transform.localPosition = new Vector3(-225, -220, 0);
                break;
            case "Item102006":
                panel.select.transform.localPosition = new Vector3(-88, -220, 0);
                break;
            case "Item102031":
                panel.select.transform.localPosition = new Vector3(46, -220, 0);
                break;
            case "Item102032":
                panel.select.transform.localPosition = new Vector3(178, -220, 0);
                break;
        }
    }
    void ClickItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        if (chooseBaller != null && chooseBaller != item.oData as TeamBaller)
        {
            if(panel.levelBallerGrid.GetCustomGridItem(chooseBaller))
                (panel.levelBallerGrid.GetCustomGridItem(chooseBaller).mScripts[8] as UISprite).gameObject.SetActive(false);
        }
        (item.mScripts[8] as UISprite).gameObject.SetActive(true);
        chooseBaller = item.oData as TeamBaller;
    }

    /// <summary>
    /// 排序
    /// </summary>
    public int CompareBaller(object x, object y)
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
                TeamBaller a = x as TeamBaller;
                TeamBaller b = y as TeamBaller;            
                if (a.inTeam > b.inTeam)
                    return -1;
                else if (a.inTeam < b.inTeam)
                    return 1;
                return a.configId.CompareTo(b.configId);
            }
        }
    }
}
