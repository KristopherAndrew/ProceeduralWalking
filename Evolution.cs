using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evolution : MonoBehaviour
{
    //
    //Create a list of 1000 creatures
    //
    //Easy
    //Have an array of type "creature" (Or game objects with creatures on them sonce creature is a component?)
    public GameObject creaturePrefab;
    public int numCreatures;
    public List<GameObject> creatures;
    public float WaitTime = 15f;
    public int numGen;
    public int currGen = 0;
    private bool testing = false;
    private int currCreature = 0;
    private GameObject focus;
    private float currTime = 0f;
    public float averageFitness;

    public void Awake()
    {
        creatures = new List<GameObject>();
        for (int i = 0; i < numCreatures; i++)
        {
            creatures.Add(Instantiate(creaturePrefab,Vector3.zero, Quaternion.identity));
            creatures[i].GetComponent<Creature>().CreateNewCreature(); 
            creatures[i].SetActive(false);
        }
    }

    public void FixedUpdate()
    {
        if (currGen < numGen)
        {
            FitnessTest();
        }
    }

    public void FastTime()
    {
        Time.timeScale = 100f;
    }

    public void NormTime()
    {
        Time.timeScale = 1f;
    }

    public void SlowTime()
    {
        Time.timeScale = 0.5f;
    }
    //
    //Test each creature to determine its fitness
    //
    //Problem. Takes time? - Think of ways to optimize
    //Load a creature
    //Give it time
    //Score it

    public void NextGeneration()
    {
        numGen++;
    }

    public void FitnessTest()
    {
        //Load a creature
        if (!testing)
        {
            focus = creatures[currCreature];
            focus.SetActive(true);
            focus.transform.position = Vector3.zero;
            focus.GetComponent<Creature>().CreateOldCreature();
            currTime = 0;
            testing = true;
        }

        //Wait for a time
        if (currTime > WaitTime)
        {
            //Unload creature
            float fitness = focus.transform.position.x;
            //Assign Fitness to a creature
            focus.GetComponent<Creature>().fitness = fitness;
            //Destroy Creature
            focus.GetComponent<Creature>().ClearCreature();
            focus.SetActive(false);
            currCreature++;
            testing = false;
        }
        currTime += Time.deltaTime;

        //when all creatures are done, begin evolution
        if (currCreature == numCreatures)
        {
            //Sort (inefficient // improve later)
            for (int j = 0; j < numCreatures; j++)
            {
                int MINindex = 0;
                float MIN = creatures[0].GetComponent<Creature>().fitness;
                for (int i = 0; i < numCreatures-j; i++)
                {
                    if (creatures[i].GetComponent<Creature>().fitness < MIN)
                    {
                        MIN = creatures[i].GetComponent<Creature>().fitness;
                        MINindex = i;
                    }
                }
                GameObject temp = creatures[MINindex];
                creatures[MINindex] = creatures[numCreatures - j - 1];
                creatures[numCreatures - j - 1] = temp;
            }

            //Get the average fitness
            float totalFitness = 0f;
            for (int i = 0; i < numCreatures; i++)
            {
                totalFitness += creatures[i].GetComponent<Creature>().fitness;
            }
            averageFitness = totalFitness / numCreatures;
            Evolve();
            currCreature = 0;
        }
    }

    
    public void Evolve()
    {
        //Kill bottom half
        for (int i = numCreatures / 2; i < numCreatures; i++)
        {
            Evolve(creatures[i - numCreatures / 2], creatures[i]);
        }
        currGen += 1;
    }

    public void Evolve(GameObject parentCreature, GameObject extinctCreature)
    {
        Creature creature = parentCreature.GetComponent<Creature>();
        Creature childCreature = extinctCreature.GetComponent<Creature>();
        int numBones = creature.numBones;

        //Copy the parent creature into the extinct creature
        childCreature.CopyCreature(creature);
        //Replace all variables with variables from the parent

        //Pick how many changes to make
        int numMute = Random.Range(0, 4);

        for (int i = 0; i < numMute; i++)
        {
            //Pick a random thing to change
            int mutation = Random.Range(1, 100); //percentage
            //Switch statement
            switch (mutation)
            {
                case 1:
                    //Change bone position
                    //Pick a bone at random
                    int changedBone = Random.Range(0, numBones - 1);
                    //Change X, Y, or Z by a random amount
                    float change = Random.Range(-1f, 1f);
                    int dir = Random.Range(0, 2);
                    if (dir == 0)
                    {
                        childCreature.bones[changedBone].position.x += change;
                    }
                    else if (dir == 1)
                    {
                        childCreature.bones[changedBone].position.y += change;
                    }
                    else
                    {
                        childCreature.bones[changedBone].position.z += change;
                    }
                    break;
                case 2:
                    //Add a new bone
                    //Create a new bone
                    //Give it a random position
                    //Attach it to the previous bone
                    break;
                case 3:
                    //Add a new muscle
                    //Connect two random bones that don't previously have a connection
                    //Try a few times before stopping.
                    //For the case of every bone connected to every bone.
                    break;
                case 4:
                    //Remove a bone
                    //Reconnect spine muscles
                    //Remove all other muscles connected to the removed bone
                    //Remove bone
                    break;
                case 5:
                    //Remove a muscle (that is not to the previous or next bone)
                    break;

            }
        }

    }

    //Mutate the top half
    //Hard?
    //Choose to do one of the following
    //Add a bone (connected to previous bone)
    //Add a muscle
    //Remove a bone (Need to remove self from every bone it is connected to AND connect next bone in list to bone before it) (Linked list?)
    //Remove a muscle (Note: do not want to remove spine muscles)
    //Change attribute (friction, muscle strength, heartbeat?, neurons?, etc.) (This one could be very complicated)
}
