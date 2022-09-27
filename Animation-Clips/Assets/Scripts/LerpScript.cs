using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LerpScript : MonoBehaviour
{
    public Slider slider;
    public Material mat;

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
    }
}
