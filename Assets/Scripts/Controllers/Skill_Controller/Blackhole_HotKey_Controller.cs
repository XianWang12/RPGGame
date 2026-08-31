using TMPro;
using UnityEngine;

public class Blackhole_HotKey_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotKey;
    private TextMeshProUGUI myText;

    public Transform myEnemy;
    public Blackhole_Skill_Controller blockHole;

    public void SetupHotKey(KeyCode _myNewHotKey,Transform _myEnemy,Blackhole_Skill_Controller _blockHole)
    {
        sr=GetComponent<SpriteRenderer>();
        myText =GetComponentInChildren<TextMeshProUGUI>();

        myEnemy = _myEnemy;
        blockHole = _blockHole;

        myHotKey = _myNewHotKey;
        myText.text= _myNewHotKey.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyUp(myHotKey))
        {
            blockHole.AddEnemyToList(myEnemy);

            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
