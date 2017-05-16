using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SkillPanel : TeamMediator
{
    private UITexture skill1;
    private UITexture skill2;
    private UITexture skill3;
    private UITexture skill4;
    private UISprite skillLevelBtn;
    private UISprite selectSkll;
    private UILabel skillName;
    private UILabel currentDesc;
    private UILabel nextDesc;
    private UILabel skillType;
    private UILabel skillLimit;
    private TD_Skill skillInfo;
    private SkillLevelInfo levelInfo;
    private CommonInfo commonInfo;
    private Transform skillMax; 

    private Transform coachPanel;
    private UITexture chooseCoachIcon;
    private UISprite chooseCoachColor;

    private Transform selectSkillPanel;
    private Transform chooseItemTrans;
    private Transform coach1Have;
    private Transform coach2Have;
    private UITexture icon1;
    private UITexture icon2;
    private UISprite helpBtn;
    private UISprite closeBtn;
    private UISprite closeSkillSelecteBtn;
    private UISprite skillSelectBtn;
    private UILabel coach1Diamod;
    private UILabel coach2Diamod;
    private UILabel nextLevel; 
    private UILabel chooseCoachLabel;
    public UILabel cloneCoachLabel;
    public UIGrid skillGrid;
    private int chooseSkillIndex = 1;
    public int chooseCoachIndex = 1;
    private int chooseSkillId = 1;
    private Vector3 pos2;
    private Vector3 pos3;
    public static List<CoachInfo> coachInfoList = new List<CoachInfo>();
    private List<ConSuleItem> ConSuleList = new List<ConSuleItem>();
    private List<object> skillList = new List<object>();
    public SkillPanel(GameObject go)
    {
        selectSkillPanel = UtilTools.GetChild<Transform>(go.transform, "selectSkillPanel");
        skillGrid = UtilTools.GetChild<UIGrid>(go.transform, "selectSkillPanel/ScrollView/skillGrid");
        closeSkillSelecteBtn = UtilTools.GetChild<UISprite>(go.transform, "selectSkillPanel/closeBtn");
        coachPanel = UtilTools.GetChild<Transform>(go.transform, "coachPanel");
        skillMax = UtilTools.GetChild<Transform>(go.transform, "skillMax");
        coach1Have = UtilTools.GetChild<Transform>(go.transform, "coachPanel/1/ishave");
        coach2Have = UtilTools.GetChild<Transform>(go.transform, "coachPanel/2/ishave");
        skill1 = UtilTools.GetChild<UITexture>(go.transform, "skill1");
        skill2 = UtilTools.GetChild<UITexture>(go.transform, "skill2");
        skill3 = UtilTools.GetChild<UITexture>(go.transform, "skill3");
        skill4 = UtilTools.GetChild<UITexture>(go.transform, "skill4");
        skillLevelBtn = UtilTools.GetChild<UISprite>(go.transform, "skillLevel");
        selectSkll = UtilTools.GetChild<UISprite>(go.transform, "selectSkll");
        helpBtn = UtilTools.GetChild<UISprite>(go.transform, "coachPanel/helpBtn");
        closeBtn = UtilTools.GetChild<UISprite>(go.transform, "coachPanel/closeBtn");
        skillName = UtilTools.GetChild<UILabel>(go.transform, "skillName");
        currentDesc = UtilTools.GetChild<UILabel>(go.transform, "currentDesc");
        nextDesc = UtilTools.GetChild<UILabel>(go.transform, "nextDesc");
        skillLimit = UtilTools.GetChild<UILabel>(go.transform, "skillLimit");
        skillType = UtilTools.GetChild<UILabel>(go.transform, "skillType");
        chooseCoachLabel = UtilTools.GetChild<UILabel>(go.transform, "coachPanel/coach/Label/");
        coach1Diamod = UtilTools.GetChild<UILabel>(go.transform, "coachPanel/1/myfight");
        coach2Diamod = UtilTools.GetChild<UILabel>(go.transform, "coachPanel/2/myfight");
        nextLevel = UtilTools.GetChild<UILabel>(go.transform, "nextLevel");
        coach1Diamod.text = CommonConfig.GetCommonInfo(12).value.ToString();
        coach2Diamod.text = CommonConfig.GetCommonInfo(13).value.ToString();
        chooseCoachIcon = UtilTools.GetChild<UITexture>(go.transform, "coachPanel/coach");
        chooseCoachColor = UtilTools.GetChild<UISprite>(go.transform, "coachPanel/coach/color");
        skillSelectBtn = UtilTools.GetChild<UISprite>(go.transform, "skillSelect");
        icon1 = UtilTools.GetChild<UITexture>(go.transform, "coachPanel/1");
        icon2 = UtilTools.GetChild<UITexture>(go.transform, "coachPanel/2");
        pos2 = skill2.transform.localPosition;
        pos3 = skill3.transform.localPosition;
        skillGrid.enabled = true;
        skillGrid.BindCustomCallBack(UpdateSkillGrid);
        skillGrid.StartCustom(); 
        UIEventListener.Get(icon1.gameObject).onClick = OnClickEx;
        UIEventListener.Get(icon2.gameObject).onClick = OnClickEx;
        UIEventListener.Get(skill1.gameObject).onClick = OnClickEx;
        UIEventListener.Get(skill2.gameObject).onClick = OnClickEx;
        UIEventListener.Get(skill3.gameObject).onClick = OnClickEx;
        UIEventListener.Get(skill4.gameObject).onClick = OnClickEx; 
        UIEventListener.Get(helpBtn.gameObject).onClick = OnClickEx;
        UIEventListener.Get(skillLevelBtn.gameObject).onClick = OnClickEx;
        UIEventListener.Get(closeBtn.gameObject).onClick = OnClickEx;
        UIEventListener.Get(skillSelectBtn.gameObject).onClick = OnClickEx;
        UIEventListener.Get(closeSkillSelecteBtn.gameObject).onClick = OnClickEx;
        ConSuleList = UtilTools.SetConSumeItemList(3, coachPanel);
        for (int i = 0; i < ConSuleList.Count; ++i)
        {
            string itemID = coachInfoList[0].cost.Split(',')[i].Split(':')[0];
            ConSuleList[i].trans.name = itemID;
            ConSuleList[i].name.text = TextManager.GetItemString(itemID);
            ConSuleList[i].count.text = ItemManager.GetBagItemCount(itemID).ToString();
            ItemInfo item = ItemManager.GetItemInfo(itemID);
            if (item == null)
                return;
            ConSuleList[i].color.spriteName = UtilTools.StringBuilder("color" + item.color);
            UtilTools.SetTextColor(ConSuleList[i].name, item.color);
            LoadSprite.LoaderItem(ConSuleList[i].Icon, itemID);
            UIEventListener.Get(ConSuleList[i].trans.FindChild("useBtn").gameObject).onClick = OnClickEx;
            UIEventListener.Get(ConSuleList[i].trans.gameObject).onClick = OnClickItemInfo;
        }
            
        for (int i = 0; i < coachInfoList.Count; ++i)
        {
            int index = i + 1;
            chooseCoachLabel = UtilTools.GetChild<UILabel>(go.transform, "coachPanel/coach/Label/");
            coachInfoList[i].icon = UtilTools.GetChild<UITexture>(go.transform, "coach" + index);
            coachInfoList[i].timeLabel = UtilTools.GetChild<UILabel>(go.transform, "coach" + index + "/Label");
            coachInfoList[i].lockSpr = UtilTools.GetChild<UISprite>(go.transform, "coach" + index + "/lock");
            coachInfoList[i].mask = UtilTools.GetChild<UISprite>(go.transform, "coach" + index + "/mask");
            coachInfoList[i].coachColor = UtilTools.GetChild<UISprite>(go.transform, "coach" + index + "/color");
            UIEventListener.Get(coachInfoList[i].icon.gameObject).onClick = OnClickEx;
            coachInfoList[i].lockSpr.gameObject.SetActive(coachInfoList[i].isLock == 0);
            coachInfoList[i].coachColor.gameObject.SetActive(coachInfoList[i].isLock == 1);
            if (coachInfoList[i].isLock == 0)
            {
                coachInfoList[i].icon.GetComponent<UIToggle>().enabled = false;
                coachInfoList[i].icon.transform.FindChild("sprite").gameObject.SetActive(false);
                coachInfoList[i].icon.mainTexture = null;
                coachInfoList[i].timeLabel.color = Color.yellow;
                if (i == 1 || i == 2)
                    coachInfoList[i].timeLabel.text = TextManager.GetUIString("rock1");
                else if (i == 3)
                    coachInfoList[i].timeLabel.text = TextManager.GetUIString("rock2");
                else if (i == 4)
                    coachInfoList[i].timeLabel.text = TextManager.GetUIString("rock3");
                continue;
            }
            else
            {
                coachInfoList[i].icon.GetComponent<UIToggle>().enabled = true;
                coachInfoList[i].icon.transform.FindChild("sprite").gameObject.SetActive(true);
                LoadSprite.LoaderHead(coachInfoList[i].icon, coachInfoList[i].icon.name);
                coachInfoList[i].coachColor.spriteName = UtilTools.StringBuilder("color", coachInfoList[i].color);
                if (coachInfoList[i].useTime < coachInfoList[i].limitTime)
                    coachInfoList[i].timeLabel.color = Color.green;
                else
                    coachInfoList[i].timeLabel.color = Color.red;
                coachInfoList[i].timeLabel.gameObject.SetActive(coachInfoList[i].useTime != 0);
                coachInfoList[i].mask.fillAmount = coachInfoList[i].useTime * 1.0f / coachInfoList[i].limitTime;
                if (coachInfoList[i].timeLabel.gameObject.activeSelf)
                    coachInfoList[i].timeLabel.text = UtilTools.formatDuring(coachInfoList[i].useTime);
                continue;
            }
        }
    }
    public override void SetChildPanel()
    {
        string[] skill = clientInfo.skill.Split(',');
        chooseSkillIndex = 1;
        LoadSprite.LoaderSkill(skill1, clientInfo.skill1);
        skill2.gameObject.SetActive(skill[1] != "0");
        skill3.gameObject.SetActive(skill[2] != "0");
        skill4.gameObject.SetActive(skill[3] != "0");
        if (skill2.gameObject.activeSelf)
        {
            clientInfo.skill2 = skill[1];
            LoadSprite.LoaderSkill(skill2, clientInfo.skill2);
        }
        if (skill3.gameObject.activeSelf && !skill2.gameObject.activeSelf)
        {
            skill3.transform.localPosition = pos2;
            clientInfo.skill3 = skill[2];
            LoadSprite.LoaderSkill(skill3, clientInfo.skill3);
        }
        if (skill4.gameObject.activeSelf)
        {
            if (skill3.transform.localPosition == skill2.transform.localPosition)
                skill4.transform.localPosition = pos3;
            else if (!skill2.gameObject.activeSelf && !skill3.gameObject.activeSelf)
                skill4.transform.localPosition = pos2;
            clientInfo.skill4 = skill[3];
            LoadSprite.LoaderSkill(skill4, clientInfo.skill4);
        }
        UpdateSkillInfo(clientInfo.skill1, skill1.transform, currentTeamBaller.skill1Lv);
    }
    /// <summary>
    /// 初始化技能信息
    /// </summary>
    void UpdateSkillInfo(string skillID, Transform go,int skillLevel)
    {
        skillSelectBtn.gameObject.SetActive(currentTeamBaller.isSelf == 1 && go == skill1.transform);
        chooseSkillId = int.Parse(skillID);
        skillInfo = Instance.Get<SkillConfig>().GetItem(skillID);
        skillName.text = UtilTools.StringBuilder(skillInfo.name, "  Lv.", skillLevel);
        PropertySkillLevelInfo propertyInfo = PropertySkillLevelConfig.GetSkillLevelInfo(chooseSkillId * 100 + skillLevel);
        if (propertyInfo != null)
            currentDesc.text = string.Format(TextManager.GetPropsString("skill" + skillID), propertyInfo.addValue);
        else
            currentDesc.text = "没配";
        selectSkll.transform.localPosition = go.localPosition;
        skillMax.gameObject.SetActive(skillLevel >= SkillLevelConfig.skillInfoList.Count);
        skillLimit.gameObject.SetActive(skillLevel < SkillLevelConfig.skillInfoList.Count);
        nextLevel.gameObject.SetActive(skillLevel < SkillLevelConfig.skillInfoList.Count);
        nextDesc.gameObject.SetActive(skillLevel < SkillLevelConfig.skillInfoList.Count);
        if (nextDesc.gameObject.activeSelf)
        {
            propertyInfo = PropertySkillLevelConfig.GetSkillLevelInfo(chooseSkillId * 100 + skillLevel + 1);
            if (propertyInfo != null)
                nextDesc.text = string.Format(TextManager.GetPropsString("skill" + skillID), propertyInfo.addValue);
            else
                nextDesc.text = "没配";
        }
        UtilTools.SetSkillType((int)skillInfo.type, skillType);
        if (skillLevel >= SkillLevelConfig.skillInfoList.Count)
            return;
        levelInfo = SkillLevelConfig.GetSkillLevelInfo(skillLevel);
        skillLimit.text = string.Format(TextManager.GetUIString("skillneed"), levelInfo.limit);
        nextLevel.text = string.Format(TextManager.GetUIString("UISkill8"), skillLevel + 1);
    }

    /// <summary>
    /// 查看道具
    /// </summary>
    void OnClickItemInfo(GameObject go)
    {
        GUIManager.ShowItemInfoPanel(PanelType.Info, go.name);
    }
    void OnClickEx(GameObject go)
    {
        if (go == skill1.gameObject)
        {
            chooseSkillIndex = 1;
            UpdateSkillInfo(clientInfo.skill1, skill1.transform, currentTeamBaller.skill1Lv);
        }
        else if (go == skill2.gameObject)
        {
            chooseSkillIndex = 2;
            UpdateSkillInfo(clientInfo.skill2, skill2.transform, currentTeamBaller.skill2Lv);
        }
        else if (go == skill3.gameObject)
        {
            chooseSkillIndex = 3;
            UpdateSkillInfo(clientInfo.skill3, skill3.transform, currentTeamBaller.skill3Lv);
        }
        else if (go == skill4.gameObject)
        {
            chooseSkillIndex = 4;
            UpdateSkillInfo(clientInfo.skill4, skill4.transform, currentTeamBaller.skill4Lv);
        }
        else if (go == skillLevelBtn.gameObject)
        {
            if (currentTeamBaller.level < levelInfo.limit)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_skill_12"));
                return;
            }
            ServerCustom.instance.SendClientMethods("onClientSkillLevelUp", chooseSkillId, chooseSkillIndex, currentTeamBaller.id);
        }
        else if (go.name == "coach1")
        {
            chooseCoachIndex = 0;
            CoachInfoPanel(chooseCoachIndex);
        }
        else if (go.name == "coach2")
        {
            if (coachInfoList[1].isLock == 0)
            {
                commonInfo = CommonConfig.GetCommonInfo(8);
                string text = string.Format(TextManager.GetUIString("UISkill1"), commonInfo.value);
                GUIManager.SetPromptInfoChoose(TextManager.GetUIString("UICreate1"), text, BuyCoach);
                return;
            }
            chooseCoachIndex = 1;
            CoachInfoPanel(chooseCoachIndex);
        }
        else if (go.name == "coach3")
        {
            if (coachInfoList[2].isLock == 0)
            {
                commonInfo = CommonConfig.GetCommonInfo(9);
                string text = string.Format(TextManager.GetUIString("UISkill1"), commonInfo.value);
                GUIManager.SetPromptInfoChoose(TextManager.GetUIString("UICreate1"), text, BuyCoach);
                return;
            }
            chooseCoachIndex = 2;
            CoachInfoPanel(chooseCoachIndex);
        }
        else if (go.name == "coach4")
        {
            commonInfo = CommonConfig.GetCommonInfo(10);
            if (PlayerMediator.playerInfo.vipLevel < commonInfo.value)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_skill_2"));
                return;
            }
            chooseCoachIndex = 3;
            CoachInfoPanel(chooseCoachIndex);
        }
        else if (go.name == "coach5")
        {
            commonInfo = CommonConfig.GetCommonInfo(11);
            if (PlayerMediator.playerInfo.vipLevel < commonInfo.value)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_skill_2"));
                return;
            }
            chooseCoachIndex = 4;
            CoachInfoPanel(chooseCoachIndex);
        }
        else if (go == closeBtn.gameObject)
        {
            coachPanel.gameObject.SetActive(false);
            TimerManager.Destroy("chooseCoachLabel");
        }
        else if (go == icon1.gameObject)
        {
            if (coach1Have.gameObject.activeSelf)
                return;
            commonInfo = CommonConfig.GetCommonInfo(12);
            string text = string.Format(TextManager.GetUIString("UISkill4"), commonInfo.value);
            GUIManager.SetPromptInfoChoose(TextManager.GetUIString("UICreate1"), text, UpCoach);
        }
        else if (go == icon2.gameObject)
        {
            if (!coach1Have.gameObject.activeSelf)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_skill_6"));
                return;
            }
            if (coach2Have.gameObject.activeSelf)
                return;
            commonInfo = CommonConfig.GetCommonInfo(13);
            string text = string.Format(TextManager.GetUIString("UISkill4"), commonInfo.value);
            GUIManager.SetPromptInfoChoose(TextManager.GetUIString("UICreate1"), text, UpCoach);
        }
        else if (go == helpBtn.gameObject)
        {
            List<object> decsList = new List<object>();
            for (int i = 10016; i <= 10018; ++i)
            {
                string Name = "state" + i.ToString();
                string text = TextManager.GetPropsString(Name);
                decsList.Add(text);
            }
            GUIManager.ShowHelpPanel(decsList);
        }
        else if (go.name == "useBtn")
        {
            if (coachInfoList[chooseCoachIndex].useTime == 0)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_skill_11"));
                return;
            }
            chooseItemTrans = go.GetComponentInParent<UITexture>().transform;
            if (int.Parse(chooseItemTrans.FindChild("num").GetComponent<UILabel>().text) < 1)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_7"));
                return;
            }
            ServerCustom.instance.SendClientMethods("onClientAddCoachTime", int.Parse(chooseItemTrans.name), chooseCoachIndex + 1);
        }
        else if (go == skillSelectBtn.gameObject)
        {
            selectSkillPanel.gameObject.SetActive(true);
            if (skillList.Count > 0)
            {
                skillGrid.UpdateCustomDataList(skillList);
                return;
            }
            SkillInfo info;
            for (int i = 0; i < clientInfo.selfOwnSkill.Split(',').Length; ++i)
            {
                info = new SkillInfo();
                info.id = int.Parse(clientInfo.selfOwnSkill.Split(',')[i]);
                info.index = i + 1;
                info.name = Instance.Get<SkillConfig>().GetItem(info.id).name;
                skillList.Add(info);
            }
            skillGrid.AddCustomDataList(skillList);
        }
        else if (go = closeSkillSelecteBtn.gameObject)
        {
            selectSkillPanel.gameObject.SetActive(false);
        }
    }
    void UpCoach()
    {
        if (commonInfo.id == 12)
            ServerCustom.instance.SendClientMethods("onClientLevelUpCoach", chooseCoachIndex + 1, 0);
        else
            ServerCustom.instance.SendClientMethods("onClientLevelUpCoach", chooseCoachIndex + 1, 1);
    }

    void BuyCoach()
    {
        if (commonInfo.id == 8)
            ServerCustom.instance.SendClientMethods("onClientUnLockCoach", 2);
        else
            ServerCustom.instance.SendClientMethods("onClientUnLockCoach", 3);
    }
    /// <summary>
    /// 教练解锁成功
    /// </summary>
    public void UnLockCoachSucess(int coach)
    {
        coachInfoList[coach].lockSpr.gameObject.SetActive(false);
        coachInfoList[coach].coachColor.gameObject.SetActive(true);
        coachInfoList[coach].icon.GetComponent<UIToggle>().enabled = true;
        coachInfoList[coach].icon.transform.FindChild("sprite").gameObject.SetActive(true);
        LoadSprite.LoaderHead(coachInfoList[coach].icon, coachInfoList[coach].icon.name);
        coachInfoList[coach].coachColor.spriteName = UtilTools.StringBuilder("color", coachInfoList[coach].color);
        coachInfoList[coach].timeLabel.color = Color.green;
        coachInfoList[coach].timeLabel.gameObject.SetActive(coachInfoList[coach].useTime != 0);
    }
    /// <summary>
    /// 技能升级成功
    /// </summary>
    public void SkillLevelUpSucess(int skillLevel, int coach)
    {
        coach--;
        if (coachInfoList[coach].useTime < coachInfoList[coach].limitTime)
            coachInfoList[coach].timeLabel.color = Color.green;
        else
            coachInfoList[coach].timeLabel.color = Color.red;
        coachInfoList[coach].timeLabel.gameObject.SetActive(true);
        skillName.text = UtilTools.StringBuilder(skillInfo.name, "  Lv.", skillLevel);
        coachInfoList[coach].mask.fillAmount = coachInfoList[coach].useTime * 1.0f / coachInfoList[coach].limitTime;
        coachInfoList[coach].timeLabel.text = UtilTools.formatDuring(coachInfoList[coach].useTime);
        PropertySkillLevelInfo propertyInfo = PropertySkillLevelConfig.GetSkillLevelInfo(chooseSkillId * 100 + skillLevel);
        if (propertyInfo != null)
            currentDesc.text = string.Format(TextManager.GetPropsString("skill" + chooseSkillId), propertyInfo.addValue);
        else
            currentDesc.text = "没配";
        if (skillLevel >= SkillLevelConfig.skillInfoList.Count)
        {
            nextDesc.gameObject.SetActive(false);
            skillLimit.gameObject.SetActive(false);
            nextLevel.gameObject.SetActive(false);
            skillMax.gameObject.SetActive(true);
            return;
        }
        propertyInfo = PropertySkillLevelConfig.GetSkillLevelInfo(chooseSkillId * 100 + skillLevel + 1);
        if (propertyInfo != null)
            nextDesc.text = string.Format(TextManager.GetPropsString("skill" + chooseSkillId), propertyInfo.addValue);
        else
            nextDesc.text = "没配";
        levelInfo = SkillLevelConfig.GetSkillLevelInfo(skillLevel);
        skillLimit.text = string.Format(TextManager.GetUIString("skillneed"), levelInfo.limit);
        nextLevel.text = string.Format(TextManager.GetUIString("UISkill8"), skillLevel + 1);
    }
    /// <summary>
    /// 打开教练信息界面
    /// </summary>
    void CoachInfoPanel(int coachIndex)
    {
        coachPanel.gameObject.SetActive(true);
        LoadSprite.LoaderHead(chooseCoachIcon, coachInfoList[coachIndex].icon.name);
        LoadSprite.LoaderHead(icon1, coachInfoList[coachIndex].icon.name);
        LoadSprite.LoaderHead(icon2, coachInfoList[coachIndex].icon.name);
        coach1Have.gameObject.SetActive(coachInfoList[coachIndex].color >= 4);
        coach2Have.gameObject.SetActive(coachInfoList[coachIndex].color >= 5);
        if (cloneCoachLabel != null)
            MonoBehaviour.Destroy(cloneCoachLabel.gameObject);
        cloneCoachLabel = GameObject.Instantiate(coachInfoList[coachIndex].timeLabel).gameObject.GetComponent<UILabel>();
        UtilTools.SetParentWithPosition(cloneCoachLabel.transform, chooseCoachLabel.parent.transform, chooseCoachLabel.transform.localPosition, Vector3.one);
        if (coach1Have.gameObject.activeSelf)
            icon1.color = Color.black;
        else
            icon1.color = Color.white;
        if (coach1Have.gameObject.activeSelf && !coach2Have.gameObject.activeSelf)
            icon2.color = Color.white;
        else
            icon2.color = Color.black;
        chooseCoachColor.spriteName = UtilTools.StringBuilder("color", coachInfoList[coachIndex].color);
    }
   
    /// <summary>
    /// 提升教练品质
    /// </summary>
    public void LevelUpCoach(int coachIndex)
    {
        coachIndex--;
        if (commonInfo.id == 12)
        {
            coach1Have.gameObject.SetActive(true);
            icon1.color = Color.black;
            icon2.color = Color.white;
        }
        else
        {
            coach2Have.gameObject.SetActive(true);
            icon2.color = Color.black;
        }
        coachInfoList[coachIndex].mask.fillAmount = coachInfoList[coachIndex].useTime * 1.0f / coachInfoList[coachIndex].limitTime;
        coachInfoList[coachIndex].coachColor.spriteName = UtilTools.StringBuilder("color", coachInfoList[coachIndex].color);
        chooseCoachColor.spriteName = UtilTools.StringBuilder("color", coachInfoList[coachIndex].color);
    }

    /// <summary>
    /// 刷新教练CD
    /// </summary>
    /// <param name="coachIndex"></param>
    public void AddCoachTime(int coachIndex)
    {
        coachIndex--;
        coachInfoList[coachIndex].mask.fillAmount = coachInfoList[coachIndex].useTime * 1.0f / coachInfoList[coachIndex].limitTime;
        coachInfoList[coachIndex].timeLabel.text = UtilTools.formatDuring(coachInfoList[coachIndex].useTime);
        chooseItemTrans.FindChild("num").GetComponent<UILabel>().text = ItemManager.GetBagItemCount(chooseItemTrans.name).ToString();
        if (coachInfoList[coachIndex].useTime == 0)
        {
            cloneCoachLabel.gameObject.SetActive(false);
            return;
        }
        cloneCoachLabel.color = coachInfoList[coachIndex].timeLabel.color;
        cloneCoachLabel.text = coachInfoList[coachIndex].timeLabel.text;
    }
    /// <summary>
    /// 技能更换成功
    /// </summary>
    public void SeleceSkillSucess(int skillID,int level )
    {
        clientInfo.skill1 = skillID.ToString();
        UpdateSkillInfo(skillID.ToString(), skill1.transform, level);
        skillGrid.UpdateCustomDataList(skillList);
    }

    void UpdateSkillGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        SkillInfo info = item.oData as SkillInfo;
        UITexture icon = item.mScripts[0] as UITexture;
        UILabel skillName = item.mScripts[1] as UILabel;
        UISprite selectBtn = item.mScripts[2] as UISprite;
        UILabel state = item.mScripts[3] as UILabel;
        UILabel skillDesc = item.mScripts[4] as UILabel;
        UIEventListener.Get(selectBtn.gameObject).onClick = OnClickSkillItem;
        LoadSprite.LoaderSkill(icon, info.id.ToString());
        selectBtn.GetComponent<BoxCollider>().enabled = info.id != int.Parse(clientInfo.skill1);
        int skillLevel = 0;
        if (info.index == 1)
            skillLevel = currentTeamBaller.skill11Lv;
        else if(info.index == 2)
            skillLevel = currentTeamBaller.skill12Lv;
        else
            skillLevel = currentTeamBaller.skill13Lv;
        skillName.text = UtilTools.StringBuilder(info.name, "  Lv.", skillLevel);
        PropertySkillLevelInfo propertyInfo = PropertySkillLevelConfig.GetSkillLevelInfo(info.id * 100 + skillLevel);
        if (propertyInfo != null)
            skillDesc.text = string.Format(TextManager.GetPropsString("skill" + info.id), propertyInfo.addValue);
        else
            skillDesc.text = "没配";
        if (info.id == int.Parse(clientInfo.skill1))
        {
            state.text = TextManager.GetUIString("UISkill7");
            selectBtn.color = Color.black;
        }
        else
        {
            state.text = TextManager.GetUIString("UISkill6");
            selectBtn.color = Color.white;
        }
        
    }
    void OnClickSkillItem(GameObject go)
    {
        ServerCustom.instance.SendClientMethods("onClientSelectSkill", (go.GetComponentInParent<UIGridItem>().oData as SkillInfo).id, currentTeamBaller.id); 
    }
    protected override void OnDestroy()
    {
        skillMax = null;
        skill1 = null;
        skill2 = null;
        skill3 = null;
        skill4 = null;
        skillLevelBtn = null;
        selectSkll = null;
        skillName = null;
        currentDesc = null;
        nextDesc = null;
        skillType = null;
        skillLimit = null;
        levelInfo = null;
        coachPanel = null;
        chooseCoachIcon = null;
        chooseCoachColor = null;
        selectSkillPanel = null;
        chooseItemTrans = null;
        coach1Have = null;
        coach2Have = null;
        icon1 = null;
        icon2 = null;
        helpBtn = null;
        closeBtn = null;
        closeSkillSelecteBtn = null;
        skillSelectBtn = null;
        coach1Diamod = null;
        coach2Diamod = null;
        nextLevel = null;
        chooseCoachLabel = null;
        cloneCoachLabel = null;
        skillGrid = null;
        skillList.Clear();
        
        TimerManager.Destroy("chooseCoachLabel");
        for (int i = 0; i < coachInfoList.Count; ++i)
        {
            coachInfoList[i].icon = null;
            coachInfoList[i].timeLabel = null;
            coachInfoList[i].lockSpr = null;
            coachInfoList[i].mask = null;
            coachInfoList[i].coachColor = null;
        }

    }
}
