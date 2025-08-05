using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    //[SerializeField] private CameraAnchor[] camAnchor;
    [SerializeField] private Canvas cv;
    [SerializeField] private Transform[] dragPos = new Transform[2];


    private void OnEnable()
    {
        Camera cam = Camera.main;
        cv.renderMode = RenderMode.ScreenSpaceCamera;
        cv.worldCamera = cam;
    }

    private void Start()
    {
        UIManager.Ins.mainCanvas.ResetUI();
    }

    public Vector3 GetDragPos(int index)
    {
        if (dragPos != null && index >= 0 && index < dragPos.Length && dragPos[index] != null)
        {
            return dragPos[index].position;
        }

        Debug.LogWarning("DragPos bị null hoặc index không hợp lệ!");
        return Vector3.zero;
    }
}
