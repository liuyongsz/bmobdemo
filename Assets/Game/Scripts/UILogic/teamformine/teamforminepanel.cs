using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;
using System;

//阵型按钮类型
public enum FormationBtnType
{
    Select=1, //选阵
    Strong=2, //升级
    Formation= 3,//布阵 
}
public class teamforminepanel : BasePanel
{
    public Transform backBtn;
    public Transform selectBtn;
    public Transform strongBtn;
    public Transform setBtn;
    public Transform toolBtn;


    public Transform formationPanel;
    public Transform selectPanel;
    public Transform strongPanel;
    public Transform benchBtn;

    public UISprite logo1;
    public UIGrid playerGrid;
    public UITable btnTable;
    public UIGrid select_list;
    public UIGrid strong_list;
    public Transform formineList;
    public Transform ball;
    public UILabel formine;
    public UILabel teamMame;
    public UILabel teamfight;
    public UITexture logo;
    public UITexture moveIcon;
    public Transform baller;
    public UIScrollView playerView;

}



public class TeamFormineMediator : UIMediator<teamforminepanel>
{
 
    public static TeamFormineMediator teamFormineMediator;

    public static selectPanel selectPanel;
    public static strongPanel strongPanel;

    public static bool mIsCanUpdata = true;
    // 阵型id
    public static int formation=1;
    public static int mCurSystem = 1;
    //改变阵型
    public bool mIsChangeFormation = false;
    public int select_level;
    public static int mIndex;
    public static int mTotalNum;
    //是否是拖拽状态
    private bool m_is_drag_state;
    //能否拓展
    private bool mIsCanDrag = true;
    private TeamBaller m_drag_baller_data;
    private UIGridItem lastBtnItem;

    private FormationBtnType mCurBtnType = FormationBtnType.Select;
    //当前系列阵型
    public static List<TeamFormation> mFormationList = new List<TeamFormation>();
    //位置
    private static Dictionary<int, TeamBaller> m_pos_baller_dict = new Dictionary<int, TeamBaller>();
    private static Dictionary<Transform, FormationBaller> m_trans_ball_dict = new Dictionary<Transform, FormationBaller>();
    public teamforminepanel panel
    {
        get
        {
            return m_Panel as teamforminepanel;
        }
    }
    public TeamFormineMediator() : base("teamforminepanel")
    {
        m_isprop = true;
    
        RegistPanelCall(NotificationID.TeamFormine_Show, OpenPanel);
        RegistPanelCall(NotificationID.TeamFormine_Hide, ClosePanel);

        RegistPanelCall(NotificationID.Formation_Change, ChangeFomation);
        RegistPanelCall(NotificationID.BagRefresh, RefreshBag);
        RegistPanelCall(NotificationID.Fight_Change, ChangeFightValue);

    }

    protected override void AddComponentEvents()
    {
        base.AddComponentEvents();
        UIEventListener.Get(panel.backBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.selectBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.strongBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.setBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.benchBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.toolBtn.gameObject).onClick = OnClick;

        OnClientFormationSystem();
    }

    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {
      
        if (teamFormineMediator == null)
        {
            teamFormineMediator = Facade.RetrieveMediator("TeamFormineMediator") as TeamFormineMediator;
        }
        formation = PlayerMediator.playerInfo.formation;

       

        panel.playerGrid.enabled = true;
        panel.playerGrid.BindCustomCallBack(UpdatePlayerItem);
        panel.playerGrid.StartCustom();

        panel.select_list.enabled = true;
        panel.select_list.BindCustomCallBack(UpdateBtnItem);
        panel.select_list.StartCustom();

        panel.strong_list.enabled = true;
        panel.strong_list.BindCustomCallBack(UpdateBtnItem);
        panel.strong_list.StartCustom();

      

        SetInfo();
        SetBallerGrid();
        SetBtnGrid();

        if (selectPanel == null)
            selectPanel = new selectPanel(panel.selectPanel.gameObject);

        if (strongPanel == null)
            strongPanel = new strongPanel(panel.strongPanel.gameObject);

        //SetSwitchFunc();

    }
    /// <summary>
    /// 设置一键强化信息
    /// </summary>
    public void SetInfo()
    {
        TeamFormation info = TeamFormationConfig.GetTeamFormation(formation);
        panel.formine.text = info.name;
        
        mCurSystem = info.type;

        List<TeamFormation> list = TeamFormationConfig.GetFormationByType(mCurSystem);

        mIndex = list.IndexOf(info);
    }
    public void SetBtnGrid()
    {
        List<int> list = TeamFormationSystemConfig.GetTeamFSId();

        List<object> listObj = new List<object>();
        int index = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (mCurSystem == list[i])
                index = i;
            listObj.Add(list[i]);
        }
        panel.select_list.AddCustomDataList(listObj);
        panel.strong_list.AddCustomDataList(listObj);
        panel.select_list.SetSelect(index);
        panel.strong_list.SetSelect(0);

        panel.btnTable.Reposition();

    }
    public void SetBallerGrid()
    {
        int fight = 0;
        List<TeamBaller> list = GetTeamBallerList();
        list.Sort(Compare);

        List<object> listObj = new List<object>();
        for (int i = 0; i < list.Count; i++)
        {
            listObj.Add(list[i]);
            fight += list[i].fightValue;
        }


        panel.teamfight.text = PlayerMediator.playerInfo.fightValue.ToString();

        panel.playerGrid.AddCustomDataList(listObj);

    }

    //设置系列阵型
   public virtual  void SetFormationInfo()
    {
        TeamFormation info = mFormationList[mIndex];
        formation = info.id;
        
        SetFormineBaller();
    }
    private void SetFormineBaller()
    {
        m_pos_baller_dict.Clear();
        //List<int> not_adaptPos_list = new List<int>();

        List<FormationBaller> listObj = new List<FormationBaller>();

        TeamFormation info = TeamFormationConfig.GetTeamFormation(formation);
        string[] posIdArr = info.playerPos.Split(',');
        string[] posArr = info.position.Split(';');
        FormationBaller baller = null;
        for(int i = posIdArr.Length-1; i>=0; i--)
        {
            baller = new FormationBaller();
            int pos_id = GameConvert.IntConvert(posIdArr[i]);
            baller.pos_id = pos_id;
            baller.vector = ConfigParseUtil.ParseVec3(posArr[i]);
            TeamBaller team_data = null;
            if (mCurBtnType == FormationBtnType.Formation||mIsChangeFormation)
            {
                team_data = GetTeamBallerByPos(pos_id);
                baller.baller = team_data;

            }

            if (mIsChangeFormation && baller.baller!=null)
            {
                baller.baller.pos = pos_id;
                OnChangeBallerPos(baller.baller.id, pos_id);
            }

            if (!m_pos_baller_dict.ContainsKey(pos_id)&& team_data!=null)
                m_pos_baller_dict.Add(pos_id,team_data);

            listObj.Add(baller);
        }

        for(int j= listObj.Count-1; j>=0; j--)
        {
            if(listObj[j].baller== null)
            {
                listObj[j].baller = GetNotHasFitBallerByPos(listObj[j].pos_id);
                if (mIsChangeFormation)
                {
                    listObj[j].baller.pos = listObj[j].pos_id;
                    OnChangeBallerPos(listObj[j].baller.id, listObj[j].pos_id);
                }
                if (!m_pos_baller_dict.ContainsKey(listObj[j].pos_id)&& listObj[j].baller!=null)
                    m_pos_baller_dict.Add(listObj[j].pos_id, listObj[j].baller);
            }
        }

        mIsChangeFormation = false;
        SetFormationList(listObj);

    }
    public void SetFormationList(List<FormationBaller> list)
    {
        m_trans_ball_dict.Clear();
        GameObject temp = null;
        for (int i = 0; i < list.Count; i++)
        {
            FormationBaller baller = list[i];
            int index = i + 1;
            if (index < teamFormineMediator.panel.formineList.transform.childCount)
            {
                temp = teamFormineMediator.panel.formineList.GetChild(index).gameObject;
            }
            else
                temp = NGUITools.AddChild(teamFormineMediator.panel.formineList.gameObject, teamFormineMediator.panel.ball.gameObject);
            LongClickEvent.Get(temp).onPress = OnPressBaller;
            //LongClickEvent.Get(temp).duration =2f;
            temp.SetActive(true);
            SetFormainBallerInfo(temp,baller);
        }
        
    }
    //设置上阵球员信息
    public void SetFormainBallerInfo(GameObject temp, FormationBaller baller)
    {
        UILabel name = temp.transform.FindChild("name").GetComponent<UILabel>();
        UILabel pos = temp.transform.FindChild("posLabel").GetComponent<UILabel>();
        UISprite color = temp.transform.FindChild("color").GetComponent<UISprite>();
        UISprite icon = temp.transform.FindChild("icon").GetComponent<UISprite>();
        GameObject bg = temp.transform.FindChild("bg").gameObject;
        GameObject gantan  = temp.transform.FindChild("gantan").gameObject;

        temp.name = "item" + baller.pos_id;
        bool isFit = false;
        if (baller.baller!=null)
        {
            TD_Player player = Instance.Get<PlayerManager>().GetItem(UtilTools.IntParse(baller.baller.configId));
            name.text = player.name;

            color.spriteName = "smallcolor" + baller.baller.star;
            icon.spriteName = baller.pos_id == 1 ? "baiyi" : "hongyi";
            isFit = IsFitPos(baller.pos_id, baller.baller);

        }
        
        gantan.SetActive(!isFit&&mCurBtnType ==  FormationBtnType.Formation);
        name.gameObject.SetActive(mCurBtnType == FormationBtnType.Formation);
        color.gameObject.SetActive(mCurBtnType == FormationBtnType.Formation);
        bg.gameObject.SetActive(mCurBtnType == FormationBtnType.Formation);

        temp.transform.localPosition = baller.vector;
        TeamPosition info = TeamPositionConfig.GetTeamPosition(baller.pos_id);
        if (info != null)
            pos.text = TextManager.GetUIString(info.name);
        if (m_trans_ball_dict.ContainsKey(temp.transform))
            m_trans_ball_dict[temp.transform] = baller;
        else
            m_trans_ball_dict.Add(temp.transform, baller);
    }
    private void UpdatePlayerItem(UIGridItem item)
    {

        if (item == null || item.mScripts == null || item.oData == null)
            return;

        TeamBaller player = item.oData as TeamBaller;

        UILabel name = item.mScripts[0] as UILabel;
        UILabel fight = item.mScripts[1] as UILabel;
        UITexture icon = item.mScripts[2] as UITexture;
        UITexture star = item.mScripts[3] as UITexture;
        NGUIGrid pos = item.mScripts[4] as NGUIGrid;
        UISprite bench = item.mScripts[5] as UISprite;

        //item.onPress = OnPress;
        LongClickEvent.Get(item.gameObject).onPress = ClickItem;
        //LongClickEvent.Get(item.gameObject).duration = 2f;

        UISprite[] player_star = UtilTools.GetChilds<UISprite>(item.transform, "star");

        bench.transform.gameObject.SetActive(player.bench>0);

        TD_Player info = Instance.Get<PlayerManager>().GetItem(UtilTools.IntParse(player.configId));
        name.text = info.name;
        fight.text = player.fightValue.ToString();
        LoadSprite.LoaderHead(icon, "Head" + info.id.ToString(), false);

        UtilTools.SetStar(player.star, player_star, info.maxstar);
        UtilTools.SetTextColor(name, player.star);
        for (int i = 0; i < pos.transform.childCount; ++i)
        {
            Transform child = pos.transform.GetChild(i);
            child.gameObject.SetActive(i < info.Job.Split(',').Length);

            if (i < info.Job.Split(',').Length)
            {
                UILabel content = child.FindChild("name").GetComponent<UILabel>();
                UISprite bg = child.FindChild("bg").GetComponent<UISprite>();

                content.text= TextManager.GetUIString(info.Job.Split(',')[i]);

                string color = "smallcolor2";
                string jobIndex = info.jobIndex.Split(',')[i];
                switch(int.Parse(jobIndex))
                {
                    case 1:
                        color= "smallcolor6";
                        break;
                    case 2:
                        color = "smallcolor3";
                        break;
                    case 3:
                        color = "smallcolor2";
                        break;
                }
                bg.spriteName = color;
            }

        }
        pos.Reposition();

    }

    private void UpdateBtnItem(UIGridItem item)
    {

        if (item == null || item.mScripts == null || item.oData == null)
            return;
        UILabel name = item.mScripts[0] as UILabel;
        UISprite suo = item.mScripts[1] as UISprite;
        UISprite btn = item.mScripts[2] as UISprite;
        UISprite bg = item.mScripts[3] as UISprite;

        item.Selected = false;
        int fomationId = GameConvert.IntConvert(item.oData);
        
        TeamFormationSystem info = TeamFormationSystemConfig.GetTeamFormationSystem(fomationId);
        
        int needLevel = info.needLevel;
        int level = PlayerMediator.playerInfo.level;
        btn.transform.gameObject.SetActive(level>=needLevel);
        bg.transform.gameObject.SetActive(needLevel > level);

        suo.transform.gameObject.SetActive(needLevel>level);
        item.onClick = OnClickBtnItem;
        name.text = TextManager.GetPropsString(string.Format("formation_sys_{0}", fomationId));

    }
    private void OnClick(GameObject go)
    {

        switch (go.transform.name)
        {
            case "backBtn":
                ClosePanel(null);
                break;
            case "selectBtn":

                formation = PlayerMediator.playerInfo.formation;
                SetInfo();
                SetBtnGrid();
                //if (mCurBtnType == FormationBtnType.Select)
                //    return;
                mCurBtnType = FormationBtnType.Select;
        
                SetSwitchFunc();
                break;
            case "strongBtn":

                panel.strong_list.SetSelect(0);
                //if (mCurBtnType == FormationBtnType.Strong)
                //    return;
                mCurBtnType = FormationBtnType.Strong;
                mCurSystem = 1;
                SetSwitchFunc();
                break;
            case "setBtn":
                if (mCurBtnType == FormationBtnType.Formation)
                    return;
                mCurBtnType = FormationBtnType.Formation;
                SetSwitchFunc();
                break;
            case "benchBtn":
                Facade.SendNotification(NotificationID.Bench_Show);
                break;
            case "toolBtn":
                PanelManager.OpenPanel("TestPanel3");
                break;
        }
    }
    private void OnClickBtnItem(UIGridItem item)
    {
        mIsChangeFormation = false;
        int sysId = GameConvert.IntConvert(item.oData);
        
        TeamFormationSystem info = TeamFormationSystemConfig.GetTeamFormationSystem(sysId);

        int needLevel = info.needLevel;
        int level = PlayerMediator.playerInfo.level;

        if(needLevel>level)
        {
            item.Selected = false;
            if (lastBtnItem != null)
                lastBtnItem.Selected = true;
            string content = TextManager.GetSystemString("ui_system_46");
            GUIManager.SetJumpText(string.Format(content,needLevel));
            return;
        }
        if (mCurSystem == sysId)
            return;

        mCurSystem = sysId;
        lastBtnItem = item;
        mIndex = 0;

        SetFormationSys();

        if (mCurBtnType == FormationBtnType.Strong)
            strongPanel.SetInfo();
        else if (mCurBtnType == FormationBtnType.Select)
            selectPanel.SetSelectInfo();


    }

    //设置阵型系统
    public virtual void SetFormationSys()
    {
        mFormationList = TeamFormationConfig.GetFormationByType(mCurSystem);
        mTotalNum = mFormationList.Count-1;

        SetFormationInfo();
    }
    //设置切换界面
    public void SetSwitchFunc()
    {
        panel.select_list.gameObject.SetActive(mCurBtnType == FormationBtnType.Select);
        panel.strong_list.gameObject.SetActive(mCurBtnType == FormationBtnType.Strong);
        panel.selectPanel.gameObject.SetActive(mCurBtnType == FormationBtnType.Select);
        panel.strongPanel.gameObject.SetActive(mCurBtnType == FormationBtnType.Strong);
        panel.formationPanel.gameObject.SetActive(mCurBtnType == FormationBtnType.Formation);

        panel.btnTable.Reposition();

        switch (mCurBtnType)
        {
            case FormationBtnType.Select:
                mIsCanDrag = false;
                SetFormationSys();
                selectPanel.SetSelectInfo();
                break;
            case FormationBtnType.Strong:
                mIsCanDrag = false;
                mIndex = 0;
                SetFormationSys();
                strongPanel.SetInfo();
                break;
            case FormationBtnType.Formation:
                mIsCanDrag = true;
                formation = PlayerMediator.playerInfo.formation;
                if (!TeamFormationConfig.m_activeId_list.Contains(formation))
                {
                    GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_42"), null);
                    panel.formationPanel.gameObject.SetActive(false);
                    return;
                }
            
                SetFormineBaller();
                break;
        }

    }
    private void ClickItem(GameObject go, bool pressed)
    {
        if (!mIsCanDrag)
            return;

        if (!pressed)
        {
            m_is_drag_state = pressed;
            TimerManager.Destroy("FormationTimer");
        }
        else
        {
            bool isRun = TimerManager.IsRunning("FormationTimer");
            if (!isRun)
            {
                TimerManager.AddDelayTimer("FormationTimer", 0.5f, delegate (object[] obj)
                {
                    m_is_drag_state = true;
                    TimerManager.Destroy("FormationTimer");
                });
            }
        }

        //m_is_drag_state = pressed;
        m_drag_baller_data = go.transform.GetComponent<UIGridItem>().oData as TeamBaller;
        Vector3 mousePosition = UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);
        panel.moveIcon.transform.position = mousePosition;
        SetDragState(go);

        if (!pressed)
        {
            ChangeFormianBaller();
        }
    }

    private void OnPressBaller(GameObject go, bool pressed)
    {
        if (!mIsCanDrag)
            return;
        //TimerManager.Instance.AddOnceTimer(GetHashCode().ToString() + "UIEffect", mliftTime, delegate (object[] obj)

        if (!pressed)
        {
            m_is_drag_state = pressed;
            TimerManager.Destroy("FormationTimer");
        }
        else
        {
            bool isRun = TimerManager.IsRunning("FormationTimer");
            if(!isRun)
            {
                TimerManager.AddDelayTimer("FormationTimer", 0.5f, delegate (object[] obj)
                {
                    m_is_drag_state = true;
                    TimerManager.Destroy("FormationTimer");

                });
            }
         
        }
    

        Vector3 mousePosition = UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);
        panel.moveIcon.transform.position = mousePosition;
        m_drag_baller_data = m_trans_ball_dict[go.transform].baller;

        SetDragState(go);

        if (!pressed)
            ExchangeBallerPos(go);
    }
    ///设置拖拽
    private void  SetDragState(GameObject go)
    {

        panel.playerView.enabled = !m_is_drag_state;
        panel.moveIcon.gameObject.SetActive(m_is_drag_state);
        if (m_is_drag_state)
        {
            LoadSprite.LoaderHead(panel.moveIcon, "Card" + m_drag_baller_data.configId, false);
            SetFitballerLight();
        }
        else
        {
            ResetFormationBaller();
        }

    }

    //交换球员位置
    private void ExchangeBallerPos(GameObject go)
    {
        Transform temp = GetChangeObj();

        if (temp == null)
            return;

        if (m_trans_ball_dict[temp].baller.id == m_drag_baller_data.id)
            return;

        TeamBaller baller = new TeamBaller();

        baller = m_drag_baller_data;

        m_trans_ball_dict[go.transform].baller = m_trans_ball_dict[temp].baller;

        m_trans_ball_dict[temp].baller = baller;

        SetFormainBallerInfo(temp.gameObject, m_trans_ball_dict[temp]);

        SetFormainBallerInfo(go, m_trans_ball_dict[go.transform]);
        
        OnClientBallerChange(m_trans_ball_dict[go.transform].baller.id, m_trans_ball_dict[temp.transform].baller.id);
    }
    //上阵
    private void ChangeFormianBaller()
    {
        Transform temp = GetChangeObj();

        if (temp == null)
            return;

        FormationBaller data =  m_trans_ball_dict[temp];

        if(data.baller.isSelf == 1)
        {
            GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_45"), null);
            return;
        }


        OnClientBallerTeamReq(m_drag_baller_data.id,data.baller.id, data.pos_id);

        data.baller = m_drag_baller_data;


        SetFormainBallerInfo(temp.gameObject, data);


    }

    //设置适合位置球员亮光
    private void SetFitballerLight()
    {
        TD_Player info = Instance.Get<PlayerManager>().GetItem(UtilTools.IntParse(m_drag_baller_data.configId));

        string[] posArr = info.adaptPos.Split(',');
        List<string> listPos = new List<string>(posArr);

        var enumor = m_trans_ball_dict.Keys.GetEnumerator();
        Transform child = null;
        while(enumor.MoveNext())
        {
            child = enumor.Current;
            FormationBaller baller = m_trans_ball_dict[child];
            int index = listPos.IndexOf(baller.pos_id.ToString());

            GameObject posLight = child.FindChild("posLight").gameObject;
            posLight.SetActive(index!=-1);
            
        }

    }

    //重置阵型内球员亮光
    private void ResetFormationBaller()
    {
        var enumor = m_trans_ball_dict.Keys.GetEnumerator();
        Transform child = null;
        while (enumor.MoveNext())
        {
            child = enumor.Current;
        
            GameObject posLight = child.FindChild("posLight").gameObject;
            posLight.SetActive(false);

        }

    }
    //使用阵型改变
    private void ChangeFomation(INotification notification)
    {
        if (GUIManager.HasView("teamforminepanel"))
        {
            mIsChangeFormation = true;
            SetFormineBaller();
            selectPanel.SetBtnState();
        }
    }

    /// <summary>
    /// 刷新材料
    /// </summary>
    /// <param name="notification"></param>
    private void RefreshBag(INotification notification)
    {
        if (GUIManager.HasView("teamforminepanel"))
        {
            int type = GameConvert.IntConvert((notification.Body as List<object>)[0]);
            if (type == (int)BagChangeType.Update || type == (int)BagChangeType.Remove)
                strongPanel.SetCostInfo();
        }
    }
    void ChangeFightValue(INotification notification)
    {
        if (GUIManager.HasView("teamforminepanel"))
        {
            panel.teamfight.text = PlayerMediator.playerInfo.fightValue.ToString();
        }
    }
    //排序
    private int Compare(TeamBaller info1, TeamBaller info2)
    {

        if (info1.bench > info2.bench)
            return -1;
        else if (info1.bench < info2.bench)
            return 1;
        else if (info1.fightValue < info2.fightValue)
            return 1;
        else if (info1.fightValue > info2.fightValue)
            return -1;
        else
            return 0;

    }
    private int CompareFight(TeamBaller info1, TeamBaller info2)
    {
        if (info1.fightValue < info2.fightValue)
            return 1;
        else if (info1.fightValue > info2.fightValue)
            return -1;
        else
            return 0;
    }
    private int CompareDis(ObjectDisInfo info1, ObjectDisInfo info2)
    {
        float dis1 = Math.Abs(info1.dis);
        float dis2 = Math.Abs(info2.dis);

        if (dis1 < dis2)
            return -1;
        else if (dis1 > dis2)
            return 1;
        else
            return 0;

    }

    /// <summary>
    /// 获取不上阵球员列表
    /// </summary>
    /// <returns></returns>
    private List<TeamBaller> GetTeamBallerList()
    {

        List<TeamBaller> list = new List<TeamBaller>();

        var card_enumerator = TeamMediator.teamList.Values.GetEnumerator();
        while (card_enumerator.MoveNext())
        {
            if (card_enumerator.Current.inTeam == 0)
                list.Add(card_enumerator.Current);
        }

        return list;

    }
    //得到上阵球员
    private List<TeamBaller> GetInTeamBallerList()
    {
        List<TeamBaller> list = new List<TeamBaller>();

        var card_enumerator = TeamMediator.teamList.Values.GetEnumerator();
        while (card_enumerator.MoveNext())
        {
            if (card_enumerator.Current.inTeam > 0)
                list.Add(card_enumerator.Current);
        }

        return list;

    }
    ///球员是否适合该位置
    private bool IsFitPos(int pos, TeamBaller baller)
    {
        TD_Player info = Instance.Get<PlayerManager>().GetItem(UtilTools.IntParse(baller.configId));
        List<string> adap_list = new List<string>(info.adaptPos.Split(','));
        int index = adap_list.IndexOf(pos.ToString());
        if (index == -1)
            return false;
        return true;

    }
    private TeamBaller GetTeamBallerByPos(int pos)
    {
        List<TeamBaller> list = new List<TeamBaller>();
        List<TeamBaller> listInTeam = GetInTeamBallerList();
        
        for (int  i =0; i< listInTeam.Count; i++)
        {
            TD_Player info = Instance.Get<PlayerManager>().GetItem(UtilTools.IntParse(listInTeam[i].configId));
            bool isHas = IsInPosTeam(listInTeam[i]);

            if (!isHas && formation == PlayerMediator.playerInfo.formation&&listInTeam[i].pos == pos&&!mIsChangeFormation)//使用阵型与显示阵型一样采用已上阵位置
            {
                return listInTeam[i];
            }
            if(mIsChangeFormation) //采取最优位置
            {
                List<string> adap_list = new List<string>(info.adaptPos.Split(','));
                int index = adap_list.IndexOf(pos.ToString());
                if (index != -1 && !isHas)
                    list.Add(listInTeam[i]);
            }
         
        }
        
        list.Sort(CompareFight);

        return list.Count > 0 ? list[0] : null;

    }
  
    private TeamBaller GetNotHasFitBallerByPos(int pos)
    {
        List<TeamBaller> listInTeam = GetInTeamBallerList();
        listInTeam.Sort(CompareFight);
        for (int i = 0; i < listInTeam.Count; i++)
        {
            bool isHas = IsInPosTeam(listInTeam[i]);
            if (!isHas)
                return listInTeam[i];
        }
        return null;
    }
    //已经上阵在位置上
    private bool IsInPosTeam(TeamBaller baller)
    {
        var card_enumerator = m_pos_baller_dict.Values.GetEnumerator();
        while (card_enumerator.MoveNext())
        {
            if (card_enumerator.Current!=null&&card_enumerator.Current.id == baller.id)
                return true;
        }

        return false;
    }
    private Transform GetChangeObj()
    {
        Transform child = null;
        ObjectDisInfo info;
        List<ObjectDisInfo> distance_list = new List<ObjectDisInfo>();
        for(int i=0; i<panel.formineList.childCount; i++)
        {

            info = new ObjectDisInfo();
            child = panel.formineList.GetChild(i);
            if (child.name == "moveIcon")
                continue;
            float dis = Vector3.Distance(panel.moveIcon.transform.localPosition, child.localPosition);

            info.temp = child;
            info.dis = dis;
            distance_list.Add(info);
        }
        distance_list.Sort(CompareDis);
        info = distance_list[0];
        if (info.dis >= 50)
            return null;
        return info.temp;

    }

  
    /// <summary>
    /// 界面关闭
    /// </summary>
    protected override void OnDestroy()
    {    
        base.OnDestroy();    
        m_pos_baller_dict.Clear();
        mCurBtnType = FormationBtnType.Select;
        selectPanel = null;
        strongPanel = null;
        mIsChangeFormation = false;


    }

    //客户端请求阵型系统信息
    private void OnClientFormationSystem()
    {
        ServerCustom.instance.SendClientMethods(FormationProxy.OnClientFormationSystem);

    }

    //客户端请求解锁阵型ID List
    public void OnClientActiveFormationIDList()
    {
        ServerCustom.instance.SendClientMethods(FormationProxy.OnClientActiveFormationIDList);

    }
    //客户端请求球员上阵

    private void OnClientBallerTeamReq(int cardId,int exchangeId,int pos)
    {
        ServerCustom.instance.SendClientMethods(FormationProxy.OnClientBallerEnterTeam, cardId, exchangeId, pos);

    }
    //客户端请求球员交换位置
    private void OnClientBallerChange(int cardId, int exchangeId)
    {
        ServerCustom.instance.SendClientMethods(FormationProxy.OnClientBallerExchangePos, cardId, exchangeId);

    }
    //客户端请求使用阵型
    public void onClientUseFormation(int id)
    {
        ServerCustom.instance.SendClientMethods(FormationProxy.OnClientUseFormation, id);
    }
    //客户端请求解锁阵型
    public void OnClientActiveFormation(int id)
    {
        ServerCustom.instance.SendClientMethods(FormationProxy.OnClientActiveFormation, id);

    }
    //修改球员上阵位置
    public void OnChangeBallerPos(int cardId,int pos)
    {
        ServerCustom.instance.SendClientMethods(FormationProxy.OnChangeBallerPos, cardId,pos);
        //Debug.Log("----OnChangeBallerPos---"+ cardId+"------"+pos);
        
    }
}

public class FormationBaller
{
    public int pos_id;
    public Vector3 vector;
    public int player_id;
    public TeamBaller baller;
    //不是合适的位置球员
    public int not_fit;
}

public class ObjectDisInfo
{

    public Transform temp;
    public float dis;
}
