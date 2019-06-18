using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Change
{
    [RequireComponent(typeof(Rigidbody))]
    public class Interactable : MonoBehaviour
    {

        [HideInInspector]
        public Change.Hand m_ActiveHand = null;

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

