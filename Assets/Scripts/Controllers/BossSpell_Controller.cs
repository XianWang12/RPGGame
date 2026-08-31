using UnityEngine;

public class BossSpell_Controller : MonoBehaviour
{
    [SerializeField] private Transform check;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private LayerMask whatIsPlayer;

    private CharacterStats myStats;

    public void SetUpSpell(CharacterStats _stats) => myStats = _stats;

    private void AnimatorTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(check.position, boxSize, whatIsPlayer);

        foreach (Collider2D hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                myStats.DoDamage(hit.GetComponent<PlayerStats>());
                //Debug.Log("player take damage");
            }
        }
    }

    private void OnDrawGizmos() => Gizmos.DrawWireCube(check.position, boxSize);

    private void SelfDestroy() => Destroy(gameObject);
}
