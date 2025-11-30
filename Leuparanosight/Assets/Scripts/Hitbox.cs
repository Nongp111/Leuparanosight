using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyPart { Body, Head, Limb }

public class Hitbox : MonoBehaviour
{
    public BodyPart bodyPart = BodyPart.Body;

    // ถ้าอยากให้หา Enemy โดยอัตโนมัติ (จะค้นหา parent)
    [HideInInspector] public Enemy parentEnemy;

    void Awake()
    {
        if (parentEnemy == null)
            parentEnemy = GetComponentInParent<Enemy>();
    }
}

