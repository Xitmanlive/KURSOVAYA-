using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Переключает управление игроком между VR-режимом и PC-режимом при старте сцены.
///
/// Вешать на тот же GameObject, что и:
///  - Character Controller
///  - Continuous Move Provider (Action-based)
///  - PCPlayerMovement (ваш скрипт WASD + мышь)
/// т.е. на XR Origin.
///
/// Логика:
///  - Если обнаружен реальный работающий VR-дисплей (шлем) -> включаем VR-локомоцию,
///    выключаем PC-скрипт.
///  - Если шлема нет (например, запуск в редакторе/на ПК без VR) -> выключаем
///    VR-локомоцию, включаем PC-скрипт и блокируем курсор мыши.
/// </summary>
[DisallowMultipleComponent]
public class GameplayInputSwitcher : MonoBehaviour
{
    [Header("Ссылки на компоненты")]
    [Tooltip("Если оставить пустым, компонент будет найден автоматически через GetComponent на этом же объекте.")]
    [SerializeField] private ActionBasedContinuousMoveProvider continuousMoveProvider;

    [Tooltip("Если оставить пустым, компонент будет найден автоматически через GetComponent на этом же объекте.")]
    [SerializeField] private PCPlayerMovement pcPlayerMovement;

    // Переиспользуемый список, чтобы не создавать мусор на каждый вызов.
    private static readonly List<XRDisplaySubsystem> s_DisplaySubsystems = new List<XRDisplaySubsystem>();

    private void Awake()
    {
        // Автопоиск компонентов, если не назначены вручную в инспекторе.
        if (continuousMoveProvider == null)
            continuousMoveProvider = GetComponent<ActionBasedContinuousMoveProvider>();

        if (pcPlayerMovement == null)
            pcPlayerMovement = GetComponent<PCPlayerMovement>();

        if (IsVRHeadsetActive())
        {
            EnableVRMode();
        }
        else
        {
            EnablePCMode();
        }
    }

    /// <summary>
    /// Проверяет, подключён ли реальный VR-шлем и активен ли его XR-дисплей.
    /// Используем современный Subsystem API вместо устаревшего XRDevice.isPresent,
    /// так как он корректно работает с XR Plugin Management (OpenXR, Oculus и т.д.).
    /// </summary>
    private bool IsVRHeadsetActive()
    {
        SubsystemManager.GetSubsystems(s_DisplaySubsystems);

        foreach (XRDisplaySubsystem display in s_DisplaySubsystems)
        {
            if (display.running)
                return true;
        }

        return false;
    }

    private void EnableVRMode()
    {
        Debug.Log("[GameplayInputSwitcher] VR-шлем обнаружен. Активирую VR-управление.");

        if (continuousMoveProvider != null)
            continuousMoveProvider.enabled = true;

        if (pcPlayerMovement != null)
            pcPlayerMovement.enabled = false;

        // Курсор в VR не нужен - на всякий случай снимаем блокировку,
        // если она осталась с прошлой сессии/сцены.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void EnablePCMode()
    {
        Debug.Log("[GameplayInputSwitcher] VR-шлем не найден. Активирую PC-управление.");

        if (continuousMoveProvider != null)
            continuousMoveProvider.enabled = false;

        if (pcPlayerMovement != null)
            pcPlayerMovement.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
