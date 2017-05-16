using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;


public class equipstrongresultpanel : BasePanel
{
    public Transform returnBtn;
    public Transform closeBtn;
    public UIGrid resultGrid;
    public UILabel goldtext;
}

/// <summary>
/// 一键强化返回数据
/// </summary>
public class StrongResultData
{

    //强化前等级
    public int preStrong;
    //强化后等级
    public int nextStrong;
    //花费
    public int cost;

}

public class EquipStrongResultMediator : UIMediator<equipstrongresultpanel>
{

    public static EquipStrongResultMediator equipStrongResultMediator;
    public static List<StrongResultData> info_list;
    private equipstrongresultpanel panel
    {
        get
        {
            return m_Panel as equipstrongresultpanel;
        }
    }
    public EquipStrongResultMediator() : base("equipstrongresultpanel")
    {
        m_isprop = true;

        RegistPanelCall(NotificationID.EquipStrongResult_Show, OpenPanel);
        RegistPanelCall(NotificationID.EquipStrongResult_Hide, ClosePanel);
    }

    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.returnBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.closeBtn.gameObject).onClick = OnClick;

    }

    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        if (equipStrongResultMediator == null)
        {
            equipStrongResultMediator = Facade.RetrieveMediator("EquipStrongResultMediator") as EquipStrongResultMediator;
        }
        InitGrid();
        SetResultInfo();
    }


    private void InitGrid()
    {
        panel.resultGrid.StartCustom();
        panel.resultGrid.BindCustomCallBack(ResultGrid_UpdateItem);
        panel.resultGrid.enabled = true;
    }

    private void ResultGrid_UpdateItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;

        StrongResultData strong_info = item.oData as StrongResultData;

        UILabel cur_lv = item.mScripts[0] as UILabel;
        UILabel next_lv = item.mScripts[1] as UILabel;
        UILabel crit_txt = item.mScripts[2] as UILabel;
        EquipInfo equip_info = EquipConfig.GetEquipInfo(int.Parse(EquipMediator.cur_equip.itemID));

        crit_txt.gameObject.SetActive(strong_info.nextStrong - strong_info.preStrong > 1);
        cur_lv.text = strong_info.preStrong.ToString();
        next_lv.text = strong_info.nextStrong.ToString();

        EquipAddInfo info = null;
        List<EquipAddInfo> prop_list = EquipConfig.GetPropAddDataListByID(-1, equip_info.id, EquipMediator.cur_equip.star, strong_info.preStrong);
        for (int i = 0; i < prop_list.Count; i++)
        {
             info = prop_list[i];

            string prop_name_label = string.Format("prop_name_{0}", i.ToString());
            UILabel prop_name = UtilTools.GetChild<UILabel>(item.transform, prop_name_label);
            prop_name.text = TextManager.GetUIString(info.prop_name);

            string cur_prop_txt_label = string.Format("cur_prop_value_{0}", i.ToString());
            UILabel cur_prop_txt = UtilTools.GetChild<UILabel>(item.transform, cur_prop_txt_label);
            cur_prop_txt.text = (info.prop_base_value + info.prop_strong_value + info.prop_star_value).ToString();

        }

        List<EquipAddInfo> next_prop_list = EquipConfig.GetPropAddDataListByID(-1, equip_info.id, EquipMediator.cur_equip.star, strong_info.nextStrong);
        for (int j = 0; j < next_prop_list.Count; j++)
        {
             info = next_prop_list[j];
            
            string next_prop_txt_label = string.Format("next_prop_value_{0}",j.ToString());
            UILabel next_prop_txt = UtilTools.GetChild<UILabel>(item.transform, next_prop_txt_label);
            next_prop_txt.text = (info.prop_base_value + info.prop_strong_value + info.prop_star_value).ToString();

        }

    }

    /// <summary>
    /// 设置一键强化信息
    /// </summary>
    /// <param name="list"></param>
    public void SetResultInfo()
    {
        int total_money = 0;
        List<object> listObj = new List<object>();
        for (int i = 0; i < info_list.Count; i++)
        {
            listObj.Add(info_list[i]);
            total_money += info_list[i].cost;
        }
        panel.resultGrid.AddCustomDataList(listObj);

        panel.goldtext.text = total_money.ToString();
        
    }



    private void OnClick(GameObject go)
    {
        Facade.SendNotification(NotificationID.EquipStrongResult_Hide);
    }

    /// <summary>
    /// 界面关闭
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

}