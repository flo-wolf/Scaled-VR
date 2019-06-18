using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Change
{
    public class Food : Interactable
    {
        [Serializable]
        public struct Emission
        {
            public float waterConsumption; // { get; private set; }
            public float gasConsumption;
        }

        public Emission emission = new Emission
        {
            waterConsumption = 1f,
            gasConsumption = 1f
        };

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
