using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FetterPanel : TeamMediator
{
    private UILabel profileDesc;
    private UILabel fetterDesc;

    public FetterPanel(GameObject go)
    {
        go.SetActive(true);
        profileDesc = UtilTools.GetChild<UILabel>(go.transform, "profileDesc");
        fetterDesc = UtilTools.GetChild<UILabel>(go.transform, "fetterDesc");
    }

    public override void SetChildPanel()
    {
        profileDesc.text = TextManager.GetPropsString(UtilTools.StringBuilder(currentTeamBaller.configId, "_profile"));
        fetterDesc.text = TextManager.GetPropsString(UtilTools.StringBuilder(currentTeamBaller.configId, "_fetter"));
    }
}
