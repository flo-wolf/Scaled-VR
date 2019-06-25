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
            public float waterLiters; // { get; private set; }
            public float gasGrams;   // gas weight
            public float areaSqrMeters;  // sqr meters
            public float kcal; // kilo calories
        }

        public Emission emission = new Emission
        {
            waterLiters = 1000f,
            gasGrams = 1000f,
            areaSqrMeters = 10f,
            kcal = 1000f,
        };
    }
}
