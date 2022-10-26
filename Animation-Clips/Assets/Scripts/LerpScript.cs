using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LerpScript : MonoBehaviour
{
    public Slider slider;
    Material mat;
    float t;
    int pointsListPos = 0;
    Vector3 lookAheadVec;
    float distanceToNext;
    float move;

    public List<Transform> points;

    public float speed;

    Color lerpColour;

    // Start is called before the first frame update
    void Start()
    {
        lookAheadVec = points[1].position;
        distanceToNext = Vector3.Distance(points[pointsListPos].position, lookAheadVec);
        mat = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        lerpColour = Color.Lerp(Color.white, Color.red, slider.value);
        mat.color = lerpColour;

        t += Time.smoothDeltaTime * speed;

        move = t / distanceToNext;


        if (t > distanceToNext)
        {
            t = 0.0f;
            pointsListPos++;

            if (pointsListPos > points.Count - 1)
            {
                pointsListPos = 0;
            }

            if (pointsListPos + 1 > points.Count - 1)
            {
                lookAheadVec = points[0].position;
            }
            else
            {
                lookAheadVec = points[pointsListPos + 1].position;
            }
            distanceToNext = Vector3.Distance(points[pointsListPos].position, lookAheadVec);

        }

        Debug.Log(move);

        transform.position = CatmullRomFunc(move, points[ClampListPos(pointsListPos - 1)].position, points[pointsListPos].position, points[ClampListPos(pointsListPos + 1)].position, points[ClampListPos(pointsListPos + 2)].position);
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

    Vector3 CatmullRomFunc(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;
        return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
    }

    int ClampListPos(int pos)
    {
        if (pos < 0)
        {
            pos = points.Count - 1;
        }
        if (pos > points.Count)
        {
            pos = 1;
        }
        else if (pos > points.Count - 1)
        {
            pos = 0;
        }

        return pos;
    }
}
