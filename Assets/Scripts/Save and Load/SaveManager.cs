using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;
    
    private GameData gameData;
    private List<ISaveManager> isaveManagers;
    private FileDataHandler dataHandler;

    [ContextMenu("Delete Saved Data")]
    private void DeleteSavedData()
    {
        dataHandler=new FileDataHandler(Application.persistentDataPath,fileName,encryptData);
        dataHandler.Delete();
    }

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);
    }

    private void Start()
    {
        dataHandler=new FileDataHandler(Application.persistentDataPath,fileName,encryptData);

        isaveManagers = FindAllISaveManagers();

        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }
    public void LoadGame()
    {
        gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            NewGame();
            Debug.Log("no saved game found");
        }

        foreach(ISaveManager isaveManager in isaveManagers)
        {
            isaveManager.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach(ISaveManager saveManager in isaveManagers)
        {
            saveManager.SaveData(ref gameData);
        }

        dataHandler.Save(gameData); 
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<ISaveManager> FindAllISaveManagers()
    {
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveManager>();
        return new List<ISaveManager>(saveManagers);
    }
}
