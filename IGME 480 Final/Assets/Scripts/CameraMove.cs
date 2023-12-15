using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float sensitivity = 1;
    public Transform cam;

    private Vector3 oldMousePos;

    private Vector3 euler;

    void LateUpdate()
    {
        if (DeckEditor.inst != null && DeckEditor.inst.gameObject.activeInHierarchy)
            return;

        if(Input.GetMouseButton(0))
        {
            Vector3 mouseDelta = Input.mousePosition - oldMousePos;

            euler.x += -mouseDelta.y * sensitivity;
            euler.y += mouseDelta.x * sensitivity;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            euler = Vector3.zero;
            cam.localPosition = new Vector3(0, 0, -16);
        }

        transform.rotation = Quaternion.Euler(euler);
        oldMousePos = Input.mousePosition;

        if(Input.mouseScrollDelta.y != 0)
        {
            Vector3 camOff = cam.localPosition;
            camOff.z += Input.mouseScrollDelta.y;
            camOff.z = Mathf.Clamp(camOff.z, -32, -16);
            cam.localPosition = camOff;
        }
    }
}
