using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public enum MANIPULATION_MODE
{
    HAND,
    FORCE
}

public class HandManipulation : MonoBehaviour
{
    private Rigidbody rb;
    private GameObject pickableObject;
    private GameObject pickedObject;
    public bool isRightHand;

    public Transform LeftHand = null;
    public Transform RightHand = null;

    private MANIPULATION_MODE   m_ManipulationMode = MANIPULATION_MODE.HAND;
    private Transform           m_HandManipulating = null;
    private SteamVR_Input_Sources m_ManipulatingHandIndex;
    private List<Transform>     m_ObjectsUnderForce = new List<Transform>();
    private Vector3 m_LastHandPosition = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pickableObject = null;
    }

    void Update()
    {

        if (SteamVR_Input._default.inActions.GrabPinch.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            m_HandManipulating = LeftHand;
            m_ManipulatingHandIndex = SteamVR_Input_Sources.LeftHand;
            StartCoroutine(ForceManipulationCoroutine());
        }
        if (SteamVR_Input._default.inActions.GrabPinch.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            m_HandManipulating = RightHand;
            m_ManipulatingHandIndex = SteamVR_Input_Sources.RightHand;
            StartCoroutine(ForceManipulationCoroutine());
        }

        if (GrabGripDown())
        {
            pickedObject = pickableObject;
        }
        else if (GrabGripUp())
        {
            Debug.Log(pickedObject);
            if (pickedObject)
            {
                if (isRightHand)
                {

                    pickedObject.GetComponent<Rigidbody>().velocity = SteamVR_Input._default.inActions.SkeletonRightHand.GetVelocity(SteamVR_Input_Sources.RightHand);
                }
                else
                {
                    pickedObject.GetComponent<Rigidbody>().velocity = SteamVR_Input._default.inActions.SkeletonLeftHand.GetVelocity(SteamVR_Input_Sources.LeftHand);
                }
                pickedObject = null;
            }
            Debug.Log(pickedObject);
        }
        if (pickedObject)
        {
            pickedObject.transform.position = transform.position;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Pickable"))
        {
            pickableObject = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickable"))
        {
            pickableObject = null;
        }
    }

    bool GrabGripDown()
    {
        if (isRightHand)
        {
            return SteamVR_Input._default.inActions.GrabGrip.GetStateDown(SteamVR_Input_Sources.RightHand);
        }
        return SteamVR_Input._default.inActions.GrabGrip.GetStateDown(SteamVR_Input_Sources.LeftHand);
    }

    bool GrabGripUp()
    {
        if (isRightHand)
        {
            return SteamVR_Input._default.inActions.GrabGrip.GetStateUp(SteamVR_Input_Sources.RightHand);
        }
        return SteamVR_Input._default.inActions.GrabGrip.GetStateUp(SteamVR_Input_Sources.LeftHand);
    }

    private IEnumerator ForceManipulationCoroutine()
    {
        bool oops = false;
        m_ManipulationMode = MANIPULATION_MODE.FORCE;

        while(m_ManipulationMode == MANIPULATION_MODE.FORCE)
        {
            if (m_LastHandPosition == Vector3.zero)
            {
                m_LastHandPosition = m_HandManipulating.position;
            }
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Grabable"))
                {
                    if (hit.collider.GetComponent<Rigidbody>())
                    {
                        if (!hit.collider.GetComponent<SpringJoint>())
                        {
                            AddAndConfigureSpringJoint(hit.collider.GetComponent<Rigidbody>());
                        }
                    }
                    
                    m_ObjectsUnderForce.Add(hit.collider.transform);
                }
            }

            foreach (Transform obj in m_ObjectsUnderForce)
            {
                //obj.transform.position = transform.position;
                if (Vector3.Distance(obj.position, rb.transform.position) < 1f)
                {
                    obj.GetComponent<Rigidbody>().isKinematic = true;

                    if (obj.GetComponent<SpringJoint>())
                    {
                        Destroy(obj.GetComponent<SpringJoint>());
                    }
                    oops = true;
                    //obj.parent = m_HandManipulating;
                    //obj.RotateAround(m_HandManipulating.position, Vector3.up, 500 * Time.deltaTime);
                }
            }

            if(oops)
            {
                foreach (Transform obj in m_ObjectsUnderForce)
                {
                    obj.position = m_HandManipulating.position;
                }

            }

            if (SteamVR_Input._default.inActions.GrabPinch.GetStateUp(m_ManipulatingHandIndex))
            {
                m_ManipulationMode = MANIPULATION_MODE.HAND;

                foreach (Transform obj in m_ObjectsUnderForce)
                {
                    if (obj.GetComponent<Rigidbody>())
                    {
                        obj.GetComponent<Rigidbody>().isKinematic = false;
                        //obj.parent = null;
                        if (obj.GetComponent<SpringJoint>())
                        {
                            Destroy(obj.GetComponent<SpringJoint>());
                        }
                        obj.GetComponent<Rigidbody>().AddForce(((m_HandManipulating.position - m_LastHandPosition) * 500) + (m_HandManipulating.forward * 50));
                        obj.GetComponent<Rigidbody>().useGravity = true;
                    }
                }
                m_ObjectsUnderForce.Clear();
            }


            m_LastHandPosition = m_HandManipulating.position;

            yield return null; 
        }

        m_HandManipulating = null;
        oops = false;
    }

    private void AddAndConfigureSpringJoint(Rigidbody iObjectRb)
    {
        iObjectRb.useGravity = false;
        SpringJoint AddedSpringJoint = iObjectRb.gameObject.AddComponent<SpringJoint>();
        AddedSpringJoint.connectedBody = rb;
        AddedSpringJoint.anchor = Vector3.zero;
        AddedSpringJoint.autoConfigureConnectedAnchor = false;
        AddedSpringJoint.connectedAnchor = Vector3.zero;
        AddedSpringJoint.spring = 10;
        AddedSpringJoint.damper = 1;
        
    }
}
