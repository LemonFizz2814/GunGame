using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Data : MonoBehaviour
{
    public int primaryWeapon;
    public int secondaryWeapon;
    public StoredAmmunition[] ammunition = new StoredAmmunition[Enum.GetValues(typeof(WeaponComponent.Ammunition)).Length];

    public int startingHealth;
    [NonSerialized]
    public int health;

    public int startingArmour;
    [NonSerialized]
    public int armour;

    [NonSerialized]
    public int armourIncrease = 5;

    public int frags;
    public int bandages;
    public int syringes;
    public int medpack;

    public int roundTokens;
    [NonSerialized]
    public int startingTokens = 10;

    public string playerName;

    public GameObject obj;

    public bool isDead;

    [Serializable]
    public struct StoredAmmunition
    {
        public WeaponComponent.Ammunition ammunition;
        public int amount;
    }

    WeaponComponent weaponComponent;

    private void Start()
    {
        weaponComponent = GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<WeaponComponent>();
        health = startingHealth;
        armour = startingArmour;
        isDead = false;
    }

    public WeaponComponent.WeaponStats GetPrimaryStats()
    {
        return weaponComponent.GetWeaponStats()[primaryWeapon];
    }
    public WeaponComponent.WeaponStats GetSecondaryStats()
    {
        return weaponComponent.GetWeaponStats()[secondaryWeapon];
    }
}
