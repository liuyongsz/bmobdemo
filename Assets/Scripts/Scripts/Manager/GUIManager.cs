using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using XLua;

[LuaCallCSharp]

public class GUIManager
{
    private static int index = 0;
    private static Transform uiLayer1;
    private static loading m_loading;
    private static Vector3 pos = new Vector3(0, 100, 0);
    public static List<GameObject> jumpList = new List<GameObject>();
    public static DictionaryEx<string, GameObject> m_panelList = new DictionaryEx<string, GameObject>();
    public static DictionaryEx<string, GameObject> m_panelExitList = new DictionaryEx<string, GameObject>();
    public static void CloseAllUI()
    {
        for (int i = 0; i < m_panelList.mList.Count; ++i)
        {
            GameObject.Destroy(m_panelList[m_panelList.mList[i]].gameObject);
        }
        for (int i = 0; i < m_panelExitList.mList.Count; ++i)
        {
            GameObject.Destroy(m_panelExitList[m_panelExitList.mList[i]].gameObject);
        }
        m_panelList.Clear();
        m_panelExitList.Clear();
    }

    /// <summary>显示加载进度条</summary>
    public static void ShowLoadingUI()
    {
        HideLoadingUI();
        GameObject obj = GameObject.Instantiate<GameObject>(Resources.Load("loadingpanel") as GameObject);
        m_loading = ScriptHelper.BindField(obj.transform, "loading") as loading;
        Transform tparent = Main.GetUIRoot().FindChild("Camera");
        m_loading.transform.SetParent(tparent);
        m_loading.transform.localPosition = Vector3.zero;
    }

    /// <summary>隐藏界面</summary>
    public static void HidePanel(string name)
    {
        if (m_panelList.ContainsKey(name))
        {
            m_panelList[name].SetActive(false);
        }
    }

    /// <summary>显示隐藏的界面</summary>
    public static void ShowPanel(string name)
    {
        if (m_panelList.ContainsKey(name))
        {
            m_panelList[name].SetActive(true);
        }
    }

    /// <summary>隐藏加载进度条</summary>
    public static void HideLoadingUI()
    {
        if(null != m_loading)
            GameObject.Destroy(m_loading.gameObject);

        m_loading = null;
    }

    public static loading GetLoading()
    {
        return m_loading;
    }

    public static void SetLoadingProgress(float val)
    {
        if(null != m_loading)
        {
            m_loading.SetProgress(val);
        }
    }

    public static bool HasView(string name)
    {

        if (m_panelList.ContainsKey(name) || m_panelExitList.ContainsKey(name))
            return true;

        return false;
    }

    /// <summary>
    /// 设置跳字 
    /// </summary>
    public static void SetJumpText(string content)
    {
        ResourceManager.Instance.LoadPrefab("jumpLabel", (name, obj) =>
        {
            obj.AddComponent<TextDestroy>();
            obj.GetComponent<UILabel>().text = content;
            Transform tparent = UICamera.mainCamera.transform;
            obj.transform.SetParent(tparent);
            obj.transform.localScale = Vector3.one;
            jumpList.Insert(0, obj);
            TweenScale.Begin(obj, 0.1f, new Vector3(1.16f, 1.16f, 1.16f));
            index++;
            TimerManager.AddTimer("index" + index, 0.1f, OnScaleFinished, obj);
            if (jumpList.Count > 1)
            {
                TweenPosition.Begin(jumpList[1].gameObject, 0.1f, jumpList[1].transform.localPosition + pos);
                TweenScale.Begin(jumpList[1].gameObject, 0.1f, new Vector3(0.9f, 0.9f, 0.9f));
            }
            if (jumpList.Count > 2)
            {
                TweenPosition.Begin(jumpList[2].gameObject, 0.1f, jumpList[2].transform.localPosition + pos);
                TweenScale.Begin(jumpList[2].gameObject, 0.1f, new Vector3(0.8f, 0.8f, 0.8f));
            }
            if (jumpList.Count > 3)
            {
                MonoBehaviour.Destroy(jumpList[3].gameObject);
                jumpList.RemoveAt(3);
            }
        });
    }
   
    static void OnScaleFinished(params object[] arg)
    {
        GameObject obj = arg[0] as GameObject;
        TweenScale tween1 = TweenScale.Begin(obj, 0, new Vector3(1, 1, 1));
    }
    /// <summary>
    /// 查看道具信息 不是背包里查看的uuid不填 
    /// </summary>
    public static void ShowItemInfoPanel(PanelType type, string itemID, string uuid = "", ItemMediator.SellItem SellItem = null,ItemMediator.UseItem UseItem = null)
    {
        ShowItemInfo showItemInfo = new ShowItemInfo();
        ItemMediator.panelType = type;
        showItemInfo.sellItem = SellItem;
        showItemInfo.useItem = UseItem;
        List<object> list = new List<object>();
        list.Add(showItemInfo);
        list.Add(itemID);
        list.Add(uuid);
        Facade.Instance.SendNotification(NotificationID.ItemInfo_Show, list);
    }

    /// <summary>
    /// 说明界面信息
    /// </summary>
    public static void ShowHelpPanel(List<object> decsList)
    {
        PromptInfo promptInfo = new PromptInfo();
        promptInfo.type = PromptType.Help;
        promptInfo.objList = decsList;
        Facade.Instance.SendNotification(NotificationID.Prompt_Show, promptInfo);
    }
    /// <summary>
    /// 设置物体层级 
    /// </summary>
    public static void ChangeLayer(GameObject obj, string layer)
    {
        List<Transform> childrens = GetAllChildTransforms(obj.transform);

        for (int i = 0; i < childrens.Count; ++i)
        {
            childrens[i].gameObject.layer = LayerMask.NameToLayer(layer);
        }
    }
    public static List<Transform> GetAllChildTransforms(Transform trs)
    {
        List<Transform> list = new List<Transform>();
        list.AddRange(trs.GetComponentsInChildren<Transform>());
        return list;
    }

    /// <summary>
    /// 设置提示语句
    /// </summary>
    public static void SetPromptInfo(string content, PromptMediator.Close close)
    {
        PromptInfo info = new PromptInfo();
        info.type = PromptType.Tishi;
        info.tishi = TextManager.GetUIString("UICreate1");
        info.content = content;
        info.close = close;
        Facade.Instance.SendNotification(NotificationID.Prompt_Show, info);
    }
    public static void PromptBuyDiamod(string content)
    {
        SetPromptInfoChoose(TextManager.GetUIString("UICreate1"), content, BuyDiamod);
    }
 
    public static void PromptBuyEuro()
    {
        SetPromptInfoChoose(TextManager.GetUIString("UICreate1"), TextManager.GetUIString("equip_error_6"), BuyEuro);
    }
    
    static void BuyDiamod()
    {
        Facade.Instance.SendNotification(NotificationID.Store_Show);
    }
    static void BuyEuro()
    {
        Facade.Instance.SendNotification(NotificationID.Power_Show, GoldType.Euro);
    }

    /// <summary>
    /// 设置竞技界面
    /// </summary>
    public static void SetPlayersFormation(ArenaInfo info)
    {
        ResourceManager.Instance.LoadPrefab("playerformation", (name, obj) =>
        {
            if (obj == null)
                return;
            obj.AddComponent<PlayerFormation>();
            PlayerFormation formation = obj.GetComponent<PlayerFormation>();
            formation.enemyFormation = info.formation;
            formation.enemyfigthValue = info.fightValue;
            formation.enemy = info.playerName;
            UtilTools.SetParentWithPosition(obj.transform, UICamera.mainCamera.transform, Vector3.zero, Vector3.one);
        });
    }

    /// <summary>
    /// 设置选项界面
    /// </summary>
    public static void SetPromptInfoChoose(string title, string content, PromptMediator.ClickOk clickOk)
    {
        PromptInfo info = new PromptInfo();
        info.type = PromptType.Choose;
        info.tishi = title;
        info.content = content;
        info.clickOk = clickOk;
        Facade.Instance.SendNotification(NotificationID.Prompt_Show, info);
    }

    /// <summary>
    /// 设置层级
    /// </summary>
    /// <param name="go"></param>
    /// <param name="layer"></param>
    public static void SetLayer(GameObject go, int layer)
    {
        go.layer = layer;
        if (go.transform.childCount > 0)
        {
            int len = go.transform.childCount;
            for (int i = 0; i < len; i++)
            {
                SetLayer(go.transform.GetChild(i).gameObject, layer);
            }
        }
    }

    /// <summary>
    /// 换shader
    /// </summary>
    /// <param name="role"></param>
    /// <param name="shader"></param>
    public static void ChangeShader(GameObject role, params string[] args)
    {
        if (args == null || args.Length == 0)
            return;

        Shader[] shaders = new Shader[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            shaders[i] = Shader.Find(args[i]);
            if (shaders[i] == null)
            {
                Debug.LogError("shader not found");
            }
        }

        Renderer[] mSkinnedMeshRender = role.GetComponentsInChildren<Renderer>(true);
        if (mSkinnedMeshRender == null)
        {
            return;
        }

        for (int i = 0; i < mSkinnedMeshRender.Length; i++)
        {
            if (mSkinnedMeshRender[i] != null && mSkinnedMeshRender[i].material != null)
            {
                for (int j = 0; j < args.Length; j++)
                {
                    if (shaders[j] == null)
                        continue;

                    string strPrefix = args[j].Substring(0, args[j].Length - 1);
                    if (mSkinnedMeshRender[i].material.shader.name.Equals(strPrefix))
                    {
                        mSkinnedMeshRender[i].material.shader = shaders[j];
                    }
                }
            }
        }
    }

    /// <summary>
    /// 更新shader
    /// </summary>
    /// <param name="role"></param>
    /// <param name="strShader"></param>
    /// <param name="strPrefix"></param>
    public static void ChangeShader(GameObject role, Shader shader, string strPrefix, string strPrefix2)
    {
        if (shader == null)
        {
            Debug.LogError("ChangeShader::LoadError");
        }
        Renderer[] mSkinnedMeshRender = role.GetComponentsInChildren<Renderer>(true);
        if (mSkinnedMeshRender == null)
        {
            return;
        }

        for (int i = 0; i < mSkinnedMeshRender.Length; i++)
        {
            if (mSkinnedMeshRender[i] != null && mSkinnedMeshRender[i].material != null)
            {
                if (mSkinnedMeshRender[i].material.shader.name.Equals(strPrefix)
                    || mSkinnedMeshRender[i].material.shader.name.Equals(strPrefix2))
                {
                    mSkinnedMeshRender[i].material.shader = shader;
                }
            }
        }
    }
    /// <summary>
    /// 设置RenderQ
    /// </summary>
    /// <param name="oModel"></param>
    public static void SetUIModelRenderQ(GameObject oModel, int renderQueue = 3100)
    {
        if (oModel == null)
            return;
        int effect = LayerMask.NameToLayer("Effect");
        Renderer[] rens = oModel.GetComponentsInChildren<Renderer>(true);
        if (rens != null)
        {
            for (int i = 0; i < rens.Length; i++)
            {
                Renderer rend = rens[i];
                if (rend != null && rend.gameObject.layer == effect)// 
                {
                    rend.material.renderQueue = renderQueue;//将特效显示在最上层
                }
            }
        }

        ParticleSystemRenderer[] particles = oModel.GetComponentsInChildren<ParticleSystemRenderer>(true);
        if (particles != null)
        {
            for (int i = 0; i < rens.Length; i++)
            {
                Renderer rend = rens[i];
                if (rend != null && rend.gameObject.layer == effect)// && 
                {
                    rend.material.renderQueue = renderQueue;//将特效显示在最上层
                }
            }
        }
    }

    /// <summary>
    /// 隐藏粒子特效
    /// </summary>
    /// <param name="oModel"></param>
    public static void HideParticles(GameObject oModel)
    {
        if (oModel == null)
            return;

        ParticleSystem[] particles = oModel.GetComponentsInChildren<ParticleSystem>(true);
        if (particles != null)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].enableEmission = false;
                particles[i].GetComponent<Renderer>().enabled = false;
            }
        }
    }

    /// <summary>
    /// 粒子特效适应界面模型
    /// </summary>
    /// <param name="oModel"></param>
    public static void AdjustParticlesToUI(GameObject oModel, float modelScale, float uiScale)
    {
        if (oModel == null)
            return;

        if (modelScale == 0f)
        {
            return;
        }

        ParticleSystem[] particles = oModel.GetComponentsInChildren<ParticleSystem>(true);
        if (particles != null)
        {
            float scaleRate = uiScale * modelScale;
            for (int i = 0; i < particles.Length; i++)
            {
                ParticleSystem particleSystem = particles[i];
                particleSystem.startSize *= scaleRate;
                particleSystem.startSpeed *= scaleRate;
                float maxParticles = particleSystem.maxParticles;
                maxParticles *= scaleRate;
                particleSystem.maxParticles = (int)maxParticles;
                ParticleSystemRenderer render = particleSystem.GetComponent<ParticleSystemRenderer>();
                if (render)
                {
                    render.maxParticleSize *= scaleRate;
                }
            }
        }
    }

    public static void SetObjRenderQ(GameObject oModel, int iLayer, int iRenderQueue)
    {
        if (oModel == null)
            return;
        Renderer[] rens = oModel.GetComponentsInChildren<Renderer>(true);
        if (rens != null)
        {
            for (int i = 0; i < rens.Length; i++)
            {
                Renderer rend = rens[i];
                if (rend != null && rend.gameObject.layer == iLayer)
                {
                    rend.material.renderQueue = iRenderQueue;//将特效显示在最上层
                }
            }
        }
    }

    /// <summary>
    /// 渲染层级
    /// </summary>
    /// <param name="oModel"></param>
    public static void SetObjRenderQ(GameObject oModel, int iRenderQueue)
    {
        if (oModel == null)
            return;
        Renderer[] rens = oModel.GetComponentsInChildren<Renderer>(true);
        if (rens != null)
        {
            for (int i = 0; i < rens.Length; i++)
            {
                Renderer rend = rens[i];
                if (rend != null)
                {
                    rend.material.renderQueue = iRenderQueue;//将特效显示在最上层
                }
            }
        }
    }
    /// <summary>
    /// Lua打开界面
    /// </summary>
    public static void OpenPanel(NotificationID notificationName)
    {
        Facade.Instance.SendNotification(notificationName);
    }
   

}



