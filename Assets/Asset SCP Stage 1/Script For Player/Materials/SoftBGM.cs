using UnityEngine;

public class SoftBGM : MonoBehaviour
{
    [Header("Audio Source & Clip")]
    public AudioSource bgmSource;  // AudioSource untuk backsound
    public AudioClip bgmClip;      // Clip 6 menit

    [Header("Volume Settings")]
    [Range(0f,1f)]
    public float targetVolume = 0.25f;  // volume soft
    public float fadeInSpeed = 0.1f;    // semakin kecil, semakin lembut fade

    void Start()
    {
        if (bgmSource == null)
            bgmSource = gameObject.AddComponent<AudioSource>();

        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = 0f;
        bgmSource.Play();
    }

    void Update()
    {
        // Fade in lembut ke volume target
        if (bgmSource.volume < targetVolume)
        {
            bgmSource.volume += fadeInSpeed * Time.deltaTime;
            if (bgmSource.volume > targetVolume)
                bgmSource.volume = targetVolume;
        }
    }
}
