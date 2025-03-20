using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : ScriptableObject
{
    public string unitName;
    public int attack = 10;
    public int health = 100;
    public float speed = 10;
    public float accuracy = 30;
    public float critChance = 30;
}
