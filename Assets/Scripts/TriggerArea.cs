using UnityEngine;

public class TriggerArea : MonoBehaviour
{
    public const string Hero = "Hero";

    Zombie zombie;

    private void Awake()
    {
        zombie = GetComponentInParent<Zombie>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Hero) && zombie != null)
        {
            zombie.SetAttack(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Hero) && zombie != null)
        {
            zombie.SetAttack(false);
        }
    }
}
