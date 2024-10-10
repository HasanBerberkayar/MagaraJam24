using UnityEngine;
using Unity.Cinemachine;

public class ControlCamera : MonoBehaviour
{
    public Transform player;
    public CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
        virtualCamera.Follow = player;
        this.transform.SetParent(null); // Kamerayý parent'tan ayýr.
    }

    void Update()
    {
        // Oyuncunun Y eksenindeki rotasyonu alýn.
        float playerRotationY = player.eulerAngles.y;

        // Eðer oyuncu düz bakýyorsa (yaklaþýk 0 derece)
        if (Mathf.Abs(playerRotationY) < 1f || Mathf.Abs(playerRotationY - 360f) < 1f)
        {
            if (virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX > 0.4f)
            {
                virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX -= 0.001f * Time.deltaTime * 100; // Daha hýzlý hareket
            }
            else
            {
                virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = 0.4f;
            }
        }
        else
        {
            // Eðer oyuncu sola veya saða döndüyse
            if (virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX < 0.6f)
            {
                virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX += 0.001f * Time.deltaTime * 100; // Daha hýzlý hareket
            }
            else
            {
                virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = 0.6f;
            }
        }
    }
}