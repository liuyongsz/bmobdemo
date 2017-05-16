
using PureMVC.Interfaces;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using AssetBundles;
using System.Collections.Generic;

/// <summary>
/// 登录页面
/// </summary>
public class ui_balloperMediator : UIMediator<ui_balloper> {

    private Dictionary<Transform, TD_SkillAI> m_itemData;
    private Dictionary<Transform, UITexture> m_itemImg;
    private Dictionary<Transform, UISprite> m_itemSelect;
    private Dictionary<Transform, bool> m_itemGray;

    private List<UISprite> m_selectList;

    private E_AtartNodeOper m_shootOrPass;
    private List<int> m_playerIds;
    private List<UISprite> m_hucheSelectList;
    private List<UIButton> m_hucheBtnList;
    private List<TD_SkillAI> m_assistSkillList;
    private List<MatchPlayerItem> m_assistMatchItemList;
    private MatchPlayerItem m_selectedAssistItem;
    private TD_SkillAI m_selectedAssistSkillAI;
    private TD_SkillAI m_selectedActiveAI;

    private List<ui_balloperHeadItemData> m_headItemList;

    private int m_headSelectedIndex = 0;

    private bool m_showOponent;

    private bool m_ballOwnerIsMyCard;
    /// <summary>我是不是进攻方</summary>
    private bool m_isMyAtk;
    private bool m_lockBaseSkill;

    private List<int> m_cardList;

    private Transform m_seletedBtn;

    private int m_setIndex;

    public ui_balloperMediator() : base("ui_balloper") {

        RegistPanelCall(NotificationID.BALLOPER_OPEN, base.OpenPanel);
        RegistPanelCall(NotificationID.BALLOPER_CLOSE, base.ClosePanel);

        RegistPanelCall(NotificationID.MatchInfo_CardList, OnRec_CardList);
    }   
    
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(m_Panel.btnPass.gameObject).onClick = OnClick_Btn;
        UIEventListener.Get(m_Panel.btnShoot.gameObject).onClick = OnClick_Btn;
        UIEventListener.Get(m_Panel.btnFroceShoot.gameObject).onClick = OnClick_Btn;

        UIEventListener.Get(m_Panel.btnSure.gameObject).onClick = OnClick_CloseBtn;
        UIEventListener.Get(m_Panel.btnLeftArrow.gameObject).onClick = OnClick_PreArrowBtn;
        UIEventListener.Get(m_Panel.btnRightArrow.gameObject).onClick = OnClick_NxtArrowBtn;
    }

    private void OnRec_CardList(INotification notification)
    {
        m_cardList = notification.Body as List<int>;

        if(null != m_Panel)
            UpdateDisplay();
    }


    protected override void OnShow(INotification notification)
    {
        m_itemGray = new Dictionary<Transform, bool>();

        m_itemGray[m_Panel.btnPass.transform] = false;
        m_itemGray[m_Panel.btnShoot.transform] = false;
        m_itemGray[m_Panel.btnFroceShoot.transform] = false;

        if (null != m_cardList)
            UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        m_setIndex = 0;
        m_itemData = new Dictionary<Transform, TD_SkillAI>();
        m_itemImg = new Dictionary<Transform, UITexture>();
        m_itemSelect = new Dictionary<Transform, UISprite>();
        MatchPlayerItem itm = MatchManager.GetPlayerItem(AllRef.Ball.owner);

        if (null == itm)
            return;

        KBEngine.Card card;
        KBEngine.Monster monster;
        KBEngine.Entity entity;
        PlayerManager pm = ProxyInstance.InstanceProxy.Get<PlayerManager>();
        MonsterConfig monsterCfg = ProxyInstance.InstanceProxy.Get<MonsterConfig>();

        m_ballOwnerIsMyCard = itm.IsMyCard ? true : false;

        List<object> listObj = new List<object>();
        m_headItemList = new List<ui_balloperHeadItemData>();

        m_showOponent = !m_ballOwnerIsMyCard;

        ui_balloperHeadItemData headItm;
        int cnt = m_cardList.Count;

        MatchPlayerItem opItm;

        if (m_cardList.Count == 0)
        {
            entity = KBEngine.KBEngineApp.app.GetEntity(itm.Did);
            card = entity as KBEngine.Card;
            monster = entity as KBEngine.Monster;

            headItm = new ui_balloperHeadItemData();

            if (null != card)
                headItm.headIcon = "Card" + card.configID_B;
            else
                headItm.headIcon = monsterCfg.GetItem(monster.configID_B).icon;

            listObj.Add(headItm);
            m_headItemList.Add(headItm);

            SetData(itm, false);
        }
        else
        {
            for (int i = 0; i < cnt; i++)
            {
                entity = KBEngine.KBEngineApp.app.GetEntity(m_cardList[i]);
                opItm = MatchManager.GetItemByDid(m_cardList[i]);

                card = entity as KBEngine.Card;
                monster = entity as KBEngine.Monster;

                headItm = new ui_balloperHeadItemData();

                if (null != card)
                    headItm.headIcon = "Card" + card.configID_B;
                else
                    headItm.headIcon = monsterCfg.GetItem(monster.configID_B).icon;
              
                headItm.Data = opItm;
                listObj.Add(headItm);
                m_headItemList.Add(headItm);

               
                if (null != opItm)
                    SetData(opItm, true, entity);
            }

            UpdateSkillShow();
        }

        m_Panel.headGrid.BindCustomCallBack(OnUpdate_HeadGridItem);
        m_Panel.headGrid.StartCustom();
        m_Panel.headGrid.AddCustomDataList(listObj);
    }

    private void SetData(MatchPlayerItem itm,bool add = false, KBEngine.Entity entity = null)
    {
        int step = MatchManager.m_doStep;
        m_playerIds = new List<int>();

        m_itemImg[m_Panel.btnPass.transform] = m_Panel.iconSkill;
        m_itemImg[m_Panel.btnShoot.transform] = m_Panel.iconSkill1;
        m_itemImg[m_Panel.btnFroceShoot.transform] = m_Panel.iconSkill2;

        m_itemSelect[m_Panel.btnPass.transform] = m_Panel.select0;
        m_itemSelect[m_Panel.btnShoot.transform] = m_Panel.select1;
        m_itemSelect[m_Panel.btnFroceShoot.transform] = m_Panel.select2;

        m_Panel.btnFroceShoot.gameObject.SetActive(false);

        string preskill = "";

        if (m_ballOwnerIsMyCard)
        {
            if (step == 1)
            {
                preskill += "0,";
                m_lockBaseSkill = true;
            }
            else if (step == 2)
            {
                preskill += "0,1,";
            }
            else if (step == 3)
            {
                preskill += "1,";
                m_lockBaseSkill = true;
            }
        }
        

        m_hucheSelectList = new List<UISprite>() { m_Panel.select0, m_Panel.select1 };
        m_hucheBtnList = new List<UIButton>() { m_Panel.btnPass, m_Panel.btnShoot };

        List<UIButton> btnLst = new List<UIButton>() { m_Panel.btnPass, m_Panel.btnShoot, m_Panel.btnFroceShoot };
        List<UITexture> iconLst = new List<UITexture>() { m_Panel.iconSkill, m_Panel.iconSkill1, m_Panel.iconSkill2 };
        List<UILabel> nameLst = new List<UILabel>() { m_Panel.name0, m_Panel.name1, m_Panel.name2 };
        List<UISprite> norLst = new List<UISprite>() { m_Panel.norIcon, m_Panel.norIcon1, m_Panel.norIcon2 };
        List<UISprite> activeLst = new List<UISprite>() { m_Panel.ativeIcon, m_Panel.ativeIcon1, m_Panel.ativeIcon2 };
        m_selectList = new List<UISprite>() { m_Panel.select0, m_Panel.select1, m_Panel.select2 };

        SkillConfig scfg = ProxyInstance.InstanceProxy.Get<SkillConfig>();
        SkillAIConfig aicfg = ProxyInstance.InstanceProxy.Get<SkillAIConfig>();

        PlayerManager pm = ProxyInstance.InstanceProxy.Get<PlayerManager>();
        KBEngine.Card card = entity as KBEngine.Card;
        TD_Player playerCfg;

        if (null == entity)
            playerCfg = itm.player;
        else
            playerCfg = pm.GetItem(card.configID_B);

        int i;
        int cnt;

        if (null != playerCfg)
        {
            //m_cardList.Count;
            //for (int si = 0; si < cnt; si++)
            //    preskill += ("," + m_cardList[si]);
            //string[] skillstrs = preskill.Split(',');
            //List<TD_SkillAI> lst = new List<TD_SkillAI>();
            List<TD_SkillAI> lst = playerCfg.GetCanUseSkill(itm, preskill);
            //cnt = skillstrs.Length;
            //for (int ix = 0; ix < cnt; ix++)
            //    lst.Add(aicfg.GetItem(skillstrs[ix]));
            cnt = lst.Count;
            TD_SkillAI skillItm;
            for (i = 0; i < cnt; i++)
            {
                skillItm = lst[i];

                bool isActive = (m_ballOwnerIsMyCard && skillItm.showFlag == Skill_ShowFlag.Atk)
                    ||
                    (!m_ballOwnerIsMyCard && skillItm.showFlag == Skill_ShowFlag.Def);


                if (m_setIndex < btnLst.Count && (isActive || skillItm.ID == 0 || skillItm.ID == 1))
                {
                    UIButton btn = btnLst[m_setIndex];

                    m_itemData[btn.transform] = skillItm;
                    btn.gameObject.SetActive(true);

                    norLst[m_setIndex].gameObject.SetActive(isActive);
                    activeLst[m_setIndex].gameObject.SetActive(!isActive);
                    nameLst[m_setIndex].text = skillItm.name;

                    LoadSprite.LoaderImage(iconLst[m_setIndex], "skill/" + scfg.GetItem(skillItm.ID).icon);

                    m_setIndex++;
                }
            }
        }
        else
        {
            //int did = itm.Card != null ? itm.Card.id : itm.Monster.id;
            //Debug.LogError("null == itm || null == itm.player : " + did);
        }

        cnt = btnLst.Count;
        i = m_setIndex;
        for (; i < cnt; i++)
        {
            btnLst[i].gameObject.SetActive(false);
        }

        if (m_Panel.btnPass.gameObject.activeSelf)
            OnClick_Btn(m_Panel.btnPass.gameObject);
        else if (m_Panel.btnShoot.gameObject.activeSelf)
            OnClick_Btn(m_Panel.btnShoot.gameObject);

        m_Panel.assistSkillGrid.BindCustomCallBack(OnUpdate_AssistSkillGridItem);
        m_Panel.assistSkillGrid.StartCustom();

        if (!add)
        {
            m_assistSkillList = new List<TD_SkillAI>();
            m_assistMatchItemList = new List<MatchPlayerItem>();
        }
       else
        {
            if(null == m_assistSkillList)
                m_assistSkillList = new List<TD_SkillAI>();

            if (null == m_assistMatchItemList)
                m_assistMatchItemList = new List<MatchPlayerItem>();
        }

        foreach (var val in MatchManager.m_matchPlayerDic)
            if (val.Value.IsMyCard == m_ballOwnerIsMyCard)
                PraseAssistSkill(val.Value);
    }

    /// <summary>更新技能显示</summary>
    public void UpdateSkillShow()
    {
        if (m_assistMatchItemList == null || m_assistSkillList == null)
            return;

        List<object> inds = new List<object>();
        int len = m_assistSkillList.Count;
        for (int i = 0; i < len; i++)
            inds.Add(i);

        UIScrollView mScrollView = NGUITools.FindInParents<UIScrollView>(m_Panel.headGrid.gameObject);
        m_Panel.assistSkillGrid.AddCustomDataList(inds);

        if (m_assistMatchItemList.Count > 0)
        {
            m_selectedAssistItem = m_assistMatchItemList[0];
            m_selectedAssistSkillAI = m_assistSkillList[0];
        }
    }

    private void PraseAssistSkill(MatchPlayerItem itm)
    {
        if(null != itm.player)
        {
            List<TD_SkillAI> lst = itm.player.GetCanUseSkill(itm);
            int cnt = lst.Count;
            TD_SkillAI skillItm;
            int i = 0;
            for (i = 0; i < cnt; i++)
            {
                skillItm = lst[i];
                if (
                    //任意时刻
                    skillItm.showFlag == Skill_ShowFlag.AllTime   
                    ||
                    //我是进攻方 并且是辅助技能
                    (m_ballOwnerIsMyCard && skillItm.showFlag == Skill_ShowFlag.AtkAssist)
                    ||
                    //我是防守方 并且是辅助技能
                    (!m_ballOwnerIsMyCard && skillItm.showFlag == Skill_ShowFlag.DefAssist)
                    )
                {
                    m_assistMatchItemList.Add(itm);
                    m_assistSkillList.Add(skillItm);
                }
            }
        }
    }

    private void OnUpdate_HeadGridItem(UIGridItem item)
    {
        ui_balloperHeadItemData headItm = item.oData as ui_balloperHeadItemData;
        ui_balloperHeadItem itm = ScriptHelper.BindField(item.transform, "ui_balloperHeadItem") as ui_balloperHeadItem;

        itm.headIcon.gameObject.SetActive(item.gameObject.activeSelf);
        LoadSprite.LoaderHead(itm.headIcon, headItm.headIcon , false);
    }

    private void OnUpdate_AssistSkillGridItem(UIGridItem item)
    {
        int ind = (int)item.oData;
        TD_SkillAI cfg = m_assistSkillList[ind];
        item.onClick = OnClick_AssistItem;
        SkillConfig scfg = ProxyInstance.InstanceProxy.Get<SkillConfig>();
        ui_balloperAssistSkillItem itm = ScriptHelper.BindField(item.transform, "ui_balloperAssistSkillItem") as ui_balloperAssistSkillItem;

        itm.skillName.text = cfg.name;
        LoadSprite.LoaderImage(itm.iconAssist, "skill/" + scfg.GetItem(cfg.ID).icon, false);
    }

    /// <summary>点击辅助技能项</summary>
    /// <param name="item"></param>
    private void OnClick_AssistItem(UIGridItem item)
    {
        int ind = (int)item.oData;
        m_selectedAssistItem = m_assistMatchItemList[ind];
        m_selectedAssistSkillAI = m_assistSkillList[ind];
    }

    #region 箭头按钮翻页
    private void OnClick_PreArrowBtn(GameObject obj)
    {
        int toInd = m_headSelectedIndex - 1;
        if (toInd >= 0)
            m_headSelectedIndex = toInd;
        
        m_Panel.headGrid.GoToPosition(m_headSelectedIndex);
    }

    private void OnClick_NxtArrowBtn(GameObject obj)
    {
        int toInd = m_headSelectedIndex + 1;
        if (toInd < m_headItemList.Count)
            m_headSelectedIndex = toInd;

        m_Panel.headGrid.GoToPosition(m_headSelectedIndex);
    }
    #endregion

    private void OnClick_Btn(GameObject obj)
    {
        TD_SkillAI skillItm;

        if (null == m_itemData)
            return;

        if(!m_itemData.TryGetValue(obj.transform,out skillItm))
            return;

        bool isNor;
        isNor = skillItm.IsNormalSkill;
        if (m_itemGray[obj.transform] && !isNor)
            return;

        foreach (var child in m_itemData)
        {
            isNor = child.Value.IsNormalSkill;
            if (child.Value.actionType != Skill_ActionType.Both)
                if (isNor || (child.Value.actionType == skillItm.actionType))
                {
                    m_itemGray[child.Key] = false;
                    DisplayUtil.SetGrayFiter(m_itemImg[child.Key], false);
                }
                else
                {
                    m_itemGray[child.Key] = true;
                    DisplayUtil.SetGrayFiter(m_itemImg[child.Key], true);
                    m_itemSelect[child.Key].gameObject.SetActive(false);
                }
            else
            {
                m_itemGray[child.Key] = false;
                DisplayUtil.SetGrayFiter(m_itemImg[child.Key], false);
            }
        }


        UISprite selectSp = m_itemSelect[obj.transform];

        E_AtartNodeOper oper = skillItm.ID == 0 ? E_AtartNodeOper.Pass : E_AtartNodeOper.Shoot;

        m_selectedActiveAI = skillItm;

        if (skillItm.IsNormalSkill)  //0,1传球和射门
        {
            m_shootOrPass = oper;

            int step = MatchManager.m_doStep;
            if (step == 1)
                MatchManager.FstChooseOper = oper;
            else if (step == 2)
                MatchManager.SecChooseOper = oper;
            else if (step == 3)
                MatchManager.ThrChooseOper = oper;

            bool seleA = selectSp == m_Panel.select0 ? true : false;

            //如果被锁定返回
            if (m_lockBaseSkill && selectSp.gameObject.activeSelf)
                return;

            selectSp.gameObject.SetActive(!selectSp.gameObject.activeSelf);

            int cnt = m_hucheSelectList.Count;
            for (int i = 0; i < cnt; i++)
                if (m_hucheSelectList[i] != selectSp)
                {
                    TD_SkillAI tmpAI;

                    if(m_itemData.TryGetValue(m_hucheBtnList[i].transform,out tmpAI) && tmpAI.ID < 2)
                        m_hucheSelectList[i].gameObject.SetActive(!selectSp.gameObject.activeSelf);
                }
                  
        }
        else
        {
            MatchPlayerItem ownItm = MatchManager.GetPlayerItem(AllRef.Ball.owner);
            KBEngine.Card card = ownItm.Card;

            selectSp.gameObject.SetActive(!selectSp.gameObject.activeSelf);

            int did;
            if (null != card)
                did = card.id;
            else
                //did = ownItm.defTarget.Card.id;
                did = m_cardList[0];

            if (m_playerIds.IndexOf(did) == -1 && selectSp.gameObject.activeSelf)
                m_playerIds.Add(did);
            else if (m_playerIds.IndexOf(did) != -1)
                m_playerIds.Remove(did);

            if (   null != m_seletedBtn
                   && m_itemData[m_seletedBtn].IsNormalSkill
                   && 
                   (m_itemData[m_seletedBtn].actionType == skillItm.actionType
                    || skillItm.actionType == Skill_ActionType.Both
                    )
                )
            {
                m_itemSelect[m_seletedBtn].gameObject.SetActive(false);

                if (m_itemSelect[m_seletedBtn] == m_Panel.select1.transform)
                    OnClick_Btn(m_Panel.select0.gameObject);
                if (m_itemSelect[m_seletedBtn] == m_Panel.select0.transform)
                    OnClick_Btn(m_Panel.select1.gameObject);
            }

            m_seletedBtn = obj.transform;
        }
    }

    private void OnClick_CloseBtn(GameObject obj)
    {
        DefineManager dman = DefineManager.Ins;
        if(null != dman)
        {
            if (MatchManager.m_doStep == 1 && dman.FstSelResult != E_StepResult.Null)
            {
                MatchManager.StepOperResult[1] = dman.FstSelResult;
                ClosePanel(null);
                return;
            }
            else if (MatchManager.m_doStep == 2 && dman.SecSelResult != E_StepResult.Null)
            {
                MatchManager.StepOperResult[2] = dman.SecSelResult;
                ClosePanel(null);
                return;
            }
            else if (MatchManager.m_doStep == 3 && dman.ThrSelResult != E_StepResult.Null)
            {
                MatchManager.StepOperResult[3] = dman.ThrSelResult;
                ClosePanel(null);
                return;
            }
        }

        if(ShootTestManager.Instace == null)
        {
            if (m_ballOwnerIsMyCard && m_shootOrPass != E_AtartNodeOper.Null)
                CloneProxy.Instance.onClientSelectOp(m_shootOrPass, m_playerIds);
            else
                CloneProxy.Instance.onClientSelectOp(E_AtartNodeOper.Def, m_playerIds);
        }
        
        MatchManager.Stop = false;

        MatchPlayerItem ballItem = MatchManager.GetPlayerItem(AllRef.Ball.owner);

        if (m_selectedActiveAI.ID > 0)
            Facade.SendNotification(NotificationID.MatchInfo_SkillCall, m_selectedActiveAI.name);
        
        if(m_selectedAssistSkillAI.ID > 0)
            Facade.SendNotification(NotificationID.MatchInfo_SkillCall, m_selectedAssistSkillAI.name);

        ClosePanel(null);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        m_cardList = null;
        m_seletedBtn = null;
        m_selectList = null;
        m_assistSkillList = null;
        m_shootOrPass = E_AtartNodeOper.Null;
        MatchManager.Stop = false;

        m_selectedAssistItem = null;
    }
}
