using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace Change
{
    public class Hand : MonoBehaviour
    {
        public SteamVR_Action_Boolean m_GrabAction = null;

        private SteamVR_Behaviour_Pose m_Pose = null;
        private FixedJoint m_Joint = null;
        private SpringJoint m_JointSpring = null;

        [SerializeField] private Interactable m_CurrentInteractable = null;
        [SerializeField] private List<Interactable> m_ContectInteractables = new List<Interactable>();


        private void Start()
        {
            m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
            m_Joint = GetComponent<FixedJoint>();
        }

        private void Update()
        {
            // Trigger Down
            if (m_GrabAction.GetStateDown(m_Pose.inputSource))
            {
                print(m_Pose.inputSource + " Trigger Down");
                Pickup();
            }

            // Trigger Up
            if (m_GrabAction.GetStateUp(m_Pose.inputSource))
            {
                print(m_Pose.inputSource + " Trigger Up");
                Drop();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Interactable"))
                return;

            Interactable interactable = other.gameObject.GetComponent<Interactable>();
            if (interactable == null)
                return;

            m_ContectInteractables.Add(interactable);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Interactable"))
                return;

            Interactable interactable = other.gameObject.GetComponent<Interactable>();
            if (interactable == null)
                return;

            m_ContectInteractables.Remove(interactable);
        }

        public void Pickup()
        {
            // Ger nearest
            m_CurrentInteractable = GeetNearestInteractable();

            // Null check
            if (!m_CurrentInteractable)
                return;

            Debug.Log("nearest on pickup: " + m_ContectInteractables);

            // already held, check
            if (m_CurrentInteractable.m_ActiveHand)
            {
                m_CurrentInteractable.m_ActiveHand.Drop();
            }

            // position
            m_CurrentInteractable.transform.position = transform.position;

            // attach
            Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
            m_Joint.connectedBody = targetBody;

            // set active hand
            m_CurrentInteractable.m_ActiveHand = this;
        }

        public void Drop()
        {
            // null check
            if (!m_CurrentInteractable)
                return;

            Debug.Log("nearest on drop: " + m_ContectInteractables);

            // apply velocity
            Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
            targetBody.velocity = m_Pose.GetVelocity();
            targetBody.angularVelocity = m_Pose.GetAngularVelocity();

            // detach
            m_Joint.connectedBody = null;

            // clear
            m_CurrentInteractable.m_ActiveHand = null;
            m_CurrentInteractable = null;
        }

        private Interactable GeetNearestInteractable()
        {
            Interactable nearest = null;
            float minDistance = float.MaxValue;
            float distance = 0f;

            foreach(Interactable interactable in m_ContectInteractables)
            {
                distance = (interactable.transform.position - transform.position).sqrMagnitude;

                if(distance < minDistance)
                {
                    minDistance = distance;
                    nearest = interactable;
                }
            }

            return nearest;
        }
    }
}