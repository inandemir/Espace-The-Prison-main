using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    #region ses tan�mlamalar�
    [SerializeField] AudioSource Sound;
    [SerializeField] AudioSource Song;
    [SerializeField] Slider SoundSlider;
    [SerializeField] Slider SongSlider;
    [SerializeField] Image soundImage;
    [SerializeField] Image songImage;
    [SerializeField] Sprite[] images;
    [SerializeField] AudioClip[] Clip;

    public float soundVolume = 1f;
    public float songVolume = 1f;

    private int index;

    #endregion


    private void Start()
    {
        SongLoad();
        SoundLoad();
    }

    public void WinSound()
    {
        Sound.clip = Clip[0];
        Sound.Play();
    }
    public void LoseSound()
    {
        Sound.clip = Clip[1];
        Sound.Play();
    }
    public void BattleSound()
    {
        index++;
        if (index % 4 == 0) // E�er battleSound metotu 4 kere �a��r�l�rsa i�ine girecek bunu yapmam�n amac� ��p adamlar k�sa s�rede �ok fazla kez �ld�kleri i�in
                            // ses �ok oynat�l�yor ve k�t� bir ses ortaya ��k�yor. silip deneyebilirsiniz tabi bazen az adam kald��� s�rada adam �ld���nde haliyle
                            // ses ��kmayabiliyor ve oyuncu da bunu g�rebiliyor bazen e�er sa�ma derseniz bu kontrol� silebilirsiniz.
        {
            Sound.clip = Clip[2];
            Sound.Play();
        }

    }
    public void GateSound()
    {
        Sound.clip = Clip[3];
        Sound.Play();
    }

    public void BossAttackSound()
    {
        Sound.clip = Clip[4];
        Sound.Play();
    }

    public void SetSoundVolume()
    {
        soundVolume = SoundSlider.value;
        Sound.volume = soundVolume;

        if (Sound.volume == 0)
        {
            soundImage.sprite = images[2];
        }
        else if (Sound.volume > 0 && Sound.volume < 0.25f)
        {
            soundImage.sprite = images[3];
        }
        else if (Sound.volume >= 0.25f && Sound.volume < 0.75f)
        {
            soundImage.sprite = images[4];
        }
        else if (Sound.volume >= 0.75f && Sound.volume <= 1)
        {
            soundImage.sprite = images[5];
        }
    }

    public void SetSongVolume()             //kullan�lm�yor?
    {
        songVolume = SongSlider.value;
        Song.volume = songVolume;
        if (Song.volume == 0)
            songImage.sprite = images[0];
        else
            songImage.sprite = images[1];
    }

    public void SaveSong(float volume)
    {
        PlayerPrefs.SetFloat("songVolume", volume);
    }

    public void SaveSound(float volume)
    {
        PlayerPrefs.SetFloat("soundVolume", volume);
    }

    private void SongLoad()
    {
        if (PlayerPrefs.HasKey("songVolume"))
        {
            float song = PlayerPrefs.GetFloat("songVolume");
            SongSlider.value = song;
            songVolume = song;
            Song.volume = song;
        }
    }

    private void SoundLoad()
    {
        if (PlayerPrefs.HasKey("soundVolume"))
        {
            float sound = PlayerPrefs.GetFloat("soundVolume");
            SoundSlider.value = sound;
            soundVolume = sound;
            Sound.volume = sound;
        }
    }
}
