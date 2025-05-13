using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;

using UnityEngine.UI; //add this whenever you're writing a script for a UI element

public class BrushStrokesSlider: MonoBehaviour
{
    public Slider SolidSlider;
    public Slider WavySlider;
    public Slider StripeSlider;
    public Slider DotSlider;
    public Slider SwirlSlider;

    public Slider StampSlider;


    public TextMeshProUGUI SolidSliderText;
    public TextMeshProUGUI WavySliderText;
    public TextMeshProUGUI StripeSliderText;
    public TextMeshProUGUI DotSliderText;
    public TextMeshProUGUI SwirlSliderText;

    public TextMeshProUGUI StampSliderText;


    public static int SolidSliderValue; //*static ensure that the var is global and can be accessed in another script
    public static int WavySliderValue;
    public static int StripeSliderValue;
    public static int DotSliderValue;
    public static int SwirlSliderValue;

    public static int StampSliderValue;



    void Start()
    {
  
        
    }

    // Update is called once per frame
    void Update()
    {
        SolidSliderText.text = SolidSlider.value.ToString("0");
        SolidSliderValue = (int) SolidSlider.value; //convert slider float value to int 

        WavySliderText.text = WavySlider.value.ToString("0");
        WavySliderValue = (int)WavySlider.value; //convert slider float value to int 

        StripeSliderText.text = StripeSlider.value.ToString("0");
        StripeSliderValue = (int)StripeSlider.value; //convert slider float value to int 

        DotSliderText.text = DotSlider.value.ToString("0");
        DotSliderValue = (int)DotSlider.value; //convert slider float value to int

        SwirlSliderText.text = SwirlSlider.value.ToString("0");
        SwirlSliderValue = (int)SwirlSlider.value; //convert slider float value to int

        StampSliderText.text = StampSlider.value.ToString("0");
        StampSliderValue = (int)StampSlider.value; //convert slider float value to int
    }
}
