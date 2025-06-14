using UnityEngine;

/// <summary>
/// 자주 사용하는 레이어의 번호와 레이어 마스크를 한 번만 변환해서 어디서나 static으로 빠르게 사용할 수 있도록 캐싱하는 유틸 클래스.
/// 프로젝트 전체에서 LayerMask.NameToLayer, (1 << layer) 연산을 반복 호출하지 않기 위해 사용.
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