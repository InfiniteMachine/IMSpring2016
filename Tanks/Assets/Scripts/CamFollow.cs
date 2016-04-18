using UnityEngine;
using System.Collections.Generic;

public class CamFollow : MonoBehaviour
{
    public static CamFollow instance;

    public float margin = 6f;
    public AnimationCurve orthoFallOff = new AnimationCurve(new Keyframe[] { new Keyframe(0.001461988f, 0.1f, 3.603517f, 3.812278f), new Keyframe(0.75f, 1, 0.0476678f, 0.0476678f), new Keyframe(1, 1, 0, 0) });
    public float orthoSpeed = 10f;
    public AnimationCurve deltaFallOff = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f, 5.737174f, 5.737174f), new Keyframe(0.5f, 1f, 0.02810661f, 0.02810661f), new Keyframe(1f, 1f, 0f, 0f) });
    public float deltaSpeed = 25f;

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
    private float lastOrtographicSize = -1f;

    private List<Transform> visibleObjects;

    private float minOrtho = 4;
    private float maxOrtho = 0;
    private float maxDelta = 4;
    //add timer, camera only does zoom thing after x time

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
        minOrtho = margin;
        maxOrtho = Mathf.Abs(upperBounds - lowerBounds) / 2;
        //Change to actually calculate maxDelta
        maxDelta = (Mathf.Abs(rightBounds - leftBounds) - (minOrtho * 2 * Camera.main.aspect)) / 2;

        if (Camera.main.orthographicSize * (2 * Camera.main.aspect) > (rightBounds - leftBounds))
        {
            Camera.main.orthographicSize = (rightBounds - leftBounds) / (2 * Camera.main.aspect);
        }
        if (Camera.main.orthographicSize * 2 > (upperBounds - lowerBounds))
        {
            Camera.main.orthographicSize = (upperBounds - lowerBounds) / 2;
        }
        CalculateWindowResize();
        ConstrainCameraToView();

    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Generate camera bounds
        Rect r = GenerateRect();
        float newOrtho = Mathf.Max((r.height) / 2, (r.width) / (2 * Camera.main.aspect));
        if (newOrtho * (2 * Camera.main.aspect) > (rightBounds - leftBounds))
        {
            newOrtho = (rightBounds - leftBounds) / (2 * Camera.main.aspect);
        }
        if (newOrtho * 2 > (upperBounds - lowerBounds))
        {
            newOrtho = (upperBounds - lowerBounds) / 2;
        }
        Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, newOrtho,
            orthoFallOff.Evaluate((newOrtho - minOrtho) / (maxOrtho - minOrtho)) * orthoSpeed * Time.deltaTime);

        Vector3 newPosition = new Vector3(r.x + (r.width / 2), r.y + (r.height / 2), Camera.main.transform.position.z);
        Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, newPosition,
            deltaFallOff.Evaluate(((Vector2)(Camera.main.transform.position - newPosition)).magnitude / maxDelta) * deltaSpeed * Time.deltaTime);
        ConstrainCameraToView();
    }

    private Rect GenerateRect()
    {
        for (int i = 0; i < visibleObjects.Count; i++)
        {
            if (visibleObjects[i] == null)
            {
                visibleObjects.RemoveAt(i);
                i--;
            }
        }
        Rect r = new Rect();
        r.yMax = r.yMin = visibleObjects[0].position.y;
        r.xMax = r.xMin = visibleObjects[0].position.x;
        for (int i = 1; i < visibleObjects.Count; i++)
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