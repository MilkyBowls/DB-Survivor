using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDropTable", menuName = "DBZ/Drop Table")]
public class DropTable : ScriptableObject
{
    [System.Serializable]
    public class DropEntry
    {
        public CollectibleData item;
        public int weight = 1;
    }

    public List<DropEntry> drops;
}
