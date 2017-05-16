using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProxyInstance;
using System;

public class FriendProxy : Proxy<FriendProxy>
{
    public FriendProxy()
        : base(ProxyID.Friend)
    {
        KBEngine.Event.registerOut(this, "onGetFriendInfo");
        KBEngine.Event.registerOut(this, "onGetApplyInfo");
        KBEngine.Event.registerOut(this, "onGetBlackInfo");
        KBEngine.Event.registerOut(this, "onGetRecommendInfo");
        KBEngine.Event.registerOut(this, "onFriendError");
        KBEngine.Event.registerOut(this, "onGetQueryInfo");
    }

	private FriendData info;

    /// <summary>
    /// 获取好友列表
    /// </summary>
    public void onGetFriendInfo(Dictionary<string, object> friends)
    {
        FriendMediator.onlineNum = 0;
        FriendMediator.friendList.Clear();
        List<object> friendsList = friends["values"] as List<object>;
        for (int i = 0; i < friendsList.Count; ++i)
        {
            info = GetfriendList(friendsList, i, FriendType.Friend);
            if (info.onlineState == -1)
            {
                FriendMediator.onlineNum += 1;
            }
            FriendMediator.friendList.Add(info);
        }       
    }
    /// <summary>
    /// 获取申请列表
    /// </summary>
    public void onGetApplyInfo(Dictionary<string, object> friends)
    {
        List<object> friendsList = friends["values"] as List<object>;      
        for (int i = 0; i < friendsList.Count; ++i)
        {
			info = GetfriendList (friendsList, i, FriendType.Apply);    
			FriendMediator.applyFriendList.Add(info);
        }
        if (FriendMediator.friendMediator == null)
            return;
        FriendMediator.friendMediator.AddFriendGridList(FriendType.Apply);
    }
    /// <summary>
    /// 获取黑名单列表
    /// </summary>
    public void onGetBlackInfo(Dictionary<string, object> friends)
    {
        List<object> friendsList = friends["values"] as List<object>;
        for (int i = 0; i < friendsList.Count; ++i)
        {
			info = GetfriendList (friendsList, i, FriendType.Black);    
			FriendMediator.blackFriendList.Add(info);
        }
        if (FriendMediator.friendMediator == null)
            return;
        FriendMediator.friendMediator.AddFriendGridList(FriendType.Black);
    }
    /// <summary>
    /// 获取推荐列表
    /// </summary>
    public void onGetRecommendInfo(Dictionary<string, object> friends)
    {
        FriendMediator.recommenFriendList.Clear();
        List<object> friendsList = friends["values"] as List<object>;
        for (int i = 0; i < friendsList.Count; ++i)
        {
			info = GetfriendList (friendsList, i, FriendType.Find);    
			FriendMediator.recommenFriendList.Add(info);
        }
        if (FriendMediator.friendMediator == null)
            return;
        FriendMediator.friendMediator.AddFriendGridList(FriendType.Find);
    }
	/// <summary>
	/// 获取好友字典中数据
	/// </summary>
	FriendData GetfriendList(List<object> friendsList,int i,FriendType type)
	{
		FriendData data = new FriendData ();
		Dictionary<string, object> Info = friendsList[i] as Dictionary<string, object>;
		data.type = type;
		data.dbid = int.Parse(Info["dbid"].ToString());
		data.photoIndex = Info["photoIndex"].ToString();
		data.name = Info["name"].ToString();
		data.level = int.Parse(Info["level"].ToString());
		data.vipLevel = int.Parse(Info["vipLevel"].ToString());
		data.fightValue = int.Parse(Info["fightValue"].ToString());
		data.clubName = Info["clubName"].ToString();
		data.onlineState = int.Parse(Info["onlineState"].ToString());
		return data;
	}

    /// <summary>
    /// 好友消息回调
    /// </summary>
    public void onGetQueryInfo(object obj)
    {
        if (obj==null)
        {
        }
    }
    public void onFriendError(object obj, object data)
    {
        int Index = int.Parse(obj.ToString());
        GUIManager.SetPromptInfo(TextManager.GetUIString(UtilTools.StringBuilder("UIFriend", Index + 2)),null);
    }

}
