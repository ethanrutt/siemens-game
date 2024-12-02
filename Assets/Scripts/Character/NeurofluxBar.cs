using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Rishi Santhanam - Neuroflux bar display for the casino. This handles
 * setting the neuroflux bar at the top of the screen in the casino whenever
 * the neuroflux value changes. Neuroflux value will change whenever a player
 * buys more neuroflux or whenever the bet in the horse game
 */
public class NeurofluxBar : MonoBehaviour
{
    // Grab slider
    // slider is itself (the object the script is attached too)
    private UnityEngine.UI.Slider slider => GetComponent<UnityEngine.UI.Slider>();

    // Grab playerData
    private PlayerData playerData => PlayerData.Instance;

    // Start is called before the first frame update
    void Awake()
    {
        // Set the slider value to the current neuroflux
        slider.value = playerData.neuroflux_meter;
    }

    public void SetNeuroflux(float value)
    {
        // Set the slider value to the current neuroflux
        slider.value = value;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the slider value to the current neuroflux
        if (slider.value != playerData.neuroflux_meter)
        {
            slider.value = playerData.neuroflux_meter;
        }
    }
}
