using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinCanvas : UICanvas
{
    [Header("===Effect===")]
    [SerializeField] private Image roateImg;
    [SerializeField] float speed = 90f;

    [Header("---Other Button---")]
    [SerializeField] private Button retryBtn;
    [SerializeField] private Button menuBtn;

    private bool isClick;

    private void OnEnable()
    {
        AudioManager.Ins.PlaySFX(AudioManager.Ins.win);
    }

    private void Start()
    {
        retryBtn.onClick.AddListener(() =>
        {
            AudioManager.Ins.PlaySFX(AudioManager.Ins.click);

            UIManager.Ins.TransitionUI<ChangeUICanvas, WinCanvas>(0.6f,
               () =>
               {
                   LevelManager.Ins.DespawnLevel();
                   UIManager.Ins.OpenUI<MainCanvas>();
                   LevelManager.Ins.SpawnLevel();
               });
        });

        menuBtn.onClick.AddListener(() =>
        {
            AudioManager.Ins.PlaySFX(AudioManager.Ins.click);

            UIManager.Ins.TransitionUI<ChangeUICanvas, WinCanvas>(0.6f,
                () =>
                {
                    LevelManager.Ins.DespawnLevel();
                    UIManager.Ins.OpenUI<StartCanvas>();
                });


        });
    }
    private void Update()
    {
        roateImg.transform.Rotate(Vector3.forward, speed * Time.deltaTime);
    }
}