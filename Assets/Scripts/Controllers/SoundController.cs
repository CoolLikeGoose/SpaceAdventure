using UnityEngine;

/// <summary>
/// Manages all changes in the sounds
/// </summary>
public class SoundController : MonoBehaviour
{
    
    public static SoundController Instance { get; private set; }

    //sources
    [SerializeField] private AudioSource fXsource = null;
    [SerializeField] private AudioSource musicSource = null;

    [SerializeField] private AudioClip enemyExplosion = null;
    [SerializeField] private AudioClip playerExplosion = null;
    [SerializeField] private AudioClip asteroidHit = null;

    [SerializeField] private AudioClip playerLaser = null;
    [SerializeField] private AudioClip enemyLaser = null;

    [SerializeField] private AudioClip coinSound = null;
    [SerializeField] private AudioClip gunSound = null;
    [SerializeField] private AudioClip bonusSound = null;

    /// <summary>
    /// Property for mute music
    /// </summary>
    public bool musicMute
    {
        get { return musicSource.mute; }
        set { musicSource.mute = value; }
    }
    /// <summary>
    /// Property for mute sound FX
    /// </summary>
    public bool soundFXMute
    {
        get { return fXsource.mute; }
        set { fXsource.mute = value; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Methods for start sound FX
    public void EnemyExplosion()
    {
        fXsource.pitch = Random.Range(0.9f, 1.1f);
        fXsource.PlayOneShot(enemyExplosion);
    }

    public void PlayerExplosion()
    {
        fXsource.PlayOneShot(playerExplosion);
    }

    public void AsteroidHit()
    {
        fXsource.PlayOneShot(asteroidHit);
    }

    public void CoinCollected()
    {
        fXsource.PlayOneShot(coinSound);
    }

    public void GearUp()
    {
        fXsource.PlayOneShot(gunSound);
    }

    /// <summary>
    /// Sound for shot
    /// </summary>
    /// <param name="index">0 for player; 1 for enemy</param>
    public void LaserShot(int index)
    {
        if (index == 0) { fXsource.PlayOneShot(playerLaser); }
        else { fXsource.PlayOneShot(enemyLaser); }
    }
}
