using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class CardInfoPanel : TeamMediator
{
    private UITexture equipBtn1;
    private UITexture equipBtn2;
    private UITexture equipBtn3;
    private UITexture equipBtn4;
    private UITexture equipBtn5;
    private UISprite suitBtn;
    private Transform suitInfo;
    private UISprite closeBtn;
    private UIGrid suitGrid;
    private List<UITexture> equipBtnList = new List<UITexture>();
    public List<EquipItemInfo> equipList = new List<EquipItemInfo>();
    private Dictionary<GameObject, EquipItemInfo> equip_data_dict = new Dictionary<GameObject, EquipItemInfo>();
    public CardInfoPanel(GameObject go)
    {      
        Init(go);
        AddListenerEvent();      
    }
    private void Init(GameObject go)
    {
        equipBtn1 = UtilTools.GetChild<UITexture>(go.transform, "equipBtn1");
        equipBtn2 = UtilTools.GetChild<UITexture>(go.transform, "equipBtn2");
        equipBtn3 = UtilTools.GetChild<UITexture>(go.transform, "equipBtn3");
        equipBtn4 = UtilTools.GetChild<UITexture>(go.transform, "equipBtn4");
        equipBtn5 = UtilTools.GetChild<UITexture>(go.transform, "equipBtn5");
        suitBtn = UtilTools.GetChild<UISprite>(go.transform, "suitBtn");
        closeBtn = UtilTools.GetChild<UISprite>(go.transform, "SuitInfo/closeBtn");
        suitInfo = UtilTools.GetChild<Transform>(go.transform, "SuitInfo");
        suitGrid = UtilTools.GetChild<UIGrid>(go.transform, "SuitInfo/Scroview/Grid");
        suitGrid.enabled = true;
        suitGrid.BindCustomCallBack(UpdateSuitGridItem);
        suitGrid.StartCustom();
        UIEventListener.Get(closeBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(equipBtn1.gameObject).onClick = OnClick;
        UIEventListener.Get(equipBtn2.gameObject).onClick = OnClick;
        UIEventListener.Get(equipBtn3.gameObject).onClick = OnClick;
        UIEventListener.Get(equipBtn4.gameObject).onClick = OnClick;
        UIEventListener.Get(equipBtn5.gameObject).onClick = OnClick;      
        UIEventListener.Get(suitBtn.gameObject).onClick = OnClick;
        equipBtnList.Add(equipBtn1);
        equipBtnList.Add(equipBtn2);
        equipBtnList.Add(equipBtn3);
        equipBtnList.Add(equipBtn4);
        equipBtnList.Add(equipBtn5);

    }
    private void AddListenerEvent()
    {
        for (int j = 0; j < equipBtnList.Count; j++)
        {

            UIEventListener.Get(equipBtnList[j].gameObject).onClick = OnClick;
        }

        GameEventManager.RegisterEvent(GameEventTypes.EquipRefresh, OnRefreshEquip);
    }
    public override void SetChildPanel()
    {
        SetEquipInfo();
    }
    void UpdateSuitGridItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        SuitInfo info = item.oData as SuitInfo;
        UILabel name = item.mScripts[0] as UILabel;
        UILabel equip = item.mScripts[1] as UILabel;
        UISprite desc = item.mScripts[2] as UISprite;
        name.text = TextManager.GetItemString(info.nameID);
        UISprite[] equipList = equip.GetComponentsInChildren<UISprite>();
        int suitIndex = 0;
        ItemInfo equipItem;
        string[] equipId = info.equipId.Split(',');
        for (int i = 0; i < equipList.Length; ++i)
        {
            if (EquipInSuit(equipId[i]))
                suitIndex++;
            equipItem = ItemManager.GetItemInfo(equipId[i]);
            if (equipItem == null)
                continue;
            equipList[i].spriteName = UtilTools.StringBuilder("color", equipItem.color);
            LoadSprite.LoaderItem(equipList[i].GetComponentInChildren<UITexture>(), equipId[i]);
        }
        UILabel[] suitDesc = desc.GetComponentsInChildren<UILabel>();
        for (int i = 0; i < suitDesc.Length; ++i)
        {
            string descText = string.Format(TextManager.GetPropsString(UtilTools.StringBuilder("suit", info.id, i + 2)), info.suitAdd.Split(',')[i]);
            if (suitIndex > i + 1)
                suitDesc[i].text = string.Format("[05FF2D]{0}[-]", descText);
            else
                suitDesc[i].text = descText;
        }
    }
    void OnClick(GameObject go)
    {
        if (go == suitBtn.gameObject)
        {
            List<object> list = new List<object>();
            foreach (SuitInfo item in SuitConfig.configList.Values)
            {
                list.Add(item);
            }
            suitGrid.AddCustomDataList(list);
            suitInfo.gameObject.SetActive(true);
        }
        else if(go == closeBtn.gameObject)
        {
            suitGrid.ClearCustomData();
            suitGrid.ClearCustomData();
            suitInfo.gameObject.SetActive(false);
        }
        else
        {
            List<object> list = new List<object>();

            if (equip_data_dict.ContainsKey(go.gameObject))
            {
                list.Add(currentTeamBaller.id);
                list.Add(equip_data_dict[go]);
                Facade.SendNotification(NotificationID.EquipInformation_Show, list);
            }
            else
            {

             
                int index = equipBtnList.IndexOf(go.transform.GetComponent<UITexture>());

                if (EquipConfig.GetEquipDataListByPos((int)(index + 1)).Count == 0)
                {
                    GUIManager.SetPromptInfo(TextManager.GetUIString("UI2052"), null);
                    return;
                }

                Equip_Pos pos = (Equip_Pos)Enum.Parse(typeof(Equip_Pos), (index + 1).ToString());
                EquipChooseData data = new EquipChooseData(currentTeamBaller.id, Equip_Select_Type.Pos, pos, 0);
                list.Add(data);

                Facade.SendNotification(NotificationID.EquipChoose_Show, list);
            }
        }       
    }
    /// <summary>
    /// 移除监听
    /// </summary>
    public void RemoveListener()
    {
        GameEventManager.UnregisterEvent(GameEventTypes.EquipRefresh, OnRefreshEquip);

    }
    /// <summary>
    /// 刷新装备
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="args"></param>
    private void OnRefreshEquip(GameEventTypes eventType, object[] args)
    {
        if (!GUIManager.HasView("teampanel"))
            return;
        if (null != currentTeamBaller && args.Length > 0 && GameConvert.IntConvert(args[0]) == currentTeamBaller.id)
            SetEquipInfo();
    }
    bool EquipInSuit(string itemID)
    {
        for (int i = 0; i < equipList.Count; ++i)
        {
            if (equipList[i].itemID == itemID)
                return true;
        }
        return false;
    }
    /// <summary>
    /// 设置装备信息
    /// </summary>
    private void SetEquipInfo()
    {
        if (EquipConfig.m_player_eqiup.Count < 1)
            return;

        equipList = EquipConfig.GetEquipDataListByPlayerID(currentTeamBaller.id);

        equip_data_dict.Clear();
        for (int i = 0; i < equipBtnList.Count; ++i)
        {
            UISprite icon = equipBtnList[i].transform.FindChild("Sprite").GetComponent<UISprite>();
            EquipItemInfo info = GetEquipInfoByPos(i + 1);
            if (info!=null)
            {
                LoadSprite.LoaderItem(equipBtnList[i], info.itemID, false);
                icon.spriteName = UtilTools.StringBuilder("color", info.star);
                equip_data_dict.Add(equipBtnList[i].gameObject, info);

            }
            else
            {
                icon.spriteName = "wupinkuang";
                LoadSprite.LoaderImage(equipBtnList[i], "jiahao", false);
            }
        }
    }

    private EquipItemInfo GetEquipInfoByPos(int pos)
    {
        EquipItemInfo info = null;
        for (int i=0; i< equipList.Count; i++)
        {
            
            EquipInfo equip = EquipConfig.GetEquipInfo(int.Parse(equipList[i].itemID));
            if(pos == equip.position)
            {
                return equipList[i];
            }
        }

        return info;

    }
    protected override void OnDestroy()
    {
        for (int i = 0; i < equipBtnList.Count; ++i)
        {
            equipBtnList[i] = null;
        }
        equipBtnList.Clear();
        equip_data_dict.Clear();
        RemoveListener();
    }
}
