using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;

/// <summary>
/// 实例化控件
/// </summary>
public class storepanel :  BasePanel
{
    public Transform privilegeBtn;
    public Transform backBtn;
    public UISlider VIPSlider;
    public UIGrid storeGrid;
    public UILabel VIPcurrent;
    public UILabel VIPnext;
    public UILabel vipdesc;
    public UILabel vipNeedMoney;
}
public class StoreMediator : UIMediator<storepanel>
{
    private storepanel panel
    {
        get
        {
            return m_Panel as storepanel;
        }
    }
    private UIGrid grid
    {
        get
        {
            return m_Panel.storeGrid;
        }
    }
    private StoreInfo storeInfo;

    private List<StoreInfo> storeInfoList = new List<StoreInfo>();
    /// <summary>
    /// 注册界面逻辑
    /// </summary>
    public StoreMediator() : base("storepanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.Store_Show, OpenPanel);
        RegistPanelCall(NotificationID.Store_Hide, ClosePanel);
    }  
    /// <summary>
    /// 绑定点击事件
    /// </summary>
    protected override void AddComponentEvents()
    {       
        UIEventListener.Get(m_Panel.privilegeBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.backBtn.gameObject).onClick = OnClick;

    }
    protected override void OnShow(INotification notification)
    {
        InitView();
    }
    /// <summary>
    /// 点击事件
    /// </summary>
    void OnClick(GameObject go)
    {
        if (go == m_Panel.backBtn.gameObject)
        {
            Facade.SendNotification(NotificationID.Store_Hide);          
        }
    }
    /// <summary>
    /// 初始化界面排列
    /// </summary>
    public void InitView()
    {
        for (int i = 0; i < StoreConfig.storeList.mList.Count; ++i)
        {
            storeInfoList.Add(StoreConfig.storeList[StoreConfig.storeList.mList[i]]);
        }
        grid.enabled = true;
        grid.BindCustomCallBack(UpdateStoreGridItem);
        grid.StartCustom();
        grid.AddCustomDataList(AddListGrid(storeInfoList));
        m_Panel.VIPcurrent.text = UtilTools.StringBuilder("VIP ", PlayerMediator.playerInfo.vipLevel);
        m_Panel.VIPnext.text = UtilTools.StringBuilder("VIP ", PlayerMediator.playerInfo.vipLevel + 1);
        VipUpInfo info = VIPUPConfig.GetVipUpInfo(PlayerMediator.playerInfo.vipLevel);
        if (info == null)
        {
            m_Panel.vipNeedMoney.text = TextManager.GetUIString("UIStore5");
            m_Panel.VIPSlider.value = 1;
            return;
        }
        m_Panel.vipNeedMoney.text = string.Format(TextManager.GetUIString("UIVIP001"), info.upgradeExp - PlayerMediator.playerInfo.rmb, PlayerMediator.playerInfo.vipLevel + 1);
        m_Panel.VIPSlider.value = PlayerMediator.playerInfo.rmb * 1.0f / info.upgradeExp;
    }
    void UpdateStoreGridItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        StoreInfo info = item.oData as StoreInfo;
        UILabel getdiamon = item.mScripts[0] as UILabel;
        UILabel give = item.mScripts[1] as UILabel;
        UILabel price = item.mScripts[2] as UILabel;
        item.onClick = OnClickItem;
        price.text = string.Format(TextManager.GetUIString("UIStore4"), info.needMoney);
        give.text = string.Format(TextManager.GetUIString("UIStore2"), info.giveDiamond);
        getdiamon.text = string.Format(TextManager.GetUIString("UIStore3"), info.diamondCount);
    }
    List<object> AddListGrid(List<StoreInfo> list)
    {
        List<object> listObj = new List<object>();
        for (int i = 0; i < list.Count; ++i)
        {
            listObj.Add(list[i]);
        }
        return listObj;
    }
    /// <summary>
    /// GridItem点击事件
    /// </summary>
    void OnClickItem(UIGridItem go)
    {
        storeInfo = go.oData as StoreInfo;
        GUIManager.SetPromptInfoChoose(TextManager.GetUIString("UICreate1"), TextManager.GetUIString("UIStore6"), ClickOK);
    }

    /// <summary>
    /// 购买钻石
    /// </summary>
    void ClickOK()
    {
        if (storeInfo == null)
        {
            return;
        }
        ServerCustom.instance.SendClientMethods("onClientCharge", storeInfo.id);
    }
    protected override void OnDestroy()
    {
        storeInfoList.Clear();
        base.OnDestroy();
    }
}

public class StoreInfo
{
    public int id;            // ID
    public int type;          // 礼包类型
    public int diamondCount;  // 钻石数量
    public int needMoney;     // 花费人民币
    public int giveDiamond;   // 赠送钻石
    public int getMoney;      // 获取的钞票数量
    public int needDiamond;   // 花费钻石
    public int getPower;      // 获得体力
}