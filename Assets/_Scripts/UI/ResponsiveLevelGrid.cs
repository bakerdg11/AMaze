using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ResponsiveLevelGrid : MonoBehaviour
{
    [SerializeField] private int columns = 5;
    [SerializeField] private int rows = 5;

    private GridLayoutGroup grid;
    private RectTransform rectTransform;

    private void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
        grid.childAlignment = TextAnchor.UpperLeft;
    }

    private void Start()
    {
        UpdateGrid();
    }

    private void OnRectTransformDimensionsChange()
    {
        if (grid == null || rectTransform == null) return;

        UpdateGrid();
    }

    private void UpdateGrid()
    {
        float width = rectTransform.rect.width - grid.padding.left - grid.padding.right;
        float height = rectTransform.rect.height - grid.padding.top - grid.padding.bottom;

        float cellWidth = (width - grid.spacing.x * (columns - 1)) / columns;
        float cellHeight = (height - grid.spacing.y * (rows - 1)) / rows;

        grid.cellSize = new Vector2(cellWidth, cellHeight);
    }




}
