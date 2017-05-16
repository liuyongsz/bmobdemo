using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using PureMVC.Interfaces;

public class selectPanel : TeamFormineMediator
{
    private UILabel name;
    private UILabel sysLevel;
    private UILabel goodLabel;
    private UILabel weekLabel;
    private UILabel conditionLabel;
    private Transform useBtn;
    private Transform alreadyUseBtn;
    private Transform lockBtn;
    private Transform right;
    private Transform left;

    public selectPanel(GameObject go)
    {      
        Init(go);
        AddListenerEvent();

    }
    private void Init(GameObject go)
    {
        name = UtilTools.GetChild<UILabel>(go.transform, "name");
        sysLevel = UtilTools.GetChild<UILabel>(go.transform, "level");
        goodLabel = UtilTools.GetChild<UILabel>(go.transform, "goodLabel");
        weekLabel = UtilTools.GetChild<UILabel>(go.transform, "weekLabel");
        conditionLabel = UtilTools.GetChild<UILabel>(go.transform, "conditionLabel");

        useBtn = UtilTools.GetChild<Transform>(go.transform, "useBtn");
        alreadyUseBtn = UtilTools.GetChild<Transform>(go.transform, "alreadyUseBtn");
        lockBtn = UtilTools.GetChild<Transform>(go.transform, "lockBtn");
        right = UtilTools.GetChild<Transform>(go.transform, "right");
        left = UtilTools.GetChild<Transform>(go.transform, "left");
        


    }
    private void AddListenerEvent()
    {
        UIEventListener.Get(useBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(alreadyUseBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(lockBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(left.gameObject).onClick = OnClick;
        UIEventListener.Get(right.gameObject).onClick = OnClick;
        
    }
    public void SetSelectInfo()
    {
        SetSelectFormationInfo();

        SetBtnState();
    }
    private void SetSelectFormationInfo()
    {
        name.text = TextManager.GetPropsString(string.Format("formation_{0}", formation)); ;
        goodLabel.text = TextManager.GetPropsString(string.Format("formation_good_{0}", formation));
        weekLabel.text = TextManager.GetPropsString(string.Format("formation_week_{0}", formation));

        FormationSysInfo info = TeamFormationSystemConfig.GetFormationSys(mCurSystem);
        int level = info == null ? 0 : info.strongLevel;
        sysLevel.text = "Lv  " + level;
        
        int isActive = TeamFormationConfig.m_activeId_list.IndexOf(formation);

        conditionLabel.gameObject.SetActive(isActive==-1);

        TeamFormation data = TeamFormationConfig.GetTeamFormation(formation);
        string sys_str = TextManager.GetPropsString(string.Format("formation_sys_{0}", mCurSystem));

        string content = "";

        if (data != null && isActive == -1&&data.unlockLevel>0)
        {
            content = TextManager.GetPropsString("formation_condition");
            conditionLabel.text = string.Format(content, sys_str,data.unlockLevel);
        }
        else
        {
            if (info!=null&&info.open == 0)
            {
                content = TextManager.GetPropsString("formation_lock");
                conditionLabel.text = string.Format(content, sys_str);
            }
            else
                conditionLabel.text = "";


        }
    }
    public void SetBtnState()
    {
        alreadyUseBtn.gameObject.SetActive(false);
        useBtn.gameObject.SetActive(false);
        lockBtn.gameObject.SetActive(false);

        alreadyUseBtn.gameObject.SetActive(formation == PlayerMediator.playerInfo.formation);
        FormationSysInfo info = TeamFormationSystemConfig.GetFormationSys(mCurSystem);

        int isActive = TeamFormationConfig.m_activeId_list.IndexOf(formation);

        useBtn.gameObject.SetActive(formation != PlayerMediator.playerInfo.formation&& isActive!=-1);

        lockBtn.gameObject.SetActive(isActive==-1);

        TeamFormation data = TeamFormationConfig.GetTeamFormation(formation);

        lockBtn.GetComponent<UIButton>().isEnabled = true;
        if (info == null || info.open == 0 || data.unlockLevel > info.strongLevel)
        {
            lockBtn.GetComponent<UIButton>().isEnabled = false;
        }

    }
    void OnClick(GameObject go)
    {
        switch(go.transform.name)
        {
            case "useBtn":
                onClientUseFormation(formation);
                break;
         
            case "lockBtn":
                
                OnClientActiveFormation(formation);
                break;
            case "right":
                if(mIndex < mTotalNum)
                {
                    mIndex++;
                    SetFormationInfo();
                    SetSelectInfo();
                }
                break;
            case "left":
                if (mIndex >0)
                {
                    mIndex--;
                    SetFormationInfo();

                    SetSelectInfo();
                }
                break;
        }
    
    }
   
    /// <summary>
    /// 移除监听
    /// </summary>
    public void RemoveListener()
    {
       

    }
    
    protected override void OnDestroy()
    {
      
    }
}
