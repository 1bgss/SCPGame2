using UnityEngine;

public class SirenHeadController : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip screamSound;
    public AudioClip attackSound;
    
    [Header("Auto Animation Settings")]
    public bool autoPlayAnimations = true;
    public float minIdleTime = 3f;
    public float maxIdleTime = 8f;
    
    private float nextActionTime;
    
    void Start()
    {
        // Auto-assign jika belum di-set
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
            
        // Set waktu action pertama
        if (autoPlayAnimations)
            nextActionTime = Time.time + Random.Range(minIdleTime, maxIdleTime);
    }
    
    void Update()
    {
        // Auto play random animations
        if (autoPlayAnimations && Time.time >= nextActionTime)
        {
            RandomAction();
            nextActionTime = Time.time + Random.Range(minIdleTime, maxIdleTime);
        }
        
        // Manual controls (untuk testing)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayScream();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayAttack();
        }
    }
    
    void RandomAction()
    {
        int random = Random.Range(0, 2);
        
        if (random == 0)
            PlayScream();
        else
            PlayAttack();
    }
    
    public void PlayScream()
    {
        // Pakai integer state atau trigger (pilih salah satu)
        // animator.SetInteger("AnimState", 1); // Jika pakai Int parameter
        animator.SetTrigger("Scream"); // Jika pakai Trigger parameter
        
        if (audioSource != null && screamSound != null)
            audioSource.PlayOneShot(screamSound);
    }
    
    public void PlayAttack()
    {
        // animator.SetInteger("AnimState", 2); // Jika pakai Int parameter
        animator.SetTrigger("Attack"); // Jika pakai Trigger parameter
        
        if (audioSource != null && attackSound != null)
            audioSource.PlayOneShot(attackSound);
    }
    
    public void PlayIdle()
    {
        // Reset ke idle
        // animator.SetInteger("AnimState", 0); // Jika pakai Int parameter
        animator.SetTrigger("Idle"); // Jika pakai Trigger parameter
    }
}