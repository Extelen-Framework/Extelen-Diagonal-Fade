using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[ExecuteAlways()]
[RequireComponent(typeof(RectTransform))]
public class DiagonalFade : MonoBehaviour
{

    //Params.

    //Non Static
    [Header("Fade References")]
    [SerializeField] private RectTransform m_rect = null;
    private float RectWidth { get => m_rect.rect.width; }

    [SerializeField] private RectTransform m_childRect = null;

    [Header("Fade Animation")]
    [SerializeField]
    private AnimationCurve m_animationCurve =
        new AnimationCurve(
            new Keyframe[2] {
                    new Keyframe(0, 0),
                    new Keyframe(1, 1)
                }
            );

    [SerializeField] private float m_animationTime = 0.5f;
    public float AnimationTime { set => m_animationTime = value; }

    [Header("Fade Values")]
    [SerializeField] private float m_rectOffset = 256 + 64;

    [SerializeField] private bool m_inverseHorizontal = false;
    [SerializeField] private bool m_inverseVertical = false;

    [Header("Fade Calls")]
    [SerializeField] private UnityEvent<float> m_loadingPercentCall = null;

    private Coroutine m_routine = null;
    public bool IsLoading { get => m_routine != null; }

    private float m_loadPercent = 0;
    public float LoadPercent { get => m_loadPercent; }

    //Methods.

    //Monobehaviour
    private void Start()
    {
        SetRectPosition((RectWidth + m_rectOffset) * (m_inverseHorizontal ? -1 : 1));
        m_childRect.localScale = new Vector3(1, m_inverseVertical ? -1 : 1, 1);

        if (Application.isPlaying)
        {
            m_childRect.gameObject.SetActive(false);
        }
    }
    private void OnValidate()
    {
        if (m_rect == null) TryGetComponent<RectTransform>(out m_rect);
    }
    private void Update()
    {
        if (!Application.isPlaying)
        {
            if (m_childRect != null)
            {
                SetRectPosition((RectWidth + m_rectOffset) * (m_inverseHorizontal ? -1 : 1));
                m_childRect.localScale = new Vector3(1, m_inverseVertical ? -1 : 1, 1);
            }
        }
    }

    //Non Static
    public void Fade(int buildIndex, UnityEvent calls = null)
    {
        if (m_routine != null) return;
        m_routine = StartCoroutine(FadeAnimation(buildIndex, calls));
    }

    private void SetRectPosition(float value)
    {
        m_childRect.offsetMin = new Vector2(value, m_childRect.offsetMin.y);
        m_childRect.offsetMax = new Vector2(value, m_childRect.offsetMax.y);
    }

    //Coroutines.
    private IEnumerator FadeAnimation(int roomIndex, UnityEvent calls)
    {
        //Dont destroy object in load.
        m_childRect.gameObject.SetActive(true);

        //Load scene async.
        AsyncOperation m_async = SceneManager.LoadSceneAsync(roomIndex);
        m_async.allowSceneActivation = false;

        //Inverse scale.
        if (m_inverseVertical) m_childRect.localScale = new Vector3(1, -1, 1);

        //Set default rect position and end position.
        float m_startValue = (RectWidth + m_rectOffset) * (m_inverseHorizontal ? -1 : 1);
        float m_endValue = 0;

        SetRectPosition(m_startValue);

        //Move to the center of the screen.
        for (float i = 0; i < m_animationTime; i += Time.unscaledDeltaTime)
        {

            SetRectPosition(Mathf.Lerp(m_startValue, m_endValue, m_animationCurve.Evaluate(i / m_animationTime)));
            yield return null;
        }

        SetRectPosition(m_endValue);

        //Wait for 100%
        if (m_async.progress >= 0.9f) m_async.allowSceneActivation = true;
        else
        {
            //Set load percent.
            m_loadPercent = m_async.progress * 100f;
            if (m_loadingPercentCall != null) m_loadingPercentCall.Invoke(m_loadPercent);
            yield return null;
        }

        //Set 100% load percent and invoke calls.
        m_loadPercent = 100;
        m_loadingPercentCall?.Invoke(100);
        calls?.Invoke();

        //Reset values.
        m_endValue = -m_startValue;
        m_startValue = 0;

        //Finish movement.
        for (float i = 0; i < m_animationTime; i += Time.unscaledDeltaTime)
        {
            SetRectPosition(Mathf.Lerp(m_startValue, m_endValue, m_animationCurve.Evaluate(i / m_animationTime)));
            yield return null;
        }

        m_routine = null;

        m_childRect.gameObject.SetActive(false);
    }
}

