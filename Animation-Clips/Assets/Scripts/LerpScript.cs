using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LerpScript : MonoBehaviour
{
    public Slider slider;
    Material mat;
    float t;

    public List<Transform> points;

    public float speed;
    bool descendTime = false;

    Color lerpColour;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }
    private void Update()
    {
        lerpColour = Color.Lerp(Color.white, Color.red, slider.value);
        mat.color = lerpColour;

        t += !descendTime ? Time.smoothDeltaTime / speed : -Time.smoothDeltaTime / speed;

        if (t > 1.0f) descendTime = true;
        if (t < 0.0f) descendTime = false;

        transform.position = BezierFunc();
    }

    Vector3 BezierFunc()
    {
        Vector3 a = Vector3.Lerp(points[0].position, points[1].position, t);
        Vector3 b = Vector3.Lerp(points[1].position, points[2].position, t);
        Vector3 c = Vector3.Lerp(points[2].position, points[3].position, t);

        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);

        Vector3 returnVec = Vector3.Lerp(ab, bc, t);
        return returnVec;
    }
}
