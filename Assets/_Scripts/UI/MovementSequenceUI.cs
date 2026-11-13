using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MovementSequenceUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject movementArrowPrefab;
    [SerializeField] private Transform contentContainer;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Highlight Colours")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color activeColor = Color.green;

    private readonly List<GameObject> spawnedArrows = new List<GameObject>();

    public void AddMovement(MovementDirection direction)
    {
        GameObject newArrow = Instantiate(movementArrowPrefab, contentContainer);

        TMP_Text arrowText = newArrow.GetComponentInChildren<TMP_Text>();

        if (arrowText != null)
        {
            switch (direction)
            {
                case MovementDirection.Up:

                    arrowText.text = "↑";
                    break;

                case MovementDirection.Down:
                    arrowText.text = "↓";
                    break;

                case MovementDirection.Left:
                    arrowText.text = "←";
                    break;

                case MovementDirection.Right:
                    arrowText.text = "→";
                    break;
            }
        }

        spawnedArrows.Add(newArrow);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentContainer as RectTransform);
        scrollRect.horizontalNormalizedPosition = 1f;
    }


    public void HighlightMovement(int index)
    {
        for (int i = 0; i < spawnedArrows.Count; i++)
        {
            TMP_Text arrowText = spawnedArrows[i].GetComponentInChildren<TMP_Text>();

            if (arrowText == null) continue;

            arrowText.color = i == index ? activeColor : normalColor;
        }

        if (index >= 0 && index < spawnedArrows.Count)
        {
            RectTransform target = spawnedArrows[index].GetComponent<RectTransform>();

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentContainer as RectTransform);

            float scrollableWidth = ((RectTransform)contentContainer).rect.width - scrollRect.viewport.rect.width;

            if (scrollableWidth > 0f)
            {
                float targetCenter = target.anchoredPosition.x + target.rect.width * 0.5f;
                float desiredScroll = targetCenter - scrollRect.viewport.rect.width * 0.5f;

                scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(desiredScroll / scrollableWidth);
            }
        }
    }




    public void ClearSequence()
    {
        foreach (GameObject arrow in spawnedArrows)
        {
            if (arrow != null) Destroy(arrow);
        }

        spawnedArrows.Clear();

        scrollRect.horizontalNormalizedPosition = 0f;
    }






}
