using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VariantWindow : EditorWindow {
	public GameObject parent;
	Vector2 scrollPos;

	[MenuItem ("Window/Variant Window")]
	public static void ShowWindow()
    {
        GetWindow<VariantWindow>(false, "Variant Window", true);
    }

	void OnGUI()
	{
		EditorGUILayout.LabelField ("View all prefab variants", EditorStyles.boldLabel);
		EditorGUILayout.Space();

		// This is the field where the prefab can be dragged into
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Parent Prefab", GUILayout.Width(100));
		parent = (GameObject)EditorGUILayout.ObjectField(parent, typeof(GameObject));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		List<Object> variants = GetVariants();

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);
		// The group is disabled because we want these ObjectFields to be read-only
		EditorGUI.BeginDisabledGroup(true);
		foreach (Object variant in variants)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Variant:", GUILayout.Width(100));
			EditorGUILayout.ObjectField(variant, typeof(Object), false);
			EditorGUILayout.EndHorizontal();

			// Show a thumbnail of the variant prefabs
			Texture2D image = AssetPreview.GetAssetPreview(variant);
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Preview:", GUILayout.Width(100));
			// The thumbnail image is inside a LabelField because DrawPreviewTexture isn't available in EditorGUILayout
			EditorGUILayout.LabelField(new GUIContent(image), GUILayout.Height(100.0f), GUILayout.Width(180.0f), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(true));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndScrollView();
	}

	List<Object> GetVariants()
	{
		// Find all prefabs
		string[] guids = AssetDatabase.FindAssets("t:Prefab");

		List<Object> variants = new List<Object>();

		for (int i=0; i<guids.Length; i++)
		{
			// Convert the prefab guid string into Object
			string path = AssetDatabase.GUIDToAssetPath(guids[i]);
        	Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);

			// Check if the prefab is a variant
			if (PrefabUtility.GetPrefabAssetType(obj) == PrefabAssetType.Variant)
			{
				// Check that the base prefab is the same as the one we've dragged into the ObjectField (this used to be PrefabUtility.GetPrefabParent)
				if (PrefabUtility.GetCorrespondingObjectFromSource(obj) == parent)
				{
					variants.Add(obj);
				}	
				else if (variants.Contains(PrefabUtility.GetCorrespondingObjectFromSource(obj)) &&(PrefabUtility.GetCorrespondingObjectFromSource(obj) == AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guids[i-1]))))
				{
					variants.Add(obj);
				}		
			}
		}
		return variants;
	}

	void OnInspectorUpdate()
	{
		// OnInspectorUpdate is called 10 times per second
		Repaint();
	}
}
