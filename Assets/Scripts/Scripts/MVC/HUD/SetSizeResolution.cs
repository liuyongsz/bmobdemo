using UnityEngine;

public class SetSizeResolution : MonoBehaviour
{
    private void Awake()
    {
 //       GUITexture gui = GetComponent<GUITexture>();

    }

    private void Start()
    {
        ResizeGUI();
    }

    private void ResizeGUI()
    {
        const float ancho = 800;
        const float alto = 600;

        float width = GetComponent<GUITexture>().pixelInset.width;
 //       float height = guiTexture.pixelInset.height;
        float x = GetComponent<GUITexture>().pixelInset.x;
        float y = GetComponent<GUITexture>().pixelInset.y;

        float FilScreenWidth = width / ancho;
        float rectWidth = FilScreenWidth * Screen.width;
//        float FilScreenHeight = height / alto;
//      float rectHeight = FilScreenHeight * Screen.height;
        float rectX = (x / ancho) * Screen.width;
        float rectY = (y / alto) * Screen.height;

        GetComponent<GUITexture>().pixelInset = new Rect(rectX, rectY, rectWidth, /*rectHeight*/ rectWidth);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}