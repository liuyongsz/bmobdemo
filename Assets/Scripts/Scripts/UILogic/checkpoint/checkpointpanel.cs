using UnityEngine;
using System.Collections;
using PureMVC.Interfaces;
using System.Collections.Generic;
using AssetBundles;
using System;

public class checkpointpanel : BasePanel {


    public UIButton cup_btn1;
    public UIButton cup_btn2;
    public UIButton cup_btn3;
    public UIButton cup_btn4;
    public UISlider star_slider;
    public UIButton change_btn;
    public UIGrid chapterGrid;              //章节Grid
    public Transform chapter_btn;
    public UIGrid checkGrid;                //关卡Grid
    public Transform checkpoint_btn;
    public UILabel checkpointname;
    public Transform awardinfo;
    public UISprite leveloff_btn;
    public UIButton saodang_btn;
    public UIButton begin_btn;
    public Transform preview;
    public UIGrid goodsGrid;                //关卡掉落预览
    public UIGrid cupGrid;                  //奖杯（宝箱）奖励
    public UIButton cupsure_btn;
    public UIButton cupoff_btn;
    public Transform checkinfo;             //关卡界面
    public UIButton of;


    public UIGrid shaoDangGrid;
    public UIButton btnShaoDangClose;
    public UIButton btnAlertClose;
    public UIButton btnAlertMax;
    public UIButton btnAlertMin;
    public UIButton btnAlertDiv;
    public UIButton btnAlertAdd;
    public UILabel txtShaDangNum;
    public UILabel targetDes;
    public UILabel txtSaodangTime;
    public UILabel txtShaoDangUse;

    public Transform panAlert;
    public UIButton btnAlertNo;
    public UIButton btnAlertOk;
}

public class CheckpointMediator : UIMediator<checkpointpanel>
{
    int presentchapter=1;            //当前章节
    int presenchaptertstar=0;      //当前章节星星数量
    int presentlevelstar;          //当前关卡星星数量
    bool isawardpanel;             //是否打开奖励界面
    bool islevelpanel;             //是否打开关卡信息界面
    bool ispress = false;

    LevelRewardInfo levelinfo;
    TD_CloneLevel chapterinfo;

    private List<CloneVO> m_data;

    private CloneVO m_selectedItem;
    private TD_Clone m_selectedLvCfg;

    private List<ShaoDangAwardVO> m_shaoDangAwards;

    public static CheckpointMediator checkpointMediator;
    private checkpointpanel panel
    {
        get
        {
            return m_Panel as checkpointpanel;
        }
    }


    private int m_saodangNum = 0;
    private int m_saodangMaxNum = 0;
    private int m_sadangUse = 0;

    public CheckpointMediator() : base("checkpointpanel")
    {
        m_isprop = true;

        RegistPanelCall(NotificationID.CheckPoint_Show, OpenPanel);
        RegistPanelCall(NotificationID.CheckPoint_Hide, ClosePanel);

        RegistPanelCall(NotificationID.Clone_Inflo, OnRec_CloneInfo);
    }

    protected override void OpenPanel(INotification notification)
    {
        base.OpenPanel(notification);

        CloneProxy.Instance.onClientGetAllCloneInfo();
    }

    private void OnRec_CloneInfo(INotification notification)
    {
        m_data = (List<CloneVO>)notification.Body;

        if (null != m_Panel)
        {
            UpdateServerDisplay();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        m_data = null;
        m_selectedItem = null;
        m_selectedLvCfg = null;
        m_saodangNum = 0;
        m_saodangMaxNum = 0;
    }

    int Levelname = 1;
    protected override void OnShow(INotification notification)
    {       
        if (checkpointMediator == null)
        {
            checkpointMediator = Facade.RetrieveMediator("CheckpointMediator") as CheckpointMediator;
        }
        //章节
        panel.chapterGrid.enabled = true;
        panel.chapterGrid.BindCustomCallBack(UpdateChapterItem);
        panel.chapterGrid.StartCustom();
       
        panel.chapterGrid.AddCustomDataList(AddListGrid(CloneLevelConfig.CupconfigDic));

        //关卡
        panel.checkGrid.enabled = true;
        panel.checkGrid.BindCustomCallBack(UpdateLevelItem);
        panel.checkGrid.StartCustom();

        SetSlider();

        //关卡掉落预览
        panel.goodsGrid.enabled = true;
        panel.goodsGrid.BindCustomCallBack(UpdateGoodsItem);
        panel.goodsGrid.StartCustom();

        //panel.goodsGrid.AddCustomDataList(AddListGrid(LevelRewardConfig.levelconfigDic));

        //奖杯奖励领取
        panel.cupGrid.enabled = true;
        panel.cupGrid.BindCustomCallBack(UpdateCupItem);
        panel.cupGrid.StartCustom();

        panel.cupGrid.AddCustomDataList(AddListGrid(CloneLevelConfig.CupconfigDic));

        panel.shaoDangGrid.enabled = true;
        panel.shaoDangGrid.BindCustomCallBack(OnUpdate_ShaoDangAwardItem);
        panel.shaoDangGrid.StartCustom();

        if (null != m_data)
        {
            UpdateServerDisplay();
        }
    }

    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.cup_btn1.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.cup_btn2.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.cup_btn3.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.cup_btn4.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.change_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.saodang_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.begin_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.leveloff_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.cupoff_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.of.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.btnAlertAdd.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.btnAlertDiv.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.btnAlertMin.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.btnAlertMax.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.btnAlertNo.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.btnAlertOk.gameObject).onClick = OnClick;
        

        UIEventListener.Get(panel.btnShaoDangClose.gameObject).onClick = OnClick_CloseBtn; 
        UIEventListener.Get(panel.btnAlertClose.gameObject).onClick = OnClick_CloseBtn;
    }

    private void OnClick_CloseBtn(GameObject obj)
    {
        ClosePanel(null);
    }

    private void UpdateServerDisplay()
    {
        int cnt = m_data.Count;
        int cfgCnt = CloneLevelConfig.CupconfigDic.Count;
        UIGridItem uitem;
        if(m_data.Count > 0)
        {
            CloneVO lastVO = m_data[cnt - 1];
            m_selectedItem = lastVO;
            m_Panel.chapterGrid.SetSelect(m_selectedItem.star);
        }
       else
        {
            m_selectedItem = null;
            m_Panel.chapterGrid.SetSelect(0);
        }

        uitem = m_Panel.chapterGrid.mSelectItem.GetComponent<UIGridItem>();
        OnClickChapter(uitem);
    }

    void OpenAward(bool isopen)
    {
        panel.awardinfo.gameObject.SetActive(isopen);

    }
    void OpenLevel(bool isopen)
    {
        panel.checkinfo.gameObject.SetActive(isopen);

        if (isopen)
        {
            panel.targetDes.text = m_selectedLvCfg.targetDes;
            panel.txtSaodangTime.text = string.Format(TextManager.GetUIString("UICloneShaoDangLastTm"), m_saodangNum, m_saodangMaxNum);
            panel.txtShaoDangUse.text = m_sadangUse.ToString();
        }
    }
    private void OnClick(GameObject go)
    {
        if (go == panel.cup_btn1.gameObject || go == panel.cup_btn2.gameObject || go == panel.cup_btn3.gameObject || go == panel.cup_btn4.gameObject)
        {
            isawardpanel = true;
            OpenAward(isawardpanel);
        }
        else if (go == panel.cupoff_btn.gameObject)
        {
            isawardpanel = false;
            OpenAward(isawardpanel);
        }
        else if (go == panel.change_btn.gameObject)
        {
            Facade.SendNotification(NotificationID.TeamFormine_Show);
        }
        else if (go== panel.saodang_btn.gameObject)
        {
            //扫荡入口
        }
        else if (go==panel.begin_btn.gameObject)
        {
            //关卡入口
            GameProxy.Instance.GotoPVE(m_selectedLvCfg.id);
        }
        else if (go == panel.leveloff_btn.gameObject)
        {
            islevelpanel = false;
            OpenLevel(islevelpanel);
        }
        else if (go == panel.cupsure_btn.gameObject)
        {
            //领取奖杯奖励
        }
        else if (go == panel.of.gameObject)
        {
            Facade.SendNotification(NotificationID.CheckPoint_Hide);
        }
        else if (go == panel.btnAlertMax.gameObject)
        {
            m_saodangNum = m_saodangMaxNum;
        }
        else if (go == panel.btnAlertMin.gameObject)
        {
            if (m_saodangMaxNum > 0)
                m_saodangNum = 1;
            else
                m_saodangMaxNum = 0;
        }
        else if (go == panel.btnAlertAdd.gameObject)
        {
            m_saodangNum++;

            if (m_saodangNum > m_saodangMaxNum)
                m_saodangNum = m_saodangMaxNum;
            UpdateShaoDangAlert();
        }
        else if (go == panel.btnAlertDiv.gameObject)
        {
            m_saodangNum--;
            if (m_saodangNum < 1)
                m_saodangNum = 0;

            if (m_saodangMaxNum == 0)
                m_saodangNum = 0;
            else if (m_saodangNum < 1)
                m_saodangNum = 0;

            if (m_saodangMaxNum > 0 && m_saodangNum < 1)
                m_saodangNum = 1;
            UpdateShaoDangAlert();
        }
        else if(go == panel.btnAlertNo)
        {
            panel.panAlert.gameObject.SetActive(false);
            UpdateShaoDangAlert();
        }
        else if (go == panel.btnAlertOk)
        {
            panel.panAlert.gameObject.SetActive(false);
            UpdateShaoDangAlert();
        }
    }
    /// <summary>
    /// 设置关卡星级
    /// </summary>
    private void SetLevelStar(Transform startParent)
    {
        UISprite[] levelstar = UtilTools.GetChilds<UISprite>(startParent, "star");
        UtilTools.SetStar(presentlevelstar, levelstar);
    }
    /// <summary>
    /// 设置奖杯进度条
    /// </summary>
    private void SetSlider()
    {
       // TD_CloneLevel info = CloneLevelConfig.GetLevelRewardInfobyID(1);
       // panel.star_slider.value = presenchaptertstar * 1.0f / info.star4;        
    }

    /// <summary>
    /// 章节按钮
    /// </summary>
    /// <param name="item"></param>
    /// 
    void UpdateChapterItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        TD_CloneLevel info = item.oData as TD_CloneLevel;
        item.onClick = OnClickChapter;
        UISprite chapter_btn = item.mScripts[0] as UISprite;
        UILabel cname = item.mScripts[1] as UILabel;
        UISprite dibian = item.mScripts[2] as UISprite;
        UISprite designate = item.mScripts[3] as UISprite;
        UISprite suo = item.mScripts[4] as UISprite;
        UISprite Sprite = item.mScripts[5] as UISprite;
        cname.text = info.id.ToString();
        //suo.gameObject.SetActive(false);

        bool bOpen = false;
        if (null == m_selectedItem)
        {
            if (item.GetIndex() == 0)
                bOpen = true;
        }
        else
            if (item.GetIndex() <= m_selectedItem.star)
                bOpen = true;

        item.Locked = !bOpen;
        suo.gameObject.SetActive(!bOpen);

        cname.text = string.Format(TextManager.GetUIString("UICloneLv"), info.id, info.name);
    }

    /// <summary>选择 章节</summary>
    private void OnSelect_ChapaterItem(int iSelect, bool active, bool select)
    {
        UIGridItem item = m_Panel.checkGrid.mSelectItem.GetComponent<UIGridItem>();

        TD_CloneLevel lvCfg = null;

        m_Panel.checkGrid.UpdateCustomDataListAll(lvCfg.GetLevels());
        panel.checkGrid.SelectItem += OnSelect_LevelItem;

        if (null == m_selectedItem)
            m_Panel.checkGrid.SetSelect(0);
        else
            m_Panel.checkGrid.SetSelect(m_selectedItem.star);
    }

    /// <summary>选择 管卡</summary>
    private void  OnSelect_LevelItem(int iSelect, bool active, bool select)
    {
        //UIGridItem item = m_Panel.checkGrid.mSelectItem as UIGridItem;
        //Transform suo = m_Panel.checkGrid.mSelectItem.transform.FindChild("suo");
        //suo.gameObject.SetActive(false);
    }

    /// <summary>
    /// 关卡按钮
    /// </summary>
    /// <param name="item"></param>
    void UpdateLevelItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;

        int index = item.GetIndex();
        item.onClick = OnClickLevel;
        UILabel name = item.mScripts[1] as UILabel;
        UISprite suo = item.mScripts[2] as UISprite;
        UITexture star = item.mScripts[4] as UITexture;

        int lvId = (int)item.oData;
        CloneConfig cloneCfg = ProxyInstance.InstanceProxy.Get<CloneConfig>();
        TD_Clone itemCfg = cloneCfg.GetItem(lvId);

        name.text = itemCfg.name;
        bool bOpen;
        if (null != m_selectedItem)
            bOpen = m_selectedItem.star + 1 >= index;
        else
            bOpen = item.GetIndex() == 0;

        item.Locked = !bOpen;
        suo.gameObject.SetActive(!bOpen);

        SetLevelStar(item.transform);
    }


    /// <summary>
    /// 掉落列表预览
    /// </summary>
    /// <param name="item"></param>
    void UpdateGoodsItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        LevelRewardInfo info = item.oData as LevelRewardInfo;
       
        UITexture goods1 = item.mScripts[0] as UITexture;
        UILabel name1 = item.mScripts[1] as UILabel;
        UITexture goods2 = item.mScripts[0] as UITexture;
        UILabel name2 = item.mScripts[1] as UILabel;
        string[] str1 = info.preview1.Split(';');
        for (int i = 0; i < str1.Length; i++)
        {
            str1[i].Split(',');
        }
        name1.text = TextManager.GetItemString(str1[0]);
        name2.text = TextManager.GetItemString(str1[1]);
        LoadSprite.LoaderItem(goods1, str1[0]);
        LoadSprite.LoaderItem(goods2, str1[1]);
    }

    /// <summary>
    /// 奖杯（宝箱）奖励
    /// </summary>
    /// <param name="item"></param>
    void UpdateCupItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        TD_CloneLevel info = item.oData as TD_CloneLevel;
        UITexture goods = item.mScripts[0] as UITexture;
        UILabel name = item.mScripts[1] as UILabel;
        UILabel count = item.mScripts[2] as UILabel;
         
    }

    /// <summary>
    /// 点击章节
    /// </summary>
    /// <param name="item"></param>
    
    
    void OnClickChapter(UIGridItem item)
    {
        chapterinfo = item.oData as TD_CloneLevel;
        TD_CloneLevel lvCfg = chapterinfo;

        bool bOpen = false;
        if (null == m_selectedItem)
        {
            if (item.GetIndex() == 0)
                bOpen = true;
        }
        else
            if (item.GetIndex() <= m_selectedItem.star)
            bOpen = true;

        if (!bOpen)
            return;

        m_Panel.checkGrid.UpdateCustomDataListAll(lvCfg.GetLevels());

        if (null == m_selectedItem)
            m_Panel.checkGrid.SetSelect(0);
        else
            m_Panel.checkGrid.SetSelect(m_selectedItem.star);
    }
    /// <summary>
    /// 点击关卡
    /// </summary>
    /// <param name="item"></param>
    void OnClickLevel(UIGridItem item)
    {
        if (null == m_selectedItem && item.GetIndex() > 0)
            return;
        else if(item.GetIndex() > m_selectedItem.star)
            return;

        int lvId = (int)item.oData;
        CloneConfig cloneCfg = ProxyInstance.InstanceProxy.Get<CloneConfig>();
        TD_Clone itemCfg = cloneCfg.GetItem(lvId);

        m_selectedLvCfg = itemCfg;

        OpenLevel(true);
    }

    List<object> AddListGrid(Dictionary<int,LevelRewardInfo> configlist)
    {

        List<object> listObj = new List<object>();
        foreach (LevelRewardInfo item in configlist.Values)
        {
            listObj.Add(item);
        }
        return listObj;
    }

    List<object> AddListGrid(Dictionary<int, TD_CloneLevel> configlist)
    {
        List<object> listObj = new List<object>();
        foreach (TD_CloneLevel item in configlist.Values)
        {
            listObj.Add(item);
        }
        return listObj;
    }

    #region 扫荡

    /// <summary>更新扫荡提示框</summary>
    private void UpdateShaoDangAlert()
    {
        panel.txtShaDangNum.text = m_saodangNum.ToString();
    }

    /// <summary>更新扫荡显示</summary>
    private void UpdateShaDangDisplay()
    {
        m_shaoDangAwards = new List<ShaoDangAwardVO>();
        List<object> objs = new List<object>();
        int cnt = m_shaoDangAwards.Count;
        for (int i = 0; i < cnt; i++)
            objs.Add(m_shaoDangAwards[i]);

        m_Panel.shaoDangGrid.UpdateCustomDataListAll(objs);
    }

    /// <summary>更新 扫荡奖励项</summary>
    private void OnUpdate_ShaoDangAwardItem(UIGridItem item)
    {
        ShaoDangAwardVO vo = (ShaoDangAwardVO)item.oData;

        UILabel name = item.mScripts[1] as UILabel;
        UITexture icon = item.mScripts[2] as UITexture;

        name.text = vo.itemInfo.itemName;
        LoadSprite.LoaderItem(icon,vo.item.itemID);

        UISprite[] levelstar = UtilTools.GetChilds<UISprite>(item.transform, "star");
        UtilTools.SetStar(vo.item.color, levelstar);
    }
    #endregion
}

