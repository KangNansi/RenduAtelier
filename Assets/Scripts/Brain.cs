using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour {
    float hunger = 1.0f;
    float thirst = 1.0f;
    float moral = 1.0f;
    float energy = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        hunger -= 0.05f * Time.deltaTime;
        thirst -= 0.03f * Time.deltaTime;
        moral -= 0.005f * Time.deltaTime;
        energy -= 0.01f * Time.deltaTime;
	}
}
