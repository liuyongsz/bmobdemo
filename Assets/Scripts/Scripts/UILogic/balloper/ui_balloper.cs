using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ui_balloper : BasePanel {

    public UITexture headIcon;

    public UITexture iconSkill;
    public UITexture iconSkill1;
    public UITexture iconSkill2;

    public UISprite ativeIcon;
    public UISprite norIcon;
    public UISprite ativeIcon1;
    public UISprite norIcon1;
    public UISprite ativeIcon2;
    public UISprite norIcon2;
    public UISprite select0;
    public UISprite select1;
    public UISprite select2;

    public UILabel name0;
    public UILabel name1;
    public UILabel name2;

    public UIButton btnSure;
    public UIButton btnPass;
    public UIButton btnShoot;
    public UIButton btnFroceShoot;
    public UIButton btnLeftArrow;
    public UIButton btnRightArrow;
    public UIGrid assistSkillGrid;
    public UIGrid headGrid;

    public UISlider progressPower;
}

public class ui_balloperHeadItemData
{
    public MatchPlayerItem Data;
    public string headIcon; 
}

public class ui_balloperHeadItem : MonoBehaviour
{
    public UITexture headIcon;
}

public class ui_balloperAssistSkillItem : MonoBehaviour
{
    public UISprite ativeIcon;
    public UISprite passiveIcon;
    public UISprite selectAssist;

    public UITexture iconAssist;
    public UILabel skillName;
}