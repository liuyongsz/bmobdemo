using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChooseInheritItem : UIGridItem
{
    private Text Level;
    private Text Name;
    private RawImage head;
    private RawImage color;
    private Slider levelSlider;
    private RawImage[] star;
    //public override void OnStart()
    //{
    //    this.transform.localPosition += new Vector3(98, -136, 0);
    //    Level = UtilTools.GetChild<Text>(this.transform, "Level");
    //    Name = UtilTools.GetChild<Text>(this.transform, "Text");
    //    head = UtilTools.GetChild<RawImage>(this.transform, "head");
    //    color = UtilTools.GetChild<RawImage>(this.transform, "color");
    //    star = UtilTools.GetChilds<RawImage>(this.transform, "star");
    //    levelSlider = UtilTools.GetChild<Slider>(this.transform, "levelSlider");
    //}

    //public override void SetData(object data)
    //{
    //    TeamBaller info = data as TeamBaller;
    //    TD_Player player = Instance.Get<PlayerManager>().GetItem(int.Parse(info.configId));
    //    Name.text = TextManager.GetItemString(info.configId);
    //    Level.text = "Lv " + info.level.ToString();
    //    LoadSprite.LoaderImage(head, info.configId, false);
    //    LoadSprite.LoaderImage(color, "color" + info.star, false);
    //    PlayerItem item = PlayerManager.GetPlayerItem(info.level);
    //    levelSlider.transform.FindChild("Num").GetComponent<Text>().text = UtilTools.StringBuilder(info.exp, "/", item.needExp);
    //    levelSlider.value = info.exp * 1.0f / item.needExp;
    //}
}
