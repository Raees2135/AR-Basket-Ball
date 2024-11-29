using UnityEngine;
using UnityEngine.UI;

public class ForceControl : MonoBehaviour
{
    public Slider forceSlider;
    public BasketballShooter basketballShooter;

    void Start()
    {
        // Set the min and max values for the slider
        forceSlider.minValue = 10;
        forceSlider.maxValue = 100;

        // Optionally, set the initial value
        forceSlider.value = 10;

        // Add a listener to update the force when the slider value changes
        forceSlider.onValueChanged.AddListener(UpdateShootForce);
    }

    void UpdateShootForce(float value)
    {
        // Update the BasketballShooter script's baseShootForce
        basketballShooter.baseShootForce = value;
    }
}
