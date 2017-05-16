using UnityEngine;

public class PlatformsWithoutDPad : MonoBehaviour
{
    private Transform[] gos;

    // Use this for initialization
    private void Start()
    {
        gos = transform.GetComponentsInChildren<Transform>();

        foreach (Transform go in gos)
        {
#if UNITY_WEBPLAYER || UNITY_EDITOR || UNITY_STANDALONE_OSX
            go.gameObject.SetActive(false);
#endif
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}