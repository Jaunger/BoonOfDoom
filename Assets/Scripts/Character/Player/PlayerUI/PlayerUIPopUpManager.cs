using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIPopUpManager : MonoBehaviour
{
    [Header("Pop Up Message")]
    [SerializeField] private GameObject popUpGameObject;

    [SerializeField] private TextMeshProUGUI popUpText;

    [Header("Item Pop Up")]
    [SerializeField] private GameObject itemPopUpGameObject;

    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemAmount;

    [Header("YOU DIED Pop Up")]
    [SerializeField] private GameObject youDiedPopUpGameObject;
    [SerializeField] private TextMeshProUGUI youDiedPopUpBackgroundText;
    [SerializeField] private TextMeshProUGUI youDiedPopUpText;
    [SerializeField] private CanvasGroup youDiedPopUpCanvasGroup;

    [Header("Boss defeated Pop Up")]
    [SerializeField] private GameObject bossDefeatedPopUpGameObject;
    [SerializeField] private TextMeshProUGUI bossDefeatedPopUpBackgroundText;
    [SerializeField] private TextMeshProUGUI bossDefeatedPopUpText;
    [SerializeField] private CanvasGroup bossDefeatedPopUpCanvasGroup;
   
    [Header("Brazier Pop Up")]
    [SerializeField] private GameObject brazierPopUpGameObject;
    [SerializeField] private TextMeshProUGUI brazierPopUpBackgroundText;
    [SerializeField] private TextMeshProUGUI brazierPopUpText;
    [SerializeField] private CanvasGroup brazierPopUpCanvasGroup;

    public void SendYouDiedPopUp()
    {
        //  Active post processing effects
        Debug.Log("You Died Pop Up Triggered");
        youDiedPopUpGameObject.SetActive(true);
        youDiedPopUpBackgroundText.characterSpacing = 0;
        StartCoroutine(StretchPopUPTextOverTime(youDiedPopUpBackgroundText, 8, 20f));
        StartCoroutine(FadeInPopUpOverTime(youDiedPopUpCanvasGroup, 3));
        StartCoroutine(WaitThenFadeOutPopUpOverTime(youDiedPopUpCanvasGroup, 2, 3));
    }

    public void SendBossDefeatedPopUp(string bossDefeatedMessage)
    {
        bossDefeatedPopUpText.text = bossDefeatedMessage;
        bossDefeatedPopUpBackgroundText.text = bossDefeatedMessage;

        bossDefeatedPopUpGameObject.SetActive(true);
        bossDefeatedPopUpBackgroundText.characterSpacing = 0;
        StartCoroutine(StretchPopUPTextOverTime(bossDefeatedPopUpBackgroundText, 8, 20f));
        StartCoroutine(FadeInPopUpOverTime(bossDefeatedPopUpCanvasGroup, 5));
        StartCoroutine(WaitThenFadeOutPopUpOverTime(bossDefeatedPopUpCanvasGroup, 2, 5));
    }

    private IEnumerator StretchPopUPTextOverTime(TextMeshProUGUI text, float duration, float strechAmount)
    {
        if (duration > 0f)
        {
            text.characterSpacing = 0;
            float timer = 0;

            yield return null;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                text.characterSpacing = Mathf.Lerp(text.characterSpacing, strechAmount, duration * (Time.deltaTime / 20));
                yield return null;
            }
        }
    }

    private IEnumerator FadeInPopUpOverTime(CanvasGroup c, float duration)
    {
        if (duration > 0f)
        {
            c.alpha = 0;
            float timer = 0;

            yield return null;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                c.alpha = Mathf.Lerp(c.alpha, 1, duration * Time.deltaTime);
                yield return null;
            }
        }

        c.alpha = 1;
        yield return null;
    }

    private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup c, float duration, float delay)
    {
        if (duration > 0f)
        {
            while (delay > 0f)
            {
                delay -= Time.deltaTime;
                yield return null;
            }

            c.alpha = 1;
            float timer = 0;

            yield return null;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                c.alpha = Mathf.Lerp(c.alpha, 1, duration * Time.deltaTime);
                yield return null;
            }
        }

        c.alpha = 0;
        yield return null;
    }

    internal void SendPlayerMessagePopUp(string msgText)
    {
        PlayerUIManager.instance.popUpIsOpen = true;
        popUpText.text = msgText;
        popUpGameObject.SetActive(true);
    }

    public void CloseAllPopUpWindows()
    {
        popUpGameObject.SetActive(false);
        itemPopUpGameObject.SetActive(false);
        PlayerUIManager.instance.popUpIsOpen = false;
    }

    public void SendBrazierPopUp(string msgText)
    {
        brazierPopUpText.text = msgText;
        brazierPopUpBackgroundText.text = msgText;
        brazierPopUpGameObject.SetActive(true);
        brazierPopUpBackgroundText.characterSpacing = 0;
        StartCoroutine(StretchPopUPTextOverTime(brazierPopUpBackgroundText, 8, 19));
        StartCoroutine(FadeInPopUpOverTime(brazierPopUpCanvasGroup, 5));
        StartCoroutine(WaitThenFadeOutPopUpOverTime(brazierPopUpCanvasGroup, 2, 5));
    }

    public void SendItemPopUp(Item item, int amount)
    {
        itemAmount.enabled = false;
        itemIcon.sprite = item.itemIcon;
        itemName.text = item.itemName;

        if (amount > 1)
        {
            itemAmount.enabled = true;
            itemAmount.text = "x" + amount.ToString();
        }

        itemPopUpGameObject.SetActive(true);
        PlayerUIManager.instance.popUpIsOpen = true;
    }
}