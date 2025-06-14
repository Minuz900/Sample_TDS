using UnityEngine;

/// <summary>
/// 스프라이트/정렬 관련 유틸리티 함수 모음
/// </summary>
public static class SortingUtils
{
    /// <summary>
    /// 하위 모든 SpriteRenderer의 sortingLayerName을 일괄 변경
    /// </summary>
    public static void SetSortingRecursively(GameObject obj, string layerName)
    {
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = layerName;
        }
        foreach (Transform child in obj.transform)
        {
            SetSortingRecursively(child.gameObject, layerName);
        }
    }
}
