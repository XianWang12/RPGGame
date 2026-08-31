using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private Player player;
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private float crystalExistTimer;

    private bool canExplode;
    private bool canMove;
    private float moveSpeed;

    private bool canGrow;
    private float growSpeed;

    private Transform closestTarget;
    [SerializeField] LayerMask whatIsEnemy;
    public void SetupCrystal(float _crystalDuration, bool _canExpode ,bool _canMove,float _moveSpeed,float _growSpeed,Transform _closestTarget,Player _player)
    {
        crystalExistTimer = _crystalDuration;
        canExplode = _canExpode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        growSpeed = _growSpeed;
        closestTarget = _closestTarget;
        player = _player;
    }

    public void ChooseRandomEnemy()
    {
        float radius = SkillManager.instance.blackhole.GetBlackholeRadius();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,radius,whatIsEnemy);

        if(colliders.Length>0)
            closestTarget = colliders[Random.Range(0,colliders.Length)].transform;
    }

    private void Update()
    {
        crystalExistTimer -= Time.deltaTime;

        if (crystalExistTimer < 0)
        {
            FinishCryStal();
        }

        if (canMove&&closestTarget!=null)
        {
            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, closestTarget.position) < 1.2)
            {
                FinishCryStal();
                canMove = false;
            }
        }
        if(canGrow)
            transform.localScale=Vector2.Lerp(transform.localScale,new Vector2(1.2f,1.2f),growSpeed*Time.deltaTime);
    }

    private void AninationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Enemy>() != null)
            {
                player.stats.DoMagicalDamage(hit.GetComponent<CharacterStats>());


                ItemData_Equipment equipedAmulet=Inventory.Instance.GetEquipment(EquipmentType.Amulet);

                if (equipedAmulet != null)
                    equipedAmulet.Effect(hit.transform);
            }
    }


    public void FinishCryStal()
    {
        if (canExplode)
        {
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
            SelfDestroy();
    }

    public void SelfDestroy()=>Destroy(gameObject);
}
