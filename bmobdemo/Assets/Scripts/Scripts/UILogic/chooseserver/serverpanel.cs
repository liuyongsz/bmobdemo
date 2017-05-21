using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;
using System;

public class serverpanel : BasePanel
{
    public UIButton chooseBtn;
    public UIButton startGameBtn;
    public UIButton backBtn;
    public UIToggle recommendBtn;
    public UIToggle oldAccountBtn;
    public Transform chooseServer;
    public UILabel serverName;
    public UISprite state;
    public UILabel stateText;
    public Transform ServerItem;
    public Transform AccountItem;
    public UIGrid ServerGrid;
}

public class ServerMediator : UIMediator<serverpanel>
{
    private serverpanel panel
    {
        get
        {
            return m_Panel as serverpanel;
        }
    }
    private string user;
    private string pass;
    public static bool loginSucess = false;
    public static ServerMediator serverMediator;
    public const string m_timerLoginKey = "loginqiyougame";
    /// <summary>
    /// 注册界面逻辑
    /// </summary>
    public ServerMediator() : base("serverpanel")
    {
        m_isprop = false;
        RegistPanelCall(NotificationID.Sever_Show, OpenPanel);
        RegistPanelCall(NotificationID.Sever_Hide, ClosePanel);
    }
    /// <summary>
    /// 绑定点击事件
    /// </summary>
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(m_Panel.startGameBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.chooseBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.backBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.recommendBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.oldAccountBtn.gameObject).onClick = OnClick;
    }

    protected override void OnShow(INotification notification)
    {
        if (serverMediator == null)
        {
            serverMediator = Facade.RetrieveMediator("serverMediator") as ServerMediator;
        }
        if (PlayerPrefs.GetString("UserServer") != string.Empty)
        {
            panel.serverName.text = ServerConfig.GetServerNameByIP(PlayerPrefs.GetString("UserServer"));
        }
        panel.ServerGrid.enabled = true;
        panel.ServerGrid.BindCustomCallBack(OnUpdateDataRow);
        panel.ServerGrid.StartCustom();
        user = (notification.Body as List<object>)[0] as string;
        pass = (notification.Body as List<object>)[1] as string;

       // KBEMain.StartClientap(PlayerPrefs.GetString("UserServer"));

    }

    List<object> AddListGrid(Dictionary<int, ServerInfo> list)
    {
        List<object> listObj = new List<object>();
        foreach (ServerInfo item in list.Values)
        {
            listObj.Add(item);
        }
        return listObj;
    }
    /// <summary>
    /// 点击事件
    /// </summary>
    private void OnClick(GameObject go)
    {
        if (go == m_Panel.startGameBtn.gameObject)
        {
            if (panel.serverName.text == string.Empty)
            {
                GUIManager.SetPromptInfo(TextManager.GetUIString("UIServer"), null);
                return;
            }
            if (loginSucess)
                return;
            loginSucess = true;

            KBEMain.StartClientap(PlayerPrefs.GetString("UserServer"));

            TimerManager.AddTimerRepeat(m_timerLoginKey, 0.5f, OnTimer_LoginQiYouGame);
        }
        else if (go == m_Panel.chooseBtn.gameObject)
        {           
            panel.chooseServer.gameObject.SetActive(true);
            panel.ServerGrid.mgridItem = panel.ServerItem.gameObject;
            panel.ServerGrid.AddCustomDataList(AddListGrid(ServerConfig.serverList));           
        }
        else if (go == m_Panel.recommendBtn.gameObject)
        {
            panel.ServerGrid.AddCustomDataList(AddListGrid(ServerConfig.serverList));
        }
        else if (go == m_Panel.oldAccountBtn.gameObject)
        {
            panel.ServerGrid.AddCustomDataList(AddListGrid(ServerConfig.serverList));        
        }
        else if (go == m_Panel.backBtn.gameObject)
        {
            panel.chooseServer.gameObject.SetActive(false);
        }
    }

    private void OnTimer_LoginQiYouGame()
    {
        if (KBEMain.gameapp.YetRun)
        {
            TimerManager.Destroy(m_timerLoginKey);

            LoginProxy.Instance.Send_Login(user, pass);
        }
    }

    void OnUpdateDataRow(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        item.onClick = OnClickServerItem;
        ServerInfo info = item.oData as ServerInfo;
        UILabel Name = item.mScripts[0] as UILabel;
        Name.text = info.Name;
        UISprite State = item.mScripts[1] as UISprite;
        UILabel stateLabel = item.mScripts[2] as UILabel;

    }
    void OnClickServerItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        loginSucess = false;
        ServerInfo info = item.oData as ServerInfo;
        panel.chooseServer.gameObject.SetActive(false);
        string ip = info.ip;
        panel.serverName.text = info.Name;
        PlayerPrefs.SetString("UserServer", ip);
    }
}
