using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    TMP_InputField widthInput;
    TMP_InputField heightInput;
    TMP_InputField depthInput;
    TMP_InputField percentageInput;

    void Start()
    {
        widthInput = GameObject.Find("WidthInput").GetComponent<TMP_InputField>();
        heightInput = GameObject.Find("HeightInput").GetComponent<TMP_InputField>();
        depthInput = GameObject.Find("DepthInput").GetComponent<TMP_InputField>();
        percentageInput = GameObject.Find("PercentageInput").GetComponent<TMP_InputField>();

        widthInput.text = PlayerPrefs.GetInt("Width", 10).ToString();
        heightInput.text = PlayerPrefs.GetInt("Height", 10).ToString();
        depthInput.text = PlayerPrefs.GetInt("Depth", 10).ToString();
        percentageInput.text = PlayerPrefs.GetInt("Percentage", 10).ToString();
    }

    void Update()
    {

    }

    public void OnPlayClicked()
    {
        int width, height, depth, percentage;

        if (int.TryParse(widthInput.text, out width) &&
            int.TryParse(heightInput.text, out height) &&
            int.TryParse(depthInput.text, out depth) &&
            int.TryParse(percentageInput.text, out percentage))
        {
            var state = FindObjectOfType<State>();
            state.Width = width;
            state.Height = height;
            state.Depth = depth;
            state.Percentage = percentage;

            PlayerPrefs.SetInt("Width", width);
            PlayerPrefs.SetInt("Height", height);
            PlayerPrefs.SetInt("Depth", depth);
            PlayerPrefs.SetInt("Percentage", percentage);

            SceneManager.LoadScene("Play");
        }
    }
}
