using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public bool healsHP;
    public int healAmount;
    public int itemAmount;
    public string description;
    public Sprite icon;
}
