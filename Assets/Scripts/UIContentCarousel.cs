/* SOURCE: https://github.com/UnityVista/Sandwich/blob/main/README.md?plain=1 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIContentCarousel : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{
    [Header("Important Information")]
    [Tooltip("Ensure that a GridLayoutGroup is assigned to CONTENT. The correct layout group is crucial for the proper functioning of the carousel.")]
    [TextArea(3, 5)]
    public string layoutGroupWarning;

    public enum LayoutType
    {
        Vertical,
        Horizontal
    }

    [Header("Layout Settings")]
    [Tooltip("Select the layout type for the content carousel.")]
    public LayoutType layoutType = LayoutType.Vertical;

    [Header("Content Viewport")]
    [Tooltip("Specify the size of each page in the carousel.")]
    public float pageSize = 600f;

    [Tooltip("The threshold for swipe detection. Adjust for sensitivity.")]
    public float swipeThreshold = 0.2f;

    [Tooltip("The speed at which the content snaps to the target position.")]
    public float snapSpeed = 8f;

    [Header("Navigation Buttons")]
    [Tooltip("Click to move to the next page.")]
    public Button nextButton;

    [Tooltip("Click to move to the previous page.")]
    public Button prevButton;

    [Header("Carousel Mode")]
    [Tooltip("Enables automatic cycling through pages.")]
    public bool carouselMode = false;

    [Tooltip("Starts automatic page cycling when enabled.")]
    public bool autoMove = false;

    [Tooltip("Adjusts the time interval between automatic page transitions (in seconds).")]
    public float autoMoveTimer = 5f;

    [Header("Navigation Dots")]
    [Tooltip("Prefab for navigation dots.")]
    public GameObject dotPrefab;

    [Tooltip("Container to hold the navigation dots.")]
    public GameObject dotsContainer;

    [Header("Dot Colors")]
    [Tooltip("Sets the color of the dot representing the current page.")]
    public Color activeDotColor = Color.yellow; // Yellow/Goldish

    [Tooltip("Sets the color of the dots representing inactive pages.")]
    public Color inactiveDotColor = Color.grey;

    [Tooltip("Controls the speed of the color transition between dots (in seconds).")]
    public float dotColorTransitionSpeed = 5f;

    [Header("Dot Scaling")]
    [Tooltip("The size of the active navigation dot.")]
    public Vector2 activeDotSize = new Vector2(20f, 10f);

    [Tooltip("The size of inactive navigation dots.")]
    public Vector2 inactiveDotSize = new Vector2(10f, 10f);

    [Tooltip("The speed at which the navigation dots scale.")]
    public float dotScalingSpeed = 5f;

    [Header("3D Rotation")]
    [Tooltip("The maximum rotation angle for content items.")]
    public float maxRotationAngle = 45f;

    [Tooltip("The speed of the rotation effect.")]
    public float rotationSpeed = 5f;

    [Header("(Experimental Features)")]
    [Header("Infinite Looping")]
    [Tooltip("Enable or disable infinite looping for the carousel.")]
    public bool infiniteLooping = true;

    [Header("Checking...")]
    [Tooltip("Indicates the total number of pages.")]
    public int totalPages;

    [Tooltip("Specifies the index of the currently displayed page.")]
    public int currentIndex = 0;

    [Tooltip("Manages the layout of page elements.")]
    public GridLayoutGroup gridLayoutGroup;

    //private variables//...
    private ScrollRect scrollRect;
    private RectTransform contentRectTransform;
    private float targetPosition;
    private bool isDragging = false;
    private Vector2 dragStartPos;
    private float lastDragTime;
    private float autoMoveTimerCountdown;

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();

        if (gridLayoutGroup == null)
        {
            Debug.LogError("GridLayoutGroup not found in children. Make sure it is present.");
            return;
        }

        contentRectTransform = gridLayoutGroup.transform as RectTransform;

        if (contentRectTransform == null)
        {
            Debug.LogError("RectTransform not found on the GridLayoutGroup. Make sure it is present.");
            return;
        }

        // Set the GridLayoutGroup's start axis based on the layout type
        gridLayoutGroup.startAxis = (layoutType == LayoutType.Vertical) ? GridLayoutGroup.Axis.Vertical : GridLayoutGroup.Axis.Horizontal;

        CalculateTotalPages();
        SetSnapTarget(0);

        if (carouselMode)
        {
            InitializeNavigationDots();
        }

        // Setup navigation button click events
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(MoveToNextPage);
        }

        if (prevButton != null)
        {
            prevButton.onClick.AddListener(MoveToPreviousPage);
        }
    }

    private void InitializeNavigationDots()
    {
        for (int i = 0; i < totalPages; i++)
        {
            GameObject dot = Instantiate(dotPrefab, dotsContainer.transform);
            SetDotSize(dot, i == currentIndex ? activeDotSize : inactiveDotSize);
            SetDotColor(dot, i == currentIndex ? activeDotColor : inactiveDotColor);
            // You may need to position the dots based on your layout preferences
        }
    }

    private void SetDotColor(GameObject dot, Color color)
    {
        Image dotImage = dot.GetComponent<Image>();
        if (dotImage != null)
        {
            dotImage.color = color;
        }
    }

    private void SetDotSize(GameObject dot, Vector2 size)
    {
        RectTransform dotRect = dot.GetComponent<RectTransform>();
        if (dotRect != null)
        {
            dotRect.sizeDelta = size;
        }
    }

    private void CalculateTotalPages()
    {
        int itemCount = gridLayoutGroup.transform.childCount;
        totalPages = Mathf.CeilToInt((float)itemCount / gridLayoutGroup.constraintCount);
    }

    private void SetSnapTarget(int page)
    {
        if (infiniteLooping)
        {
            int totalVisiblePages = totalPages * 2; // Duplicate the pages to allow for looping
            int offsetPage = (page + totalVisiblePages) % totalPages;
            targetPosition = -pageSize * offsetPage;
        }
        else
        {
            targetPosition = -pageSize * page;
        }

        currentIndex = page;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        dragStartPos = eventData.position;
        lastDragTime = Time.unscaledTime;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        float dragDistance = Mathf.Abs(eventData.position.x - dragStartPos.x);
        float dragSpeed = eventData.delta.x / (Time.unscaledTime - lastDragTime);

        if (autoMove)
        {
            autoMoveTimerCountdown = autoMoveTimer;
        }

        if (carouselMode)
        {
            // Enable swiping to the next or previous content based on drag distance and speed
            if (dragDistance > pageSize * swipeThreshold || Mathf.Abs(dragSpeed) > swipeThreshold)
            {
                int currentPage = Mathf.RoundToInt(contentRectTransform.anchoredPosition.x / -pageSize);

                if (dragSpeed > 0)
                {
                    MoveToPreviousPage();
                }
                else
                {
                    MoveToNextPage();
                }
            }
            else
            {
                // Snap back to the current page if swipe distance is not enough
                SetSnapTarget(currentIndex);
            }
        }
    }

    private void Update()
    {
        if (autoMove)
        {
            autoMoveTimerCountdown -= Time.deltaTime;
            if (autoMoveTimerCountdown <= 0f)
            {
                MoveToNextPage();
                autoMoveTimerCountdown = autoMoveTimer;
            }
        }

        if (!isDragging)
        {
            contentRectTransform.anchoredPosition = Vector2.Lerp(
                contentRectTransform.anchoredPosition,
                new Vector2(targetPosition, contentRectTransform.anchoredPosition.y),
                Time.deltaTime * snapSpeed
            );

            // Smoothly update dot sizes based on the current index
            UpdateDotSizes();

            // Rotate content based on its position
            RotateContent();
        }
    }

    private void RotateContent()
    {
        for (int i = 0; i < totalPages; i++)
        {
            GameObject content = gridLayoutGroup.transform.GetChild(i).gameObject;
            float rotationAngle = Mathf.Lerp(0f, maxRotationAngle, Mathf.Abs(currentIndex - i) / (float)totalPages);
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, rotationAngle);

            content.transform.rotation = Quaternion.Slerp(content.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void MoveToPreviousPage()
    {
        if (infiniteLooping)
        {
            int prevPage = (currentIndex - 1 + totalPages) % totalPages;
            SetSnapTarget(prevPage);
        }
        else
        {
            int prevPage = Mathf.Clamp(currentIndex - 1, 0, totalPages - 1);
            SetSnapTarget(prevPage);
        }
    }

    private void MoveToNextPage()
    {
        if (infiniteLooping)
        {
            int nextPage = (currentIndex + 1) % totalPages;
            SetSnapTarget(nextPage);
        }
        else
        {
            int nextPage = Mathf.Clamp(currentIndex + 1, 0, totalPages - 1);
            SetSnapTarget(nextPage);
        }
    }

    private void UpdateDotSizes()
    {
        for (int i = 0; i < dotsContainer.transform.childCount; i++)
        {
            GameObject dot = dotsContainer.transform.GetChild(i).gameObject;
            Vector2 targetSize = i == currentIndex ? activeDotSize : inactiveDotSize;
            RectTransform dotRect = dot.GetComponent<RectTransform>();

            if (dotRect != null)
            {
                // Smoothly interpolate between current size and target size
                dotRect.sizeDelta = Vector2.Lerp(dotRect.sizeDelta, targetSize, Time.deltaTime * dotScalingSpeed);
            }

            // Smoothly interpolate between current color and target color
            Image dotImage = dot.GetComponent<Image>();
            if (dotImage != null)
            {
                Color targetColor = i == currentIndex ? activeDotColor : inactiveDotColor;
                dotImage.color = Color.Lerp(dotImage.color, targetColor, Time.deltaTime * dotColorTransitionSpeed);
            }
        }
    }
}