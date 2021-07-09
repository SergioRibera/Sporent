﻿using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {
    [SerializeField] bool follow;
    [Header("Follow Player")]
    [SerializeField] Vector3 followOffset;
    [SerializeField] [Range(0f, 1f)]
    float followSmoothSpeed;

    [Header("Visual Effect")]
    [SerializeField] Image mask;
    [SerializeField] RectTransform canvas;

    Transform target;
    Vector3 velocity = Vector3.zero;

    float screenHeight = 0, screenWidth = 0,
          radius = 0, objetiveRadius = 0;
    bool canAnimateVision = false;

    public static CameraController Main = null;

    void Awake() {
        if (Main == null)
            Main = this;
        else
            Destroy(gameObject);
    }

    void Start() {
        target = CreaturePlayable.Main.transform;
        GetCharacterPositionToMask();
    }

    void GetCharacterPositionToMask() {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
        float characterScreenWidth = 0,
              characterScreenHeight = 0;
        screenHeight = Screen.height;
        screenWidth = Screen.width;

        if (screenWidth < screenHeight) { // Portrait
            mask.rectTransform.sizeDelta = new Vector2(canvas.rect.height, canvas.rect.height);
            float newScreenPosX = screenPos.x + (screenHeight - screenWidth) / 2;
            characterScreenWidth = (newScreenPosX * 100) / screenHeight;
            characterScreenWidth /= 100;

            characterScreenHeight = (screenPos.y * 100) / screenHeight;
            characterScreenHeight /= 100;
        } else { // Landscape
            mask.rectTransform.sizeDelta = new Vector2(canvas.rect.width, canvas.rect.width);
            float newScreenPosY = screenPos.y + (screenWidth - screenHeight) / 2;
            characterScreenWidth = (screenPos.x * 100) / screenWidth;
            characterScreenWidth /= 100;

            characterScreenHeight = (newScreenPosY * 100) / screenWidth;
            characterScreenHeight /= 100;
        }

        mask.material.SetFloat("Center_X", characterScreenWidth);
        mask.material.SetFloat("Center_Y", characterScreenHeight);
    }

    public void ChangeVision(float radiusVision) => objetiveRadius = radiusVision;
    // TODO: On show game after show pause/part/adn menu
    public void CanAnimateVision(bool can) => canAnimateVision = can;
    
    void LateUpdate() {
        if (!follow) return;
        // Siguiendo al jugador
        Vector3 desiredPosition = target.position + followOffset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, followSmoothSpeed);
    }

    void Update() {
        // Efecto de Vision
        if (radius != 1)
            GetCharacterPositionToMask();
        if (!canAnimateVision) return;
        if (radius == objetiveRadius)
            canAnimateVision = false;
        else if (radius < objetiveRadius)
            radius += .01f;
        else
            radius -= .01f;
        mask.material.SetFloat("Radius", radius);
    }
}
