using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using PureMVC.Patterns;
using PureMVC.Interfaces;

public enum FriendType
{
    Friend, // 好友
    Find,   // 推荐
    Apply,  // 申请
    Black,  // 黑名单
}
public class FriendData
{
    public FriendType type;
    public int dbid;
    public string photoIndex;
    public string name;
    public int level;
    public int vipLevel;
    public int fightValue;
    public string clubName;
    public int onlineState; // -1为在线 不在线显示时间戳
}

    public class friendpanel : BasePanel
{
    public Transform findui;
    public Transform Blackui;
    public Transform applyui;
    public Transform selectFriend;
    public UIButton backBtn;
    public UIButton blackBtn;
    public UIToggle toogle_1;
    public UIToggle toogle_2;
    public UIToggle toogle_3;
    public UIButton updateBtn;
    public UIButton findBtn;
    public UIButton allyesBtn;
    public UIButton allnoBtn;
    public UILabel friendsnum;
    public UIGrid FrindeGrid;
    public UISprite selectBtn;  
    public UILabel myId;
    public UIInput friendID;
    public UISprite CloseBtn;
}

public class FriendMediator : UIMediator<friendpanel>
{
    // 好友类型
    private FriendType currentType = FriendType.Friend;

    // 好友在线人数
    public static int onlineNum = 0;

    public static FriendMediator friendMediator;

    // 好友列表
    public static List<FriendData> friendList = new List<FriendData>();

    // 好友申请列表
    public static List<FriendData> applyFriendList = new List<FriendData>();

    // 黑名单列表
    public static List<FriendData> blackFriendList = new List<FriendData>();

    // 好友推荐列表
    public static List<FriendData> recommenFriendList = new List<FriendData>();
    public FriendMediator() : base("friendpanel")
    {
        effect = true;
        m_isprop = false;
        RegistPanelCall(NotificationID.Friend_Show, OpenPanel);
        RegistPanelCall(NotificationID.Friend_Hide, ClosePanel);
    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(m_Panel.backBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.blackBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.toogle_1.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.toogle_2.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.toogle_3.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.updateBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.findBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.allyesBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.allnoBtn.gameObject).onClick = OnClick; 
        UIEventListener.Get(m_Panel.selectBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.CloseBtn.gameObject).onClick = OnClick;
    }
    protected override void OnShow(INotification notification)
    {
        if (friendMediator == null)
        {
            friendMediator = Facade.RetrieveMediator("FriendMediator") as FriendMediator;
        }
        currentType = FriendType.Friend;
        m_Panel.friendsnum.text = UtilTools.StringBuilder(onlineNum, "/", friendList.Count);
        m_Panel.FrindeGrid.enabled = true;
        m_Panel.FrindeGrid.BindCustomCallBack(UpdateFriendGrid);
        m_Panel.FrindeGrid.StartCustom();
        m_Panel.FrindeGrid.AddCustomDataList(AddListGrid(friendList));
        SetBtnState(FriendType.Friend);
    }

    /// <summary>
    /// 刷新界面
    /// </summary>
    /// <param name="type"></param>
    public void AddFriendGridList(FriendType type)
    {
        if (type == FriendType.Friend)
        {
            m_Panel.friendsnum.text = UtilTools.StringBuilder(onlineNum, "/", friendList.Count);
            m_Panel.FrindeGrid.AddCustomDataList(AddListGrid(friendList));           
        }
        if (type == FriendType.Find)
        {
            m_Panel.FrindeGrid.AddCustomDataList(AddListGrid(recommenFriendList));         
        }
        if (type == FriendType.Apply)
        {
            m_Panel.FrindeGrid.AddCustomDataList(AddListGrid(applyFriendList));           
        }
        if (type == FriendType.Black)
        {
            m_Panel.FrindeGrid.AddCustomDataList(AddListGrid(blackFriendList));
        }
    }

    void UpdateFriendGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        FriendData info = item.oData as FriendData;
        UITexture head = item.mScripts[0] as UITexture;
        UILabel friendName = item.mScripts[1] as UILabel;
        UILabel level = item.mScripts[2] as UILabel;
        UILabel fight = item.mScripts[3] as UILabel;
        UILabel club = item.mScripts[4] as UILabel;
        UILabel VIP = item.mScripts[5] as UILabel;
        UIButton addBtn = item.mScripts[6] as UIButton;
        UISprite find = item.mScripts[7] as UISprite;
        UISprite yes = item.mScripts[8] as UISprite;
        UISprite no = item.mScripts[9] as UISprite;
        UISprite black = item.mScripts[10] as UISprite;
        UISprite friends = item.mScripts[11] as UISprite;
        UILabel state = item.mScripts[12] as UILabel;
        UISprite mail = item.mScripts[13] as UISprite;
        UISprite chat = item.mScripts[14] as UISprite;
        UIEventListener.Get(head.gameObject).onClick = OnClickItem;
        UIEventListener.Get(addBtn.gameObject).onClick = OnClickItem;
        UIEventListener.Get(yes.gameObject).onClick = OnClickItem;
        UIEventListener.Get(no.gameObject).onClick = OnClickItem;
        UIEventListener.Get(black.gameObject).onClick = OnClickItem;
        UIEventListener.Get(mail.gameObject).onClick = OnClickItem;
        UIEventListener.Get(chat.gameObject).onClick = OnClickItem;
        friends.gameObject.SetActive(info.type == FriendType.Friend);
        addBtn.gameObject.SetActive(info.type == FriendType.Find);
        find.gameObject.SetActive(info.type == FriendType.Apply);
        black.gameObject.SetActive(info.type == FriendType.Black);
        chat.gameObject.SetActive(info.type == FriendType.Friend && info.onlineState == -1);
        mail.gameObject.SetActive(info.type == FriendType.Friend && info.onlineState != -1);
        if (info.onlineState != -1)
        {
            TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            int cstName = (int)timeSpan.TotalSeconds;
            state.color = Color.gray;
            state.text = string.Format(TextManager.GetUIString("UIFriend2"), UtilTools.formatDuring(cstName - info.onlineState));
        }
        else
        {
            state.color = Color.green;
            state.text = TextManager.GetUIString("UIFriend1");
        }
        friendName.text = info.name;
        level.text = info.level.ToString();
        VIP.text = UtilTools.StringBuilder("VIP", info.vipLevel);
        club.text = info.clubName;
        fight.text = info.fightValue.ToString();
    }
    List<object> AddListGrid(List<FriendData> list)
    {
        List<object> listObj = new List<object>();
        for (int i = 0; i < list.Count; ++i)
        {
            listObj.Add(list[i]);
        }
        return listObj;
    }
    void OnClickItem(GameObject go)
    {
        FriendData info = go.GetComponentsInParent<UIGridItem>()[0].oData as FriendData;
        if (go.name == "add")
        {
            // 申请加好友      
            recommenFriendList.Remove(info);
            m_Panel.FrindeGrid.DeleteCustomData(info,true);
            ServerCustom.instance.SendClientMethods("onClientApplyAddFriend", info.dbid);            
        }
        else if (go.name == "yes")
        {
            // 同意好友
            applyFriendList.Remove(info);
            m_Panel.FrindeGrid.DeleteCustomData(info, true);
            ServerCustom.instance.SendClientMethods("onClientAgreeAddFriend", info.dbid);
        }
        else if (go.name == "no")
        {
            // 拒绝好友
            applyFriendList.Remove(info);
            m_Panel.FrindeGrid.DeleteCustomData(info, true);
            ServerCustom.instance.SendClientMethods("onClientRejectAddFriend",info.dbid);
        }
        else if (go.name == "black")
        {
            // 移除黑名单
            blackFriendList.Remove(info);
            info.type = FriendType.Friend;
            friendList.Add(info);
            m_Panel.FrindeGrid.DeleteCustomData(info, true);
            ServerCustom.instance.SendClientMethods("onClientRemoveFromBlack", info.dbid);
        }
        else if (go.name == "head")
        {
            // 查看好友信息
            ServerCustom.instance.SendClientMethods("onClientGetPlayerInfo", info.dbid);
        }
       
    }
    /// <summary>
    /// 点击事件
    /// </summary>
    void OnClick(GameObject go)
    {
        if (go == m_Panel.backBtn.gameObject)
        {
            ClosePanel(null);
        }
        else if (go == m_Panel.toogle_1.gameObject)
        {
            if (currentType != FriendType.Friend)
            {
                AddFriendGridList(FriendType.Friend);
                SetBtnState(FriendType.Friend);
                currentType = FriendType.Friend;
            }
        }
        else if (go == m_Panel.toogle_2.gameObject)
        {
            if (currentType != FriendType.Find)
            {
                ServerCustom.instance.SendClientMethods("onClientRecommendList");
                SetBtnState(FriendType.Find);
                currentType = FriendType.Find;
            }
        }
        else if (go == m_Panel.toogle_3.gameObject)
        {
            if (currentType != FriendType.Apply)
            {
                ServerCustom.instance.SendClientMethods("onClientGetApplyList");
                SetBtnState(FriendType.Apply);
                currentType = FriendType.Apply;
            }
        }
        else if (go == m_Panel.blackBtn.gameObject)
        {
            if (currentType != FriendType.Black)
            {
                ServerCustom.instance.SendClientMethods("onClientGetBlackList");
                currentType = FriendType.Black;
            }
        }
        else if (go == m_Panel.findBtn.gameObject)
        {
            // 搜索好友            
            m_Panel.selectFriend.gameObject.SetActive(true);
            m_Panel.myId.text = PlayerMediator.playerInfo.roleId.ToString();

        }
        else if (go == m_Panel.updateBtn.gameObject)
        {
            // 刷新推荐列表
            ServerCustom.instance.SendClientMethods("onClientRecommendList");
        }
        else if (go == m_Panel.allyesBtn.gameObject)
        {
            // 全部同意
            applyFriendList.Clear();
            m_Panel.FrindeGrid.ClearCustomGrid();
            ServerCustom.instance.SendClientMethods("onClientAgreeAllAddFriend");
        }
        else if (go == m_Panel.allnoBtn.gameObject)
        {
            // 全部拒绝
            applyFriendList.Clear();
            m_Panel.FrindeGrid.ClearCustomGrid();
            ServerCustom.instance.SendClientMethods("onClientRejectAllAddFriend");
        }
        else if (go == m_Panel.selectBtn.gameObject)
        {
            // 搜素
            if (m_Panel.friendID.value == string.Empty)
            {
                //提示不能为空
                GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_30"), null);
                return;
            }
            char[] value = m_Panel.friendID.value.ToCharArray();
            for (int i = 0; i < value.Length; ++i)
            {
                if (value[i] < 48 || value[i] > 58)
                {
                    GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_33"), null);
                    return;
                }
            }
            int fakeID = int.Parse(m_Panel.friendID.label.text);
            if (fakeID <= 10000000)
            {
                GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_34"), null);
                return;
            }
            fakeID = fakeID - 10000000;
            ServerCustom.instance.SendClientMethods("onClientQueryFriendInfo", (System.UInt64)(fakeID));
        }
        else if (go == m_Panel.CloseBtn.gameObject)
        {
            m_Panel.friendID.value = string.Empty;
            m_Panel.selectFriend.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 设置按钮
    /// </summary>
    void SetBtnState(FriendType type)
    {
        m_Panel.findui.gameObject.SetActive(type== FriendType.Find);
        m_Panel.Blackui.gameObject.SetActive(type == FriendType.Friend);
        m_Panel.applyui.gameObject.SetActive(type == FriendType.Apply);
    }

    /// <summary>
    /// 界面关闭时调用
    /// </summary>
    protected override void OnDestroy()
    {
        recommenFriendList.Clear();
        applyFriendList.Clear();
        blackFriendList.Clear();
        onlineNum = 0;
        base.OnDestroy();
    }
}
