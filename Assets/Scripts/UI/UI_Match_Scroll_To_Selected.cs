using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Match_Scroll_To_Selected : MonoBehaviour
{

    [SerializeField] GameObject currentSelected;
    [SerializeField] GameObject previouslySelected;
    [SerializeField] RectTransform currentSelectedTransform;
    [SerializeField] RectTransform contentPanel;
    [SerializeField] ScrollRect scrollRect;


    private void Update()
    {
        currentSelected = EventSystem.current.currentSelectedGameObject;


        if (currentSelectedTransform != null )
        {
            if (currentSelected == null)
                return;
            if (currentSelected != previouslySelected)
            {
                previouslySelected = currentSelected;
                currentSelectedTransform = currentSelected.GetComponent<RectTransform>();
                SnapTo(currentSelectedTransform);    
            }
        }


    }

    private void SnapTo(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        Vector2 newPos = (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position) - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
    
        newPos.x = 0;

        contentPanel.anchoredPosition = newPos; 
    }
}
