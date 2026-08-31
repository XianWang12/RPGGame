using System.Collections;
using UnityEngine;

public class AreaSound : MonoBehaviour
{
    [SerializeField] private int areaSoundIndex;
    [SerializeField] private AudioSource areaSound;

    private Coroutine currentCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>()!=null)
        {
            if (currentCoroutine != null)
            { 
                StopCoroutine(currentCoroutine);
                currentCoroutine=null;
            }
            areaSound.volume = 1;

            AudioManager.instance.PlayBGM(areaSoundIndex);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
            if(gameObject.activeInHierarchy&&areaSound!=null)
                currentCoroutine = StartCoroutine(DecreaseVolume(areaSound));
    }

    public IEnumerator DecreaseVolume(AudioSource _audio)
    {
        float defaultVolume = _audio.volume;
        while (_audio.volume > .05f)
        {
            _audio.volume -= _audio.volume * .2f;
            yield return new WaitForSeconds(.25f);

            if (_audio.volume <= .05f)
            {
                _audio.Stop();
                _audio.volume = defaultVolume;
                break;
            }
        }
    }
}
