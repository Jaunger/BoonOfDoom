using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHudManager : MonoBehaviour
{
    [SerializeField] CanvasGroup[] canvasGroups;

    [Header("STAT BARS")]
    [SerializeField] UI_StatBar healthBar;
    [SerializeField] UI_StatBar staminaBar;
    [SerializeField] UI_StatBar focusBar;

    [Header("Crosshair")]
    public GameObject crossHair;


    [Header("Souls")]
    [SerializeField] float soulsUpdateCountDelayTimer = 2.5f;
    private int pendingSouls = 0;
    private Coroutine waitThenAddSouls;
    [SerializeField] TextMeshProUGUI soulCountText;
    [SerializeField] TextMeshProUGUI soulsToAddText;

    [Header("Quick Slots")]
    [SerializeField] Image rightWepSlotIcon;
    [SerializeField] Image leftWepSlotIcon;
    [SerializeField] Image quickSlotIcon;
    [SerializeField] TextMeshProUGUI consumableCount;

    [Header("Boss Health Bar")]
    public Transform bossHealthBarPosition; 
    public GameObject bossHealthBarObject;
    [HideInInspector] public UI_Boss_HP_Bar currentBossHealthBar;

    public void ToggleHUD(bool status)
    {
        if (status)
        {
            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = 1;
            }
        }
        else
        {
            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = 0;
            }
        }
    }

    public void RefreshHUD()
    {
        healthBar.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(true);
        staminaBar.gameObject.SetActive(false);
        staminaBar.gameObject.SetActive(true);
    }

    public void SetSoulsCount(int soulsToAdd)
    {
        pendingSouls += soulsToAdd;

        if (waitThenAddSouls != null)
            StopCoroutine(waitThenAddSouls);

        waitThenAddSouls = StartCoroutine(WaitThenUpdateSoulCount());


    }

    private IEnumerator WaitThenUpdateSoulCount()
    {
        float timer = soulsUpdateCountDelayTimer;
        int soulsToAdd = pendingSouls;

        if (soulsToAdd >= 0)
        {
            soulsToAddText.text = "+" + soulsToAdd.ToString();
        }
        else
        {
            soulsToAddText.text = "-" + Mathf.Abs(soulsToAdd).ToString();
        }

        soulsToAddText.enabled = true;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            if (soulsToAdd != pendingSouls)
            {
                soulsToAdd = pendingSouls;
                soulsToAddText.text = "+" + soulsToAdd.ToString();
            }

            yield return null;
        }

        soulsToAddText.enabled = false;
        Debug.Log("Adding souls: " + soulsToAdd);
        soulCountText.text = PlayerUIManager.instance.player.playerStatManager.souls < 0 ?
            "0" : PlayerUIManager.instance.player.playerStatManager.souls.ToString(); 
        pendingSouls = 0;
        yield return null;
    }

    public void SetNewStaminaValue(float oldValue, float newValue)
    {
        staminaBar.SetStat(Mathf.RoundToInt(newValue));
    }

    public void SetMaxStaminaValue(int maxStamina)
    {
        staminaBar.SetMaxStat(maxStamina);
    }

    public void SetNewHealthValue(float oldValue, float newValue)
    {
        healthBar.SetStat(Mathf.RoundToInt(newValue));
    }

    public void SetMaxHealthValue(int maxHealth)
    {
        healthBar.SetMaxStat(maxHealth);
    }

    public void SetNewFocusValue(float oldValue, float newValue)
    {
        focusBar.SetStat(Mathf.RoundToInt(newValue));
    }

    public void SetMaxFocusValue(float oldValue, float newValue)
    {
        focusBar.SetMaxStat(Mathf.RoundToInt(newValue));
    }

    public void SetRightWeapSlotIcon(int weaponID)
    {
        WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);

        if (weapon == null)
        {
            rightWepSlotIcon.enabled = false;
            rightWepSlotIcon.sprite = null;
            return;
        }

        if (weapon.itemIcon == null)
        {
            // Debug.Log(weapon.itemName + " does not have an icon");
            rightWepSlotIcon.enabled = false;
            rightWepSlotIcon.sprite = null;
            return;
        }

        rightWepSlotIcon.sprite = weapon.itemIcon;
        rightWepSlotIcon.enabled = true;

    }

    public void SetLeftWeapSlotIcon(int weaponID)
    {
        WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);

        if (weapon == null)
        {
            leftWepSlotIcon.enabled = false;
            leftWepSlotIcon.sprite = null;
            return;
        }

        if (weapon.itemIcon == null)
        {
            // Debug.Log(weapon.itemName + " does not have an icon");
            leftWepSlotIcon.enabled = false;
            leftWepSlotIcon.sprite = null;
            return;
        }

        leftWepSlotIcon.sprite = weapon.itemIcon;
        leftWepSlotIcon.enabled = true;

    }

    public void SetQuickSlotIcon(QuickSlotItem quickSlotItem)
    {

        if (quickSlotItem == null)
        {
            quickSlotIcon.enabled = false;
            consumableCount.enabled = false;
            quickSlotIcon.sprite = null;
            return;
        }

        if (quickSlotItem.isConsumable)
        {
            consumableCount.enabled = true;
            consumableCount.text = quickSlotItem.itemAmount == 0 ? "" : quickSlotItem.itemAmount.ToString();
        }
        else
        {
            consumableCount.enabled = false;
        }

        if (quickSlotItem.itemIcon == null)
        {
            // Debug.Log(item.itemName + " does not have an icon");
            quickSlotIcon.enabled = false;
            quickSlotIcon.sprite = null;
            return;
        }
        if (quickSlotItem is FlaskItem && quickSlotItem.itemAmount == 0 && (quickSlotItem as FlaskItem).emptyFlaskIcon != null)
            quickSlotIcon.sprite = (quickSlotItem as FlaskItem).emptyFlaskIcon;
        else
            quickSlotIcon.sprite = quickSlotItem.itemIcon;
        quickSlotIcon.enabled = true;

    }

}
