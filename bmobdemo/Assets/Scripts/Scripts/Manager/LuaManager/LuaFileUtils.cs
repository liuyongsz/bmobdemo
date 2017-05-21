using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Text;

public class LuaFileUtils
{
    private const string ASSETPOSTFIX = ".bundle|";

    public static LuaFileUtils Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LuaFileUtils();
            }

            return instance;
        }

        protected set
        {
            instance = value;
        }
    }
    
    protected List<string> searchPaths = new List<string>();
    protected Dictionary<string, AssetBundle> zipMap = new Dictionary<string, AssetBundle>();

    protected static LuaFileUtils instance = null;

    public LuaFileUtils()
    {
        instance = this;
    }

    public virtual void Dispose()
    {
        if (instance != null)
        {
            instance = null;
            searchPaths.Clear();

            try
            {
                foreach (KeyValuePair<string, AssetBundle> iter in zipMap)
                {
                    iter.Value.Unload(true);
                }
            }
            catch
            {

            }

            zipMap.Clear();
        }
    }

    //格式: 路径/?.lua
    public bool AddSearchPath(string path, bool front = false)
    {
        int index = searchPaths.IndexOf(path);

        if (index >= 0)
        {
            return false;
        }

        if (front)
        {
            searchPaths.Insert(0, path);
        }
        else
        {
            searchPaths.Add(path);
        }

        return true;
    }

    public bool RemoveSearchPath(string path)
    {
        int index = searchPaths.IndexOf(path);

        if (index >= 0)
        {
            searchPaths.RemoveAt(index);
            return true;
        }

        return false;
    }

    public string GetPackagePath()
    {
        StringBuilder sb = StringBuilderCache.Acquire();
        sb.Append(";");

        for (int i = 0; i < searchPaths.Count; i++)
        {
            sb.Append(searchPaths[i]);
            sb.Append(';');
        }

        return StringBuilderCache.GetStringAndRelease(sb);
    }

    public void AddSearchBundle(string name, AssetBundle bundle)
    {
        zipMap[name] = bundle;            
    }

    public virtual string FindFile(string oldfileName)
    {
        string fileName = null;
        GetZipMapName(oldfileName, out fileName);

        if (fileName == string.Empty)
        {
            return string.Empty;
        }

        if (Path.IsPathRooted(fileName))
        {                
            if (!fileName.EndsWith(".lua"))
            {
                fileName += ".lua";
            }

            return fileName;
        }
            
        if (fileName.EndsWith(".lua"))
        {
            fileName = fileName.Substring(0, fileName.Length - 4);
        }

        string fullPath = null;

        for (int i = 0; i < searchPaths.Count; i++)
        {
            fullPath = searchPaths[i].Replace("?", fileName);

            if (File.Exists(fullPath))
            {
                return fullPath;
            }
        }

        return null;
    }

    public virtual byte[] ReadFile(string fileName)
    {
        if (!LuaConst.beBundle)
        {
            string path = FindFile(fileName);
            byte[] str = null;

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
#if !UNITY_WEBPLAYER
                str = File.ReadAllBytes(path);
#else
                throw new LuaException("can't run in web platform, please switch to other platform");
#endif
            }

            return str;
        }
        else
        {
            return ReadZipFile(fileName);
        }
    }
    
    private byte[] ReadZipFile(string oldfileName)
    {
        AssetBundle zipFile = null;
        byte[] buffer = null;
        string zipName = null;

        string fileName = null;
        //判断资源所在的资源包name
        zipName = GetZipMapName(oldfileName, out fileName);

        //获取资源name
        int pos = fileName.LastIndexOf('/');
        if (pos > 0)
            fileName = fileName.Substring(pos + 1);
        if (!fileName.EndsWith(".lua"))
            fileName += ".lua";
        fileName += ".bytes";

        zipMap.TryGetValue(zipName, out zipFile);

        if (zipFile != null)
        {
            TextAsset luaCode = zipFile.LoadAsset<TextAsset>(fileName);

            if (luaCode != null)
            {
                buffer = luaCode.bytes;
                Resources.UnloadAsset(luaCode);
            }
        }
        return buffer;
    }
    
    private string GetZipMapName(string fileName, out string newFileName)
    {

        StringBuilder sb = StringBuilderCache.Acquire();
        int prefixIndex = fileName.IndexOf(ASSETPOSTFIX);
        string prefixstr = "";
        if (prefixIndex > 0)
        {
            prefixstr = fileName.Substring(0, prefixIndex + 8);
            sb.Append(prefixstr.Replace(ASSETPOSTFIX, ""));
        }
        else
        {
            sb.Append("lua");
        }
        if (string.IsNullOrEmpty(prefixstr))
        {
            newFileName = fileName;
        }
        else
        {
            newFileName = fileName.Replace(prefixstr, "");
        }
        string zipName = StringBuilderCache.GetStringAndRelease(sb);
        return zipName;
    }
}

