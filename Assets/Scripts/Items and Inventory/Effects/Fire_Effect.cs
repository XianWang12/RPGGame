using UnityEngine;

[CreateAssetMenu(fileName = "Fire effect", menuName = "Data/Item effect/Fire ")]
public class Fire_Effect : ItemEffect
{
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private float xVelocity;

    public override void ExecuteEffect(Transform _respondPosition)
    {
        Player player=PlayerManager.instance.player;

        bool thirdAttack = player.primaryAttack.comboCounter == 2;

        if (thirdAttack)
        {
            GameObject newFire = Instantiate(firePrefab, _respondPosition.position, player.transform.rotation);
            newFire.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * player.facingDir, 0);
            Destroy(newFire, .7f);
        }

    }

}
