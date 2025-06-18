using UnityEngine;

[CreateAssetMenu(fileName = "NewCollectible", menuName = "DBZ/Collectible")]
public class CollectibleData : ScriptableObject
{
    public string itemName;
    public GameObject prefab;
    public int xpAmount;
    public Sprite icon;
}