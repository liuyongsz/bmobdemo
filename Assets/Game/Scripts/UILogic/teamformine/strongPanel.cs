using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using PureMVC.Interfaces;

public class strongPanel : TeamFormineMediator
{

    private UILabel name;
    private UILabel sysLevel;
    private Transform prop;
    private Transform activeBtn;
    private Transform updataBtn;
    private Transform costView;
    private UIGrid costGrid;
    public strongPanel(GameObject go)
    {      
        Init(go);
        AddListenerEvent();

    }
    private void Init(GameObject go)
    {

        name = UtilTools.GetChild<UILabel>(go.transform, "name");
        sysLevel = UtilTools.GetChild<UILabel>(go.transform, "level");

        costView = UtilTools.GetChild<Transform>(go.transform, "costView");
        costGrid = UtilTools.GetChild<UIGrid>(costView, "costGrid");

        prop = UtilTools.GetChild<Transform>(go.transform, "prop");
        activeBtn = UtilTools.GetChild<Transform>(go.transform, "activeBtn");
        updataBtn = UtilTools.GetChild<Transform>(go.transform, "updataBtn");

        costGrid.enabled = true;
        costGrid.BindCustomCallBack(OnUpdateCost);
        costGrid.StartCustom();


    }
    private void AddListenerEvent()
    {
        UIEventListener.Get(activeBtn.gameObject).onClick = OnClick;
        LongClickEvent.Get(updataBtn.gameObject).onPress = OnPress;
        LongClickEvent.Get(updataBtn.gameObject).duration = 0.5f;

    }
    public void SetInfo()
    {

        SetCostInfo();
        SetProp();
        SetBtnState();

    }
    public void SetBtnState()
    {

        bool imaterial = IsEnoughMaterial();
        updataBtn.GetComponent<UIButton>().isEnabled = imaterial;
       
        name.text = TextManager.GetPropsString(string.Format("formation_{0}", formation)); ;
        FormationSysInfo info = TeamFormationSystemConfig.GetFormationSys(mCurSystem);

        int level = info == null ? 0 : info.strongLevel;

        sysLevel.text = "Lv  " + level;
    }
    private void SetProp()
    {
        FormationSysInfo info = TeamFormationSystemConfig.GetFormationSys(mCurSystem);
        
        int level = info == null ? 0 : info.strongLevel;

        level += 1;

        if (level >= 100)
            level = 100;

        TeamStrong strongInfo = TeamStrongConfig.GetTeamStrong(level);
        string[] propArr = strongInfo.prop.Split(';');

        for(int i=0; i<propArr.Length; i++)
        {
            string[] propStr = propArr[i].Split(':');

            string name = propStr[0];
            string value = propStr[1];

            string prop_name_label = string.Format("prop_name_{0}", i.ToString());
            UILabel prop_name = UtilTools.GetChild<UILabel>(prop, prop_name_label);

            string prop_value_label = string.Format("prop_value_{0}", i.ToString());
            UILabel prop_value = UtilTools.GetChild<UILabel>(prop, prop_value_label);

            prop_name.text = TextManager.GetUIString(name);
            prop_value.text = value;
        }


    }
    /// <summary>
    /// 消耗信息
    /// </summary>
    public void SetCostInfo()
    {
        TeamFormationSystem sysInfo = TeamFormationSystemConfig.GetTeamFormationSystem(mCurSystem);
        string[] need_str_arr = sysInfo.material.Split(';');
        // 默认打开球队卡牌界面
        List<object> listObj = new List<object>();
        for (int i = 0; i < need_str_arr.Length; i++)
        {
            EquipCostInfo info = new EquipCostInfo();
            string[] info_arr = need_str_arr[i].Split(':');
            info.item_id = GameConvert.IntConvert(info_arr[0]);
            info.need_num = GameConvert.IntConvert(info_arr[1]);
            listObj.Add(info);
        }
        costGrid.AddCustomDataList(listObj);

    }
    public void SetGridInfo()
    {
        TeamFormationSystem sysInfo = TeamFormationSystemConfig.GetTeamFormationSystem(mCurSystem);
        
    }
    private void OnUpdateCost(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        EquipCostInfo info = item.oData as EquipCostInfo;

        UISprite color = item.mScripts[0] as UISprite;
        UITexture icon = item.mScripts[1] as UITexture;
        UILabel num = item.mScripts[2] as UILabel;

        ItemInfo item_info = ItemManager.GetItemInfo(info.item_id.ToString());
        color.spriteName = "color" + item_info.color;

        LoadSprite.LoaderItem(icon, info.item_id.ToString(), false);


        int total_num = ItemManager.GetBagItemCount(info.item_id.ToString());
        num.text = string.Format("{0}/{1}", total_num, info.need_num.ToString());
        num.color = total_num >= info.need_num ? Color.white : Color.red;
    }

    private void OnClick(GameObject go)
    {
        Facade.SendNotification(NotificationID.FormationSysActive_Show);

    }
    void OnPress(GameObject go,bool isPress)
    {
        if (!isPress)
            return;
        switch(go.transform.name)
        {
            case "updataBtn":
                bool ismaterial = IsEnoughMaterial();
                if (!mIsCanUpdata)
                    return;
                if (!ismaterial)
                {
                    GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_7"), null);
                    return;
                }

                FormationSysInfo info = TeamFormationSystemConfig.GetFormationSys(mCurSystem);

                TeamFormationSystem formationSys = TeamFormationSystemConfig.GetTeamFormationSystem(mCurSystem);
                if (formationSys.maxStrongLevel <= info.strongLevel)
                {
                    GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_44"), null);

                    return;
                }
                OnClientFormationStrong(mCurSystem);
                mIsCanUpdata = false;
                break;
         
       
        
        }
    
    }
    /// <summary>
    /// 移除监听
    /// </summary>
    public void RemoveListener()
    {
       

    }
  
    /// <summary>
    /// 是否材料满足
    /// </summary>
    /// <returns></returns>
    private bool IsEnoughMaterial()
    {
        TeamFormationSystem sysInfo = TeamFormationSystemConfig.GetTeamFormationSystem(mCurSystem);

        string[] need_str_arr = sysInfo.material.Split(';');
        List<object> listObj = new List<object>();
        for (int i = 0; i < need_str_arr.Length; i++)
        {
            string[] info_arr = need_str_arr[i].Split(':');
            int item_id = GameConvert.IntConvert(info_arr[0]);
            int need_num = GameConvert.IntConvert(info_arr[1]);
            int total_num = ItemManager.GetBagItemCount(item_id.ToString());
            if (need_num > total_num)
                return false;
        }
        return true;
    }


    //阵型系统升级
    private void OnClientFormationStrong(int sysId)
    {
        ServerCustom.instance.SendClientMethods(FormationProxy.OnClientFormationStrong, sysId);

    }

    protected override void OnDestroy()
    {
      
    }
}
