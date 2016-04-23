using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class uGUITools : MonoBehaviour {
	[MenuItem("uGUI/Anchors to Corners %[")]
	static void AnchorsToCorners(){
        Undo.RecordObjects(Selection.gameObjects, "uGUITools: AnchorsToCorners");
        foreach (Transform transform in Selection.transforms){
			RectTransform t = transform as RectTransform;
			RectTransform pt = Selection.activeTransform.parent as RectTransform;

			if(t == null || pt == null) return;
			
			Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
			                                    t.anchorMin.y + t.offsetMin.y / pt.rect.height);
			Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
			                                    t.anchorMax.y + t.offsetMax.y / pt.rect.height);

			t.anchorMin = newAnchorsMin;
			t.anchorMax = newAnchorsMax;
			t.offsetMin = t.offsetMax = new Vector2(0, 0);
		}
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
	}

	[MenuItem("uGUI/Corners to Anchors %]")]
	static void CornersToAnchors(){
        Undo.RecordObjects(Selection.gameObjects, "uGUITools: CornersToAnchors");
        foreach (Transform transform in Selection.transforms){
			RectTransform t = transform as RectTransform;

			if(t == null) return;

			t.offsetMin = t.offsetMax = new Vector2(0, 0);
		}
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

	[MenuItem("uGUI/Mirror Horizontally Around Anchors %;")]
	static void MirrorHorizontallyAnchors(){
		MirrorHorizontally(false);
	}

	[MenuItem("uGUI/Mirror Horizontally Around Parent Center %:")]
	static void MirrorHorizontallyParent(){
		MirrorHorizontally(true);
	}

	static void MirrorHorizontally(bool mirrorAnchors){
        Undo.RecordObjects(Selection.gameObjects, "uGUITools: MirrorHorizontally");
        foreach (Transform transform in Selection.transforms){
			RectTransform t = transform as RectTransform;
			RectTransform pt = Selection.activeTransform.parent as RectTransform;
			
			if(t == null || pt == null) return;

			if(mirrorAnchors){
				Vector2 oldAnchorMin = t.anchorMin;
				t.anchorMin = new Vector2(1 - t.anchorMax.x, t.anchorMin.y);
				t.anchorMax = new Vector2(1 - oldAnchorMin.x, t.anchorMax.y);
			}

			Vector2 oldOffsetMin = t.offsetMin;
			t.offsetMin = new Vector2(-t.offsetMax.x, t.offsetMin.y);
			t.offsetMax = new Vector2(-oldOffsetMin.x, t.offsetMax.y);

			t.localScale = new Vector3(-t.localScale.x, t.localScale.y, t.localScale.z);
		}
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

	[MenuItem("uGUI/Mirror Vertically Around Anchors %'")]
	static void MirrorVerticallyAnchors(){
		MirrorVertically(false);
	}
	
	[MenuItem("uGUI/Mirror Vertically Around Parent Center %\"")]
	static void MirrorVerticallyParent(){
		MirrorVertically(true);
	}
	
	static void MirrorVertically(bool mirrorAnchors){
        Undo.RecordObjects(Selection.gameObjects, "uGUITools: MirrorVertically");
        foreach (Transform transform in Selection.transforms){
            RectTransform t = transform as RectTransform;
			RectTransform pt = Selection.activeTransform.parent as RectTransform;
			
			if(t == null || pt == null) return;
			
			if(mirrorAnchors){
				Vector2 oldAnchorMin = t.anchorMin;
				t.anchorMin = new Vector2(t.anchorMin.x, 1 - t.anchorMax.y);
				t.anchorMax = new Vector2(t.anchorMax.x, 1 - oldAnchorMin.y);
			}
			
			Vector2 oldOffsetMin = t.offsetMin;
			t.offsetMin = new Vector2(t.offsetMin.x, -t.offsetMax.y);
			t.offsetMax = new Vector2(t.offsetMax.x, -oldOffsetMin.y);
			
			t.localScale = new Vector3(t.localScale.x, -t.localScale.y, t.localScale.z);
		}
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    [MenuItem("uGUI/ApplyChangesToPrefabs %.")]
    static void ApplyToPrefab()
    {
        Undo.RecordObjects(Selection.gameObjects, "uGUITools: ApplyPrefabs");
        foreach (Transform transform in Selection.transforms)
        {
            GameObject go = PrefabUtility.FindRootGameObjectWithSameParentPrefab(transform.gameObject);
            Object prefab = PrefabUtility.GetPrefabParent(go);
            PrefabUtility.ReplacePrefab(go, prefab, ReplacePrefabOptions.ConnectToPrefab);
        }
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
}
