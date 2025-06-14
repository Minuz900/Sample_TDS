using UnityEngine;

/// <summary>
/// ��������Ʈ/���� ���� ��ƿ��Ƽ �Լ� ����
/// </summary>
public static class SortingUtils
{
    /// <summary>
    /// ���� ��� SpriteRenderer�� sortingLayerName�� �ϰ� ����
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
