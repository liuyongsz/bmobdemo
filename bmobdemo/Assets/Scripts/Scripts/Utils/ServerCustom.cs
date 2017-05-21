using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

[LuaCallCSharp]
public class ServerCustom
{
    /// <summary>
    /// 设置单例
    /// </summary>
    public static ServerCustom m_instance = null;

    public static ServerCustom instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = Instance.Get<ServerCustom>();
            }
            return m_instance;
        }
    }
    public bool fromOtherBtn = false;
    /// <summary>
    /// 发送自定义消息给服务端
    /// </summary>
    public void SendClientMethods(params object[] args)
    {
        List<object> list = new List<object>();
        for (int i = 0; i < args.Length; ++i)
        {
            list.Add(args[i]);
        }
        KBEngine.Event.fireIn("callServerMethod", list);       
    }
}
