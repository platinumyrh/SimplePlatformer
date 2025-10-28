using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeAnchor : MonoBehaviour
{
    [System.Serializable]
    public struct AnchorState
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localScale;
        public int hp;
        public float duration;
    }

    [System.Serializable]
    struct Sample
    {
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
        public int hp;
        public float t;
    }

    [Header("Refs")]
    public PlayerController player;
    public Animator animator;
    public GameObject ghostPrefab;

    [Header("Record Settings")]
    public int recordFramesCapacity = 1200;
    public float maxRecordSeconds = 20f;
    public bool disablePhysicsDuringRewind = true;

    [Header("Rewind Settings")]
    public float rewindSpeedMultiplier = 1f;
    public float rewindTimeScale = 0.2f;
    public float rewindMoveSpeed = 0.5f;

    private List<Sample> samples = new List<Sample>(1024);
    private float recordStartTime;
    private bool isRecording = false;
    private bool isRewinding = false;

    private AnchorState anchor;
    private GameObject ghostInstance;
    private float originalTimeScale;
    private float originalFixedDeltaTime;

    public System.Action onRewindComplete;

    void Update()
    {
        // ��E������ê��
        if (Input.GetKeyDown(KeyCode.E))
            StartAnchorRecording();

        // ��R���ֶ�����
        if (Input.GetKeyDown(KeyCode.R) && !isRewinding)
            StartRewind();
    }

    void FixedUpdate()
    {
        if (!isRecording || isRewinding) return;

        float t = Time.time - recordStartTime;

        // �Ƴ�������
        while (samples.Count > 0 && samples[0].t + maxRecordSeconds < t)
        {
            samples.RemoveAt(0);
        }

        if (t <= maxRecordSeconds)
        {
            samples.Add(new Sample
            {
                pos = player.transform.position,
                rot = player.transform.rotation,
                scale = player.transform.localScale,
                hp = player.currentHealth,
                t = t
            });
        }
    }

    public void StartAnchorRecording()
    {
        if (isRewinding)
        {
            Debug.Log("���ڻ����У��޷���ʼ¼��");
            return;
        }

        try
        {
            if (animator.isActiveAndEnabled)
            {
                animator.StopPlayback();
                animator.StopRecording();
            }

            anchor = new AnchorState
            {
                position = player.transform.position,
                rotation = player.transform.rotation,
                localScale = player.transform.localScale,
                hp = player.currentHealth,
                duration = 0f
            };

            if (ghostInstance != null) 
                Destroy(ghostInstance);
            
            if (ghostPrefab != null)
                ghostInstance = Instantiate(ghostPrefab, anchor.position, anchor.rotation);

            samples.Clear();
            recordStartTime = Time.time;
            isRecording = true;

            samples.Add(new Sample
            {
                pos = player.transform.position,
                rot = player.transform.rotation,
                scale = player.transform.localScale,
                hp = player.currentHealth,
                t = 0f
            });

            if (animator.isActiveAndEnabled)
            {
                animator.StartRecording(recordFramesCapacity);
            }

            Debug.Log("ê�����óɹ���Ѫ��: " + anchor.hp);
        }
        catch (System.Exception e)
        {
            Debug.LogError("����ê��ʱ����: " + e.Message);
            isRecording = false;
        }
    }

    public void StartRewind()
    {
        if (isRewinding) return;

        if (!isRecording)
        {
            Debug.Log("��������ê�㣨��E����");
            return;
        }

        if (samples.Count <= 5)
        {
            Debug.Log("¼��ʱ��̫�̣��޷�����");
            return;
        }

        Debug.Log("��ʼ�ֶ�����");
        StopAllCoroutines();
        StartCoroutine(RewindAlongPath());
    }

    private IEnumerator RewindAlongPath()
    {
        isRewinding = true;
        player.isRewinding = true;

        // ����ԭʼʱ������
        originalTimeScale = Time.timeScale;
        originalFixedDeltaTime = Time.fixedDeltaTime;
        
        Time.timeScale = rewindTimeScale;
        Time.fixedDeltaTime = 0.02f * rewindTimeScale;

        if (player.healthBar != null)
            player.healthBar.gameObject.SetActive(false);

        try
        {
            if (animator.isActiveAndEnabled)
            {
                animator.StopRecording();
                animator.StartPlayback();
            }

            anchor.duration = samples.Count > 0 ? samples[samples.Count - 1].t : 0f;
            int idxStep = Mathf.Max(1, Mathf.RoundToInt(rewindSpeedMultiplier));

            var rb2d = player.rb;
            bool originalSimulated = rb2d != null ? rb2d.simulated : false;
            
            if (disablePhysicsDuringRewind && rb2d != null)
                rb2d.simulated = false;

            // ���ݹ���
            for (int i = samples.Count - 1; i >= 0; i -= idxStep)
            {
                if (!isRewinding) break;

                var s = samples[i];

                if (animator.isActiveAndEnabled)
                {
                    float playbackTime = Mathf.Clamp(s.t, 0, anchor.duration);
                    animator.playbackTime = playbackTime;
                    animator.Update(0f);
                }

                player.transform.position = Vector3.Lerp(player.transform.position, s.pos, rewindMoveSpeed);
                player.transform.rotation = s.rot;
                player.transform.localScale = s.scale;
                player.SetHealthWithoutNotification(s.hp);

                if (rb2d != null)
                    rb2d.velocity = Vector2.zero;

                yield return new WaitForSecondsRealtime(0.02f);
            }

            if (isRewinding)
            {
                player.transform.position = anchor.position;
                player.transform.rotation = anchor.rotation;
                player.transform.localScale = anchor.localScale;
                player.SetHealthWithoutNotification(anchor.hp);
            }
        }
        finally
        {
            CleanupRewind();
        }
    }

    private void CleanupRewind()
    {
        try
        {
            // �ָ�ʱ������
            Time.timeScale = originalTimeScale;
            Time.fixedDeltaTime = originalFixedDeltaTime;

            // �ָ�Ѫ����ʾ
            if (player.healthBar != null)
            {
                player.healthBar.gameObject.SetActive(true);
                player.healthBar.UpdateHealth();
            }

            if (ghostInstance != null)
                Destroy(ghostInstance);

            if (animator.isActiveAndEnabled)
            {
                animator.StopPlayback();
                animator.Rebind();
            }

            if (player.rb != null && disablePhysicsDuringRewind)
                player.rb.simulated = true;

            player.isRewinding = false;
            isRewinding = false;

            // ȷ��Ѫ����ȷ
            player.SetHealth(anchor.hp);

            samples.Clear();
            isRecording = false;

            onRewindComplete?.Invoke();

            Debug.Log("�ֶ��������");
        }
        catch (System.Exception e)
        {
            Debug.LogError("�������ʱ����: " + e.Message);
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
    }

    public void ForceStopRewind()
    {
        if (isRewinding)
        {
            StopAllCoroutines();
            CleanupRewind();
            Debug.Log("ǿ��ֹͣ����");
        }
    }

    // ���������
    public bool IsRecording => isRecording;
    public bool IsRewinding => isRewinding;
    public int SampleCount => samples.Count;
    public AnchorState Anchor => anchor;
}