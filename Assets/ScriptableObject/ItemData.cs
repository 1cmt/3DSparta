using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//각각의 아이템은 활용법이 다르겠지 이걸 enum으로 
public enum ItemType
{
    Equipable, //장착이 가능한 아이템
    Consumable, //섭취(소비)가 가능한
    Resource, //돌같은 단순자원
}

//소비(섭취)를 하더라도 체력을 늘려주는건지.. 배고픔을 늘려주는건지.. 구분할 필요 있음
public enum ConsumableType
{
    Health,
    Hunger,
    Boost
}

[Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public ItemType type;
    public Sprite icon;
    public GameObject dropPrefab;

    [Header("Stacking")]
    public bool canStack; //여러개 가지고 있을 수 있는 아이템인지
    public int maxStackAmount; //얼마나 가질 수 있는지

    [Header("Consumable")]
    public ItemDataConsumable[] consumables; //섭취를 통해 체력도 배고픔도 늘려줄 수 있음 

    [Header("Equip")]
    public GameObject equipPrefab;
}

