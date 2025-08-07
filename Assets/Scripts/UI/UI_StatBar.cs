using UnityEngine;
using UnityEngine.UI;

public class UI_StatBar : MonoBehaviour
{

    protected Slider slider;
    protected RectTransform rectTransform;


    [Header("Bar Options")]
    [SerializeField] protected bool sclaeBarLengthWithStats = true;
    [SerializeField] protected float widthScaleMultiplier = 1;

    //SECONDARY FOR POLISH - how much stamina / hp lost


    protected virtual void Awake()
    {
        slider = GetComponent<Slider>();    
        rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void Start()
    {

    }
    public virtual void SetStat(int newValue)
    {
        slider.value = newValue;
    }

    public virtual void SetMaxStat(int maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = maxValue;

        if (sclaeBarLengthWithStats)
        {
            rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y);
            PlayerUIManager.instance.hudManager.RefreshHUD();
        }
    }
}
