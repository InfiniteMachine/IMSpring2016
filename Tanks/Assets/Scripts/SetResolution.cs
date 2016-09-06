using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SetResolution : MonoBehaviour {
    private static Camera backgroundCam;
    public Vector2 screenRatio = new Vector2(16, 9);
    
    void Awake()
    {
        ResetScreen();
    }

    void LevelWasLoaded(Scene s, LoadSceneMode mode)
    {
        if(s.name != "Menu" && s.name != "Player_Select_Screen")
            ResetScreen();
    }

    private void ResetScreen()
    {
        if (Camera.main == null)
            return;
        float targetHeight = Screen.width * (screenRatio.y / screenRatio.x);
        float percentageHeight = 1f;
        float percentageWidth = 1f;
        float targetX = 0f;
        float targetY = 0f;
        float half = 0f;

        percentageHeight = targetHeight / Screen.height;
        if (percentageHeight > 1)
        {
            float targetWidth = Screen.height * (screenRatio.x / screenRatio.y);
            percentageWidth = targetWidth / Screen.width;
            half = (1f - percentageWidth) / 2;
            targetX = half;
        }
        else
        {
            half = (1f - percentageHeight) / 2f;
            targetY = half;
        }
        Camera.main.rect = new Rect(targetX, targetY, percentageWidth, percentageHeight);
        if (backgroundCam == null)
        {
            backgroundCam = new GameObject("BackgroundCam", typeof(Camera)).GetComponent<Camera>();
            DontDestroyOnLoad(backgroundCam);
            backgroundCam.depth = int.MinValue;
            backgroundCam.clearFlags = CameraClearFlags.SolidColor;
            backgroundCam.backgroundColor = Color.black;
            backgroundCam.cullingMask = 0;
        }
    }
}
