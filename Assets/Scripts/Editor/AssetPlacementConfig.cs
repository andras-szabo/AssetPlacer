using UnityEngine;

[System.Serializable]
public class AssetPlacementConfig : ScriptableObject
{
	public GameObject[] assetsToSpawn = new GameObject[3];
	public float maxDistance = 200f;
	public LayerMask hitLayers;
}
