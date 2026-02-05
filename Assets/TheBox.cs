using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TheBox : MonoBehaviour
{
    public float size = 1f;
    public Vector3 center = Vector3.zero;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateBoxMesh(size);
    }

    // Update is called once per frame
    public void CreateBoxMesh(float newsize)
    {
        Mesh mesh = new Mesh();
        float h = newsize/2f; 

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(center.x - h, center.y - h, center.z - h),
            new Vector3(center.x + h, center.y - h, center.z - h),
            new Vector3(center.x + h, center.y + h, center.z - h),
            new Vector3(center.x - h, center.y + h, center.z - h),
            new Vector3(center.x - h, center.y - h, center.z + h),
            new Vector3(center.x + h, center.y - h, center.z + h),
            new Vector3(center.x + h, center.y + h, center.z + h),
            new Vector3(center.x - h, center.y + h, center.z + h),
        };

        int[] indices = new int[]
        {
            0, 1, 1, 2, 2, 3, 3, 0,
            4, 5, 5, 6, 6, 7, 7, 4,
            0, 4, 1, 5, 2, 6, 3, 7,
        };

        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Lines, 0);

        GetComponent<MeshFilter>().mesh = mesh;
    }


}
