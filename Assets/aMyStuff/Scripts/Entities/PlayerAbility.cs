using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerAbility : MonoBehaviour
{
    public Ability ability;
    [Range(0, 4)] public int abilitySlot; // 0 = Not assigned
    
    private KeyCode key;
    private Image abilityIcon;
    private Image cooldownIcon;
    private TextMeshProUGUI iconText;

    private float cooldownTimeRemaining;
    private float castTimeRemaining;

    // Indicator fields
    private Canvas indicatorCanvas;
    private Canvas rangeCanvas;
    private bool hasClicked = false;
    private bool waitingOnInput;
    private Vector3 position;
    private RaycastHit hit;
    private Ray ray;

    private Animator animator;
    private Movement movement;

    enum AbilityState
    {
        ready,
        active,
        cooldown
    }
    AbilityState state = AbilityState.ready;

    public void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        movement = gameObject.GetComponent<Movement>();

        // Set activation key depending on what ability slot is selected or if unassigned (0),
        // attempt to set key from ability property.
        // This allows us to interchangeably swap out abilities to different slots,
        // while also allowing abilities that are always active separate to the 4 slotted abilities (basic attack).
        switch (abilitySlot)
        {
            case 0:
                key = ability.Key;
                break;
            case 1:
                key = KeyCode.Alpha1;
                break;
            case 2:
                key = KeyCode.Alpha2;
                break;
            case 3:
                key = KeyCode.Alpha3;
                break;
            case 4:
                key = KeyCode.Alpha4;
                break;
        }

        // Link slotted ability to ability slot UI elements.
        if (abilitySlot > 0)
        {
            GameObject slot = GameObject.Find("Ability_" + abilitySlot);
            Image icon = slot?.transform.Find("Ability_" + abilitySlot + "_Icon")?.GetComponent<Image>();
            Image cooldown = slot?.transform.Find("Ability_" + abilitySlot + "_Icon_Cooldown")?.GetComponent<Image>();
            TextMeshProUGUI text = slot?.transform.Find("Ability_" + abilitySlot + "_Icon_Text")?.GetComponent<TextMeshProUGUI>();

            if (icon)
            {
                icon.enabled = true;
                abilityIcon = icon;
                abilityIcon.sprite = ability.sprite;
            }

            if (cooldown)
            {
                cooldown.enabled = true;
                cooldownIcon = cooldown;
                cooldownIcon.sprite = ability.sprite;
                cooldownIcon.fillAmount = 0;
            }

            if (text)
            {
                text.enabled = true;
                iconText = text;
                iconText.text = "";
            }

            // Set ability indicator canvas based on ability.Type.
            Canvas temp;
            switch (ability.type) {
                case Ability.AbilityType.Basic:
                    return;

                case Ability.AbilityType.Skillshot:
                    temp = transform.Find("Skillshot Indicator").GetComponent<Canvas>();
                    if (temp) indicatorCanvas = temp;
                    return;

                case Ability.AbilityType.Cone:
                    temp = transform.Find("Cone Indicator").GetComponent<Canvas>();
                    if (temp) indicatorCanvas = temp;
                    return;

                case Ability.AbilityType.AoE:
                    temp = transform.Find("AoE Indicator").GetComponent<Canvas>();
                    if (temp) indicatorCanvas = temp;

                    temp = transform.Find("Range Indicator").GetComponent<Canvas>();
                    if (temp) rangeCanvas = temp;

                    return;
            }
        }
    }

    public void Update()
    {
        switch (state)
        {
            // Waiting for input.
            case AbilityState.ready:
                if (Input.GetKeyDown(key))
                {
                    state = AbilityState.active;
                    castTimeRemaining = Mathf.Max(ability.castTime, 0.1f);
                }
                break;

            // Process casting time.
            case AbilityState.active:
                if (!hasClicked)
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    // Check if its an ability with a canvas.
                    if (indicatorCanvas)
                    {
                        indicatorCanvas.enabled = true;
                        processAbilityIndicator();

                        // Wait for user interaction, then begin cast.
                        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(key))
                        {
                            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) 
                            { 
                                if (movement && ability.stopsMovement)
                                {
                                    movement.StopMovement();
                                    movement.Rotation(position);
                                }
                                if (animator && ability.hasAnimation) animator.SetBool("isCasting", true);
                                if (indicatorCanvas) indicatorCanvas.enabled = false;
                                if (rangeCanvas) rangeCanvas.enabled = false;

                                ability.Activate(gameObject, hit);
                            }
                            hasClicked = true;
                        }
                    }
                    else
                    {
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) ability.Activate(gameObject, hit);
                        hasClicked = true;
                    }
                }

                // Begin activation, then trigger cooldown.
                else
                {
                    if (castTimeRemaining > 0) castTimeRemaining -= Time.deltaTime;
                    else
                    {
                        if (animator) animator.SetBool("isCasting", false);
                        hasClicked = false;
                        Cursor.visible = true;

                        //ability.Cooldown(gameObject); needed?
                        state = AbilityState.cooldown;
                        cooldownTimeRemaining = ability.cooldownTime;
                    }
                }
                break;

            // Process cooldown time.
            case AbilityState.cooldown:
                if (cooldownTimeRemaining > 0)
                {
                    cooldownTimeRemaining -= Time.deltaTime;
                    if (cooldownIcon != null)
                    {
                        cooldownIcon.fillAmount = cooldownTimeRemaining / ability.cooldownTime;
                    }
                    if (iconText != null)
                    {
                        iconText.text = Mathf.Ceil(cooldownTimeRemaining).ToString();
                    }
                }
                else
                {
                    state = AbilityState.ready;
                    if (cooldownIcon != null)
                    {
                        cooldownIcon.fillAmount = 0;
                    }
                    if (iconText != null)
                    {
                        iconText.text = "";
                    }
                }
                break;
        }
    }

    private void processAbilityIndicator()
    {
        switch (ability.type) {
            // Point indicator towards cursor.
            case Ability.AbilityType.Basic:
            case Ability.AbilityType.Skillshot:
            case Ability.AbilityType.Cone:
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    position = hit.point;
                    if (indicatorCanvas != null)
                    {
                        Quaternion rotation = Quaternion.LookRotation(position - transform.position);
                        rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
                        indicatorCanvas.transform.rotation = Quaternion.Lerp(rotation, indicatorCanvas.transform.rotation, 0);
                    }
                } 
                return;

            // Replace cursor with indicator, dynamically resize range indicator based on ability range.
            case Ability.AbilityType.AoE:
                if (rangeCanvas)
                {
                    rangeCanvas.GetComponentInChildren<Image>().rectTransform.sizeDelta = new Vector2(ability.range * 2, ability.range * 2);
                    rangeCanvas.enabled = true;
                }
                Cursor.visible = false;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    position = hit.point;
                    var hitPosDir = (hit.point - transform.position).normalized;
                    float distance = Vector3.Distance(hit.point, transform.position);
                    distance = Mathf.Min(distance, ability.range);

                    var newHitPos = transform.position + hitPosDir * distance;
                    indicatorCanvas.transform.position = newHitPos;
                }
                return;
        }
    }
}
