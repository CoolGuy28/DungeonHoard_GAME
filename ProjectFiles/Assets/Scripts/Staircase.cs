using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staircase : MonoBehaviour
{
    [SerializeField] private int sceneChange;
    [SerializeField] private Vector2 position;

    public int GetSceneChangeIndex()
    {
        return sceneChange;
    }

    public Vector2 GetSceneChangePos()
    {
        return position;
    }
}
