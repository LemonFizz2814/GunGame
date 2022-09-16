using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour, IPointerClickHandler
{
    [Header("General")]
    //public WeaponComponent weaponComponent;
    public int bandageCost;
    public int syringeCost;
    public int medkitCost;

    public int bandageHealthIncrease;
    public int syringeHealthIncrease;
    public int syringeArmourIncrease;
    public int medkitHealthIncrease;
    public int medkitArmourIncrease;

    public float distanceMultiplier;

    public float displayHitsTimer;

    public CameraScript cameraScript;

    public GameObject movePoint;
    public OverlayScript overlayScript;

    public List<GameObject> allPlayers;
    public List<GameObject> allEnemies;

    [Header("UI Menu data")]
    public GameObject gameUI;
    public MenuManager menuUI;

    [Header("Enemy screen data")]
    public GameObject enemyInfoBox;
    public GameObject shootButton;
    public AccuracyGraph accuracyGraph;
    public Slider enemyHealthSlider;
    public Slider enemyArmourSlider;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI estimatedDamageText;
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI enemyHealthText;
    public TextMeshProUGUI enemyArmourText;

    [Header("Player screen data")]
    public GameObject playerInfoBox;
    public PlayerScreenUI playerScreenUI;

    [Header("Hit screen data")]
    public GameObject hitScreen;
    public TextMeshProUGUI targetNameText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI hitText;
    public Slider targetsHealthSlider;
    public Slider targetsArmourSlider;
    public Slider targetsHealthDamageSlider;
    public Slider targetsArmourDamageSlider;
    public TextMeshProUGUI targetsHealthText;
    public TextMeshProUGUI targetsArmourText;
    public Transform hitPoints;
    public Transform missPoints;

    [Header("Selected weapon data")]
    public GameObject selectedWeaponScreen;
    public TextMeshProUGUI selectedWeaponText;
    public Image selectedWeaponIcon;

    [Header("Move select data")]
    public GameObject moveSelectScreen;
    public TextMeshProUGUI moveText;

    GameObject selectedPlayer;
    GameObject selectedTarget;

    bool primaryReady = false;
    bool secondaryReady = false;

    bool moveReady = false;

    bool canInteract;

    int wallMask = 1 << 6;

    private void Start()
    {
        playerInfoBox.SetActive(false);
        enemyInfoBox.SetActive(false);
        selectedWeaponScreen.SetActive(false);
        hitScreen.SetActive(false);
        moveSelectScreen.SetActive(false);

        movePoint.SetActive(false);

        canInteract = true;
    }

    void Update()
    {
    }
    public void WorldButtonPressed()
    {
        //Debug.Log("world button pressed");

        if (canInteract)
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log(hit.collider.gameObject.name);

                Data data = hit.collider.gameObject.GetComponent<Data>();

                if (data != null && !data.isDead)
                {
                    switch (hit.collider.transform.tag)
                    {
                        case "Player":
                            selectedPlayer = hit.collider.gameObject;
                            MoveCameraToPoint(selectedPlayer);
                            DisplayPlayerInfoScreen(selectedPlayer);
                            //canInteract = false;
                            break;

                        case "Enemy":
                            selectedTarget = hit.collider.gameObject;
                            //canInteract = false;
                            MoveCameraToPoint(selectedTarget);
                            DisplayEnemyInfoScreen(data.startingHealth, data.health, data.startingArmour, data.armour, data.playerName, data.obj);
                            break;

                        default:
                            //playerInfoBox.SetActive(false);
                            //enemyInfoBox.SetActive(false);
                            break;
                    }
                }
                else
                {
                    movePoint.transform.position = new Vector3(Mathf.Round(hit.point.x / 2) * 2, 0, Mathf.Round(hit.point.z / 2) * 2);

                    if (moveReady)
                    {
                        int distance = GetDistance(movePoint, selectedPlayer);
                        DisplayMoveSelectScreen(distance, (int)Mathf.Ceil(distance / distanceMultiplier));
                    }
                }
            }
            //}
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick: " + eventData.pointerCurrentRaycast.gameObject.name);
    }

    public WeaponComponent.WeaponStats GetPrimaryOrSecondary()
    {
        Data playerData = selectedPlayer.GetComponent<Data>();

        if (primaryReady)
        {
            return playerData.GetPrimaryStats();
        }
        else if(secondaryReady)
        {
            return playerData.GetSecondaryStats();
        }

        Debug.LogError("Couldn't find data for primary or secondary");
        return playerData.GetPrimaryStats();
    }

    public void CloseEnemyPressed()
    {
        selectedTarget = null;
        enemyInfoBox.SetActive(false);
        cameraScript.SetCanMove(true);
        canInteract = true;
    }
    public void ClosePlayerPressed()
    {
        //selectedPlayer = null;
        playerInfoBox.SetActive(false);
        cameraScript.SetCanMove(true);
        canInteract = true;
    }

    void MoveCameraToPoint(GameObject _obj)
    {
        cameraScript.SetCanMove(false);
        cameraScript.MoveCamera(_obj.transform.position);
    }

    public void TargetShot()
    {
        Data targetData = selectedTarget.GetComponent<Data>();
        Data playerData = selectedPlayer.GetComponent<Data>();

        if (!CheckWallsBetweenTargets(selectedPlayer.transform, selectedTarget.transform))
        {
            List<string> hitData = new List<string>();
            List<int> healthDmgData = new List<int>();
            List<int> armourDmgData = new List<int>();
            int healthDamage = 0;
            int armourDamage = 0;

            float distance = GetDistance(selectedPlayer, selectedTarget);
            float accuracy = GetAccuracy(distance);

            for (int i = 0; i < GetPrimaryOrSecondary().bulletsPerShot; i++)
            {
                int rand = Random.Range(0, 100);

                if (rand <= accuracy)
                {
                    int healthDmg = (targetData.armour > 0) ? (int)Mathf.Round(GetPrimaryOrSecondary().healthDamage / 2) : GetPrimaryOrSecondary().healthDamage;

                    armourDamage += GetPrimaryOrSecondary().armourDamage;
                    healthDamage += healthDmg;
                    hitData.Add("Hit -" + healthDmg + "/-" + GetPrimaryOrSecondary().armourDamage);
                    healthDmgData.Add(healthDmg);
                    armourDmgData.Add(GetPrimaryOrSecondary().armourDamage);
                }
                else
                {
                    hitData.Add("Miss");
                    healthDmgData.Add(0);
                    armourDmgData.Add(0);
                }
            }

            int previousHealth = targetData.health;
            int previousArmour = targetData.armour;
            targetData.health = Mathf.Clamp(targetData.health - healthDamage, 0, targetData.startingHealth);
            targetData.armour = Mathf.Clamp(targetData.armour - armourDamage, 0, targetData.startingArmour);
            playerData.roundTokens -= GetPrimaryOrSecondary().useCost;

            if (targetData.health <= 0)
            {
                selectedTarget.GetComponent<EnemyScript>().EnemyKilled();
            }

            overlayScript.UpdateOverlayDisplays();

            //hitData.Sort();

            CancelWeaponPressed();

            DisplayHitScreen(targetData.startingHealth, targetData.health, targetData.startingArmour, targetData.armour, previousHealth, previousArmour, targetData.playerName, hitData, healthDamage, armourDamage, healthDmgData, armourDmgData);
        }
    }

    void DisplayHitScreen(int _enemyHealthMax, int _enemyHealthValue, int _enemyArmourMax, int _enemyArmourValue, int _previousHealth, int _previousArmour, string _enemyName, List<string> _hitData, int _healthDamage, int _armourDamage, List<int> _healthDmgData, List<int> _armourDmgData)
    {
        hitScreen.SetActive(true);
        CloseEnemyPressed();
        ClosePlayerPressed();
        ResetHitPoints();

        targetsHealthSlider.maxValue    = _enemyHealthMax;
        targetsHealthDamageSlider.maxValue = _enemyHealthMax;
        targetsHealthSlider.value       = _previousHealth;//_enemyHealthValue;
        targetsHealthDamageSlider.value = _previousHealth;

        targetsArmourSlider.maxValue    = _enemyArmourMax;
        targetsArmourDamageSlider.maxValue = _enemyArmourMax;
        targetsArmourSlider.value       = _previousArmour;//_enemyArmourValue;
        targetsArmourDamageSlider.value = _previousArmour;

        targetsHealthText.text          = _enemyHealthValue + "/" + _enemyHealthMax;
        targetsArmourText.text          = _enemyArmourValue + "/" + _enemyArmourMax;

        targetNameText.text             = _enemyName;

        damageText.text                 = "Health dmg: " + 0 + "\nArmour dmg: " + 0;

        hitText.text = "";
        /*for (int i = 0; i < _hitData.Count; i++)
        {
            hitText.text += _hitData[i] + "\n";
        }*/

        float time = displayHitsTimer / _hitData.Count;

        StartCoroutine(DisplayLoop(time, 0, _hitData, _healthDmgData, _armourDmgData, 0, 0));
    }

    IEnumerator DisplayLoop(float _wait, int _i, List<string> _hitData, List<int> _healthDmgData, List<int> _armourDmgData, int _healthDmg, int _armourDmg)
    {
        hitText.text += _hitData[_i] + "\n";

        targetsArmourSlider.value -= _healthDmgData[_i];
        targetsHealthSlider.value -= _armourDmgData[_i];

        _healthDmg += _healthDmgData[_i];
        _armourDmg += _armourDmgData[_i];

        damageText.text = "Health dmg: " + _healthDmg + "\nArmour dmg: " + _armourDmg;

        if (_hitData[_i].Substring(0, 1).ToLower() == "h")
        {
            hitPoints.GetChild(Random.Range(0, hitPoints.childCount)).gameObject.SetActive(true);
        }
        else
        {
            missPoints.GetChild(Random.Range(0, missPoints.childCount)).gameObject.SetActive(true);
        }

        if (_i + 1 < _hitData.Count)
        {
            _i++;
            yield return new WaitForSeconds(_wait);
            StartCoroutine(DisplayLoop(_wait, _i, _hitData, _healthDmgData, _armourDmgData, _healthDmg, _armourDmg));
        }
        else
        {
            overlayScript.UpdateOverlayDisplays();
        }
    }

    void ResetHitPoints()
    {
        foreach(Transform child in hitPoints)
        {
            child.gameObject.SetActive(false);
        }
        foreach(Transform child in missPoints)
        {
            child.gameObject.SetActive(false);
        }
    }

    void DisplayPlayerInfoScreen(GameObject _selectedPlayer)
    {
        playerInfoBox.SetActive(true);
        enemyInfoBox.SetActive(false);

        Data playerData   = _selectedPlayer.GetComponent<Data>();
        int primaryCost   = playerData.GetPrimaryStats().useCost;
        int secondaryCost = playerData.GetSecondaryStats().useCost;

        playerScreenUI.DisplayPlayerInfo(playerData.startingHealth, playerData.health, playerData.startingArmour, playerData.armour, playerData.playerName, primaryCost, secondaryCost, playerData.roundTokens);
        playerScreenUI.ShowMainInfo();
    }

    void DisplayEnemyInfoScreen(int _enemyHealthMax, int _enemyHealthValue, int _enemyArmourMax, int _enemyArmourValue, string _enemyName, GameObject _enemyObj)
    {
        playerInfoBox.SetActive(false);
        enemyInfoBox.SetActive(true);

        enemyHealthSlider.maxValue = _enemyHealthMax;
        enemyHealthSlider.value    = _enemyHealthValue;

        enemyArmourSlider.maxValue = _enemyArmourMax;
        enemyArmourSlider.value    = _enemyArmourValue;

        enemyHealthText.text       = _enemyHealthValue + "/" + _enemyHealthMax;
        enemyArmourText.text       = _enemyArmourValue + "/" + _enemyArmourMax;

        enemyNameText.text         = _enemyName;

        shootButton.SetActive(GetPrimaryReady() || GetSecondaryReady());

        if (selectedPlayer != null)
        {
            float distance = GetDistance(selectedPlayer, _enemyObj);
            distanceText.text = "Distance: " + distance + "m";
            accuracyText.text = "Accuracy: " + GetAccuracy(distance) + "%";

            float estimatedHealthDmg = (GetPrimaryOrSecondary().healthDamage * GetPrimaryOrSecondary().bulletsPerShot) * (GetAccuracy(distance) / 100) / ((selectedTarget.GetComponent<Data>().armour > 0) ? 2 : 1);
            float estimatedArmourDmg = (GetPrimaryOrSecondary().armourDamage * GetPrimaryOrSecondary().bulletsPerShot) * (GetAccuracy(distance) / 100);

            estimatedDamageText.text = "Est Dmg: -" + estimatedHealthDmg + "/-" + estimatedArmourDmg;

            accuracyGraph.CreateGraph(GetPrimaryOrSecondary());
            accuracyGraph.CreateLine((int)distance);
        }
        else
        {
            distanceText.text = "";
            accuracyText.text = "";
            estimatedDamageText.text = "";
        }
    }

    public int GetAccuracy(float _distance)
    {
        int accuracy = 0;

        WeaponComponent.WeaponStats weaponStats = GetPrimaryOrSecondary();

        for(int i = 0; i < weaponStats.accuracy.Length; i++)
        {
            if (i + 1 < weaponStats.accuracy.Length)
            {
                if (weaponStats.accuracy[i].distance <= _distance && weaponStats.accuracy[i + 1].distance > _distance)
                {
                    Vector2 p1 = new Vector2(weaponStats.accuracy[i].distance, weaponStats.accuracy[i].accuracy);
                    Vector2 p2 = new Vector2(weaponStats.accuracy[i + 1].distance, weaponStats.accuracy[i + 1].accuracy);

                    float angle = Mathf.Atan2((p2.y - p1.y), (p2.x - p1.x));
                    float y = _distance * Mathf.Tan(angle);
                    accuracy = (int)(weaponStats.accuracy[i].accuracy + y);

                    break;
                }
            }
            else
            {
                accuracy = weaponStats.accuracy[i].accuracy;
            }
        }

        return accuracy;
    }

    void UpdatePlayerHealthAndArmour()
    {
        Data playerData = selectedPlayer.GetComponent<Data>();
        playerScreenUI.UpdateHealthAndArmour(playerData.startingHealth, playerData.health, playerData.startingArmour, playerData.armour);
        overlayScript.UpdateOverlayDisplays();
    }

    public void DisplaySelectedWeaponScreen()
    {
        selectedWeaponScreen.SetActive(true);
        selectedWeaponText.text = GetPrimaryOrSecondary().name;
        selectedWeaponIcon.sprite = GetPrimaryOrSecondary().icon;
    }
    public void CancelWeaponPressed()
    {
        selectedWeaponScreen.SetActive(false);
        primaryReady = false;
        secondaryReady = false;
    }

    public bool CheckWallsBetweenTargets(Transform _target1, Transform _target2)
    {
        Vector3 direction = (_target2.position - _target1.position).normalized;
        float distance = Vector3.Distance(_target1.position, _target2.position);

        RaycastHit[] rayHits;

        Debug.DrawRay(_target1.position, direction * distance, Color.red, 5);
        rayHits = Physics.RaycastAll(_target1.position, direction, distance, wallMask);

        if (rayHits.Length > 0)
        {
            for (int i = 0; i < rayHits.Length; i++)
            {
                if (rayHits[i].transform.CompareTag("MetalWall"))// || rayHits[i].transform.CompareTag("WoodenWall"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void DisplayMoveSelectScreen(int _distance, int _cost)
    {
        moveSelectScreen.SetActive(true);
        moveText.text = "Distance: " + _distance + "m\nCost: " + _cost;
    }
    public void MovePressed(bool _activate)
    {
        moveReady = _activate;
        playerInfoBox.SetActive(!_activate);
        moveSelectScreen.SetActive(_activate);

        canInteract = _activate;
        cameraScript.SetCanMove(_activate);
        cameraScript.MoveCamera(selectedPlayer.transform.position);

        movePoint.SetActive(_activate);
        movePoint.transform.position = selectedPlayer.transform.position;
    }
    public void MovePlayerPressed()
    {
        int distance = GetDistance(movePoint, selectedPlayer);
        int cost = (int)Mathf.Ceil(distance / distanceMultiplier);

        if (cost <= selectedPlayer.GetComponent<Data>().roundTokens)
        {
            selectedPlayer.transform.position = movePoint.transform.position;
            selectedPlayer.GetComponent<Data>().roundTokens -= cost;
            MovePressed(false);
            overlayScript.UpdateOverlayDisplays();
            canInteract = true;
        }
        else
        {
            Debug.Log("not enough tokens");
        }
    }

    public void EndTurnPressed()
    {
        for (int i = 0; i < allPlayers.Count; i++)
        {
            Data playerData = allPlayers[i].GetComponent<Data>();

            int armourIncrease = playerData.roundTokens * playerData.armourIncrease;
            playerData.armour = Mathf.Clamp(playerData.armour + armourIncrease, 0, playerData.startingArmour);

            playerData.roundTokens = playerData.startingTokens;
        }

        canInteract = false;

        StartOfTurn();
    }

    void StartOfTurn()
    {
        canInteract = true;
    }

    public void PlayerDied(GameObject _gameObject)
    {
        allPlayers.Remove(_gameObject);
    }
    public void EnemyDied(GameObject _gameObject)
    {
        allEnemies.Remove(_gameObject);
        overlayScript.RemoveEnemy(_gameObject);
    }

    public void CloseHitScreen()
    {
        hitScreen.SetActive(false);
    }

    public bool GetPrimaryReady()
    {
        return primaryReady;
    }
    public void SetPrimaryReady(bool _isReady)
    {
        primaryReady = _isReady;
    }
    public bool GetSecondaryReady()
    {
        return secondaryReady;
    }
    public void SetSecondaryReady(bool _isReady)
    {
        secondaryReady = _isReady;
    }

    public void MenuOpenPressed()
    {
        menuUI.OpenMenu();
        gameUI.SetActive(false);
        canInteract = false;
        cameraScript.SetCanMove(false);
    }
    public void ExitedMenu()
    {
        canInteract = true;
        gameUI.SetActive(true);
        cameraScript.SetCanMove(true);
    }

    public void PrimaryButtonPressed()
    {
        Data data = selectedPlayer.GetComponent<Data>();

        if (data.roundTokens >= data.GetPrimaryStats().useCost)
        {
            SetPrimaryReady(true);
            SetSecondaryReady(false);
            ClosePlayerPressed();
            DisplaySelectedWeaponScreen();
        }
    }
    public void SecondaryButtonPressed()
    {
        Data data = selectedPlayer.GetComponent<Data>();

        if (data.roundTokens >= data.GetSecondaryStats().useCost)
        {
            SetPrimaryReady(false);
            SetSecondaryReady(true);
            ClosePlayerPressed();
            DisplaySelectedWeaponScreen();
        }
    }
    public void BandagePressed()
    {
        Data data = selectedPlayer.GetComponent<Data>();
        if (data.roundTokens >= bandageCost)
        {
            data.roundTokens -= bandageCost;
            data.health = Mathf.Clamp(bandageHealthIncrease, 0, data.startingHealth);
            overlayScript.UpdateOverlayDisplays();
        }
        UpdatePlayerHealthAndArmour();
    }
    public void SyringePressed()
    {
        Data data = selectedPlayer.GetComponent<Data>();
        if (data.roundTokens >= syringeCost)
        {
            data.roundTokens -= syringeCost;
            data.health = Mathf.Clamp(syringeHealthIncrease, 0, data.startingHealth);
            data.armour = Mathf.Clamp(syringeArmourIncrease, 0, data.startingArmour);
            overlayScript.UpdateOverlayDisplays();
        }
        UpdatePlayerHealthAndArmour();
    }
    public void MedkitPressed()
    {
        Data data = selectedPlayer.GetComponent<Data>();

        if (data.roundTokens >= medkitCost)
        {
            data.roundTokens -= medkitCost;
            data.health = Mathf.Clamp(medkitHealthIncrease, 0, data.startingHealth);
            data.armour = Mathf.Clamp(medkitArmourIncrease, 0, data.startingArmour);
            overlayScript.UpdateOverlayDisplays();
        }
        UpdatePlayerHealthAndArmour();
    }

    public int GetDistance(GameObject _objectA, GameObject _objectB)
    {
        return (int)(Mathf.Round(Vector3.Distance(_objectA.transform.position, _objectB.transform.position) / 2) * distanceMultiplier);
    }

    public List<GameObject> GetAllPeople()
    {
        List<GameObject> playersAndEnemies = new List<GameObject>();
        playersAndEnemies.AddRange(allPlayers);
        playersAndEnemies.AddRange(allEnemies);
        //print("playersAndEnemies " + playersAndEnemies.Count);
        return playersAndEnemies;
    }
}
