using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;

public class promptpanel : BasePanel
{
    public Transform tishiUI;
    public Transform helpUI;
    public Transform propertiesInfo;
    public UISprite box;
    public UISprite ok;
    public UISprite no;
    public UILabel content;
    public UILabel helpText;
    public UISprite closeBtn;
}
public class PromptInfo
{
    public string tishi = string.Empty;
    public string content = string.Empty;
    public PromptType type = PromptType.Choose;
    public PromptMediator.ClickOk clickOk;
    public PromptMediator.Close close;
    public List<object> objList;
    public Dictionary<string, PropertiesInfo> propertiesInfoList = new Dictionary<string, PropertiesInfo>();
}
public enum PromptType
{
    Tishi,
    Choose,
    Help,
    Properties,
}

public class PromptMediator : UIMediator<promptpanel>
{
    //  确定点击回调
    public delegate void ClickOk();
    public ClickOk clickOk;
    private UIGrid infoGrid;
    private UISprite closeBtn;
    public delegate void Close();
    public Close close;

    /// <summary>
    /// 注册界面逻辑
    /// </summary>
    public PromptMediator() : base("promptpanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.Prompt_Show, OpenPanel);
        RegistPanelCall(NotificationID.Prompt_Hide, ClosePanel);
    }
    
    /// <summary>
    /// 绑定点击事件
    /// </summary>
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(m_Panel.ok.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.no.gameObject).onClick = OnClick; 
        UIEventListener.Get(m_Panel.box.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.closeBtn.gameObject).onClick = OnClick;
    }
    /// <summary>
    /// 界面显示时调用
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        PromptInfo info = notification.Body as PromptInfo;
        if (info.type == PromptType.Choose)
        {
            m_Panel.tishiUI.gameObject.SetActive(true);
            m_Panel.ok.gameObject.SetActive(true);
            m_Panel.no.gameObject.SetActive(true);
            m_Panel.content.text = info.content;
            clickOk = info.clickOk;
        }
        else if (info.type == PromptType.Tishi)
        {
            m_Panel.tishiUI.gameObject.SetActive(true);
            m_Panel.ok.gameObject.SetActive(false);
            m_Panel.no.gameObject.SetActive(false);
            m_Panel.content.text = info.content;
            close = info.close;
        }
        else if (info.type == PromptType.Help)
        {
            m_Panel.tishiUI.gameObject.SetActive(false);
            m_Panel.helpUI.gameObject.SetActive(true);
            m_Panel.helpText.text = string.Empty;
            if (info.objList.Count > 1)
            {
                List<object> list = info.objList;
                for (int i = 0; i < list.Count; ++i)
                {
                    if (i > 0)
                    {
                        m_Panel.helpText.text += "\n\n" + (list[i] as string);
                        continue;
                    }
                    m_Panel.helpText.text += (list[i] as string);
                }
            }
            else
            {
                m_Panel.helpUI.FindChild("ScrollView").GetComponent<UIScrollView>().gameObject.SetActive(false);
                m_Panel.helpUI.FindChild("biaoti/di").GetComponent<UISprite>().SetDimensions(360, 236);
                m_Panel.helpUI.FindChild("biaoti/di").GetComponent<UISprite>().transform.localPosition = new Vector3(-163, 124, 0);
                m_Panel.helpUI.FindChild("Label").GetComponent<UILabel>().text = info.objList[0] as string;
                m_Panel.helpUI.transform.localPosition = new Vector3(0, -103, 0);
            }
        }
        else if (info.type == PromptType.Properties)
        {
            m_Panel.tishiUI.gameObject.SetActive(false);
            m_Panel.propertiesInfo.gameObject.SetActive(true);
            infoGrid = UtilTools.GetChild<UIGrid>(m_Panel.propertiesInfo.transform, "ScrollView/infoGrid");
            closeBtn = UtilTools.GetChild<UISprite>(m_Panel.propertiesInfo.transform, "biaoti/closeBtn");
            UIEventListener.Get(closeBtn.gameObject).onClick = OnClick;
            infoGrid.enabled = true;
            infoGrid.BindCustomCallBack(UpdatePropertiesInfoGrid);
            infoGrid.StartCustom();
            List<object> list = new List<object>();
            foreach (PropertiesInfo item in info.propertiesInfoList.Values)
                list.Add(item);
            infoGrid.AddCustomDataList(list);
        }
        else
        {
            LogSystem.LogError("----------------promptpanel------info----error");
            ClosePanel(null);
        }
    }
    void UpdatePropertiesInfoGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        PropertiesInfo info = item.oData as PropertiesInfo;
        UILabel Name = item.mScripts[0] as UILabel;
        UILabel value = item.mScripts[1] as UILabel;
        Name.text = TextManager.GetUIString(info.propertiesName);
        value.text = "+" + info.propertiesValue.ToString();
    }
    /// <summary>
    /// 界面关闭时调用
    /// </summary>
    protected override void OnDestroy()
    {
        m_Panel.tishiUI = null;
        m_Panel.helpUI = null;
        m_Panel.propertiesInfo = null;
        m_Panel.box = null;
        m_Panel.ok = null;
        m_Panel.no = null;
        m_Panel.content = null;
        m_Panel.helpText = null;
        m_Panel.closeBtn = null;
        base.OnDestroy();
    }
    void OnClick(GameObject go)
    {
        if (go == m_Panel.ok.gameObject && clickOk != null)
        {          
            clickOk();         
        }
        else if (go == m_Panel.box.gameObject)
        {
            if (close != null)
            {
                close();
            }
        }
        else if (go == m_Panel.closeBtn.gameObject)
        {
            if (close != null)
            {
                close();
            }
        }
        ClosePanel(null);
    }
}