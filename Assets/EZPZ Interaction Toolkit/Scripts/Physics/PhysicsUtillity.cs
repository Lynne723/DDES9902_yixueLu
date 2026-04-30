//EZPZ Interaction Toolkit
//by Matt Cabanag
//created 09 Mar 2024

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsUtillity : MonoBehaviour
{
    public Rigidbody rBody;
    public float forceFactor = 10;
    public float randomComponent = 0;

    [Header("Audio Settings")]
    public AudioSource interactionAudio;

    
    [Header("UI Feedback Settings")]
    public GameObject feedbackCanvas; 
    public float uiDisplayTime = 2f;

    private Coroutine uiCoroutine;    

    void Start()
    {
        if (rBody == null)
            rBody = GetComponent<Rigidbody>();

       
        if (feedbackCanvas != null)
            feedbackCanvas.SetActive(false);
    }

    
    private void TriggerFeedback()
    {
      
        if (interactionAudio != null)
        {
            interactionAudio.Stop();
            interactionAudio.Play();
        }

        
        if (feedbackCanvas != null)
        {
            
            if (uiCoroutine != null)
                StopCoroutine(uiCoroutine);

            uiCoroutine = StartCoroutine(UIShowAndHide());
        }
    }

    
    private IEnumerator UIShowAndHide()
    {
        feedbackCanvas.SetActive(true);    
        yield return new WaitForSeconds(uiDisplayTime); 
        feedbackCanvas.SetActive(false);   
        uiCoroutine = null;
    }

    public void SpinAxis(Vector3 axis, float force)
    {
        rBody.AddRelativeTorque(axis * force * (forceFactor + RandomRoll()));
        TriggerFeedback(); 
    }

    public void SpinAxisX(float force)
    {
        SpinAxis(Vector3.right, force);
    }

    public void SpinAxisY(float force)
    {
        SpinAxis(Vector3.up, force);
    }

    public void SpinAxisZ(float force)
    {
        SpinAxis(Vector3.forward, force);
    }

    public void AddForce(Vector3 axis, float force)
    {
        rBody.AddRelativeForce(axis * force * (forceFactor + RandomRoll()));
        TriggerFeedback(); 
    }

    public void AddForce(float force)
    {
        AddForceZ(force);
    }

    public void AddForceX(float force)
    {
        AddForce(Vector3.right, force * forceFactor);
    }

    public void AddForceY(float force)
    {
        AddForce(Vector3.up, force * forceFactor);
    }

    public void AddForceZ(float force)
    {
        AddForce(Vector3.forward, force * forceFactor);
    }

    public float RandomRoll()
    {
        return Random.Range(0, randomComponent);
    }
}