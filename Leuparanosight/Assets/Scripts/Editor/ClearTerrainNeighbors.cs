using UnityEditor; 
using UnityEngine;


public class ClearTerrainNeighbors : MonoBehaviour
{
    
    [MenuItem("Terrain/Clear Neighbors of Selected Terrain")]
    static void ClearSelectedTerrainNeighbors()
    {
        
        Terrain terrain = Selection.activeGameObject.GetComponent<Terrain>();

        if (terrain == null)
        {
            Debug.LogError("Error: Please select a GameObject with a Terrain Component in the Hierarchy before running the command..");
            return;
        }


        terrain.SetNeighbors(null, null, null, null);

        Debug.Log($"[Terrain Fixer] Neighbors ของ Terrain '{Selection.activeGameObject.name}' Reset ");
        
        EditorUtility.SetDirty(terrain);
    }
}