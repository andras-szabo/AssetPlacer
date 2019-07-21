using UnityEngine;
using UnityEditor;

public class AssetPlacer : EditorWindow
{
	private static AssetPlacer window;
	public static AssetPlacer Window { get { Init(); return window; } }

	public AssetPlacementConfig serialized;
	public Transform parent;

	[MenuItem("Tools/AssetPlacer/Show")]
	private static void Init()
	{
		// Get existing open window or if none, make a new one:
		window = (AssetPlacer)EditorWindow.GetWindow(typeof(AssetPlacer));
		window.Show();
	}

	private const string path = "Assets/AssetPlacementConfig.asset";

	private void OnEnable()
	{
		hideFlags = HideFlags.HideAndDontSave;

		if (AssetDatabase.LoadAssetAtPath(path, typeof(AssetPlacementConfig)) == null)
		{
			serialized = CreateInstance<AssetPlacementConfig>();
		}
		else
		{
			serialized = (AssetPlacementConfig)AssetDatabase.LoadAssetAtPath(path, typeof(AssetPlacementConfig));
		}
	}

	private void OnDestroy()
	{
		if (AssetDatabase.LoadAssetAtPath(path, typeof(AssetPlacementConfig)) == null)
		{
			AssetDatabase.CreateAsset(serialized, path);
		}
		else
		{
			EditorUtility.SetDirty(serialized);
			AssetDatabase.SaveAssets();
		}
	}

	private void OnGUI()
	{
		parent = (Transform)EditorGUI
					.ObjectField(position: new Rect(3, 3, position.width - 6, 20),
								 "Parent", parent, typeof(Transform), true);

		for (int i = 0; i < serialized.assetsToSpawn.Length; ++i)
		{
			serialized.assetsToSpawn[i] = (GameObject)EditorGUI
								.ObjectField(position: new Rect(3, 3 + (23 * (i + 1)), position.width - 6, 20),
								string.Format("Asset {0} to spawn", i + 1), serialized.assetsToSpawn[i], typeof(GameObject), false);
		}
	}


	[MenuItem("Tools/AssetPlacer/Spawn asset 1 #1")]
	public static void SpawnAsset1()
	{
		SpawnAsset(0);
	}

	[MenuItem("Tools/AssetPlacer/Spawn asset 2 #2")]
	public static void SpawnAsset2()
	{
		SpawnAsset(1);
	}

	[MenuItem("Tools/AssetPlacer/Spawn asset 3 #3")]
	public static void SpawnAsset3()
	{
		SpawnAsset(2);
	}

	public static void SpawnAsset(int index)
	{
		var currentEditorWindow = EditorWindow.focusedWindow;

		var wnd = Window;
		if (wnd == null || wnd.serialized.assetsToSpawn == null
			|| index < 0 || index >= wnd.serialized.assetsToSpawn.Length
			|| wnd.serialized.assetsToSpawn[index] == null)
		{
			return;
		}

		var mousePos = Event.current.mousePosition;
		var sceneView = SceneView.lastActiveSceneView;

		var style = (GUIStyle)"GV Gizmo DropDown";
		Vector2 ribbon = style.CalcSize(sceneView.titleContent);
		float vertAdjust = -ribbon.y;

		var absoluteScreenPos = Event.current.mousePosition + currentEditorWindow.position.min;
		var sceneViewPos = absoluteScreenPos - sceneView.position.min;

		var viewportH = sceneViewPos.x / (double)sceneView.position.width;
		var viewportV = (vertAdjust + sceneViewPos.y) / (double)sceneView.position.height;

		var sceneViewportPos = new Vector3((float)viewportH, 1f - (float)viewportV);

		var ray = sceneView.camera.ViewportPointToRay(sceneViewportPos);

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, wnd.serialized.maxDistance, wnd.serialized.hitLayers.value))
		{
			var go = GameObject.Instantiate<GameObject>(wnd.serialized.assetsToSpawn[index], 
														hit.point, Quaternion.identity,
														wnd.parent);
		}
	}
}
