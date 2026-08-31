using UnityEngine;

public class ShockStrike_Controller : MonoBehaviour
{
    [SerializeField] private CharacterStats targetStats;
    [SerializeField] private float speed;
    private int damage;

    private Animator anim;
    private bool trigger;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();

    }

    public void Setup(int _damage, CharacterStats _targetStats)
    {
        damage = _damage;
        targetStats = _targetStats;
    }

    void Update()
    {
        if (targetStats == null)
            return;
        if (trigger)
            return;

        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, speed * Time.deltaTime);
        transform.right = transform.position - targetStats.transform.position;


        if (Vector2.Distance(transform.position, targetStats.transform.position) < .7f)
        {
            anim.transform.localPosition = new Vector2(0, 1);
            anim.transform.localRotation = Quaternion.identity;
            transform.localRotation = Quaternion.identity;
            anim.transform.localScale = new Vector3(3, 3);

            Invoke("DamageAndSelfDestory", .2f);
            trigger = true;
            anim.SetTrigger("Hit");
        }
    }

    private void DamageAndSelfDestory()
    {
        targetStats.ApplyShock(true);

        targetStats.TakeDamage(damage);
        Destroy(gameObject, .4f);

    }

}
