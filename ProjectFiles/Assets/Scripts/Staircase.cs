using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staircase : MonoBehaviour
{
    [SerializeField] private bool changeScene;
    [SerializeField] private int sceneChange;
    [SerializeField] private Vector2 position;

    public bool GetChangeScene()
    {
        return changeScene;
    }
    public int GetSceneChangeIndex()
    {
        return sceneChange;
    }

    public Vector2 GetSceneChangePos()
    {
        return position;
    }
}
