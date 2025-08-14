using UnityEngine;

public class LightSideBehaviour : SidesBehaviour
{
    private enum State
    {
        Normal,
        Focused
    }

    [SerializeField] private LightSideMovement movement;

    [SerializeField] private LightSource circleLight;
    [SerializeField] private float baseCircleLightRange;
    [SerializeField] private float focusedCircleLightRange;

    [SerializeField] private LightSource distantLight;
    [SerializeField] private float focusedDistantLightRange;
    [SerializeField] private float lightFollowSpeed;

    private State state;
    private Transform distantLightTransform;
    
    private void Start()
    {
        distantLightTransform = distantLight.gameObject.GetComponent<Transform>();
        SetState(State.Normal);
    }
    private void Update()
    {
        switch (state)
        {
            case State.Normal:
                NormalBehaviour();
                break;
            case State.Focused:
                FocusedBehaviour();
                break;
        }
    }
    private void SetState(State newState)
    {
        state = newState;
        switch (state)
        {
            case State.Normal:
                SetNormalSettings();
                break;
            case State.Focused:
                SetFocusedSettings();
                break;
        }
    }
    private void SetNormalSettings()
    {
        circleLight.range = baseCircleLightRange;
        distantLight.gameObject.SetActive(false);
        movement.moveSpeed = movement.baseMovSpeed;
    }
    private void SetFocusedSettings()
    {
        circleLight.range = focusedCircleLightRange;
        distantLight.gameObject.SetActive(true);
        distantLight.range = focusedDistantLightRange;
        distantLightTransform.rotation = CalculateRotationAngle();
        movement.moveSpeed = movement.focusedMovSpeed;
    }
    private void NormalBehaviour()
    {
        if (InputManager.leftMouseBtnDown)
        {
            SetState(State.Focused);
        }
    }
    private void FocusedBehaviour()
    {
        if (InputManager.leftMouseBtnUp)
        {
            SetState(State.Normal);
        }
        distantLightTransform.rotation = Quaternion.Lerp(distantLightTransform.rotation, CalculateRotationAngle(), Time.deltaTime * lightFollowSpeed);
    }
    private Quaternion CalculateRotationAngle()
    {
        Vector3 vectorToTarget = MouseTracker.mousePosition - distantLightTransform.position;
        return Quaternion.Euler(0, 0, Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg);
    }
}
