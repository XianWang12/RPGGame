using UnityEngine;

[CreateAssetMenu(fileName = "Ice effect", menuName = "Data/Item effect/Ice ")]
public class Ice_Effect : ItemEffect
{
    [SerializeField] private GameObject icePrefab;
    [SerializeField] private float xVelocity;

    public override void ExecuteEffect(Transform _respondPosition)
    {
        Player player=PlayerManager.instance.player;

        bool thirdAttack = player.primaryAttack.comboCounter == 2;

        if (thirdAttack)
        {
            GameObject newIce=Instantiate(icePrefab, _respondPosition.position,player.transform.rotation);
            newIce.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * player.facingDir, 0);
            Destroy(newIce,.7f);
        }
        
    }
}
