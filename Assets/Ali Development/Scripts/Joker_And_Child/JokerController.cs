using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JokerController : MonoBehaviour
{
    public GameObject[] meshes;
    // Start is called before the first frame update
    void Start()
    {
        float timer = UnityEngine.Random.Range(2.0f, 16.0f);
        Invoke(nameof(ActivateJokerMesh), timer);
    }
    void ActivateJokerMesh()
    {
        foreach (var jokerMesh in meshes)
        {
            jokerMesh.SetActive(false);
        }
        int index = UnityEngine.Random.Range(0, meshes.Length);

        meshes[index].SetActive(true);
    }

}
