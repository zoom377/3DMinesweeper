using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorLerper : MonoBehaviour
{
    [SerializeField] Renderer[] _affectedRenderers;
    [SerializeField] float _lerpSpeed;
    [SerializeField] Color _defaultColor;

    Color _targetColor;

    void Start()
    {
        _targetColor = _defaultColor;
    }

    void Update()
    {
        foreach (var renderer in _affectedRenderers)
        {
            if (renderer.material.HasProperty("_FaceColor"))
            {
                //TMP_Text special behaviour as shader does not have standard Unity color/tint property
                var curColor = renderer.material.GetColor("_FaceColor");
                var newColor = Color.Lerp(curColor, _targetColor, _lerpSpeed * Time.deltaTime);
                renderer.material.SetColor("_FaceColor", newColor);
                renderer.material.SetColor("_OutlineColor", newColor);
            }
            else
            {
                renderer.material.color = Color.Lerp(renderer.material.color, _targetColor, _lerpSpeed * Time.deltaTime);
            }
        }
    }

    public void SetColor(Color color)
    {
        _targetColor = color;
    }

    public void SetColorToDefault()
    {
        _targetColor = _defaultColor;
    }
}
