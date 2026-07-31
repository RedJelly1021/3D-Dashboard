using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class ToastItem : MonoBehaviour
{
    public TMP_Text label;
    private CanvasGroup _cg;
    //private Coroutine _lifeCycleRoutine;

    private void Awake()
    {
        _cg = GetComponent<CanvasGroup>();
    }

    public void Setup(string message, float displayDuration, float fadeDuration)
    {
        label.text = message;
        StartCoroutine(LifeCycle(displayDuration, fadeDuration));
    }

    IEnumerator LifeCycle(float displayDuration, float fadeDuration)
    {
        _cg.alpha = 0f;

        // fade in
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            _cg.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        _cg.alpha = 1f;

        // wait for display duration
        yield return new WaitForSeconds(displayDuration);

        yield return FadeOutAndDestroy(fadeDuration);
    }

    // 외부(ToastUI)에서 강제로 즉시 사라지게 할 때도 이 메서드 재사용
    public void DismissImmediately(float fadeDuration = 0.2f)
    {
        StopAllCoroutines(); // TODO: 코루틴을 추가하게 된다면 코루틴 변수를 사용하여서 특정 코루틴만 중지하도록 수정 필요
        StartCoroutine(FadeOutAndDestroy(fadeDuration));
    }

    IEnumerator FadeOutAndDestroy(float fadeDuration)
    {
        float t = 0f;
        float startAlpha = _cg.alpha;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            _cg.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            yield return null;
        }
        Destroy(gameObject);
    }
}
