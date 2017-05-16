using PureMVC.Patterns;
using PureMVC.Interfaces;
using UnityEngine;
using AssetBundles;
using UnityEngine.SceneManagement;

class SceneCommand : SimpleCommand
{
    private bool isLoadingScene;
    public override void Execute(INotification notification)
    {
        SceneVO msg = notification.Body as SceneVO;

        CommonFun.Players = null;
        CommonFun.Oponents = null;
        GUIManager.ShowLoadingUI();

        if (NotificationID.CHANGE_SCENE==notification.Name)
        {
            if (isLoadingScene)
            {
                Debug.LogError("已经在切换场景中。。。。");
                return;
            }

            if(msg.scenetype == EScene.PVE)
            {
                CloneProxy.Instance.Send_EnterClone(CloneProxy.Instance.IntoCloneID);
            }

            isLoadingScene = true;

            AssetManager.UnloadAllCacheRes(true);
            // Debug.LogError("loading scene start" + Time.realtimeSinceStartup.ToString());
            Facade.SendNotification(NotificationID.UPDATE_SCENE_MEDIATOR, msg);
            ResourceManager.Instance.LoadScene(msg.sceneName, false, OnSceneLoadComplete, msg);
        }
    }

    private void OnLoadingSceneLoad(object o)
    {
        isLoadingScene = false;
        Debug.LogError("loading scene finish" + Time.realtimeSinceStartup);
        SceneVO vo = o as SceneVO;
        vo.loading = true;
        GameManager.Instance.CurrentScene = vo;
        //   AssetManager.UnloadAssetBundle("s_loading");

        AssetManager.UnloadAllCacheRes(true);

        Facade.SendNotification(NotificationID.UPDATE_SCENE_MEDIATOR, vo);
        Debug.LogError("loading scene start"+ vo.sceneName + Time.realtimeSinceStartup.ToString());
        ResourceManager.Instance.LoadScene(vo.sceneName, true, OnSceneLoadComplete, vo);
    }

    
    private void OnSceneLoadComplete(object o)
    {
        isLoadingScene = false;
        SceneVO vo = o as SceneVO;
        vo.loading = false;
        // Debug.LogError("loading scene finish" + msg.m_scenename + Time.realtimeSinceStartup.ToString());
        //Application.LoadLevel(_msg.m_scenename);
        //GameManager.Instance.CurrentScene = vo;
        GUIManager.CloseAllUI();
        GUIManager.HideLoadingUI();
        GameProxy.Instance.RealEnterScene(vo.sceneName, vo.scenetype);

        CommonFun.Debug("CHANGE_SCENE : " + vo.sceneName);
        //卸载场景assetboundle

        AssetManager.UnloadAssetBundle(vo.abname);

        if(vo.scenetype == EScene.PVE)
        {
            GameManager.Instance.LoadPrefab("player001");
            GameManager.Instance.LoadPrefab("zuqiuchang");
            GameManager.Instance.LoadPrefab("goalmesh_left");
            GameManager.Instance.LoadPrefab("goalmesh_right");
            GameManager.Instance.LoadPrefab("goalkeeper_left");
            GameManager.Instance.LoadPrefab("goalkeeper_right");
        }
    }
}

