using UnityEngine;

public class SpellEffect : MonoBehaviour
{
    public int damage = 2;

    // через сколько наноситс€ урон
    public float damageDelay = 1f;

    // радиус попадани€
    public float hitRadius = 1.5f;

    private bool hasDamaged;

    private void Start()
    {
        Invoke(nameof(DealDamage), damageDelay);

        Destroy(gameObject, 2f);
    }

    void DealDamage()
    {
        if (hasDamaged) return;

        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            hitRadius,
            LayerMask.GetMask("Player")
        );

        if (hit != null)
        {
            PlayerHealth hp =
                hit.GetComponent<PlayerHealth>();

            if (hp != null)
            {
                hp.TakeDamage(damage);

                hasDamaged = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        Gizmos.DrawWireSphere(
            transform.position,
            hitRadius
        );
    }
}