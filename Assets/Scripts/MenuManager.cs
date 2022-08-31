using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public Transform lineParent;
    public RectTransform weaponScrollView;
    public TextMeshProUGUI informationText;
    public TextMeshProUGUI titleText;
    public Image weaponIcon;
    public WeaponComponent weaponComponent;

    [Header("Menus")]
    public GameObject weaponsSelectionMenu;
    public GameObject weaponInfoMenu;
    public GameObject gunMenu;

    [Header("Prefabs")]
    public GameObject weaponButtonPrefab;

    [Header("Weapon select data")]
    public float Xdist;
    public float Ydist;

    [Header("Graph data")]
    public float graphX;
    public float graphY;
    public float lineThickness;

    private void Start()
    {
        //DisplayWeaponInfo(0);
        //DisplayWeaponList();
        BackToGunMenuPressed();
    }

    void DisplayWeaponInfo(int _weaponNum)
    {
        weaponsSelectionMenu.SetActive(false);
        weaponInfoMenu.SetActive(true);
        gunMenu.SetActive(false);

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

        CreateGraph(weaponStats);
    }

    void CreateGraph(WeaponComponent.WeaponStats _weaponStats)
    {
        foreach(Transform child in lineParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < _weaponStats.accuracy.Length - 1; i++)
        {
            Vector2 positionA = new Vector3((_weaponStats.accuracy[i].distance * graphX) + 3.0f, (_weaponStats.accuracy[i].accuracy * graphY) + 3.0f);
            Vector2 positionB = new Vector3((_weaponStats.accuracy[i + 1].distance * graphX) + 3.0f, (_weaponStats.accuracy[i + 1].accuracy * graphY) + 3.0f);

            GameObject lineObj = new GameObject("lineObj" + i, typeof(Image));
            lineObj.transform.SetParent(lineParent);

            Vector2 dir = (positionA - positionB).normalized;
            //float angle = Vector2.Angle(positionA, positionB);
            float angle = Mathf.Atan2(positionB.y - positionA.y, positionB.x - positionA.x) * Mathf.Rad2Deg;
            float distance = Vector3.Distance(positionA, positionB);

            RectTransform rectTransform = lineObj.transform.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(distance, lineThickness);
            rectTransform.anchoredPosition =  positionA + dir * distance * -0.5f;
            rectTransform.localEulerAngles = new Vector3(0, 0, angle);
        }
    }

    void DisplayWeaponList(WeaponComponent.GunType _gunType)
    {
        weaponsSelectionMenu.SetActive(true);
        weaponInfoMenu.SetActive(false);
        gunMenu.SetActive(false);

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

            weaponButtonObj.name = weaponList[i].number + "WeaponButton";
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
        gunMenu.SetActive(false);
    }
    public void BackToGunMenuPressed()
    {
        weaponsSelectionMenu.SetActive(false);
        weaponInfoMenu.SetActive(false);
        gunMenu.SetActive(true);
    }

    public void WeaponPressed(GameObject _buttonObj)
    {
        int weaponNum = int.Parse(_buttonObj.name.Substring(0, 1));
        DisplayWeaponInfo(weaponNum);
    }

    public void ExitMenu()
    {
        weaponsSelectionMenu.SetActive(false);
        weaponInfoMenu.SetActive(false);
        gunMenu.SetActive(false);
    }
}
