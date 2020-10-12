﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]

public class Oscillator : MonoBehaviour
{
  [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
  [SerializeField] float period = 2f;


  [Range(0, 1)] [SerializeField] float movementFactor; // 0 for not moved, 1 for fully moved.

  Vector3 startingPos;

  // Start is called before the first frame update
  void Start()
  {
    startingPos = transform.position;
  }

  // Update is called once per frame
  void Update()
  {
    // set movement factor

    if (period <= Mathf.Epsilon) { return; }

    float cycles = Time.time / period; //grows continually from 0  

    const float tau = Mathf.PI * 2;//about 6.28
    /**
     *  Mathf.PI * 2  = 360°
     */

    float rawSinWave = Mathf.Sin(cycles * tau);
    
    movementFactor = rawSinWave / 2f + .5f;


    Vector3 offset = movementVector * movementFactor;
    transform.position = startingPos + offset;
  }
}


