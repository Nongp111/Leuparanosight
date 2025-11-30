using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MedkitUI : MonoBehaviour
{
    public Player player;       // อ้างอิง Player
    public Text medkitText;     // UI Text ที่จะแสดงจำนวนยา

    void Update()
    {
        if (player != null && medkitText != null)
        {
            medkitText.text = "Medkits: " + player.medkitCount;
        }
    }
}
