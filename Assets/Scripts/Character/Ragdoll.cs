using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] Renderer[] renderers = null;


    public void SetColor(Color color) {
        foreach (var renderer in renderers) {
            renderer.material.color = color;
        }
    }
}
