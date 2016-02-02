#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class AutoSnap : EditorWindow
{
    private Vector3 prevPosition;
    private float snapValue = 1;

    [System.Serializable]
    private class AxisSnap
    {
        public static implicit operator bool(AxisSnap aSnap)
        {
            return aSnap != null && (aSnap.x || aSnap.y || aSnap.z);
        }

        public bool x = false, y = false, z = false;
    }

    private AxisSnap axisSnap = new AxisSnap();

    private Transform lastSelect;

    [MenuItem("Custom Editor Functions/Auto Snap %#l", false, 2)]
    static void Init()
    {
        var window = (AutoSnap)EditorWindow.GetWindow(typeof(AutoSnap));
        window.maxSize = new Vector2(200, 100);
    }

    public void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        axisSnap.x = EditorGUILayout.Toggle("X-Axis", axisSnap.x );
        axisSnap.y = EditorGUILayout.Toggle("Y-Axis", axisSnap.y);
        axisSnap.z = EditorGUILayout.Toggle("Z-Axis", axisSnap.z);
        EditorGUILayout.EndVertical();
        snapValue = EditorGUILayout.FloatField("Snap Value", snapValue);
    }

    public void Update()
    {
        if (axisSnap){
            if(Selection.transforms.Length > 0){
                if(lastSelect == Selection.transforms[0]){
                    if(!EditorApplication.isPlaying && Selection.transforms[0].position != prevPosition){
                        Snap();
                        prevPosition = Selection.transforms[0].position;
                    }
                }else{
                    lastSelect = Selection.transforms[0];
                    prevPosition = lastSelect.transform.position;
                }
            }else{
                lastSelect = null;
            }
        }
    }

    private void Snap()
    {
        Vector3 t = Selection.transforms[0].transform.position;
        for (int i = 0; i < Selection.transforms.Length; i++)
        {
            t = Selection.transforms[i].transform.position;
            if (axisSnap.x)
                t.x = Round(t.x);
            if (axisSnap.y)
                t.y = Round(t.y);
            if (axisSnap.z)
                t.z = Round(t.z);
            Selection.transforms[i].transform.position = t;
        }
    }

    private float Round(float input)
    {
        return snapValue * Mathf.Round((input / snapValue));
    }
}
#endif