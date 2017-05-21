using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProxyInstance;

public class ChatProxy : Proxy<ChatProxy>
{

    public ChatProxy()
        : base(ProxyID.Chat)
    {
        KBEngine.Event.registerOut(this, "onWorldChat");
        KBEngine.Event.registerOut(this, "onAdChat");
        KBEngine.Event.registerOut(this, "onPrivateChat");
        KBEngine.Event.registerOut(this, "onChatError");
    }

	private ChatInfo info;
    /// <summary>
    /// 获取世界聊天信息
    /// </summary>
    public void onWorldChat(object obj,Dictionary<string, object> chats)
    {
        info = GetChatList(chats,ChatType.World);
        info.dbid = UtilTools.IntParse(obj.ToString());
        ChatMediator.worldChatsList.Add(info);
        if (GUIManager.HasView("chatpanel"))
        {
            ChatMediator.chatMediator.SendChatInfoToPanel(ChatType.World, ChatMediator.worldChatsList[ChatMediator.worldChatsList.Count - 1]);
        }
    }
    /// <summary>
    /// 获取广告信息
    /// </summary>
    public void onAdChat(object obj,Dictionary<string, object> chats)
    {
        info = GetChatList(chats, ChatType.Advertising);
        info.dbid = UtilTools.IntParse(obj.ToString());
        ChatMediator.adChatsList.Add(info);
    }

    /// <summary>
    /// 获取私聊信息
    /// </summary>
    public void onPrivateChat(object obj,Dictionary<string, object> chats)
    {
        info = GetChatList(chats, ChatType.Private);
        info.dbid = UtilTools.IntParse(obj.ToString());
        ChatMediator.privateChatsList.Add(info);
        if (GUIManager.HasView("chatpanel"))
        {
            ChatMediator.chatMediator.SendChatInfoToPanel(ChatType.Private, ChatMediator.privateChatsList[ChatMediator.privateChatsList.Count - 1]);
        }
    }

    /// <summary>
    /// 获取聊天字典中数据
    /// </summary>
    ChatInfo GetChatList(Dictionary<string, object> Info, ChatType type)
    {
        ChatInfo data = new ChatInfo();
        data.type = type;
        data.message = Info["message"].ToString();
        data.photoIndex = Info["photoIndex"].ToString();
        data.name = Info["name"].ToString();
        data.level = int.Parse(Info["level"].ToString());
        data.vipLevel = int.Parse(Info["vipLevel"].ToString());
        data.sendTime =double.Parse(Info["sendTime"].ToString());
        return data;
    }

    /// <summary>
    /// 聊天错误回调
    /// </summary>
    public void onChatError(object obj)
	{
        int Index = int.Parse(obj.ToString());
        GUIManager.SetPromptInfo(TextManager.GetUIString(UtilTools.StringBuilder("UIChat", Index)), null);	
    }
}
public class ChatInfo
{
	public ChatType type;	   //类型
    public int dbid;           //dbid
    public string message;     //信息
	public string photoIndex;  //头像
	public string name;		   //姓名
	public int level;		   //等级
	public int vipLevel;	   //vip等级
	public double sendTime;	   //发送
}

public class SystemChatInfo
{
    public ChatType type;	   //类型
    public int dbid;           //dbid
    public string message;     //信息
    public double sendTime;	   //发送
}