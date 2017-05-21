using UnityEngine;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProxyInstance;

public class CheckpointProxy : Proxy<CheckpointProxy>
{

    public CheckpointProxy()
        : base(ProxyID.Chapter)
    {
       
    }
}
