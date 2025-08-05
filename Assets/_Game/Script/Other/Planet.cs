using EZCameraShake;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Planet : GameUnit
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float dragSpeed = 0.01f; // điều chỉnh tốc độ kéo

    public bool HasLanded { get; private set; }

    private bool isDragging;
    private bool isAbleToDrag = true;
    private Vector3 lastWorldTouchPos;
    private Coroutine loseCheckCoroutine;

    public static event Action<Planet> OnPlanetMerged;

    private void OnEnable()
    {
        Init(false);
    }

    private void OnDisable()
    {
        HasLanded = false;
    }

    public void Init(bool immediateDrop = false)
    {
        HasLanded = false;
        isDragging = false;
        isAbleToDrag = !immediateDrop;
        rb.gravityScale = immediateDrop ? 1f : 0f;
    }



    #region Drag
    private void Update()
    {
        if (!isAbleToDrag)
            return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Vector3 worldTouchPos = Camera.main.ScreenToWorldPoint(touch.position);
            worldTouchPos.z = 0;

            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                lastWorldTouchPos = worldTouchPos;
                rb.gravityScale = 0; // tắt rơi khi đang kéo
            }

            if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector3 delta = worldTouchPos - lastWorldTouchPos;
                lastWorldTouchPos = worldTouchPos;

                // Di chuyển theo trục X
                Vector3 newPos = transform.position + new Vector3(delta.x, 0, 0) * dragSpeed;

                // Clamp vị trí nếu cần
                float minX = LevelManager.Ins.curLevel.GetDragPos(0).x;
                float maxX = LevelManager.Ins.curLevel.GetDragPos(1).x;

                newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
                transform.position = newPos;
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
                isAbleToDrag = false;
                rb.gravityScale = 1; // bật lại rơi
            }
        }
    }
    #endregion

    #region Merge
    private void MergeIfSameType(Planet other)
    {
        if (other.poolType == this.poolType)
        {
            if (MergeTable.nextMap.TryGetValue(poolType, out PoolType nextType))
            {
                Vector3 spawnPos = (this.transform.position + other.transform.position) / 2f;

                SimplePool.Despawn(this);
                SimplePool.Despawn(other);

                if (nextType == PoolType.Nothing)
                {
                    //Play Particle
                    UIManager.Ins.TransitionUI<ChangeUICanvas, MainCanvas>(0.5f,
                       () =>
                       {
                           LevelManager.Ins.DespawnLevel();
                           UIManager.Ins.OpenUI<WinCanvas>();
                       });
                    return;
                }

                Planet merged = SimplePool.Spawn<Planet>(nextType, spawnPos, Quaternion.identity);
                ParticlePool.Play(ParticleType.MergeEff, spawnPos, Quaternion.identity);
                AudioManager.Ins.PlaySFX(AudioManager.Ins.merge);
                merged.Init(true);

                CameraShaker.Instance.ShakeOnce(2f, 2f, 0.1f, 1f);

                OnPlanetMerged?.Invoke(merged); // GỬI thằng mới
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Loose"))
        {
            if (loseCheckCoroutine == null)
            {
                loseCheckCoroutine = StartCoroutine(CheckLoseAfterDelay(other));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Loose"))
        {
            // Nếu rời khỏi vạch thua trước 1 giây thì huỷ kiểm tra
            if (loseCheckCoroutine != null)
            {
                StopCoroutine(loseCheckCoroutine);
                loseCheckCoroutine = null;
            }
        }
    }

    private IEnumerator CheckLoseAfterDelay(Collider2D other)
    {
        yield return new WaitForSeconds(1f);

        // Kiểm tra xem vẫn còn chạm vạch thua không
        if (other != null && other.IsTouching(GetComponent<Collider2D>()))
        {
            Debug.Log("Thua vì chạm vạch thua hơn 1 giây!");

            UIManager.Ins.TransitionUI<ChangeUICanvas, MainCanvas>(0.5f,
                () =>
                {
                    LevelManager.Ins.DespawnLevel();
                    UIManager.Ins.OpenUI<LooseCanvas>();
                });
        }

        loseCheckCoroutine = null;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            HasLanded = true;
        }

        Planet planet = Cache.GetPlanet(other.gameObject);
        if (planet != null && this.GetInstanceID() > planet.GetInstanceID())
        {
            MergeIfSameType(planet);
        }
    }

    #endregion
}

public static class MergeTable
{
    public static Dictionary<PoolType, PoolType> nextMap = new Dictionary<PoolType, PoolType>()
    {
        { PoolType.Mercury, PoolType.Venus },
        { PoolType.Venus, PoolType.Earth },
        { PoolType.Earth, PoolType.Mars },
        { PoolType.Mars, PoolType.Jupiter },
        { PoolType.Jupiter, PoolType.Saturn },
        { PoolType.Saturn, PoolType.Uranus },
        { PoolType.Uranus, PoolType.Neptune },
        { PoolType.Neptune, PoolType.Sun },
        { PoolType.Sun, PoolType.BlackHole },
        { PoolType.BlackHole, PoolType.Nothing }
    };
}
