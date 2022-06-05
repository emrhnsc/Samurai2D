using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Animator camAnim;

    public void CamShakeRight()
    {
        camAnim.SetTrigger("ShakeR");
    }

    public void CamShakeLeft()
    {
        camAnim.SetTrigger("ShakeL");
    }
}
