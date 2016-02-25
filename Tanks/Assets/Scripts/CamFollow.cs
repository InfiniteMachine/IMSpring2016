using UnityEngine;
using System.Collections.Generic;

public class CamFollow : MonoBehaviour
{
    public static CamFollow instance;
    
    public float margin = 2f;
    public float moveFastSpeed = 3f;
    public float moveSlowSpeed = 3f;
    public float orthoGrowSpeed = 1f;
    public float orthoShrinkSpeed = 5f;
    public float distancePadding = 4f;
    [HideInInspector]
    public float leftBounds = -5;
    [HideInInspector]
    public float rightBounds = 5;
    [HideInInspector]
    public float upperBounds = 5;
    [HideInInspector]
    public float lowerBounds = -5;
    
    private float screenWorldWidth;
    private float screenWorldHeight;
    private Vector3 lastPosition;
    private float lastOrtographicSize = 5f;

    private List<Transform> visibleObjects;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        visibleObjects = new List<Transform>();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            visibleObjects.Add(go.transform);
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Baton"))
            visibleObjects.Add(go.transform);

        lastPosition = transform.position;
        lastOrtographicSize = Camera.main.orthographicSize;
        CalculateWindowResize();
        ConstrainCameraToView();
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Generate camera bounds
        Rect r = GenerateRect();
        float newOrtho = Mathf.Max((r.height) / 2, (r.width) / (2 * Camera.main.aspect));
        if(newOrtho * (2 * Camera.main.aspect) > (rightBounds - leftBounds))
        {
            newOrtho = (rightBounds - leftBounds) / (2 * Camera.main.aspect);
        }
        if(newOrtho * 2 > (upperBounds - lowerBounds))
        {
            newOrtho = (upperBounds - lowerBounds) / 2;
        }
        if (newOrtho < Camera.main.orthographicSize)
        {
            Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, newOrtho, orthoShrinkSpeed * Time.deltaTime);
        }
        else
        {
            Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, newOrtho, orthoGrowSpeed * Time.deltaTime);
        }
        Vector3 newPosition = new Vector3(r.x + (r.width / 2), r.y + (r.height / 2), Camera.main.transform.position.z);
        if(Vector2.Distance(Camera.main.transform.position, newPosition) > distancePadding)
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, newPosition, moveFastSpeed * Time.deltaTime);
        else
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, newPosition, moveSlowSpeed * Time.deltaTime);
        ConstrainCameraToView();
    }

    private Rect GenerateRect()
    {
        Rect r = new Rect();
        r.yMax = r.yMin = visibleObjects[0].position.y;
        r.xMax = r.xMin = visibleObjects[0].position.x;
        for(int i = 1; i < visibleObjects.Count; i++)
        {
            Vector3 pos = visibleObjects[i].position;
            if (pos.y < r.yMin)
                r.yMin = pos.y;
            else if (pos.y > r.yMax)
                r.yMax = pos.y;
            if (pos.x < r.xMin)
                r.xMin = pos.x;
            else if (pos.x > r.xMax)
                r.xMax = pos.x;
        }
        r.yMax = Mathf.Min(r.yMax + margin, upperBounds);
        r.yMin = Mathf.Max(r.yMin - margin, lowerBounds);
        r.xMax = Mathf.Min(r.xMax + margin, rightBounds);
        r.xMin = Mathf.Max(r.xMin - margin, leftBounds);
        return r;
    }

    private void ConstrainCameraToView()
    {
        CalculateWindowResize();
        if (lastPosition != transform.position)
        {
            lastPosition.x = Mathf.Clamp(transform.position.x, leftBounds + screenWorldWidth, rightBounds - screenWorldWidth);
            lastPosition.y = Mathf.Clamp(transform.position.y, lowerBounds + screenWorldHeight, upperBounds - screenWorldHeight);
            transform.position = lastPosition;
        }
    }

    public void CalculateWindowResize()
    {
        if (lastOrtographicSize != Camera.main.orthographicSize)
        {
            float rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
            float topBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;

            screenWorldHeight = topBorder - transform.position.y;
            screenWorldWidth = rightBorder - transform.position.x;
            lastOrtographicSize = Camera.main.orthographicSize;
        }
    }

    public float GetLeftBound()
    {
        return leftBounds;
    }

    public float GetRightBound()
    {
        return rightBounds;
    }

    public float GetTopBound()
    {
        return upperBounds;
    }

    public float GetLowerBound()
    {
        return lowerBounds;
    }
}