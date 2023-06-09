using System;
using UnityEngine;
using Random = System.Random;

public class MeshBall : MonoBehaviour
{
    private static int baseColorID = Shader.PropertyToID("_BaseColor");
    private static int cutoffID = Shader.PropertyToID("_Cutoff");
    private static MaterialPropertyBlock block;

    [SerializeField] private Mesh mesh = default;
    [SerializeField] private Material material = default;

    private Matrix4x4[] matrices = new Matrix4x4[1023];
    private Vector4[] colors = new Vector4[1023];
    private float[] cuts = new float[1023];

    private void Awake()
    {
        for (int i = 0; i < matrices.Length; i++)
        {
            matrices[i] = Matrix4x4.TRS(UnityEngine.Random.insideUnitSphere * 10f, Quaternion.identity, Vector3.one);
            colors[i] = new Vector4(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            cuts[i] = UnityEngine.Random.Range(0.0f,0.2f);
        }
    }

    private void Update()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
            block.SetVectorArray(baseColorID, colors);
            block.SetFloatArray(cutoffID, cuts);
        }
        Graphics.DrawMeshInstanced(mesh, 0, material, matrices, 1023, block);
    }
}
