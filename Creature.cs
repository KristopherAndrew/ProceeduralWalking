using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public GameObject bonePrefab;

    public int numBones = 5;
    public int numMuscles = 2;
    public float sizeX, sizeY, sizeZ = 10f;

    public float spring = 1f;
    public float restDistance = 1f;

    public float Heartbeat = 25.0f;
    private float lastbeat = 0f;
    private bool beat = true;

    //public for visibilty in runtime
    public float fitness = 0f;
    
    public List<Bone> bones;

    private void Awake()
    {
        /*
        if (bones == null)
        {
            CreateNewCreature();
        }
        CreateOldCreature();
        */
    }

    public void FixedUpdate()
    {
        //Create Heartbeat
        //Check time
        //if enough time has passed
        //Change spring variables
        if (lastbeat > Heartbeat)
        {
            lastbeat = 0.0f;
            //change spring variables
            if (beat)
            {
                //spring = 1f;
                restDistance = 1f;
                beat = false;
            }
            else
            {
                //spring = 1f;
                restDistance = 5f;
                beat = true;
            }
        }
        lastbeat += Time.deltaTime;

        //Add forces
        for (int i = 0; i < numBones; i++)
        {
            GameObject currBone = bones[i].physical;
            for (int j = 0; j < bones[i].neighbors.Count; j++)
            {
                GameObject nextBone = bones[i].neighbors[j].physical;

                float springX, springY, springZ;

                //Calculate spring force in each direction
                springX = -spring * (Mathf.Abs(currBone.transform.position.x - nextBone.transform.position.x) - restDistance);
                springY = -spring * (Mathf.Abs(currBone.transform.position.y - nextBone.transform.position.y) - restDistance);
                springZ = -spring * (Mathf.Abs(currBone.transform.position.z - nextBone.transform.position.z) - restDistance);
                
                //Reverse direction to match coordinate system
                if ((currBone.transform.position.x - nextBone.transform.position.x) < 0)
                {
                    springX = -springX;
                }
                if ((currBone.transform.position.y - nextBone.transform.position.y) < 0)
                {
                    springY = -springY;
                }
                if ((currBone.transform.position.z - nextBone.transform.position.z) < 0)
                {
                    springZ = -springZ;
                }

                //Apply forces
                currBone.GetComponent<Rigidbody>().AddForce( springX,  springY,  springZ);
                nextBone.GetComponent<Rigidbody>().AddForce(-springX, -springY, -springZ);
            }
        }

        float avgX = 0, avgY = 0, avgZ = 0;
        for (int i = 0; i < numBones; i++)
        {
            LineRenderer muscle = bones[i].physical.GetComponent<LineRenderer>();
            for (int j = 0; j < bones[i].neighbors.Count * 2; j = j + 2)
            {
                muscle.SetPosition(j, bones[i].physical.transform.position);
                muscle.SetPosition(j + 1, bones[i].neighbors[j / 2].physical.transform.position);
            }

            avgX += bones[i].physical.transform.position.x;
            avgY += bones[i].physical.transform.position.y;
            avgZ += bones[i].physical.transform.position.z;
        }

        this.transform.position = new Vector3(avgX/numBones, avgY/numBones, avgZ/numBones);
    }

    public void CreateNewCreature()
    {
        bones = new List<Bone>();
        //Create bones
        for (int i = 0; i < numBones; i++)
        {
            float tempX = Random.Range(0.0f, sizeX);
            float tempY = Random.Range(0.0f, sizeY);
            float tempZ = Random.Range(0.0f, sizeZ);
            Bone newBone = new Bone();
            newBone.position = new Vector3(tempX, tempY, tempZ);
            bones.Add(newBone);
        }
        
        //Connect all bones to a previous bone
        for (int i = 1; i < numBones; i++)
        {
            bones[i].neighbors.Add(bones[i - 1]);
            bones[i-1].neighbors.Add(bones[i]);
        }

        //Create connections
        for (int i = 0; i < numMuscles; i++)
        {
            //Pick a random bone
            //Pick another random bone
            //Add neighbors
            int bone1 = Random.Range(0, numBones - 1);
            int bone2 = Random.Range(0, numBones - 1);
            bones[bone1].neighbors.Add(bones[bone2]);
            bones[bone2].neighbors.Add(bones[bone1]);
        }

    }

    public void CreateOldCreature()
    {
        //
        if (bones == null)
        {
            CreateNewCreature();
        }
        for(int i = 0; i < numBones; i++)
        {
            bones[i].physical = Instantiate(bonePrefab, bones[i].position, Quaternion.identity);
        }

        //Draw all the connections
        for (int i = 0; i < numBones; i++)
        {
            LineRenderer muscle = bones[i].physical.GetComponent<LineRenderer>();
            muscle.widthMultiplier = 1.0f;
            muscle.positionCount = bones[i].neighbors.Count * 2;
            for (int j = 0; j < bones[i].neighbors.Count * 2; j = j + 2)
            {
                muscle.SetPosition(j, bones[i].physical.transform.position);
                muscle.SetPosition(j + 1, bones[i].neighbors[j / 2].physical.transform.position);
            }
        }
    }

    public void CopyCreature(Creature parentCreature)
    {
        this.ClearCreature();
        bones = parentCreature.bones;
        numBones = parentCreature.numBones;
        numMuscles = parentCreature.numMuscles;
        spring = parentCreature.spring;
        restDistance = parentCreature.restDistance;
        Heartbeat = parentCreature.Heartbeat;
    }

    public void ClearCreature()
    {
        for (int i = 0; i < numBones; i++)
        {
            Destroy(bones[i].physical);
        }
    }

    
}
