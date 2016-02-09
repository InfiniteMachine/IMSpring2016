#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CamFollow))]
public class CamFollowEditor : Editor
{
    private CamFollow c;
    private bool backgroundObject;

    [MenuItem("Custom 2D/Add CamFollow")]
    static void camFollow(MenuCommand menuCommand)
    {
        Undo.RecordObject(Camera.main, "Add CamFollow to MainCamera.");
        Camera.main.gameObject.AddComponent<CamFollow>();
    }

    void OnEnable()
    {
        c = (CamFollow)target;
    }

    void OnSceneGUI()
    {
        float left = c.leftBounds,
              right = c.rightBounds,
              up = c.upperBounds,
              down = c.lowerBounds,
              margin = c.margin;

        Handles.DrawSolidRectangleWithOutline(new Vector3[] { new Vector3(left, up), new Vector3(right, up), new Vector3(right, down), new Vector3(left, down), },
            new Color(0, 0, 0, 0.15f), new Color(255, 0, 255));
        Handles.color = new Color(255, 0, 255);

        Vector2 corner = Handles.FreeMoveHandle(new Vector2(left, up), Quaternion.identity, HandleUtility.GetHandleSize(new Vector2(left, up)) * 0.05f, Vector3.zero, Handles.DotCap);
        left = corner.x;
        up = corner.y;

        corner = Handles.FreeMoveHandle(new Vector2(right, up), Quaternion.identity, HandleUtility.GetHandleSize(new Vector2(right, up)) * 0.05f, Vector3.zero, Handles.DotCap);
        right = corner.x;
        up = corner.y;

        corner = Handles.FreeMoveHandle(new Vector2(right, down), Quaternion.identity, HandleUtility.GetHandleSize(new Vector2(right, up)) * 0.05f, Vector3.zero, Handles.DotCap);
        right = corner.x;
        down = corner.y;

        corner = Handles.FreeMoveHandle(new Vector2(left, down), Quaternion.identity, HandleUtility.GetHandleSize(new Vector2(left, down)) * 0.05f, Vector3.zero, Handles.DotCap);
        left = corner.x;
        down = corner.y;

        left = Handles.Slider(new Vector2(left, (up + down) * 0.5f), -Vector2.right, HandleUtility.GetHandleSize(new Vector2(right, up)) * 0.05f, Handles.DotCap, 0).x;
        right = Handles.Slider(new Vector2(right, (up + down) * 0.5f), Vector2.right, HandleUtility.GetHandleSize(new Vector2(right, up)) * 0.05f, Handles.DotCap, 0).x;

        up = Handles.Slider(new Vector2((right + left) * 0.5f, up), Vector2.up, HandleUtility.GetHandleSize(new Vector2(right, up)) * 0.05f, Handles.DotCap, 0).y;
        down = Handles.Slider(new Vector2((right + left) * 0.5f, down), -Vector2.up, HandleUtility.GetHandleSize(new Vector2(right, up)) * 0.05f, Handles.DotCap, 0).y;

        if (right < left)
        {
            float temp = right;
            right = left;
            left = temp;
        }
        if (up < down)
        {
            float temp = up;
            up = down;
            down = temp;
        }

        if (GUI.changed)
        {
            Undo.RecordObject(c, "CamFollow: Resize Map Boundries.");
            c.leftBounds = left;
            c.rightBounds = right;
            c.upperBounds = up;
            c.lowerBounds = down;
            EditorUtility.SetDirty(target);
        }

        if (GUI.changed)
        {
            Undo.RecordObject(c, "CamFollow: Change Object Follow Margin.");
            c.margin = margin;
            EditorUtility.SetDirty(target);
        }

        Handles.color = Color.white;
    }
}
#endif