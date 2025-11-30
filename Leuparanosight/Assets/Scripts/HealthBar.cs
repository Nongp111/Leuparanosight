using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Player player;

    void Start()
    {
        slider.maxValue = player.maxHealth;
        slider.value = player.health;
    }

    void Update()
    {
        slider.value = player.health;
    }
}
