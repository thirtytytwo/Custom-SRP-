using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour
{
    private static int baseColorID = Shader.PropertyToID("_BaseColor");

    [SerializeField] private Color baseColor = Color.white;

    private static MaterialPropertyBlock block;

    private void OnValidate()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
        }
        block.SetColor(baseColorID, baseColor);
        GetComponent<Renderer>().SetPropertyBlock(block);
    }
}
