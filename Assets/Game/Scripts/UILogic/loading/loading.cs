using UnityEngine;
using UnityEngine.UI;
public class loading : MonoBehaviour
{
    public UILabel txtWarn;
    public UIProgressBar progress;

    public void Start()
    {
        SetProgress(0);

        transform.localScale = Vector3.one;
    }

    public void SetProgress(float val)
    {
        progress.value = val;
        CommonFun.Debug("loading progress " + val);
    }
}
