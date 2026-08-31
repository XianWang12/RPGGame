using UnityEngine;

public class GameManager : MonoBehaviour,ISaveManager
{
    public static GameManager instance;

    [SerializeField] private CheckPoint[] checkPoints;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);
        
        checkPoints = FindObjectsOfType<CheckPoint>();
    }

    public void LoadData(GameData _data)
    {
        foreach(var pair in _data.checkPoints)
        {
            foreach(var checkpoint in checkPoints)
            {
                if (checkpoint.id == pair.Key && pair.Value)
                    checkpoint.ActivateCheckPoint();
            }
        }

        foreach(var checkpoint in checkPoints)
        {
            if (checkpoint.id == _data.closestCheckPointId)
            {
                PlayerManager.instance.player.transform.position = checkpoint.transform.position;
                break;
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        if(FindClosestCheckPoint() != null)
            _data.closestCheckPointId = FindClosestCheckPoint().id;
        _data.checkPoints.Clear();

        foreach (var checkpoint in checkPoints)
        {
            _data.checkPoints.Add(checkpoint.id, checkpoint.activationStatus);
        }
    }

    private CheckPoint FindClosestCheckPoint()
    {
        CheckPoint closestCheckPoint = null;
        float closestDistance = Mathf.Infinity;
        foreach (var checkpoint in checkPoints)
        {
            float distance = Vector2.Distance(PlayerManager.instance.player.transform.position, checkpoint.transform.position);
            
            if (distance < closestDistance && checkpoint.activationStatus)
            {
                closestDistance = distance;
                closestCheckPoint = checkpoint;
            }
        }
        return closestCheckPoint;
    }

    public void pauseGame(bool _pause)
    {
        if(_pause)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }
}
