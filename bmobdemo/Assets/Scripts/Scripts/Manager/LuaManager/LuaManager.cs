using UnityEngine;
using System.Collections;
using XLua;
using System.IO;
using System.Text;
using System;
using AssetBundles;
public class LuaManager :  ManagerTemplate<LuaManager>
{
    private static LuaEnv luaenv = null;
    
    private Action luaStart;
    public static bool isDispose = false;

    protected override void InitManager()
    {
#if THREAD_SAFT || HOTFIX_ENABLE
        luaenv = new LuaEnv();
        luaenv.AddLoader((ref string filename) =>
        {
            return LuaFileUtils.Instance.ReadFile(filename);
        });

        Add3dPartyDll();
        LoadLuaFiles();
#endif
    }

    private void LoadLuaFiles()
    {
        if (!LuaConst.beBundle)
        {
            LoadLuaCodes();
            StartLuaEnv();
        }
        else
        {
            LoadLuaBundles();
        }
    }

    private void LoadLuaCodes()
    {
        AddSearchPath(LuaConst.luaDir);
    }

    private void LoadLuaBundles()
    {
        AddSearchBundle("lua.bundle");
    }

    private void StartLuaEnv()
    {
        luaenv.DoString("require 'main'");
        luaStart = luaenv.Global.Get<Action>("mainStart");
        if (luaStart != null)
            luaStart();
    }

    public void AddSearchPath(string fullPath)
    {
        if (!Path.IsPathRooted(fullPath))
        {
            throw new LuaException(fullPath + " is not a full path");
        }

        fullPath = ToPackagePath(fullPath);
        LuaFileUtils.Instance.AddSearchPath(fullPath);
    }

    public void AddSearchBundle(string bundleName)
    {
        ResourceManager.Instance.LoadAB(bundleName, OnLoad_Complete,null);
    }

    private void OnLoad_Complete(AssetBundle ab)
    {
        AssetBundle bundle = ab;
        string name = ab.name.Replace(".bundle", "");

        //string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName("lua.bundle", "main");
        LuaFileUtils.Instance.AddSearchBundle(name.ToLower(), bundle);

        StartLuaEnv();
    }

    public void Add3dPartyDll()
    {
        //luaenv.AddBuildin("rapidjson", XLua.LuaDLL.Lua.LoadRapidJson);
    }

    string ToPackagePath(string path)
    {
        StringBuilder sb = StringBuilderCache.Acquire();
        sb.Append(path);
        sb.Replace('\\', '/');

        if (sb.Length > 0 && sb[sb.Length - 1] != '/')
        {
            sb.Append('/');
        }

        sb.Append("?.lua");
        return StringBuilderCache.GetStringAndRelease(sb);
    }

    public static LuaEnv luaEnv
    {
        get { return luaenv; }
    }

    void Update()
    {
        if (luaenv != null)
        {
            luaenv.Tick();
        }
    }

    protected override void OnDestroy()
    {
        luaStart = null;
        if(luaenv != null)
            luaenv.Dispose();
        isDispose = true;
        LuaFileUtils.Instance.Dispose();

        base.OnDestroy();
    }
}
