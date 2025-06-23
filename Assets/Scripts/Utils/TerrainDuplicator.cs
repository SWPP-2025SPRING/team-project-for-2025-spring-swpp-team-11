using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class TerrainDuplicator : MonoBehaviour
{
    [MenuItem("Tools/Duplicate Selected Terrain With New Data")]
    static void DuplicateTerrain()
    {
        Terrain original = Selection.activeGameObject?.GetComponent<Terrain>();
        if (original == null)
        {
            Debug.LogError("선택된 오브젝트에 Terrain 컴포넌트가 없습니다.");
            return;
        }

        // TerrainData 복제
        TerrainData newData = Object.Instantiate(original.terrainData);
        newData.name = original.terrainData.name + "_Copy";

        // 새 Terrain 오브젝트 만들기
        GameObject newTerrainObj = Object.Instantiate(original.gameObject);
        newTerrainObj.name = original.gameObject.name + "_Copy";

        // TerrainData 할당
        Terrain newTerrain = newTerrainObj.GetComponent<Terrain>();
        newTerrain.terrainData = newData;

        // 선택
        Selection.activeGameObject = newTerrainObj;

        Debug.Log("Terrain 복제 완료 (TerrainData도 분리됨)");
    }
}


#endif