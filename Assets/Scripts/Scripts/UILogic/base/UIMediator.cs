using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;
using XLua;

[LuaCallCSharp]

public class UIMediator<T> :  BaseMediator where T:BasePanel
{
    private bool m_isremoving = false;
    public bool m_isopening = false;   

    /// <summary>
    /// 是否为弹窗类
    /// </summary>
    public bool m_isprop = true;

    /// <summary>
    /// 是否加效果
    /// </summary>
    public bool effect = false;
    /// <summary>
    /// 是否隐藏主界面
    /// </summary>
    public bool m_ismain = false;
    /// <summary>
    /// UI面板
    /// </summary>
    protected T m_Panel;
    /// <summary>
    /// 面板名称
    /// </summary>
    protected string m_PanelName = "";

    private bool bDestory = true;
    public UIMediator() : this(typeof(T).Name)
    {

    }
    public UIMediator(string name)
        : base(name)
    {
        m_mediatorName = this.GetType().ToString();
        m_PanelName = name;
    }

    /// <summary>
    /// 设置层级
    /// </summary>
    public int setDepth { get; set; }


    /// <summary>
    /// 打开面板
    /// </summary>
    /// <param Name="notification"></param>
    protected virtual void OpenPanel(INotification notification)
    {
        if (m_isopening) return;
        m_isopening = true;
        if (!m_isprop)
        {
            for (int i = 0; i < GUIManager.m_panelList.Count; i++)
            {
                if (!m_ismain)
                {
                    if (GUIManager.m_panelList.mList[i] == "mainpanel")
                    {
                        continue;
                    }
                }
                GameObject.DestroyImmediate(GUIManager.m_panelList[GUIManager.m_panelList.mList[i]]);             
                GUIManager.m_panelList.Remove(GUIManager.m_panelList.mList[i]);               
            }
        }     
        OpenPanelUI(notification);
    }
    private void OpenPanelUI(INotification notification)
    {        
        if (m_Panel != null)//如果面板存在，直接显示
        {
            m_Panel.Show();
            SetOrgPos();
            this.OnStart(notification);
            this.OnShow(notification);
            m_isopening = false;
        }
        else//如果不存在，加载面板
        {
            if (!string.IsNullOrEmpty(m_PanelName))
            {               
                ResourceManager.Instance.LoadPrefab(m_PanelName, (name, obj) =>
                { //面板加载成功
                    if (m_isremoving)
                    {
                        GameObject.DestroyImmediate(obj);
                        return;
                    }                                        
                    LoadUIPrefab(obj, notification);
                }, OnPanelResLoadError);
            }
            else
            {
                if (null != notification)
                {
                    Debug.LogError("OpenPanel error " + notification.Name);
                }
                else
                {
                    Debug.LogError("OpenPanel error notification is null");
                }
                m_isopening = false;
            }
        }
    }

    /// <summary>
    /// 加载对象
    /// </summary>
    /// <param name="go"></param>
    /// <param name="notification"></param>
    void LoadUIPrefab(GameObject go, INotification notification)
    {
        go.SetActive(true);
        m_Panel = ScriptHelper.BindField(go.transform, m_PanelName) as T;
        this.OnStart(notification);
        if (effect)
        {
            m_Panel.transform.localScale = Vector3.zero;
        }
        if (setDepth != 0)
        {
            m_Panel.GetComponent<UIPanel>().depth = setDepth;
        }
        Transform tparent = Main.GetUIRoot().FindChild("Camera");
        m_Panel.transform.SetParent(tparent);
        SetOrgPos();

        GameObject panObj;
        if (!GUIManager.m_panelList.TryGetValue(m_PanelName,out panObj) && !m_isprop)
        {
            GUIManager.m_panelList.Add(m_PanelName, go);
        }
        else
        {
            if (!GUIManager.m_panelExitList.ContainsKey(m_PanelName))
                GUIManager.m_panelExitList.Add(m_PanelName, go);
        }

        m_Panel.Show();
        this.AddComponentEvents();
        this.OnShow(notification);
        m_isopening = false;
    }
    /// <summary>
    /// 关闭面板
    /// </summary>
    /// <param Name="notification"></param>
    protected virtual void ClosePanel(INotification notification)
    {
        if (m_Panel != null)
        {
            if (GUIManager.m_panelList.ContainsKey(m_Panel.name.Replace("(Clone)", "")))
            {
                GUIManager.m_panelList.Remove(m_Panel.name.Replace("(Clone)", ""));
            }
            else if (GUIManager.m_panelExitList.ContainsKey(m_Panel.name.Replace("(Clone)", "")))
            {
                GUIManager.m_panelExitList.Remove(m_Panel.name.Replace("(Clone)", ""));
            }
            if (notification == null|| notification.Body == null)
            {
                bDestory = true;
                this.OnDestroy();
            }
            else
            {
                bDestory = false;
                this.OnDestroy();
            }          
        }
        else
        {
			//Debug.LogError("close pane "+m_PanelName + " is null");
        }

    }

    /// <summary>
    /// 面板关闭或隐藏
    /// </summary>
    /// <param Name="bShow"></param>
    protected virtual void OnDestroy()
    {
        if (null != m_Panel)
        {
            m_Panel.Destroy(bDestory);
        }        
    }
    /// <summary>
    /// 界面显示之前调用
    /// </summary>
    protected virtual void OnStart(INotification notification)
    {
       
    }
    /// <summary>
    /// 界面显示
    /// </summary>
    protected virtual void OnShow(INotification notification)
    {
       // 隐藏场景
       // GlobalFunction.SetSceneObjActive(false);
    }
    /// <summary>
    /// 面板加载失败
    /// </summary>
    private void OnPanelResLoadError(string resname)
    {
        m_isopening = false;
    }
    protected virtual void AddComponentEvents()
    {

    }

	private void DisposePanel(INotification notification)
    {
		m_Panel = null;
    }


    public override void OnRemove()
    {
        base.OnRemove();

        if (null!=m_Panel)
        {
            m_isremoving = true;
            GameObject.DestroyImmediate(m_Panel.gameObject);
        }
        m_Panel = null;
      
    }

    private void SetOrgPos()
    {
        if (effect)
        {
            OnComplete();
            iTween.ScaleTo(m_Panel.gameObject, Vector3.one, 0.5f);
        }     
        else
        {
            OnComplete();
            m_Panel.transform.localScale = Vector3.one;
        }  
    }
    private void OnComplete()
    {
        m_Panel.transform.localPosition = Vector3.zero;
        m_Panel.transform.localRotation = Quaternion.Euler(0, 0, 0);
        m_Panel.transform.localScale = Vector3.one;
    }
}
