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

    // --- 新增：UI 控制设置 ---
    [Header("UI Feedback Settings")]
    public GameObject feedbackCanvas; // 指定的 Canvas 或 UI 面板
    public float uiDisplayTime = 2f;  // 公开变量：等待时间

    private Coroutine uiCoroutine;    // 用于管理协程，防止多次触发冲突

    void Start()
    {
        if (rBody == null)
            rBody = GetComponent<Rigidbody>();

        // 初始确保 Canvas 是失活的
        if (feedbackCanvas != null)
            feedbackCanvas.SetActive(false);
    }

    // --- 修改：统一的反馈逻辑（音效 + UI） ---
    private void TriggerFeedback()
    {
        // 播放音效
        if (interactionAudio != null)
        {
            interactionAudio.Stop();
            interactionAudio.Play();
        }

        // 激活 UI 并开始倒计时
        if (feedbackCanvas != null)
        {
            // 如果之前已经在倒计时，先停止它，重新开始计时
            if (uiCoroutine != null)
                StopCoroutine(uiCoroutine);

            uiCoroutine = StartCoroutine(UIShowAndHide());
        }
    }

    // 新增：处理 UI 显示和隐藏的协程
    private IEnumerator UIShowAndHide()
    {
        feedbackCanvas.SetActive(true);    // 激活
        yield return new WaitForSeconds(uiDisplayTime); // 等待公开变量设定的时间
        feedbackCanvas.SetActive(false);   // 失活
        uiCoroutine = null;
    }

    public void SpinAxis(Vector3 axis, float force)
    {
        rBody.AddRelativeTorque(axis * force * (forceFactor + RandomRoll()));
        TriggerFeedback(); // 触发反馈
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
        TriggerFeedback(); // 触发反馈
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