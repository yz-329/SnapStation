using System.Collections;
using UnityEngine;

public class CameraSlide : MonoBehaviour
{
    public Transform[] cameraPositions;

    public float slideSpeed = 2f;

    private bool isMoving = false;

    public void MoveToStep(int stepIndex)
    {
        if (!isMoving)
        {
            StartCoroutine(SmoothMove(cameraPositions[stepIndex-1].position));
        }
    }

    IEnumerator SmoothMove(Vector3 targetPos)
    {
        isMoving = true;

        Vector3 startPos = transform.position;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * slideSpeed;

            float smoothT = Mathf.SmoothStep(0, 1, t);

            transform.position = Vector3.Lerp(startPos, targetPos, smoothT);

            yield return null;
        }

        transform.position = targetPos;

        isMoving = false;
    }
}
