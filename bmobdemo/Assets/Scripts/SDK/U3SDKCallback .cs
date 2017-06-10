using UnityEngine;
using System.Collections.Generic;
public class U3SDKCallback : MonoBehaviour
{
 
    private static U3SDKCallback _instance;
 
    private static object _lock = new object();
 
    //初始化回调对象
    public static U3SDKCallback InitCallback()
    {
        UnityEngine.Debug.LogError("Callback->InitCallback");
 
        lock (_lock)
        {
            if (_instance == null)
            {
                GameObject callback = GameObject.Find("(u3sdk_callback)");
                if (callback == null)
                {
                    callback = new GameObject("(u3sdk_callback)");
                    UnityEngine.Object.DontDestroyOnLoad(_instance);
                    _instance = callback.AddComponent<U3SDKCallback>();
 
                }
                else
                {
                    _instance = callback.GetComponent<U3SDKCallback>();
                }
            }
 
            return _instance;
        }
    }
 
 
    //初始化成功回调
    public void OnInitSuc()
    {
        //一般不需要处理
        UnityEngine.Debug.LogError("Callback->OnInitSuc");
    }
 
    //登录成功回调
    public void OnLoginSuc(string jsonData)
    {
        UnityEngine.Debug.LogError("Callback->OnLoginSuc");
 
        U3LoginResult data = parseLoginResult(jsonData);
        if (data == null)
        {
            UnityEngine.Debug.LogError("The data parse error." + jsonData);
            return;
        }
 
        if (U3SDKInterface.Instance.OnLoginSuc != null)
        {
            U3SDKInterface.Instance.OnLoginSuc.Invoke(data);
        }
    }
 
    //切换帐号回调
    public void OnSwitchLogin()
    {
 
        UnityEngine.Debug.LogError("Callback->OnSwitchLogin");
 
        if (U3SDKInterface.Instance.OnLogout != null)
        {
            U3SDKInterface.Instance.OnLogout.Invoke();
        }
    }
 
    //登出回调
    public void OnLogout()
    {
        UnityEngine.Debug.LogError("Callback->OnLogout");
 
        if (U3SDKInterface.Instance.OnLogout != null)
        {
            U3SDKInterface.Instance.OnLogout.Invoke();
        }
    }
 
    //支付回调，网游不需要实现该接口，该接口用于单机游戏
    public void OnPaySuc(string jsonData)
    {
        //Nothing...
    }
 
    private U3LoginResult parseLoginResult(string str)
    {
        object jsonParsed = JsonUtility.FromJson<object>(str);
        if (jsonParsed != null)
        {
            Dictionary<string, object> jsonMap = jsonParsed as Dictionary<string, object>;
            U3LoginResult data = new U3LoginResult();
            if (jsonMap.ContainsKey("isSuc"))
            {
                data.isSuc = bool.Parse(jsonMap["isSuc"].ToString());
            }
            if (jsonMap.ContainsKey("isSwitchAccount"))
            {
                data.isSwitchAccount = bool.Parse(jsonMap["isSwitchAccount"].ToString());
            }
            if (jsonMap.ContainsKey("userID"))
            {
                data.userID = jsonMap["userID"].ToString();
            }
            if (jsonMap.ContainsKey("sdkUserID"))
            {
                data.sdkUserID = jsonMap["sdkUserID"].ToString();
 
            }
            if (jsonMap.ContainsKey("username"))
            {
                data.username = jsonMap["username"].ToString();
            }
 
            if (jsonMap.ContainsKey("sdkUsername"))
            {
                data.sdkUsername = jsonMap["sdkUsername"].ToString();
            }
            if (jsonMap.ContainsKey("token"))
            {
                data.token = jsonMap["token"].ToString();
            }
 
            return data;
        }
 
        return null;
    }
}