using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

    [SerializeField] Vector3 movementVector=new Vector3(10,10,10);
    Vector3 startingPos;
   [Range (1,60)] [SerializeField] float period=2;

	// Use this for initialization
	void Start () {
        startingPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        float cycles = Time.time / period; // grows continually from 0
        const float tau = Mathf.PI * 2;
        float rawSineWave = Mathf.Sin(tau * cycles); // range from -1 to +1
        float movementFactor = rawSineWave / 2f + 0.5f;
        transform.position = startingPos + movementVector * movementFactor;
	}
}
