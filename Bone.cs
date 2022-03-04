using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone
{
    //This is a node of a creature
    //Position - Handled by being a gameobject?
    //Connected Nodes
    public GameObject physical;
    public Vector3 position;
    public List<Bone> neighbors;

    public Bone()
    {
        position = Vector3.zero;
        neighbors = new List<Bone>();
        //physical = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //physical.AddComponent<Rigidbody>();
        //physical.AddComponent<LineRenderer>();
    }
}
