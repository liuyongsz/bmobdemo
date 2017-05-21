using System.Collections.Generic;
using TinyBinaryXml;


public class AbilityConfig : ConfigBase
{
    

    public void LoadXml(UnityEngine.Events.UnityAction loadedFun = null)
    {
        LoadData("PlayerPower", loadedFun);
    }

    public override void onloaded(AssetBundles.NormalRes data)
    {
     
        byte[] asset = (data as AssetBundles.BytesRes).m_bytes;
        if (asset == null)
            return;

      
        base.onloaded(data);
    }

}
