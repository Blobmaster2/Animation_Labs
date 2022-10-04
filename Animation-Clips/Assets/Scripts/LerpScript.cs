using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LerpScript : MonoBehaviour
{
    public Slider slider;
    Material mat;
    float t;

    int currentPoint = 0;
    int nextPoint = 1;

    public List<Transform> points;

    public float speed;

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

        t += Time.smoothDeltaTime / speed;

        transform.position = Vector3.Lerp(points[currentPoint].position, points[nextPoint].position, t);

        if (t > 1)
        {
            t = 0;

            nextPoint++;
            currentPoint++;

            if (nextPoint > points.Count - 1)
            {
                nextPoint = 0;
            }
            if (currentPoint > points.Count - 1)
            {
                currentPoint = 0;
            }
        }

        transform.Rotate(Vector3.one / 100);
    }
}
