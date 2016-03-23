using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
// CopyComponents - by Michael L. Croswell for Colorado Game Coders, LLC
// March 2010
//Modified by Kristian Helle Jespersen
//June 2011
//Modified by Connor Cadellin McKee for Excamedia
//April 2015
//Modified by Fernando Medina (fermmmm)
//April 2015
public class ReplaceGameObjects : ScriptableWizard
{
    public GameObject Prefab;
    public GameObject[] ObjectsToReplace;
    public bool KeepOriginalNames = false;
    [MenuItem("Custom/Replace GameObjects %h")]
    static void CreateWizard()
    {
        var replaceGameObjects = DisplayWizard<ReplaceGameObjects>("Replace GameObjects", "Replace");
        replaceGameObjects.ObjectsToReplace = Selection.gameObjects;
    }
    [MenuItem("Custom/RemoveColliders %j")]
    static void RemoveColliders()
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            Stack<Collider> cols = new Stack<Collider>();
            Collider[] c = go.GetComponents<Collider>();
            for (int i = 0; i < c.Length; i++)
                cols.Push(c[i]);
            c = null;
            while (cols.Count > 0)
                DestroyImmediate(cols.Pop());
            Stack<Collider2D> cols2D = new Stack<Collider2D>();
            Collider2D[] c2D = go.GetComponents<Collider2D>();
            for (int i = 0; i < c2D.Length; i++)
                cols2D.Push(c2D[i]);
            c2D = null;
            while (cols2D.Count > 0)
                DestroyImmediate(cols2D.Pop());
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
    void OnWizardCreate()
    {
        foreach (GameObject go in ObjectsToReplace)
        {
            GameObject newObject;
            newObject = (GameObject)PrefabUtility.InstantiatePrefab(Prefab);
            newObject.transform.SetParent(go.transform.parent, true);
            newObject.transform.localPosition = go.transform.localPosition;
            newObject.transform.localRotation = go.transform.localRotation;
            newObject.transform.localScale = go.transform.localScale;
            if (KeepOriginalNames)
                newObject.transform.name = go.transform.name;
            DestroyImmediate(go);
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}