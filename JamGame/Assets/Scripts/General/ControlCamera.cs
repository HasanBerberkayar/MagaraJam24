using UnityEngine;
using Unity.Cinemachine;

public class ControlCamera : MonoBehaviour
{
    public Transform player;
    public CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
        virtualCamera.Follow = player;
        this.transform.SetParent(null); // Kameray� parent'tan ay�r.
    }

    void Update()
    {
        // Oyuncunun Y eksenindeki rotasyonu al�n.
        float playerRotationY = player.eulerAngles.y;

        // E�er oyuncu d�z bak�yorsa (yakla��k 0 derece)
        if (Mathf.Abs(playerRotationY) < 1f || Mathf.Abs(playerRotationY - 360f) < 1f)
        {
            if (virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX > 0.4f)
            {
                virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX -= 0.001f * Time.deltaTime * 100; // Daha h�zl� hareket
            }
            else
            {
                virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = 0.4f;
            }
        }
        else
        {
            // E�er oyuncu sola veya sa�a d�nd�yse
            if (virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX < 0.6f)
            {
                virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX += 0.001f * Time.deltaTime * 100; // Daha h�zl� hareket
            }
            else
            {
                virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = 0.6f;
            }
        }
    }
}