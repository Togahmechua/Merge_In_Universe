using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private CameraAnchor[] camAnchor;
    [SerializeField] private Canvas cv;


    private void OnEnable()
    {
        Camera cam = Camera.main;
        cv.renderMode = RenderMode.ScreenSpaceCamera;
        cv.worldCamera = cam;
    }

    private void Start()
    {
        //UIManager.Ins.mainCanvas.ResetUI();
    }
}
