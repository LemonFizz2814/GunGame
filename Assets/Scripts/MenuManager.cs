using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public RectTransform weaponScrollView;
    public TextMeshProUGUI informationText;
    public TextMeshProUGUI titleText;
    public Image weaponIcon;
    public WeaponComponent weaponComponent;
    public UIManager uiManager;
    public AccuracyGraph accuracyGraph;

    [Header("Menus")]
    public GameObject weaponsSelectionMenu;
    public GameObject weaponInfoMenu;
    public GameObject weaponCategoryMenu;

    [Header("Prefabs")]
    public GameObject weaponButtonPrefab;

    [Header("Weapon select data")]
    public float Xdist;
    public float Ydist;

    private void Start()
    {
        //DisplayWeaponInfo(0);
        //DisplayWeaponList();
        //BackToGunMenuPressed();
    }

    void DisplayWeaponInfo(int _weaponNum)
    {
        weaponsSelectionMenu.SetActive(false);
        weaponInfoMenu.SetActive(true);
        weaponCategoryMenu.SetActive(false);

        WeaponComponent.WeaponStats weaponStats = weaponComponent.weaponStats[_weaponNum];

        informationText.text =
            weaponStats.name + "\n" +
            weaponStats.type.ToString() + "\n" +
            weaponStats.ammunition.ToString() + "\n" + "\n" +
            weaponStats.bulletsPerShot.ToString() + "\n" +
            weaponStats.shotsTillReload.ToString() + "\n" +
            "-" + weaponStats.healthDamage.ToString() + "\n" +
            "-" + weaponStats.armourDamage.ToString() + "\n" +
            "+" + weaponStats.recoil.ToString() + "%\n" + "\n" +
            weaponStats.useCost.ToString() + "\n" +
            weaponStats.reloadCost.ToString();

        weaponIcon.sprite = weaponStats.icon;

        accuracyGraph.CreateGraph(weaponStats);
    }

    void DisplayWeaponList(WeaponComponent.GunType _gunType)
    {
        weaponsSelectionMenu.SetActive(true);
        weaponInfoMenu.SetActive(false);
        weaponCategoryMenu.SetActive(false);

        foreach (Transform child in weaponScrollView.transform)
        {
            Destroy(child.gameObject);
        }

        List<WeaponComponent.WeaponStats> weaponList = new List<WeaponComponent.WeaponStats>();

        for(int i = 0; i < weaponComponent.GetWeaponStats().Length; i++)
        {
            if(weaponComponent.GetWeaponStats()[i].type == _gunType || (int)_gunType == -1)
            {
                weaponList.Add(weaponComponent.GetWeaponStats()[i]);
            }
        }

        weaponScrollView.sizeDelta = new Vector2(0, Ydist * Mathf.Ceil((float)weaponList.Count / 2));

        for (int i = 0; i < weaponList.Count; i++)
        {
            GameObject weaponButtonObj = Instantiate(weaponButtonPrefab);
            weaponButtonObj.transform.SetParent(weaponScrollView.transform);

            weaponButtonObj.name = "WeaponButton" + weaponList[i].number;
            weaponButtonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = weaponList[i].name;
            weaponButtonObj.transform.GetChild(1).GetComponent<Image>().sprite = weaponList[i].icon;

            RectTransform rectTransform = weaponButtonObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 1);
            rectTransform.anchorMax = new Vector2(0.5f, 1);
            rectTransform.pivot = new Vector2(0.5f, 1.02f);
            rectTransform.anchoredPosition = new Vector2(Xdist * ((i % 2 == 0) ? -1 : 1), -Ydist * Mathf.Floor(i / 2));

            weaponButtonObj.GetComponent<Button>().onClick.AddListener(() => { WeaponPressed(weaponButtonObj); });
        }

        if (weaponList.Count > 0)
        {
            titleText.text = "" + weaponList[0].type;
        }
        if ((int)_gunType == -1)
        {
            titleText.text = "All Guns";
        }
    }

    public void GunButtonPressed(int _gunType)
    {
        WeaponComponent.GunType guntype = (WeaponComponent.GunType)_gunType;
        DisplayWeaponList(guntype);
    }

    public void BackToSelectionMenuPressed()
    {
        weaponsSelectionMenu.SetActive(true);
        weaponInfoMenu.SetActive(false);
        weaponCategoryMenu.SetActive(false);
    }
    public void BackToGunMenuPressed()
    {
        weaponsSelectionMenu.SetActive(false);
        weaponInfoMenu.SetActive(false);
        weaponCategoryMenu.SetActive(true);
    }

    public void WeaponPressed(GameObject _buttonObj)
    {
        int weaponNum = int.Parse(_buttonObj.name.Substring(12, _buttonObj.name.Length - 12));

        DisplayWeaponInfo(weaponNum);
    }

    public void ExitMenu()
    {
        weaponsSelectionMenu.SetActive(false);
        weaponInfoMenu.SetActive(false);
        weaponCategoryMenu.SetActive(false);
        uiManager.ExitedMenu();
    }

    public void OpenMenu()
    {
        weaponsSelectionMenu.SetActive(false);
        weaponInfoMenu.SetActive(false);
        weaponCategoryMenu.SetActive(true);
    }
}
