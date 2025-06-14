using UnityEngine;

/// <summary>
/// ���� ����ϴ� ���̾��� ��ȣ�� ���̾� ����ũ�� �� ���� ��ȯ�ؼ� ��𼭳� static���� ������ ����� �� �ֵ��� ĳ���ϴ� ��ƿ Ŭ����.
/// ������Ʈ ��ü���� LayerMask.NameToLayer, (1 << layer) ������ �ݺ� ȣ������ �ʱ� ���� ���.
/// </summary>
/// 
public static class LayerCache
{
    public static readonly int ZombieRow1 = LayerMask.NameToLayer("Zombie_Row1");
    public static readonly int ZombieRow2 = LayerMask.NameToLayer("Zombie_Row2");
    public static readonly int ZombieRow3 = LayerMask.NameToLayer("Zombie_Row3");

    public static readonly int GroundRow1 = LayerMask.NameToLayer("Ground_Row1");
    public static readonly int GroundRow2 = LayerMask.NameToLayer("Ground_Row2");
    public static readonly int GroundRow3 = LayerMask.NameToLayer("Ground_Row3");

    public static readonly LayerMask AllZombieRows =
        (1 << ZombieRow1) | (1 << ZombieRow2) | (1 << ZombieRow3);

    public static int GetGroundLayerMask(int index)
    {
        switch (index)
        {
            case 0:
                return GroundRow1;
            case 1:
                return GroundRow2;
            default:
                return GroundRow3;
        }
    }
}