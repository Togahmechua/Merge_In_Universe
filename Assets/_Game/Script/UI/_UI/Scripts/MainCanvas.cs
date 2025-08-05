using EasyTextEffects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : UICanvas
{
    [SerializeField] private Button pauseBtn;

    [SerializeField] private GameObject bloodyScreen;
    [SerializeField] private TextMeshProUGUI pointTxt;
    [SerializeField] private TextEffect txtEff;
    private Coroutine bloodyCoroutine;


    private void OnEnable()
    {
        UIManager.Ins.mainCanvas = this;
    }

    private void Start()
    {
        pauseBtn.onClick.AddListener(() =>
        {
            //AudioManager.Ins.PlaySFX(AudioManager.Ins.click);
            UIManager.Ins.OpenUI<PauseCanvas>();
            UIManager.Ins.CloseUI<MainCanvas>();
        });
    }

    #region Bloody Screen
    public void Hit()
    {
        // Nếu đang có coroutine máu đang chạy → dừng lại trước
        if (bloodyCoroutine != null)
        {
            StopCoroutine(bloodyCoroutine);
            bloodyCoroutine = null;
        }

        // Bắt đầu hiệu ứng mới và lưu lại coroutine
        bloodyCoroutine = StartCoroutine(BloodyScreenEffect());
    }
    private IEnumerator BloodyScreenEffect()
    {
        if (!bloodyScreen.activeInHierarchy)
            bloodyScreen.SetActive(true);

        var image = bloodyScreen.GetComponentInChildren<Image>();

        Color startColor = image.color;
        startColor.a = 1f;
        image.color = startColor;

        float duration = 3f;
        float t = 0f;

        while (t < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, t / duration);
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;

            t += Time.deltaTime;
            yield return null;
        }

        if (bloodyScreen.activeInHierarchy)
            bloodyScreen.SetActive(false);

        // ✅ Xóa biến coroutine sau khi xong
        bloodyCoroutine = null;
    }

    public void ResetUI()
    {
        if (bloodyCoroutine != null)
        {
            StopCoroutine(bloodyCoroutine);
            bloodyCoroutine = null;
        }

        if (bloodyScreen.activeInHierarchy)
            bloodyScreen.SetActive(false);
    }
    #endregion
}
