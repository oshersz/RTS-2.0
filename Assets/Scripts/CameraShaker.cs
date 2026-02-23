using UnityEngine;

public enum Shake
{
    Weak,
    Medium,
    Strong,
    Short,
    Moderate,
    Long
}

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker singleton { private set; get; }

    public float shakeValue;
    private float shakeReducer;
    public bool shaking;
    public Vector3 shakePos;

    public Vector3 originalPos;

    private void Awake()
    {
        singleton = this;
        originalPos = transform.localPosition;
    }
    private void Update()
    {
        if (shaking)
        {
            shakePos = new Vector3(Random.Range(-shakeValue, shakeValue), 0, Random.Range(-shakeValue, shakeValue));
            transform.localPosition = originalPos + shakePos;
            shakeValue *= (1 - (Time.deltaTime * shakeReducer));
            if (shakeValue<=0.05f)
            {
                shaking = false;
                transform.localPosition = originalPos;
            }
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            ShakeCamera(Shake.Weak);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            ShakeCamera(Shake.Medium);
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            ShakeCamera(Shake.Strong);
        }
    }

    public void ShakeCamera(Shake shakeAmount)
    {
        shaking = true;
        shakeReducer = 7f;
        if (shakeAmount == Shake.Weak)
        {
            /*
            shakeValue = 0.15f;
            shakeReducer = 7f;
            */
            shakeValue = 0.1f;
        }
        else if (shakeAmount == Shake.Medium)
        {
            shakeValue = 0.25f; //0.45f
        }
        else if (shakeAmount == Shake.Strong)
        {
            shakeValue = 0.5f; //0.6f
        }



    }

    public void ShakeCamera(Shake shakeAmount, Shake shakeDuration)
    {

    }
}
