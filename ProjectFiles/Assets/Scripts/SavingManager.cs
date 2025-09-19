using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingManager : MonoBehaviour
{
    //[SerializeField] string saveFileName = "data.json";

    public void SaveToFile()
    {
        GameManager.instance.SaveGame();

    }
}
