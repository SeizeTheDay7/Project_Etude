using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class StartDirector : MonoBehaviour
{
    [SerializeField] private Cinemachine.CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject show_player;
    [SerializeField] private GameObject player;
    [SerializeField] private float titleFadeDuration;
    [SerializeField] private float PressAnyKeyFadeDuration;
    [SerializeField] private float IntroMusicFadeDuration;
    [SerializeField] private GameObject canvas;
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private TextMeshProUGUI PressAnyKey;
    [SerializeField] private GameObject middle;
    [SerializeField] private float orthTargetSize;
    [SerializeField] private float orthSmoothTime;
    [SerializeField] private AudioSource Metronome;
    [SerializeField] private AudioSource IntroMusic;
    private bool game_start = false;
    private float velocity = 0.0f;
    SpriteRenderer showPlayerSprite;

    // Start is called before the first frame update
    void Start()
    {
        showPlayerSprite = show_player.GetComponent<SpriteRenderer>();
        Metronome.Play();
        Metronome.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && !game_start)
        {
            StartCoroutine(TitleFadeOut());
            StartCoroutine(PressAnykeyFadeOut());
            StartCoroutine(IntroMusicFadeOut());
        }
        if (!game_start)
        {
            SetTextAlpha(PressAnyKey, Mathf.PingPong(Time.time * 1f, 0.75f) + 0.25f);
        }
    }

    private IEnumerator TitleFadeOut()
    {
        game_start = true;
        yield return new WaitForSeconds(1.5f); // 1초 대기
        float elapsedTime = 0f;
        player.SetActive(true);
        show_player.SetActive(true);

        while (elapsedTime < titleFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            SetTextAlpha(Title, Mathf.SmoothStep(1f, 0f, elapsedTime / titleFadeDuration)); // 초반에 급하게 사라지다가 후반에 느리게 사라지게
            yield return null;
        }

        yield return new WaitForSeconds(0.5f); // 0.5초 대기

        // 완전히 투명해진 뒤 오브젝트 비활성화
        canvas.SetActive(false);
        elapsedTime = 0f;

        while (elapsedTime < titleFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            // 플레이어 sprite renderer alpha 서서히 증가시키기
            if (showPlayerSprite != null)
            {
                showPlayerSprite.color = new Color(showPlayerSprite.color.r, showPlayerSprite.color.g, showPlayerSprite.color.b, Mathf.SmoothStep(0f, 1f, elapsedTime / titleFadeDuration));
            }
            yield return null;
        }

        virtualCamera.Follow = middle.transform;
        StartCoroutine(SmoothOrthographicSizeChange());
    }

    private IEnumerator PressAnykeyFadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < PressAnyKeyFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            SetTextAlpha(PressAnyKey, Mathf.Lerp(1f, 0f, elapsedTime / PressAnyKeyFadeDuration)); // 서서히 Alpha 값을 0으로
            yield return null;
        }
    }

    private IEnumerator SmoothOrthographicSizeChange()
    {
        while (Mathf.Abs(virtualCamera.m_Lens.OrthographicSize - orthTargetSize) > 0.01f)
        {
            virtualCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(
                virtualCamera.m_Lens.OrthographicSize,
                orthTargetSize,
                ref velocity,
                orthSmoothTime
            );
            yield return null;
        }

        // 플레이어 스크립트 활성화
        player.GetComponent<Bar_Judge_Movement>().enabled = true;
    }

    private IEnumerator IntroMusicFadeOut()
    {
        float startVolume = IntroMusic.volume;

        // fadeDuration 동안 volume을 점진적으로 줄임
        for (float t = 0; t < IntroMusicFadeDuration; t += Time.deltaTime)
        {
            IntroMusic.volume = Mathf.Lerp(startVolume, 0, t / IntroMusicFadeDuration);
            yield return null;
        }

        // 완전히 꺼진 후
        IntroMusic.volume = 0;
        IntroMusic.Stop();
    }

    public void StartGame()
    {
        Metronome.volume = 0;
        float elapsedTime = 0f;
        player.SetActive(false);

        Debug.Log("Game Start");
        GameManager.Instance.LoadNextMap();
    }

    private void SetTextAlpha(TextMeshProUGUI text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}
