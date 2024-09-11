using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [SerializeField] private GameObject caravanPrefab;
    [SerializeField] private GameObject villagerPrefab;

    [SerializeField] private int initialCaravanCount = 1;
    [SerializeField] private int initialVillagerCount = 2;

    [SerializeField] private float velocity = 10.0f;

    private const int MAX_OBJS_PER_DRAWCALL = 1000;

    private Mesh caravanMesh;
    private Mesh villagerMesh;

    private Material caravanMaterial;
    private Material villagerMaterial;

    private Vector3 caravanScale;
    private Vector3 villagerScale;

    private List<uint> entities;

    private int caravanCount = 0;
    private int villagerCount = 0;

    void Start()
    {
        ECSManager.Init();
        entities = new List<uint>();
        for (int i = 0; i < initialCaravanCount; i++)
        {
            CreateCaravan();
        }

        for (int i = 0; i < initialVillagerCount; i++)
        {
            CreateVillager();
        }

        villagerMesh = villagerPrefab.GetComponent<MeshFilter>().sharedMesh;
        villagerMaterial = villagerPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        villagerScale = villagerPrefab.transform.localScale;

        caravanMesh = caravanPrefab.GetComponent<MeshFilter>().sharedMesh;
        caravanMaterial = caravanPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        caravanScale = caravanPrefab.transform.localScale;

    }

    void Update()
    {
        ECSManager.Tick(Time.deltaTime);
    }

    void LateUpdate()
    {
        /*
        foreach (KeyValuePair<uint, GameObject> entity in entities)
        {
            PositionComponent position = ECSManager.GetComponent<PositionComponent>(entity.Key);
            entity.Value.transform.SetPositionAndRotation(new Vector3(position.X, position.Y, position.Z), Quaternion.identity);
        }
        */

        List<Matrix4x4[]> drawMatrix = new List<Matrix4x4[]>();
        int meshes = entities.Count;
        for (int i = 0; i < entities.Count; i += MAX_OBJS_PER_DRAWCALL)
        {
            drawMatrix.Add(new Matrix4x4[meshes > MAX_OBJS_PER_DRAWCALL ? MAX_OBJS_PER_DRAWCALL : meshes]);
            meshes -= MAX_OBJS_PER_DRAWCALL;
        }

        SetTRS(drawMatrix, caravanScale);
        SetTRS(drawMatrix, villagerScale);

        DrawMeshes(caravanCount, caravanMesh, caravanMaterial, drawMatrix);
        DrawMeshes(villagerCount, villagerMesh, villagerMaterial, drawMatrix, caravanCount);

    }

    private void CreateAgent()
    {
        uint entityID = ECSManager.CreateEntity();
        ECSManager.AddComponent<PositionComponent>(entityID, new PositionComponent(0, 0, 0));
        ECSManager.AddComponent<VelocityComponent>(entityID, new VelocityComponent(velocity, Vector3.right.x, Vector3.right.y, Vector3.right.z));
        entities.Add(entityID);
    }

    public void CreateCaravan()
    {
        CreateAgent();
        caravanCount++;
    }
    public void CreateVillager()
    {

        CreateAgent();
        villagerCount++;
    }

    private void SetTRS(List<Matrix4x4[]> drawMatrix, Vector3 scale)
    {
        Parallel.For(0, entities.Count, i =>
        {
            PositionComponent position = ECSManager.GetComponent<PositionComponent>(entities[i]);
            drawMatrix[(i / MAX_OBJS_PER_DRAWCALL)][(i % MAX_OBJS_PER_DRAWCALL)]
            .SetTRS(new Vector3(position.X, position.Y, position.Z), Quaternion.identity, scale);
        });
    }

    private void DrawMeshes(int meshCount, Mesh mesh, Material material, List<Matrix4x4[]> drawMatrix, int threshold = 0)
    {
        for (int i = 0; i < meshCount; i++)
        {
            Graphics.DrawMeshInstanced(mesh, 0, material, drawMatrix[i+threshold]);
        }
    }
}
