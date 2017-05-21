using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;
using System;

/// <summary>
/// 实例化控件
/// </summary>

public class equipmainpanel : BasePanel
{
    public UITexture cup;
    public UIGrid playerGrid;
    public UIGrid equipGrid;
    public UIToggle strongBtn;
    public UIToggle starBtn;
    public UIToggle insetBtn;
    public UIToggle inheritBtn;
    public UIToggle makeBtn;
    public UIToggle compoundBtn;

    public Transform backBtn;
    public Transform playerItemInfo;
    public Transform equipItem;

    public UIScrollView equipView;

}


public enum Equip_Func_Type
{
    Null,
    Star,               //升星
    Strong,          //强化
    Inset,             //镶嵌
    Inherit,          //传承
    Make,           //制作
    Compound, //合成
}

public class EquipMediator : UIMediator<equipmainpanel>
{

    public static EquipMediator equipMediator;
    //选中的装备
    public static EquipItemInfo cur_equip;
    public static int cur_select_player_id=-1;
    public  List<EquipItemInfo> equipList = new List<EquipItemInfo>();

    public Equip_Func_Type cur_type = Equip_Func_Type.Strong;
    public NotificationID cur_open_equip = NotificationID.EquipStrong_Hide;
    private UIGridItem last_equip_item;

    private equipmainpanel panel
    {
        get
        {
            return m_Panel as equipmainpanel;
        }
    }
    public EquipMediator() : base("equipmainpanel")
    {
        m_isprop = true;
        setDepth = 4;
        RegistPanelCall(NotificationID.EquipMain_Show, OpenPanel);
        RegistPanelCall(NotificationID.EquipMain_Hide, ClosePanel);
    }

    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        if (equipMediator == null)
        {
            equipMediator = Facade.RetrieveMediator("EquipMediator") as EquipMediator;
        }

        bool isHas = EquipConfig.IsHasEquip();

        cur_type = isHas ? Equip_Func_Type.Strong : Equip_Func_Type.Make;
        cur_open_equip = isHas ? NotificationID.EquipStrong_Hide : NotificationID.EquipMake_Hide;

        Init();

        RefreshBtnStates();

        SetPlayerInfo();
        SetEquipGridInfo(cur_select_player_id);
        OpenFunction();

    }
    private void Init()
    {
        panel.playerGrid.enabled = true;
        panel.playerGrid.BindCustomCallBack(OnUpdateDataRow);
        panel.playerGrid.StartCustom();

        panel.equipGrid.enabled = true;
        panel.equipGrid.BindCustomCallBack(OnUpdateEquip);
        panel.equipGrid.StartCustom();

    }

    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.backBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.strongBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.starBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.insetBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.inheritBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.makeBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.compoundBtn.gameObject).onClick = OnClick;



    }

    /// <summary>
    /// 刷新按钮状态
    /// </summary>
    private void RefreshBtnStates()
    {
        panel.strongBtn.GetComponent<UIToggle>().value = Equip_Func_Type.Strong == cur_type;
        panel.starBtn.GetComponent<UIToggle>().value = Equip_Func_Type.Star == cur_type;
        panel.insetBtn.GetComponent<UIToggle>().value = Equip_Func_Type.Inset == cur_type;
        panel.inheritBtn.GetComponent<UIToggle>().value = Equip_Func_Type.Inherit == cur_type;
        panel.makeBtn.GetComponent<UIToggle>().value = Equip_Func_Type.Make == cur_type;
        panel.compoundBtn.GetComponent<UIToggle>().value = Equip_Func_Type.Compound == cur_type;

    }


    private void OnClick(GameObject go)
    {
        Equip_Func_Type type = Equip_Func_Type.Null;
        NotificationID open_type = NotificationID.NULL;

        if (go.transform.name == "backBtn")
            Facade.SendNotification(NotificationID.EquipMain_Hide);
      
    
        switch (go.transform.name)
        {
          
            case "strongBtn":
                type = Equip_Func_Type.Strong;
                open_type = NotificationID.EquipStrong_Hide;
                break;
            case "starBtn":
                type = Equip_Func_Type.Star;
                open_type = NotificationID.EquipStar_Hide;
                break;
            case "insetBtn":
                type = Equip_Func_Type.Inset;
                open_type = NotificationID.EquipInset_Hide;

                break;
            case "inheritBtn":
                type = Equip_Func_Type.Inherit;
                open_type = NotificationID.EquipInherit_Hide;

                break;
            case "makeBtn":
                type = Equip_Func_Type.Make;
                open_type = NotificationID.EquipMake_Hide;

                break;
            case "compoundBtn":
                type = Equip_Func_Type.Compound;
                open_type = NotificationID.GemCompound_Hide;

                break;
        }

        if(open_type != cur_open_equip)
        {
            Facade.SendNotification(cur_open_equip);
            cur_open_equip = open_type;
        }

        if (type != cur_type)
        {
            cur_type = type;
            OpenFunction();
        }
    }
    private void OnUpdateEquip(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        item.onClick = ClickEquipItem;
        EquipItemInfo info = item.oData as EquipItemInfo;

        UISprite color = item.mScripts[0] as UISprite;
        UILabel level = item.mScripts[1] as UILabel;
        UITexture icon = item.mScripts[2] as UITexture;
        UILabel name = item.mScripts[3] as UILabel;
        UISprite select = item.mScripts[4] as UISprite;
        UITexture star = item.mScripts[5] as UITexture;
        bool show = false;
        if (cur_equip != null)
             show = cur_select_player_id==0? info.uuid == cur_equip.uuid: info.itemID==cur_equip.itemID;
        select.transform.gameObject.SetActive(show);
        if (show)
            last_equip_item = item;

        Transform addbg = UtilTools.GetChild<Transform>(item.transform, "addbg");
        Transform content = UtilTools.GetChild<Transform>(item.transform, "content");
        UISprite[] equip_star = UtilTools.GetChilds<UISprite>(content, "star");

        addbg.gameObject.SetActive(string.IsNullOrEmpty(info.itemID));
        content.gameObject.SetActive(!string.IsNullOrEmpty(info.itemID));
        

        if (string.IsNullOrEmpty(info.itemID))
        {
            UILabel pos_txt = UtilTools.GetChild<UILabel>(addbg, "pos");
            pos_txt.text =TextManager.GetUIString(string.Format("position_{0}", info.position)) ;
            return;
        }

        ItemInfo item_info = ItemManager.GetItemInfo(info.itemID.ToString());
        if (item_info == null)
            return;


        level.text =TextManager.GetUIString("UI2010") + info.strongLevel.ToString();

        name.text = TextManager.GetItemString(info.itemID);
        color.spriteName = "color" + info.star;
        LoadSprite.LoaderItem(icon, info.itemID, false);

        //UtilTools.SetTextColor(name, info.star);
        UtilTools.SetStar(info.star, equip_star);

    }
    private void OnUpdateDataRow(UIGridItem item)
    {
       

    }
    private void SetPlayerInfo()
    {

        // 默认打开球队卡牌界面
        List<object> listObj = new List<object>();
        List<int> equip_player_list = EquipConfig.GetHasEquipPlayerList();
        equip_player_list.Sort(ComparePlayer);

        for (int i=0;i< equip_player_list.Count; i++)
        {
            int id = equip_player_list[i];
            listObj.Add(id);
            if (cur_select_player_id < 0)
                cur_select_player_id = id;
        }
        
        panel.playerGrid.AddCustomDataList(listObj);
        
    }

    /// <summary>
    /// 设置装备
    /// </summary>
    /// <param name="player_id"></param>
    public void SetEquipGridInfo(int player_id = 0)
    {
        if (player_id != cur_select_player_id)
            return;

        equipList = EquipConfig.GetEquipDataListByPlayerID(player_id);
        equipList.Sort(CompareEquip);

        int max_count = player_id == 0 ? equipList.Count : 5;
        List <object> listObj = new List<object>();
        EquipItemInfo equip = null;
        for (int i = 0; i < max_count; i++)
        {
            int pos = i + 1;

            if (player_id != 0)
                equip = GetEquipData(pos, equipList);
            else
                equip = equipList[i];

            listObj.Add(equip);

        }

        if (cur_equip == null&& equipList.Count>0)
            cur_equip = equipList[0];

        panel.equipView.enabled = listObj.Count > 5;


        int index =GetEquipIndex(cur_equip, listObj);
        panel.equipGrid.AddCustomDataList(listObj);
        panel.equipGrid.SetSelect(index);
        last_equip_item = panel.equipGrid.GetSelectedGridItem();
        if (listObj.Count > 5)
            panel.equipGrid.GoToPosition(index-1);
        

    }

    /// <summary>
    /// 点击球员
    /// </summary>
    /// <param name="data"></param>
    /// <param name="go"></param>
    private void ClickPlayerItem(UIGridItem item)
    {

        if (item == null || item.mScripts == null || item.oData == null)
            return;

        MyUIToggle toggle = item.GetComponent<MyUIToggle>();
        if (toggle.Value == true)
            return;
        cur_equip = null;
        toggle.Value = true;

        int player_id = GameConvert.IntConvert(item.oData);
        cur_select_player_id = player_id;
        SetEquipGridInfo(player_id);
        OpenFunction();

    }


    /// <summary>
    /// 点击装备
    /// </summary>
    /// <param name="data"></param>
    /// <param name="go"></param>
    private void ClickEquipItem(UIGridItem item)
    {

        EquipItemInfo data = item.oData as EquipItemInfo;

        if(cur_equip!=null)
        {
            if (cur_select_player_id == 0 && data.uuid == cur_equip.uuid
         || cur_select_player_id != 0 && data.itemID == cur_equip.itemID)
                return;
        }
     
     
        if (string.IsNullOrEmpty(data.itemID))
        {

            Equip_Pos pos = (Equip_Pos)Enum.Parse(typeof(Equip_Pos), data.position.ToString());
            item.Selected = false;
            if (last_equip_item != null)
                last_equip_item.Selected = true;
            if (EquipConfig.GetEquipDataListByPos((int)pos).Count == 0)
            {
                GUIManager.SetPromptInfo(TextManager.GetUIString("UI2052"), null);
                return;
            }

            List<object> list = new List<object>();
            EquipChooseData info = new EquipChooseData(cur_select_player_id, Equip_Select_Type.Pos, pos, 0);
            list.Add(info);
            

            Facade.SendNotification(NotificationID.EquipChoose_Show, list);
            return;
        }
        
        cur_equip = data;
        if (last_equip_item != null)
            last_equip_item.Selected = false;
        last_equip_item = item;
        OpenFunction();
    }
    /// <summary>
    /// 打开功能界面
    /// </summary>
    /// <param name="type"></param>
    public void OpenFunction()
    {
        bool isopen = false;

      

        bool ishas = EquipConfig.GetEquipDataListByPlayerID(cur_select_player_id).Count > 0;

        if (!ishas)
        {
            Facade.SendNotification(cur_open_equip);
            if (cur_type == Equip_Func_Type.Compound)
                Facade.SendNotification(NotificationID.GemCompound_Show);
            else if (cur_type == Equip_Func_Type.Make)
                Facade.SendNotification(NotificationID.EquipMake_Show);
            return;
        }

        if (cur_type == Equip_Func_Type.Compound)
            Facade.SendNotification(NotificationID.GemCompound_Show);
        else if (cur_type == Equip_Func_Type.Make)
            Facade.SendNotification(NotificationID.EquipMake_Show);

        switch (cur_type)
        {
            case Equip_Func_Type.Star:
                isopen = GUIManager.HasView("equipstarpanel");
                if (isopen)
                    EquipStarMediator.equipStarMediator.SetEquip();
                else
                    Facade.SendNotification(NotificationID.EquipStar_Show);
                break;
            case Equip_Func_Type.Strong:
                isopen = GUIManager.HasView("equipstrongpanel");
                if (isopen)
                    EquipStrongMediator.equipStrongMediator.SetEquip();
                else
                    Facade.SendNotification(NotificationID.EquipStrong_Show);
                break;
            case Equip_Func_Type.Inset:
                isopen = GUIManager.HasView("equipinsetpanel");
                if (isopen)
                    EquipInsetMediator.equipInsetMediator.SetEquip();
                else
                    Facade.SendNotification(NotificationID.EquipInset_Show);
                break;
            case Equip_Func_Type.Inherit:
                isopen = GUIManager.HasView("equipinheritpanel");
                if (isopen)
                    EquipInheritMediator.equipInheritMediator.SetEquip();
                else
                    Facade.SendNotification(NotificationID.EquipInherit_Show);
                break;
      
        }
    }
    //刷新功能界面数据
    public void RefreshOpenFunc(int star ,int level)
    {

       if( GUIManager.HasView("equipstarpanel")|| GUIManager.HasView("equipstrongpanel")|| 
            GUIManager.HasView("equipinsetpanel") || GUIManager.HasView("equipinheritpanel")
            || GUIManager.HasView("gemcompoundpanel"))
        {
            switch (cur_type)
            {
                case Equip_Func_Type.Star:
                    EquipStarMediator.equipStarMediator.SetInfo(star, level);
                    break;
                case Equip_Func_Type.Strong:
                    EquipStrongMediator.equipStrongMediator.SetInfo(star, level);
                    break;
                case Equip_Func_Type.Inset:
                    EquipInsetMediator.equipInsetMediator.SetInfo();
                    break;
                case Equip_Func_Type.Inherit:
                    EquipInheritMediator.equipInheritMediator.SetInfo();
                    break;
            }
        }      
    }
    /// <summary>
    /// 玩家礼拜排序
    /// </summary>
    /// <param name="id1"></param>
    /// <param name="id2"></param>
    /// <returns></returns>
    private int ComparePlayer(int id1,int id2)
    {

       

        return 0;
    }

    //通过装备位置获取玩家装备数据
    private EquipItemInfo GetEquipData(int pos , List<EquipItemInfo> list)
    {
        for(int i=0;i<list.Count; i++)
        {
            EquipInfo info = EquipConfig.GetEquipInfo(int.Parse(list[i].itemID));
            if (info.position == pos)
                return list[i];
        }

        EquipItemInfo equip = new EquipItemInfo();
        equip.uuid = "";
        equip.position = pos;
        return equip;
    }
    private void ResetInfo()
    {
        cur_type = Equip_Func_Type.Strong;
        cur_open_equip = NotificationID.EquipStrong_Hide;
        cur_equip = null;

    }
    //装备按照位置排序
    private int CompareEquip(EquipItemInfo info1, EquipItemInfo info2)
    {
        EquipInfo equip1 = EquipConfig.GetEquipInfo(int.Parse(info1.itemID));
        EquipInfo equip2 = EquipConfig.GetEquipInfo(int.Parse(info2.itemID));

        if (equip1.position > equip2.position)
            return 1;
        else if (equip1.position < equip2.position)
            return -1;
        else if (info1.star > info1.star)
            return -1;
        else if (info1.star < info1.star)
            return 1;
        else if (info1.strongLevel > info1.strongLevel)
            return -1;
        else if (info1.strongLevel < info1.strongLevel)
            return 1;
        return 0;

    }
    /// <summary>
    /// 获取装备Index
    /// </summary>
    /// <param name="item"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    private int GetEquipIndex(EquipItemInfo item, List<object> list)
    {
        int index = -1;
        if (cur_equip == null)
            return index;
        for(int i=0; i<list.Count; i++)
        {
            EquipItemInfo info = list[i] as EquipItemInfo;
            if (cur_equip.uuid== info.uuid&& cur_equip.itemID == info.itemID)
            {
                index = i;
                break;
            }
        }

        return index;
    }
    /// <summary>
    /// 界面关闭
    /// </summary>
    protected override void OnDestroy()
    {
        Facade.SendNotification(cur_open_equip);
        ResetInfo();
        base.OnDestroy();
    }
    
}

