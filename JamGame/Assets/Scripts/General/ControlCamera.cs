using UnityEngine;
using Unity.Cinemachine;

public class ControlCamera : MonoBehaviour
{
    public Transform player;
    public CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
        virtualCamera.Follow = player;
        this.transform.SetParent(null); 
    }

    void Update()
    {
        float playerRotationY = player.eulerAngles.y;

        if (Mathf.Abs(playerRotationY) < 1f || Mathf.Abs(playerRotationY - 360f) < 1f)
        {
            if (virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX > 0.4f)
            {
                virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX -= 0.001f;
            }
            else
            {
                virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = 0.4f;
            }
        }
        else
        {
            if (virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX < 0.6f)
            {
                virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX += 0.001f;
            }
            else
            {
                virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = 0.6f;
            }
        }
    }
}