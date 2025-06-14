using UnityEngine;

public class ZombiePartsRandomizer : MonoBehaviour
{
    [Header("좀비 파츠 세트")]
    public ZombiePartsSet partsSet;

    [Header("파츠별 SpriteRenderer 연결")]
    public SpriteRenderer leftArmRenderer;
    public SpriteRenderer rightArmRenderer;
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer headRenderer;
    public SpriteRenderer leftLegRenderer;
    public SpriteRenderer rightLegRenderer;

    public void RandomizeParts()
    {
        int idx = Random.Range(0, partsSet.rightArms.Length);
        rightArmRenderer.sprite = partsSet.rightArms[idx];
        leftArmRenderer.sprite = partsSet.leftArms[idx];
        rightLegRenderer.sprite = partsSet.rightLegs[idx];
        leftLegRenderer.sprite = partsSet.leftLegs[idx];
        headRenderer.sprite = partsSet.heads[idx];
        bodyRenderer.sprite = partsSet.bodies[idx];
    }
}
