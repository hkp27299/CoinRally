using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBehavior : MonoBehaviour
{
    public float roadOffsetY = 0;
    public float roadSpeed = 1;

    public Material roadMaterial;
    public Material sides;
    public Material side2;
    // Start is called before the first frame update
    void Start()
    {
        
        roadMaterial = gameObject.GetComponent<MeshRenderer>().material;
        sides=GameObject.Find("side").GetComponent<MeshRenderer>().material;
        side2=GameObject.Find("side2").GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        roadOffsetY += roadSpeed * Time.deltaTime;
        roadMaterial.SetTextureOffset("_MainTex", new Vector2(0, -roadOffsetY));
        sides.SetTextureOffset("_MainTex", new Vector2(roadOffsetY*(0.1f), 0));
        side2.SetTextureOffset("_MainTex", new Vector2(-roadOffsetY*(0.1f), 0));
    }
}
