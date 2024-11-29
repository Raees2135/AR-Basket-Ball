using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HoopScore : MonoBehaviour
{
    public BasketballShooter basketballShooter;
    public AudioSource winSoundFx;
    public GameObject scoreImage;
    public Material plane;

    private void Start()
    {
        basketballShooter = FindObjectOfType<BasketballShooter>();
        scoreImage.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Basketball"))
        {
            winSoundFx.Play();
            basketballShooter.IncreaseScore();
            Destroy(other.gameObject, 1f); // Destroy the ball after scoring
            StartCoroutine(ShowScoreImage()); // Show the animated image
        }
    }

    IEnumerator ShowScoreImage()
    {
        scoreImage.SetActive(true); // Activate the image
        plane.color = Color.green;
        yield return new WaitForSeconds(1f); // Wait for 1 second
        scoreImage.SetActive(false); // Deactivate the image
        plane.color = Color.white;
    }
}
