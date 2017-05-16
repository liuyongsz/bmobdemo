using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;
using System;

public class chatpanel : BasePanel
{
	public UIToggle worldBtn;
	public UIToggle guildBtn;
	public UIToggle privateBtn;
    public UIButton advertiseBtn;
    public UIButton sendBtn;
    public UIButton expressionBtn;
    public UIButton chooseFreindBtn;
    public UIButton sendAdBtn;
    public UIButton closeAdBtn;
    public UIButton backBtn;
    public UILabel chooseFriendName;
    public UILabel chooseFreindLabel;
    public UIInput message;
    public UIInput adContent;
    public ChatGrid SystemGrid;
    public Transform ChatContent;
    public Transform ChatItem;
    public ChatGrid ChatGrid;
    public UIGrid FriendGrid;
    public Transform mask;
    public Transform noFriend;
    public Transform sendAd;
    public Transform AdItem;
    public Transform LeftChatItem;
    public Transform RightChatItem;
    public Transform SystemItem;
}

public enum ChatType
{
	World,		//世界
	club,		//公会
	Advertising,//广告
	Private,	//私聊
    SystemAd,	//系统广播
}

public class ChatMediator : UIMediator<chatpanel>
{

    private chatpanel panel
    {
        get
        {
            return m_Panel as chatpanel;
        }
    }
    private GameObject adObj;
    private static int adSeconds = 0;
    private int privateDbid = 0;
    private string privateName = string.Empty;
    private bool isChooseFreind = true;
    private List<object> listObj = new List<object>();
    private FriendData friendData;
    private ChatType currentChatType = ChatType.World;
    //聊天记录最大缓存
    public const int MAXCACHE = 30;

    public static ChatMediator chatMediator;

    public static List<ChatInfo> worldChatsList = new List<ChatInfo>();

    public static List<ChatInfo> clubChatsList = new List<ChatInfo>();

    public static List<ChatInfo> privateChatsList = new List<ChatInfo>();

    public static List<ChatInfo> adChatsList = new List<ChatInfo>();
    //聊天prefabs缓存区
    private List<ChatItem> mPrefabsList = new List<ChatItem>();

    //聊天prefabs缓存区
    private List<SystemChatItem> mSysPrefabsList = new List<SystemChatItem>();
    public ChatMediator() : base("chatpanel")
    {  
        m_isprop = false;
        RegistPanelCall(NotificationID.Chat_Show, OpenPanel);
        RegistPanelCall(NotificationID.Chat_Hide, ClosePanel);
    }

    /// <summary>
    /// 界面显示
    /// </summary>
    /// <param name="notification"></param>
    protected override void OnShow(INotification notification)
    {
        if (notification.Body != null)
        {
            List<object> list = notification.Body as List<object>;
            privateDbid = (int)list[0];
            privateName = list[1] as string;
            panel.chooseFreindLabel.text = privateName;
            currentChatType = ChatType.Private;
            ShowInfos(ChatType.Private);
            panel.privateBtn.value = true;
            return;
        }
        panel.message.value = string.Empty;
        panel.message.label.text = TextManager.GetUIString("UIChat8");
        GameObject cell = panel.LeftChatItem.gameObject;
        cell.AddComponent<LeftChatItem>();
        PoolManager.CreatePrefabPools(PoolManager.PoolKey.Prefab_LeftChatItem, cell, false);

        cell = panel.RightChatItem.gameObject;
        cell.AddComponent<RightChatItem>();
        PoolManager.CreatePrefabPools(PoolManager.PoolKey.Prefab_RightChatItem, cell, false);

        cell = panel.SystemItem.gameObject;
        cell.AddComponent<SystemChatItem>();
        PoolManager.CreatePrefabPools(PoolManager.PoolKey.Prefab_SystemChatItem, cell, false);

        Facade.SendNotification(NotificationID.Gold_Hide);
        if (chatMediator == null)
        {
            chatMediator = Facade.RetrieveMediator("ChatMediator") as ChatMediator;
        }
        ShowInfos(ChatType.World);
        TimerManager.AddTimerRepeat("showAd", 0.1f, UpdateAdInfo);
    }
    /// <summary>
    /// 界面关闭
    /// </summary>
    protected override void OnDestroy()
    {
        privateDbid = 0;
        privateName = string.Empty;
        Facade.SendNotification(NotificationID.Gold_Show);
        TimerManager.Destroy("showAd");
        base.OnDestroy();
    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.worldBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.guildBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.privateBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.advertiseBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.sendBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.expressionBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.chooseFreindBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.sendAdBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.closeAdBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.backBtn.gameObject).onClick = OnClick;
    }

    private void OnClick(GameObject go)
    {
        if (go == panel.worldBtn.gameObject)
        {
            if (currentChatType != ChatType.World)
            {
                if (!isChooseFreind)
                    MoveChooseFriend();
                panel.chooseFreindBtn.gameObject.SetActive(false);
                ShowInfos(ChatType.World);
                currentChatType = ChatType.World;
            }
        }
        else if (go == panel.privateBtn.gameObject)
        {
            if (currentChatType != ChatType.Private)
            {
                panel.chooseFreindBtn.gameObject.SetActive(true);
                ShowInfos(ChatType.Private);
                currentChatType = ChatType.Private;
            }
        }
        else if (go == panel.advertiseBtn.gameObject)
        {       
            panel.adContent.value = string.Empty;
            panel.sendAd.gameObject.SetActive(true);
        }
        else if (go == panel.guildBtn.gameObject)
        {
            if (currentChatType != ChatType.club)
            {
                if (!isChooseFreind)
                    MoveChooseFriend();
                panel.chooseFreindBtn.gameObject.SetActive(false);
                ShowInfos(ChatType.club);
                currentChatType = ChatType.club;
            }
        }
        // 发消息
        else if (go == panel.sendBtn.gameObject)
        {
            if (panel.message.value == string.Empty)
            {
                GUIManager.SetPromptInfo(TextManager.GetUIString(UtilTools.StringBuilder("UIChat")), null);
                return;
            }
            if (currentChatType == ChatType.World && PlayerMediator.playerInfo.level < 20)
            {
                GUIManager.SetPromptInfo(TextManager.GetUIString("UIChat7"), null);
                return;
            }
            ChatInfo info = new ChatInfo();
            info.type = currentChatType;
            info.name = PlayerMediator.playerInfo.name;
            info.photoIndex = "1";
            info.message = panel.message.value;
            info.level = PlayerMediator.playerInfo.level;
            info.vipLevel = PlayerMediator.playerInfo.vipLevel;
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            info.sendTime = ts.TotalSeconds;
            switch (currentChatType)
            {
                case ChatType.World:
                    ServerCustom.instance.SendClientMethods("onClientWorldChat", panel.message.value);
                    break;
                case ChatType.Private:            
                    if (privateDbid == 0)
                    {
                        GUIManager.SetPromptInfo(TextManager.GetUIString("UIEmail004"), null);
                        return;
                    }
                    ServerCustom.instance.SendClientMethods("onClientPrivate", privateDbid, panel.message.value);               
                    break;
                case ChatType.club:
                    ServerCustom.instance.SendClientMethods("onClientClubChat", panel.message.value);
                    break;
            }
            panel.message.value = string.Empty;
            panel.message.label.text = TextManager.GetUIString("UIChat8");
        }
        else if (go == panel.sendAdBtn.gameObject)
        {
            if (panel.adContent.value == string.Empty)
            {
                GUIManager.SetPromptInfo(TextManager.GetUIString(UtilTools.StringBuilder("UIChat")), null);
                return;
            }
            ServerCustom.instance.SendClientMethods("onClientAdChat", panel.adContent.value);
            panel.sendAd.gameObject.SetActive(false);
        }
        else if (go == panel.closeAdBtn.gameObject)
        {
            panel.sendAd.gameObject.SetActive(false);
        }
        else if (go == panel.chooseFreindBtn.gameObject)
        {
            MoveChooseFriend();
        }
        else if (go == panel.backBtn.gameObject)
        {
            Facade.SendNotification(NotificationID.Chat_Hide);
        }
    }
    void MoveChooseFriend()
    {
        List<object> list = new List<object>();
        for (int i = 0; i < FriendMediator.friendList.Count; ++i)
        {
            if (FriendMediator.friendList[i].onlineState == -1)
                list.Add(FriendMediator.friendList[i]);
        }
        if (isChooseFreind)
        {
            if (list.Count > 0)
            {
                m_Panel.FriendGrid.enabled = true;
                m_Panel.FriendGrid.BindCustomCallBack(UpdateFriend);
                m_Panel.FriendGrid.StartCustom();                     
            }
            TweenPosition.Begin(panel.mask.gameObject, 0.2f, new Vector3(-106, -165, 0));
            isChooseFreind = false;
        }
        else
        {
            TweenPosition.Begin(panel.mask.gameObject, 0.2f, new Vector3(-106, -480, 0));          
            isChooseFreind = true;
        }
    }

    void UpdateFriend(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        FriendData info = item.oData as FriendData;
    }

    /// <summary>
    /// 发送聊天消息
    /// </summary>
    /// <param name="type"></param>
    /// <param name="message"></param>
    public void SendChatInfoToPanel(ChatType type, ChatInfo info)
    {
        Show(info);
        panel.ChatGrid.Reposition();
    }
    public void SendSystemChatInfoToPanel(ChatType type, SystemChatInfo info)
    {
        SystemChatInfo data = new SystemChatInfo();
        SystemShow(data);
        panel.SystemGrid.Reposition();
    }
    
    /// <summary>
    /// 查看玩家信息
    /// </summary>
    private void OnClickChatItem(GameObject go)
    {
        ChatItem item = go.transform.parent.GetComponent<ChatItem>();
        if (item != null)
        {
            ChatInfo info = item.Data;
            ServerCustom.instance.SendClientMethods("onClientGetPlayerInfo", info.dbid);
        }
    }

    /// <summary>
    /// 查看系统提示
    /// </summary>
    private void OnClickSystemItem(GameObject go)
    {

    }

    /// <summary>
    /// 选择私聊好友
    /// </summary>
    void ChooseFriend(object data, GameObject go)
    {
        friendData = data as FriendData;
        panel.chooseFriendName.text = friendData.name;
        if (currentChatType != ChatType.Private)
        {
            panel.privateBtn.value = true;
            ShowInfos(ChatType.Private);
            currentChatType = ChatType.Private;
        }
        privateDbid = friendData.dbid;
        MoveChooseFriend();
    }
    /// <summary>
    /// 加载广告信息
    /// </summary>
    public void UpdateAdInfo()
    {
        if (adObj == null)
        {
            if (adChatsList.Count > 0)
            {
                CreatAdItem();
            }
            adSeconds = 0;
            return;
        }
        adSeconds++;
        if (adObj != null)
        {
            if (adSeconds >= 100)
            {                           
                if (adChatsList.Count > 0)
                {
                    adSeconds = 0;
                    MonoBehaviour.Destroy(adObj);
                    adObj = null;
                    CreatAdItem();
                }
            }
        }
    }
    void CreatAdItem()
    {         
        ChatInfo info = adChatsList[0];
        adChatsList.RemoveAt(0);
        adObj = GameObject.Instantiate(panel.AdItem).gameObject;
        adObj.transform.SetParent(panel.transform);
        adObj.transform.localPosition = panel.AdItem.localPosition;
        adObj.transform.localRotation = panel.AdItem.localRotation;
        adObj.transform.localScale = panel.AdItem.localScale;
        adObj.SetActive(true);
        adObj.transform.FindChild("content").GetComponent<UILabel>().text = info.message;
        adObj.transform.FindChild("time").GetComponent<UILabel>().text = UtilTools.GetDataTime();
        TimerManager.Destroy("deleAd");
        TimerManager.AddTimer("deleAd", 20, DestoryAdItem);
    }
    void  DestoryAdItem()
    {
        if (adObj != null)
        {
            MonoBehaviour.Destroy(adObj);
            adObj = null;
        }
    }
    private void ShowInfos(ChatType type)
    {
        Reset();
        List<ChatInfo> list = new List<ChatInfo>();
        if (type == ChatType.World)
        {
            list = worldChatsList;
        }
        else if (type == ChatType.club)
        {
            list = clubChatsList;
        }
        else if (type == ChatType.Private)
        {
            list = privateChatsList;
        }
        foreach (ChatInfo item in list)
        {
            Show(item);
            panel.ChatGrid.Reposition();
        }
    }
    /// <summary>
    /// 显示数据
    /// </summary>
    /// <param name="info"></param>
    private void Show(ChatInfo info)
    {
        ChatItem item = GetChatItem(info);
        item.Data = info;
    }
    private void SystemShow(SystemChatInfo info)
    {
        SystemChatItem item = GetSystemItem(info);
        item.data = info;
    }
    private ChatItem GetChatItem(ChatInfo info)
    {
        ChatItem item;
        GameObject go;
        if (mPrefabsList.Count >= MAXCACHE)
        {
            item = mPrefabsList[0];
            mPrefabsList.Remove(item);
            if (item.GetComponent<RightChatItem>() != null)
            {
                PoolManager.PushPrefab(PoolManager.PoolKey.Prefab_RightChatItem, item.gameObject);
            }
            else if (item.GetComponent<LeftChatItem>() != null)
            {
                PoolManager.PushPrefab(PoolManager.PoolKey.Prefab_LeftChatItem, item.gameObject);
            }
        }
        if (info.name == PlayerMediator.playerInfo.name)
        {
            go = GameObject.Instantiate(PoolManager.PopPrefab(PoolManager.PoolKey.Prefab_RightChatItem));
            Vector3 vTemp = Vector3.zero;
            go.transform.localPosition = vTemp;
            item = go.GetComponent<RightChatItem>();
        }
        else
        {
            go = GameObject.Instantiate(PoolManager.PopPrefab(PoolManager.PoolKey.Prefab_LeftChatItem));
            go.transform.localPosition = Vector3.zero;
            item = go.GetComponent<LeftChatItem>();
        }
        go.transform.parent = panel.ChatGrid.transform;
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.gameObject.SetActive(true);
        UIEventListener.Get(item.head.gameObject).onClick = OnClickChatItem;
        mPrefabsList.Add(item);
        return item;
    }
    private SystemChatItem GetSystemItem(SystemChatInfo info)
    {
        SystemChatItem item;
        GameObject go;
        if (mSysPrefabsList.Count >= MAXCACHE)
        {
            item = mSysPrefabsList[0];
            mSysPrefabsList.Remove(item);
            if (item.GetComponent<SystemChatItem>() != null)
            {
                PoolManager.PushPrefab(PoolManager.PoolKey.Prefab_SystemChatItem, item.gameObject);
            }
        }
        go = GameObject.Instantiate(PoolManager.PopPrefab(PoolManager.PoolKey.Prefab_SystemChatItem));
        go.transform.localPosition = Vector3.zero;
        item = go.GetComponent<SystemChatItem>();
        go.transform.parent = panel.SystemGrid.transform;
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.gameObject.SetActive(true);
        mSysPrefabsList.Add(item);
        return item;
    }
    /// <summary>
    /// 重置聊天列表
    /// </summary>
    private void Reset()
    {
        ChatItem item;

        DestoryTempObject();

        while (mPrefabsList.Count > 0)
        {
            item = mPrefabsList[0];

            mPrefabsList.Remove(item);
            if (item != null)
            {
                if (item.GetComponent<RightChatItem>() != null)
                {
                    PoolManager.PushPrefab(PoolManager.PoolKey.Prefab_RightChatItem, item.gameObject);
                }
                else if (item.GetComponent<LeftChatItem>() != null)
                {
                    PoolManager.PushPrefab(PoolManager.PoolKey.Prefab_LeftChatItem, item.gameObject);
                }
            }
        }

        SystemChatItem items;

        while (mSysPrefabsList.Count > 0)
        {
            items = mSysPrefabsList[0];

            mSysPrefabsList.Remove(items);
            if (items != null)
            {
                if (items.GetComponent<SystemChatItem>() != null)
                {
                    PoolManager.PushPrefab(PoolManager.PoolKey.Prefab_SystemChatItem, items.gameObject);
                }
            }
        }
    }
    /// <summary>
    /// 清除多余对象
    /// </summary>
    private void DestoryTempObject()
    {
        ChatItem item;
        while (mPrefabsList.Count > MAXCACHE)
        {
            item = mPrefabsList[0];
            mPrefabsList.Remove(item);

            if (item != null)
                GameObject.Destroy(item.gameObject);
        }

        SystemChatItem items;
        while (mSysPrefabsList.Count > MAXCACHE)
        {
            items = mSysPrefabsList[0];
            mSysPrefabsList.Remove(items);

            if (items != null)
                GameObject.Destroy(items.gameObject);
        }
    }
    List<object> AddListGrid(List<ChatInfo> list)
    {
        if (listObj.Count!=0)
        {
            listObj.Clear();
        }
        for (int i = 0; i < list.Count; ++i)
        {
            listObj.Add(list[i]);
        }
        return listObj;
    }
}
