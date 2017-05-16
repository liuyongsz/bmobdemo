using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;
using System;
using System.Linq;

public enum BallType
{
    Info = 1,
    Skill = 2,
    Fetter = 3,
    Strike = 4,
    Slevel = 5,
    Inherit = 6,
    Ability = 7,
    Mentality = 8,
}

public class PropertiesInfo
{
    public string propertiesName;
    public int propertiesValue;
}
public class teampanel : BasePanel
{
    public UIGrid ballerClipGrid;
    public UISprite backBtn;
    public UISprite levelBtn;
    public UISprite left;
    public UISprite right;
    public UISprite babyState;
    public UILabel techAdd;
    public UILabel healthAdd;
    public UIToggle allballerBtn;
    public UIToggle ballTeamBtn;
    public UILabel ballerLevel;
    public UILabel ballerName;
    public UILabel fightValue;
    public Transform togGroup;
    public UIToggle info;
    public UIToggle skill;
    public UIToggle fetter;
    public UIToggle strike;
    public UIToggle slevel;
    public UIToggle inherit;
    public UIToggle power;
    public UIToggle mentality;
    public UILabel shootValue;
    public UILabel passValue;
    public UILabel reelValue;
    public UILabel keepValue; 
    public UILabel controlValue;
    public UILabel defValue; 
    public UILabel trickValue;
    public UILabel stealValue;
    public UILabel techValue;
    public UILabel healthValue;
    public UISprite position_1;
    public UISprite position_2;
    public UISprite position_3;
    public UISlider ExpSlider;
    public UITexture ballerIcon;
    public UITexture mohu;
    public Transform cardInfoPanel;
    public Transform skillPanel;
    public Transform fetterPanel;
    public Transform strikePanel;
    public Transform slevelPanel;
    public Transform inheritPanel;
    public Transform abilityPanel;
    public Transform ballerRateyInfo;
    public Transform mentalityPanel;
    public Transform modelInfo;
    public Transform ballerInfo;
    public Transform stars;
    public Transform addInfo;
}


public class TeamMediator : UIMediator<teampanel>
{

    public teampanel panel
    {
        get
        {
            return m_Panel as teampanel;
        }
    }
    private int chooseGrid = 0;
    public UISprite[] stars;
    private CardInfoPanel cardInfoPanel;
    public static SkillPanel skillPanel;
    private FetterPanel fetterPanel;
    public static StrikePanel strikePanel;
    public static SlevelPanel slevelPanel;
    public static InheritPanel inheritPanel;
    public static AbilityPanel abilityPanel;
    public static MentalityPanel mentalityPanel;
    public static BallerRateyInfo ballerRateyInfo;
    public static TeamBaller currentTeamBaller;
    public static TD_Player clientInfo;
    private int closeFlag = 0;
    private int currentIndex = 0;
    public static BallType currentType;
    public static TeamMediator teamMediator;
    public static DictionaryEx<string, UILabel> ballerInfoList = new DictionaryEx<string, UILabel>();
    public static DictionaryEx<string, UILabel> addInfoList = new DictionaryEx<string, UILabel>();
    public static Dictionary<string, BallerPiece> clipList = new Dictionary<string, BallerPiece>();
    public static Dictionary<string, TeamBaller> teamList = new Dictionary<string, TeamBaller>();
    private List<TeamBaller> selectBallerList = new List<TeamBaller>();
    private List<UISprite> textList = new List<UISprite>();

    public TeamMediator() : base("teampanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.Team_Show, OpenPanel);
        RegistPanelCall(NotificationID.Team_Hide, ClosePanel);
    }

    /// <summary>
    /// 选择球队里的球员
    /// </summary>
    public virtual void SetChildPanel()
    {
        for (int i = 0; i < selectBallerList.Count; ++i)
        {
            if (selectBallerList[i].configId == currentTeamBaller.configId)
            {
                currentIndex = i;
                break;
            }
        }
        clientInfo = Instance.Get<PlayerManager>().GetItem(int.Parse(currentTeamBaller.configId));
        
        for (int i = 0; i < textList.Count; ++i)
        {
            if (i < clientInfo.Job.Split(',').Length)
            {
                textList[i].gameObject.SetActive(true);
                textList[i].transform.FindChild("job" + (i + 1)).GetComponent<UILabel>().text = TextManager.GetUIString(clientInfo.Job.Split(',')[i]);
            }
            else
            {
                textList[i].gameObject.SetActive(false);
            }
        }
        closeFlag = 2;
        panel.inherit.gameObject.SetActive(currentTeamBaller.isSelf != 1);
        panel.strike.gameObject.SetActive(currentTeamBaller.isSelf != 1);
        panel.slevel.gameObject.SetActive(currentTeamBaller.isSelf != 1);
        if (currentTeamBaller.isSelf == 1)
        {
            panel.power.transform.localPosition = panel.strike.transform.localPosition;
            panel.mentality.transform.localPosition = panel.slevel.transform.localPosition;
        }
        else
        {
            panel.power.transform.localPosition = new Vector3(568, -197,0);
            panel.mentality.transform.localPosition = new Vector3(568, -270, 0);
        }
        panel.babyState.gameObject.SetActive(currentTeamBaller.isSelf == 1);
        panel.addInfo.gameObject.SetActive(currentType == BallType.Slevel|| currentType == BallType.Strike);
        if (panel.babyState.gameObject.activeSelf)
        {
            BabyStar info = BabyLikingConfig.GetBabyStarByLiking(BabyMediator.babyInfo.liking);
            panel.babyState.spriteName = UtilTools.StringBuilder("state", info.state);
            panel.techAdd.text = UtilTools.StringBuilder(TextManager.GetUIString("tech"), "  +", info.addvalue.Split(',')[0], "%");
            panel.healthAdd.text = UtilTools.StringBuilder(TextManager.GetUIString("health"), "  +", info.addvalue.Split(',')[1], "%");
        }       
        panel.levelBtn.gameObject.SetActive(currentTeamBaller.isSelf != 1);
        int needExp = PlayerManager.GetPlayerItem(currentTeamBaller.level).needExp;
        panel.ExpSlider.transform.FindChild("Text").GetComponent<UILabel>().text = currentTeamBaller.exp + "/" + needExp;
        panel.ExpSlider.value = currentTeamBaller.exp * 1.0f / needExp;
        if (currentTeamBaller.brokenLayer != 0)
            panel.ballerName.text = TextManager.GetItemString(currentTeamBaller.configId) + string.Format("[05FF2D]{0}[-]", "  +" + currentTeamBaller.brokenLayer);
        else
            panel.ballerName.text = TextManager.GetItemString(currentTeamBaller.configId);
        UtilTools.SetStar(currentTeamBaller.star, stars, clientInfo.maxstar);
        LoadSprite.LoaderPlayerSkin(panel.ballerIcon, clientInfo.id.ToString(), false);
        ShowBallerInfo();     
        if (currentType != BallType.Info)
            return;
        panel.allballerBtn.gameObject.SetActive(false);
        panel.ballTeamBtn.gameObject.SetActive(false);
        panel.ballerClipGrid.gameObject.SetActive(false);
        panel.togGroup.gameObject.SetActive(true);
        panel.modelInfo.gameObject.SetActive(true);
        panel.ballerInfo.gameObject.SetActive(true);
        panel.cardInfoPanel.gameObject.SetActive(true);
        if (cardInfoPanel == null)
        {
            cardInfoPanel = new CardInfoPanel(panel.cardInfoPanel.gameObject);
        }
        cardInfoPanel.SetChildPanel();
       
    }

    /// <summary>
    /// 刷新球员属性
    /// </summary>
    public void ShowBallerInfo()
    {
        foreach (string key in ballerInfoList.Keys)
        {
            string propBase = GameConvert.StringConvert( currentTeamBaller.GetType().GetField(key).GetValue(currentTeamBaller));

            if(currentTeamBaller.inTeam==1)
            {
                int formationAdd = TeamFormationConfig.GetFormationPropValue(key);
                int relateAdd = TeamFormationConfig.GetRelatePropValue(currentTeamBaller.id, key);
                propBase = (float.Parse(propBase) + float.Parse(formationAdd.ToString()) + float.Parse(relateAdd.ToString())).ToString();
            }

            if (key == "tech" || key == "health")
            {
                ballerInfoList[key].text = float.Parse(propBase) * 100 + "%";
            }
            else if (key == "keep")
            {
                ballerInfoList[key].text = int.Parse(propBase) * (1 + currentTeamBaller.keepPercent) + "";
            }
            else if (key == "controll")
            {
                ballerInfoList[key].text = int.Parse(propBase) * (1 + currentTeamBaller.controllPercent) + "";
            }
            else if (key == "defend")
            {
                ballerInfoList[key].text = int.Parse(propBase) * (1 + currentTeamBaller.defendPercent) + "";
            }
            else if (key == "shoot")
            {
                ballerInfoList[key].text = int.Parse(propBase) * (1 + currentTeamBaller.shootPercent) + "";
            }
            else
                ballerInfoList[key].text = propBase;

            if (panel.slevelPanel.gameObject.activeSelf)
            {
                PlayerItem info;
                if (clientInfo.maxstar == currentTeamBaller.star)
                    info = null;
                else
                    info = PlayerManager.GetPlayerSlevelInfo(currentTeamBaller.star);
                UpdateSlevelAddValue(key, info);
            }
            else if(panel.strikePanel.gameObject.activeSelf)
            {
                PlayerItem info;
                if (currentTeamBaller.brokenLayer == 20)
                    info = null;
                else
                    info = PlayerManager.GetStrikeInfoByID(clientInfo.initialstar * 100 + currentTeamBaller.brokenLayer + 1);
                UpdateStrikeAddValue(key, info);
            }          
        }
    }
    void UpdateSlevelAddValue(string name, PlayerItem info)
    {
        if (name == "level" || name == "fightValue")
            return;
        if (name == "tech" || name == "health")
        {
            addInfoList[name].text = string.Empty;
            return;
        }
           
        string addValue = string.Empty;
        if (info == null)
        {
            addValue = TextManager.GetUIString("UIMax");
            addInfoList[name].text = addValue;
            return;
        }
        switch (name)
        {
            case "shoot":
                addValue = info.shoot.ToString();
                break;
            case "defend":
                addValue = info.def.ToString();
                break;
            case "controll":
                addValue = info.control.ToString();
                break;
            case "keep":
                addValue = info.keep.ToString();
                break;
            case "trick":
                addValue = info.trick.ToString();
                break;
            case "steal":
                addValue = info.steal.ToString();
                break;
            case "passBall":
                addValue = info.pass.ToString();
                break;
            case "reel":
                addValue = info.reel.ToString();
                break;
        }
        addInfoList[name].text = "+" + addValue;
    }

    void UpdateStrikeAddValue(string name, PlayerItem info)
    {
        if (name == "level" || name == "fightValue")
            return;
        string addValue = string.Empty;
        if (info == null)
        {
            addValue = TextManager.GetUIString("UIMax");
            addInfoList[name].text = addValue;
            return;
        }
        switch (name)
        {
            case "shoot":
                addValue = info.shoot.ToString();
                break;
            case "defend":
                addValue = info.def.ToString();
                break;
            case "controll":
                addValue = info.control.ToString();
                break;
            case "keep":
                addValue = info.keep.ToString();
                break;
            case "trick":
                addValue = info.trick.ToString();
                break;
            case "steal":
                addValue = info.steal.ToString();
                break;
            case "passBall":
                addValue = info.pass.ToString();
                break;
            case "reel":
                addValue = info.reel.ToString();
                break;
            case "tech":
                addValue = UtilTools.StringBuilder("+",info.tech ,"%");
                addInfoList[name].text = addValue;
                return;
            case "health":
                addValue = UtilTools.StringBuilder("+",info.health,"%");
                addInfoList[name].text = addValue;
                return;
        }
        addInfoList[name].text = "+" + addValue;
    }
    /// <summary>
    /// 界面显示
    /// </summary>
    /// <param name="notification"></param>
    protected override void OnShow(INotification notification)
    {
        ServerCustom.instance.SendClientMethods("onClientGetCoachList");
        LoadSprite.LoaderImage(panel.mohu, "bg/qiuchang", false);
        if (teamMediator == null)
        {
            teamMediator = Facade.RetrieveMediator("TeamMediator") as TeamMediator;
        }
        foreach (TeamBaller item in teamList.Values)
        {
            selectBallerList.Add(item);
        }      
        currentType = BallType.Info;
        textList.Add(panel.position_1);
        textList.Add(panel.position_2);
        textList.Add(panel.position_3);
        ballerInfoList.Add("level", panel.ballerLevel);
        ballerInfoList.Add("fightValue", panel.fightValue);
        ballerInfoList.Add("shoot", panel.shootValue);
        ballerInfoList.Add("passBall", panel.passValue);
        ballerInfoList.Add("reel", panel.reelValue);
        ballerInfoList.Add("keep", panel.keepValue);
        ballerInfoList.Add("controll", panel.controlValue);
        ballerInfoList.Add("defend", panel.defValue);
        ballerInfoList.Add("trick", panel.trickValue);
        ballerInfoList.Add("steal", panel.stealValue);
        ballerInfoList.Add("tech", panel.techValue);
        ballerInfoList.Add("health", panel.healthValue);
        UILabel[] labels = panel.addInfo.GetComponentsInChildren<UILabel>();
        addInfoList.Add("shoot", labels[0]);
        addInfoList.Add("passBall", labels[1]);
        addInfoList.Add("reel", labels[2]);
        addInfoList.Add("keep", labels[3]);
        addInfoList.Add("tech", labels[4]);
        addInfoList.Add("controll", labels[5]);
        addInfoList.Add("defend", labels[6]);
        addInfoList.Add("trick", labels[7]);
        addInfoList.Add("steal", labels[8]);
        addInfoList.Add("health", labels[9]);
        closeFlag = 0;       
        foreach (BallerPiece item in clipList.Values)
        {
            if (IsTeam(item.configID))
            {
                item.isHave = 1;
            }
        }
        Dictionary<string, BallerPiece> dicSort = (from objDic in clipList orderby objDic.Value.isHave descending select objDic).ToDictionary(k => k.Key, v => v.Value);
        clipList = dicSort;
        stars = panel.stars.GetComponentsInChildren<UISprite>();
        panel.ballerClipGrid.enabled = true;
        panel.ballerClipGrid.BindCustomCallBack(UpdateTeamGrid);
        panel.ballerClipGrid.StartCustom();
        chooseGrid = 0;
        panel.ballerClipGrid.AddCustomDataList(AddTeamGridLsit());
        if (notification.Body != null)
        {
            string itemID = notification.Body as string;          
        } 
    }
    List<object> AddTeamGridLsit()
    {
        // 默认打开球队卡牌界面
        List<object> listObj = new List<object>();
        TeamBaller info;
        int configId = 6000 + PlayerMediator.playerInfo.job + 1;
        teamList.TryGetValue(configId.ToString(), out info);
        if (info != null)
        {
            listObj.Add(info);
        }
        foreach (TeamBaller item in teamList.Values)
        {
            if (item.configId != configId.ToString())
            {
                listObj.Add(item);
            }
        }
        listObj.Sort(CompareBaller);
        return listObj;
    }
    void UpdateTeamGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        UILabel Name = item.mScripts[0] as UILabel;
        UISprite color = item.mScripts[1] as UISprite;
        UITexture head = item.mScripts[2] as UITexture;
        UISprite isTeam = item.mScripts[3] as UISprite;
        UILabel desc = item.mScripts[4] as UILabel;
        UISlider slider = item.mScripts[5] as UISlider;
        UILabel num = item.mScripts[6] as UILabel;
        UILabel level = item.mScripts[7] as UILabel;
        UISprite suipian = item.mScripts[9] as UISprite;
        UITexture stars = item.mScripts[8] as UITexture;
        UISprite btn = item.mScripts[10] as UISprite;     
        UISprite[] star = stars.GetComponentsInChildren<UISprite>();
        switch (chooseGrid)
        {
            case 0:
                btn.gameObject.SetActive(false);
                suipian.gameObject.SetActive(false);
                level.gameObject.SetActive(true);
                slider.gameObject.SetActive(true);
                TeamBaller baller = item.oData as TeamBaller;
                TD_Player _player = Instance.Get<PlayerManager>().GetItem(int.Parse(baller.configId));
                item.onClick = ClickBallerItem;
                color.spriteName = UtilTools.StringBuilder("color", baller.star);               
                UtilTools.SetTextColor(Name, baller.star);
                UtilTools.SetStar(baller.star, star, _player.maxstar);
                if (baller.brokenLayer < 1)
                    Name.text = TextManager.GetItemString(baller.configId);
                else
                {
                    string strikeLevel = string.Format("[05FF2D]{0}[-]", "  +" + baller.brokenLayer);
                    Name.text = UtilTools.StringBuilder(TextManager.GetItemString(baller.configId), strikeLevel);
                }
                LoadSprite.LoaderHead(head, UtilTools.StringBuilder("Card", baller.configId), false);
                isTeam.gameObject.SetActive(baller.inTeam == 1);
                level.text = UtilTools.StringBuilder("Lv ", baller.level.ToString());
                desc.text = TextManager.GetUIString("UI1087");
                PlayerItem playerInfo = PlayerManager.GetPlayerItem(baller.level);
                slider.value = baller.exp * 1.0f / playerInfo.needExp;
                num.text = UtilTools.StringBuilder(baller.exp, "/", playerInfo.needExp);
                break;
            case 1:
                level.gameObject.SetActive(false);
                BallerPiece info = item.oData as BallerPiece;
                TD_Player player = Instance.Get<PlayerManager>().GetItem(int.Parse(info.configID));
                LoadSprite.LoaderHead(head, UtilTools.StringBuilder("Card", info.configID), false);
                isTeam.gameObject.SetActive(info.isHave == 1);
                desc.text = TextManager.GetUIString("UI1078");
                Name.text = TextManager.GetItemString(info.configID);
                UtilTools.SetTextColor(Name, player.initialstar);
                UtilTools.SetStar(player.initialstar, star, player.maxstar);
                MaterialItemInfo itemInfo = ItemManager.GetMaterialInfo(info.itemID);
                color.spriteName = UtilTools.StringBuilder("color", player.initialstar);
                if (info.isHave == 1)
                {
                    btn.gameObject.SetActive(false);
                    suipian.gameObject.SetActive(false);
                    slider.gameObject.SetActive(false);
                }
                else
                {
                    slider.gameObject.SetActive(info.amount < itemInfo.needAmount);
                    suipian.gameObject.SetActive(info.amount < itemInfo.needAmount);
                    slider.value = info.amount * 1.0f / itemInfo.needAmount;
                    num.text = UtilTools.StringBuilder(info.amount, "/", itemInfo.needAmount);
                    btn.gameObject.SetActive(info.amount >= itemInfo.needAmount);
                    UIEventListener.Get(btn.gameObject).onClick = onClientCommbine;
                }
                break;
        }
    }
    void onClientCommbine(GameObject go)
    {
        BallerPiece data = go.GetComponentInParent<UIGridItem>().oData as BallerPiece;
        if (data == null)
            return;
        ServerCustom.instance.SendClientMethods("onClientCommbine", long.Parse(data.uuid));
        clipList[data.itemID].isHave = 1;
        List<object> listObj = new List<object>();
        foreach (BallerPiece item in clipList.Values)
        {
            listObj.Add(item);
        }
        listObj.Sort(CompareBallerPiece);
        chooseGrid = 1;
        panel.ballerClipGrid.UpdateCustomDataList(listObj);
    }
    public void CommbineSucess(TeamBaller baller)
    {
        teamList.Add(baller.configId, baller);
        selectBallerList.Add(baller);     
    }

    private void ClickBallerItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        currentType = BallType.Info;
        currentTeamBaller = item.oData as TeamBaller;
        clientInfo = Instance.Get<PlayerManager>().GetItem(int.Parse(currentTeamBaller.configId));
        SetChildPanel();
    }
    /// <summary>
    /// 界面关闭时调用，释放内存
    /// </summary>
    protected override void OnDestroy()
    {
        stars = null;
        skillPanel = null;
        fetterPanel = null;
        if (strikePanel != null)
        {
            strikePanel.OnDestroy();
            strikePanel = null;
        }
        if (cardInfoPanel != null)
        {
            cardInfoPanel.OnDestroy();
            cardInfoPanel = null;
        }
        if (skillPanel != null)
        {
            skillPanel.OnDestroy();
            skillPanel = null;
        }
        skillPanel = null;
        slevelPanel = null;
        inheritPanel = null;
        abilityPanel = null;
        mentalityPanel = null;
        ballerRateyInfo = null;
        currentTeamBaller = null;
        clientInfo = null;
        closeFlag = 0;
        currentType = BallType.Info;
        teamMediator = null;
        if (textList.Count > 0)
        {
            for (int i = 0; i < textList.Count; ++i)
            {
                textList[i] = null;
            }
            textList.Clear();
        }
        for (int i = 0; i < ballerInfoList.mList.Count; ++i)
            ballerInfoList[ballerInfoList.mList[i]] = null;
        for (int i = 0; i < addInfoList.mList.Count; ++i)
            addInfoList[addInfoList.mList[i]] = null;
        ballerInfoList.Clear();
        addInfoList.Clear();
        TimerManager.Destroy("coachTime");
        base.OnDestroy();
    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(m_Panel.backBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.allballerBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.ballTeamBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.levelBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.info.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.skill.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.fetter.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.strike.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.slevel.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.inherit.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.power.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.mentality.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.left.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.right.gameObject).onClick = OnClick; 
        UIEventListener.Get(m_Panel.babyState.gameObject).onClick = OnClick;
    }
    Color SetTextColor(BallType type)
    {
        if (currentType == type)
            return Color.black;
        else
            return Color.white;
    }
    private void OnClick(GameObject go)
    {
        if (go == panel.backBtn.gameObject)
        {
            if (closeFlag == 2)
            {
                panel.info.value = true;
                DisappearUI(currentType);
                panel.ballerClipGrid.AddCustomDataList(AddTeamGridLsit());
                panel.allballerBtn.gameObject.SetActive(true);
                panel.ballTeamBtn.gameObject.SetActive(true);
                panel.ballerClipGrid.gameObject.SetActive(true);
                panel.togGroup.gameObject.SetActive(false);
                panel.modelInfo.gameObject.SetActive(false);
                panel.addInfo.gameObject.SetActive(false);
                panel.ballerInfo.gameObject.SetActive(false);
                TweenPosition.Begin(panel.ballerInfo.gameObject, 0, new Vector3(0, 0, 0));
                closeFlag = 0;
                return;
            }
            Facade.SendNotification(NotificationID.Team_Hide);
        }
        else if (go == panel.levelBtn.gameObject)
        {
            List<object> ballerList = new List<object>();
            ballerList.Add(teamList[currentTeamBaller.configId]);
            foreach (TeamBaller item in teamList.Values)
            {
                if (item.configId.Contains("600") || item.isSelf == 1 || item.configId == currentTeamBaller.configId)
                    continue;
                ballerList.Add(item);
            }
            Facade.SendNotification(NotificationID.BallerLevel_Show, ballerList);
        }
        else if (go == panel.left.gameObject)
        {
            if (currentIndex == 0)
            {
                currentTeamBaller = teamList[selectBallerList[selectBallerList.Count - 1].configId];
                currentIndex = selectBallerList.Count - 1;
            }
            else
            {
                currentIndex--;
                currentTeamBaller = teamList[selectBallerList[currentIndex].configId];
            }
            clientInfo = Instance.Get<PlayerManager>().GetItem(int.Parse(currentTeamBaller.configId));
            bool isHide = currentType == BallType.Slevel || currentType == BallType.Strike || currentType == BallType.Inherit;
            if (currentTeamBaller.isSelf == 1 && isHide)
            {
                currentType = BallType.Info;
                panel.info.value = true;
                panel.cardInfoPanel.gameObject.SetActive(true);
                panel.inheritPanel.gameObject.SetActive(false);
                panel.slevelPanel.gameObject.SetActive(false);
                panel.strikePanel.gameObject.SetActive(false);
                SetChildPanel();
                TweenPosition.Begin(panel.ballerInfo.gameObject, 0, new Vector3(0, 0, 0));
                return;
            }
            SetChildPanel();
            UpdateInfoPanel();
        }
        else if (go == panel.right.gameObject)
        {
            if (currentIndex == selectBallerList.Count - 1)
            {
                currentTeamBaller = teamList[selectBallerList[0].configId];
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
                currentTeamBaller = teamList[selectBallerList[currentIndex].configId];
            }
            clientInfo = Instance.Get<PlayerManager>().GetItem(int.Parse(currentTeamBaller.configId));
            bool isHide = currentType == BallType.Slevel || currentType == BallType.Strike || currentType == BallType.Inherit;
            if (currentTeamBaller.isSelf == 1 && isHide)
            {
                currentType = BallType.Info;
                panel.info.value = true;
                panel.cardInfoPanel.gameObject.SetActive(true);
                panel.inheritPanel.gameObject.SetActive(false);
                panel.slevelPanel.gameObject.SetActive(false);
                panel.strikePanel.gameObject.SetActive(false);
                TweenPosition.Begin(panel.ballerInfo.gameObject, 0, new Vector3(0, 0, 0));
                SetChildPanel();
                return;
            }
            SetChildPanel();
            UpdateInfoPanel();
        }
        else if (go == panel.allballerBtn.gameObject)
        {
            if (closeFlag == 1)
            {
                return;
            }

            List<object> listObj = new List<object>();
            foreach (BallerPiece item in clipList.Values)
            {
                listObj.Add(item);
            }
            listObj.Sort(CompareBallerPiece);
            chooseGrid = 1;
            panel.ballerClipGrid.AddCustomDataList(listObj);
            closeFlag = 1;
        }
        else if (go == panel.ballTeamBtn.gameObject)
        {
            if (closeFlag == 0)
            {
                return;
            }
            chooseGrid = 0;
            panel.ballerClipGrid.AddCustomDataList(AddTeamGridLsit());
            closeFlag = 0;
        }
        // 球员信息
        else if (go == panel.info.gameObject)
        {
            if (currentType == BallType.Info)
                return;
            currentType = BallType.Info;
            SetToggleState(currentType);
            TweenPosition.Begin(panel.ballerInfo.gameObject, 0, new Vector3(0, 0, 0));
            if (currentTeamBaller == null)
            {
                cardInfoPanel = new CardInfoPanel(panel.cardInfoPanel.gameObject);
            }
            cardInfoPanel.SetChildPanel();
            ShowBallerInfo();
        }
        // 球员技能
        else if (go == panel.skill.gameObject)
        {
            if (currentType == BallType.Skill)
                return;
            currentType = BallType.Skill;
            SetToggleState(currentType);
            if (skillPanel == null)
            {
                skillPanel = new SkillPanel(panel.skillPanel.gameObject);
            }
           
            skillPanel.SetChildPanel();
        }
        // 球员羁绊
        else if (go == panel.fetter.gameObject)
        {
            if (currentType == BallType.Fetter)
                return;
            currentType = BallType.Fetter;
            SetToggleState(currentType);
            if (fetterPanel == null)
            {
                fetterPanel = new FetterPanel(panel.fetterPanel.gameObject);
            }
            fetterPanel.SetChildPanel();
        }
        // 球员突破
        else if (go == panel.strike.gameObject)
        {
            if (currentType == BallType.Strike)
                return;
            currentType = BallType.Strike;
            TweenPosition.Begin(panel.ballerInfo.gameObject, 0, new Vector3(0, 53, 0));
            SetToggleState(currentType);
            if (strikePanel == null)
            {
                strikePanel = new StrikePanel(panel.strikePanel.gameObject);
            }
            strikePanel.SetChildPanel();
            ShowBallerInfo();
        }
        // 球员进阶
        else if (go == panel.slevel.gameObject)
        {
            if (currentType == BallType.Slevel)
                return;
            currentType = BallType.Slevel;
            TweenPosition.Begin(panel.ballerInfo.gameObject, 0, new Vector3(0, 53, 0));
            SetToggleState(currentType);
            if (slevelPanel == null)
            {
                slevelPanel = new SlevelPanel(panel.slevelPanel.gameObject);
            }
            slevelPanel.SetChildPanel();
            ShowBallerInfo();
        }
        // 球员传承
        else if (go == panel.inherit.gameObject)
        {
            if (currentType == BallType.Inherit)
                return;
            if (currentTeamBaller.isSelf == 1)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_50"));
                return;
            }
            currentType = BallType.Inherit;
            SetToggleState(currentType);
            if (inheritPanel == null)
            {
                inheritPanel = new InheritPanel(panel.inheritPanel.gameObject);
            }
            inheritPanel.SetChildPanel();
        }
        // 球员能力
        else if (go == panel.power.gameObject)
        {
            if (currentType == BallType.Ability)
                return;
            currentType = BallType.Ability;
            SetToggleState(currentType);
            if (abilityPanel == null)
            {
                abilityPanel = new AbilityPanel(panel.abilityPanel.gameObject);
            }
            if (ballerRateyInfo == null)
            {
                ballerRateyInfo = new BallerRateyInfo(panel.ballerRateyInfo.gameObject);
            }
            ballerRateyInfo.SetChildPanel();
            abilityPanel.SetChildPanel();
        }
        // 球员意识
        else if (go == panel.mentality.gameObject)
        {
            if (currentType == BallType.Mentality)
                return;
            currentType = BallType.Mentality;
            SetToggleState(currentType);
            if (mentalityPanel == null)
            {
                mentalityPanel = new MentalityPanel(panel.mentalityPanel.gameObject);
            }
            if (ballerRateyInfo == null)
            {
                ballerRateyInfo = new BallerRateyInfo(panel.ballerRateyInfo.gameObject);
            }
            ballerRateyInfo.SetChildPanel();
            mentalityPanel.SetChildPanel();
        }
        else if (go == panel.babyState.gameObject)
        {
            if (BabyMediator.babyInfo.clothesInfoList.Count < 1)
                ServerCustom.instance.SendClientMethods("onClientGetBabyItemInfo");
            else
                Facade.SendNotification(NotificationID.Baby_Show, 1);
        }
        panel.addInfo.gameObject.SetActive(currentType == BallType.Slevel || currentType == BallType.Strike);
    }
    void UpdateInfoPanel()
    {
        if (panel.cardInfoPanel.gameObject.activeSelf)
        {
            cardInfoPanel.SetChildPanel();
        }
        else if (panel.fetterPanel.gameObject.activeSelf)
        {
            fetterPanel.SetChildPanel();
        }
        else if (panel.inheritPanel.gameObject.activeSelf)
        {
            inheritPanel.SetChildPanel();
        }
        else if (panel.strikePanel.gameObject.activeSelf)
        {
            strikePanel.SetChildPanel();
        }
        else if (panel.slevelPanel.gameObject.activeSelf)
        {
            slevelPanel.SetChildPanel();
        }
        else if (panel.skillPanel.gameObject.activeSelf)
        {
            skillPanel.SetChildPanel();
        }
        else if (panel.abilityPanel.gameObject.activeSelf)
        {
            ballerRateyInfo.SetChildPanel();
            abilityPanel.SetChildPanel();
        }
        else if (panel.mentalityPanel.gameObject.activeSelf)
        {
            ballerRateyInfo.SetChildPanel();
            mentalityPanel.SetChildPanel();
        }
    }
    /// <summary>
    /// 选中功能
    /// </summary>
    void SetToggleState(BallType type)
    {
        panel.cardInfoPanel.gameObject.SetActive(type == BallType.Info);
        panel.ballerInfo.gameObject.SetActive(type == BallType.Info || type == BallType.Strike|| type == BallType.Slevel);
        panel.ballerRateyInfo.gameObject.SetActive(type == BallType.Mentality || type == BallType.Ability);
        panel.skillPanel.gameObject.SetActive(type == BallType.Skill);
        panel.fetterPanel.gameObject.SetActive(type == BallType.Fetter);
        panel.strikePanel.gameObject.SetActive(type == BallType.Strike);
        panel.slevelPanel.gameObject.SetActive(type == BallType.Slevel);
        panel.inheritPanel.gameObject.SetActive(type == BallType.Inherit);
        panel.abilityPanel.gameObject.SetActive(type == BallType.Ability);
        panel.mentalityPanel.gameObject.SetActive(type == BallType.Mentality);
        panel.ballerRateyInfo.gameObject.SetActive(type == BallType.Mentality || type == BallType.Ability);
    }
 
    /// <summary>
    /// 返回到一级界面时隐藏所有二级界面
    /// </summary>
    void DisappearUI(BallType type)
    {
        switch(type)
        {
            case BallType.Info:
                panel.cardInfoPanel.gameObject.SetActive(false);
                break;
            case BallType.Skill:
                panel.skillPanel.gameObject.SetActive(false);
                break;
            case BallType.Fetter:
                panel.fetterPanel.gameObject.SetActive(false);
                break;
            case BallType.Strike:
                panel.strikePanel.gameObject.SetActive(false);
                break;
            case BallType.Slevel:
                panel.slevelPanel.gameObject.SetActive(false);
                break;
            case BallType.Inherit:
                panel.inheritPanel.gameObject.SetActive(false); 
                break;
            case BallType.Ability:
                panel.abilityPanel.gameObject.SetActive(false);
                panel.ballerRateyInfo.gameObject.SetActive(false);
                break;
            case BallType.Mentality:
                panel.mentalityPanel.gameObject.SetActive(false);
                panel.ballerRateyInfo.gameObject.SetActive(false);
                break;
        }
    }
    
    bool IsTeam(string configID)
    {
        if (teamList.ContainsKey(configID))
        {
            return true;
        }
        return false;
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
                if (a.isSelf > b.isSelf)
                    return -1;
                else if (a.isSelf < b.isSelf)
                    return 1;

                if (a.inTeam > b.inTeam)
                    return -1;
                else if (a.inTeam < b.inTeam)
                    return 1;
                return a.configId.CompareTo(b.configId);
            }
        }
    }
    /// <summary>
    /// 排序
    /// </summary>
    public int CompareBallerPiece(object x, object y)
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
                BallerPiece a = x as BallerPiece;
                BallerPiece b = y as BallerPiece;
    
                return a.isHave.CompareTo(b.isHave);
            }
        }
    }
}