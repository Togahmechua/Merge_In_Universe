using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Planet : GameUnit
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float dragSpeed = 0.01f; // điều chỉnh tốc độ kéo

    private bool isDragging;
    private bool isAbleToDrag = true;
    private Vector3 lastWorldTouchPos;

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
   /* private void MergeIfSameType(Planet other)
    {
        if (other.poolType == this.poolType)
        {
            if (MergeTable.nextMap.TryGetValue(poolType, out PoolType nextType))
            {
                Vector3 spawnPos = (this.transform.position + other.transform.position) / 2f;

                // Despawn hai object hiện tại
                SimplePool.Despawn(this);
                SimplePool.Despawn(other);

                // Nếu merge ra BlackHole thì không spawn gì thêm
                if (nextType == PoolType.BlackHole)
                {
                    // (Tuỳ chọn) Hiệu ứng hoặc âm thanh
                    
                    return;
                }

                // Nếu kết quả là Nothing (không còn loại mới), cũng không spawn
                if (nextType == PoolType.None || nextType == PoolType.Nothing)
                    return;

                // Spawn planet mới
                GameUnit prefab = PoolHelper.GetPrefab(nextType);
                if (prefab != null)
                {
                    GameUnit newUnit = SimplePool.Spawn(prefab, spawnPos, Quaternion.identity) as GameUnit;
                    newUnit.poolType = nextType;
                }
            }
        }
    }*/


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Planet planet = Cache.GetPlanet(collision.gameObject);
        if (planet != null)
        {

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
