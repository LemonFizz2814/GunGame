using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    [System.Serializable]
    public struct WeaponStats
    {
        public string name;
        public Sprite icon;
        public GunType type;
        public Ammunition ammunition;
        public PrimaryOrSecondary primaryOrSecondary;
        public int bulletsPerShot;
        public int shotsTillReload;
        public int healthDamage;
        public int armourDamage;
        public int recoil;
        public int useCost;
        public int reloadCost;
        public AccuracyRange[] accuracy;

        [System.NonSerialized]
        public int number;
    }

    [System.Serializable]
    public struct AccuracyRange
    {
        public int distance;
        public int accuracy;
    }

    public enum GunType
    {
        Rifle,
        SMG,
        Sniper,
        LMG,
        Shotgun,
        Pistol,
        Revolver,
        AutoPistol,
    }
    public enum Ammunition
    {
        pistol,
        light,
        assualt,
        heavy,
        pellet,
        explosive,
    }
    public enum PrimaryOrSecondary
    {
        Primary,
        Secondary,
    }

    public WeaponStats[] weaponStats;

    private void Awake()
    {
        for(int i = 0; i < weaponStats.Length; i++)
        {
            weaponStats[i].number = i;
        }
    }

    public WeaponStats[] GetWeaponStats()
    {
        return weaponStats;
    }
}
