using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class StaminaDisplay : MonoBehaviour
{
    [SerializeField] private PlayerControl player;
    [SerializeField] private Image staminaImage;
    [SerializeField] private float warningThreshold = .25f;
    [SerializeField] private Color warningColor = Color.red;
    [SerializeField] private float rechargeFlashRate = 5f;
    [SerializeField] private float visibleThreshold;

    private void Start()
    {
        // todo quick fix
        var constraintSource = new ConstraintSource
        {
            sourceTransform = GameObject.Find("Ground").transform, weight = 1f
        };
        staminaImage.GetComponent<RotationConstraint>().AddSource(constraintSource);
    }

    private void Update()
    {
        if (Math.Abs(player.Stamina - player.MaxStamina) < 0.001f)
        {
            staminaImage.enabled = false;
            return;
        }
        staminaImage.enabled = true;

        var stamNorm = player.Stamina / player.MaxStamina;
        staminaImage.fillAmount = stamNorm;

        if (player.LostAllStamina)
        {
            var col = staminaImage.color;
            col.a = (Mathf.Cos(Time.time * rechargeFlashRate) + 1f) / 2f;
            staminaImage.color = col;
            return;
        }

        if (stamNorm >= visibleThreshold)
        {
            var col = Color.white;
            col.a = Mathf.Lerp(0f, 1f, (1f - stamNorm) / (1f - visibleThreshold));
            staminaImage.color = col;
            return;
        }

        if (stamNorm <= warningThreshold)
        {
            staminaImage.color = Color.Lerp(warningColor, Color.white, stamNorm / warningThreshold);
        }
        else
        {
            staminaImage.color = Color.white;
        }
    }
}
