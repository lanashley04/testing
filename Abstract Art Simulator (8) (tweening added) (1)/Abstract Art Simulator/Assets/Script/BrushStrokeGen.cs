using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.VisualScripting;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;
using static Unity.Burst.Intrinsics.X86.Avx;
using TMPro;
using System.Linq;
using JetBrains.Annotations;

public class BrushStrokeGen : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject solidStroke;
    public GameObject wavyStroke;
    public GameObject stripeStroke;
    public GameObject dotStroke;
    public GameObject swirlStroke;

    public List<GameObject> specialStampList; //a list of special stamps 
    public BoxCollider2D CanvaBound; //boundary of the canva


    public Button GenerateButton;
    public Button EraseButton;
 
    public List<GameObject> brushStrokes = new List<GameObject>(); //store all your spawned brushstrokes here
    public List<int> strokesSliderValues = new List<int>(); //store all your int slider values here 

   
    public AudioSource PaintingSound;
    public AudioSource EraseSound;

    public TextMeshProUGUI AssessmentText; 

    public int coherentCheck = 0; //how many of brushtrokes types have similar values?
    public int creativeCheck = 0; // how many brushstrokes types have different values?


    void Start()
    {

        Button generateButton = GenerateButton.GetComponent<Button>(); //reference to the button that you want to use
        generateButton.onClick.AddListener(OnGenClick); //the code that will run when the player pressed that button

        Button eraseButton = EraseButton.GetComponent<Button>(); //reference to the button that you want to use
        eraseButton.onClick.AddListener(OnEraseClick); //the code that will run when the player pressed that button


    }
    void OnGenClick()
    {
        PaintingSound.Play();
        StartCoroutine(GenSolidStrokes()); //generate brushstrokes upon pressing the button
        StartCoroutine(GenWavyStrokes());
        StartCoroutine(GenStripeStrokes());
        StartCoroutine(GenDotStroke());
        StartCoroutine(GenSwirlStroke());
        StartCoroutine(GenStamps());

        StartCoroutine(Assessment()); //generate assessment comment based on the spawned brushstrokes on the canva 
        GenerateButton.interactable = false; // once you generate a painting, you have to reset the canva before you can generate again. 


    }

    void OnEraseClick()
    {
        coherentCheck = 0;
        foreach (GameObject Strokes in brushStrokes) //// destroy all brushstroke on screen (the ones stored in the list)
        {
            Destroy(Strokes);
        }
        brushStrokes.Clear();
        GenerateButton.interactable = true;

        EraseSound.Play(); //**the audio code for erase has to be put here to prevent any bug for some reasons

        AssessmentText.text = ""; //clear any prexisting text in the assessment box
        strokesSliderValues.Clear(); //reset every value store in the StrokesSliderValuelist (to take in new values for new assessment comparisons)
        coherentCheck = 0; //reset coherent and creative points
        creativeCheck = 0;

    }

    IEnumerator Assessment()
    {

        strokesSliderValues.Add(BrushStrokesSlider.SolidSliderValue); //add the brushstrokes values to a seperate StrokesSliderValue list 
        strokesSliderValues.Add(BrushStrokesSlider.WavySliderValue);
        strokesSliderValues.Add(BrushStrokesSlider.StripeSliderValue);
        strokesSliderValues.Add(BrushStrokesSlider.DotSliderValue);
        strokesSliderValues.Add(BrushStrokesSlider.SwirlSliderValue);


        var highestValue = Mathf.Max(strokesSliderValues.ToArray()); //find the highest values in the StrokesSliderValue list

        if (highestValue != 0)
        {
            strokesSliderValues.Remove(highestValue); //if the highest value isn't 0, remove any brush strokes that has the highest value in the list to prevent same number comparison

        }

        else // if it is 0, that means there are no brushstrokes values to compare 
        {
            AssessmentText.text = "You don't have anything on your canva yet.";
        }

        for (int i = 0; i < strokesSliderValues.Count; i++) // run through each value in the strokeSliderValue list
        {
           
            if (strokesSliderValues[i] < 10 && highestValue != 0) // if any of the brushstrokes values is less than 10, while the highest value is not equal to 0, write this
            {
            
                AssessmentText.text = "Please add more brush types for a more comprehensive assessment.";
            }
           
            if (strokesSliderValues[i] >= 10 && strokesSliderValues[i] >= Mathf.Abs(highestValue/2) && BrushStrokesSlider.StampSliderValue < 15) 
                 // if any of the brushstrokes values are greater than 10 and close to the highest value (greater than its half) + stampValues are less than 15, add one point to the CreativeCheck
            {
                creativeCheck++;

             }
        
             if (strokesSliderValues[i] >= 10 && BrushStrokesSlider.StampSliderValue < 15 && strokesSliderValues[i] < Mathf.Abs(highestValue/2))
             {
                //f any of the brushstrokes values are greater than 10 and far from the highest value (lower than its half) + stampValues are less than 15, add one point to the CoherenceCheck
                coherentCheck++;
            }


            if (strokesSliderValues[i] >= 10 && BrushStrokesSlider.StampSliderValue >= 15) //Special case: if more than 15 stamps are used, write this instead
            {
                AssessmentText.text = "Your abstract painting highlights your personality and artistic expression. The abundant usage of many stamps exudes charm and intimacy.";
            }

        }

        if (creativeCheck != 0 || coherentCheck != 0) // if the creative or coherent points are not equal to 0, set these conditions
            //** this is to prevent unity from making any comparisons when they are both 0 
        {
            if (coherentCheck <= creativeCheck) //painting is creative/versatile
            {
                AssessmentText.text = "Your abstract painting boasts a versatile amount of colors and patterns. A visual feast that is both unique and striking";
            }

            if (coherentCheck > creativeCheck) //painting is coherent
            {
                AssessmentText.text = " Your abstract painting place an emphasis on certain patterns, creating an elegant sense of coherence that is not often seen in this style.";
            }
        }


        yield return null;

    }
    // Update is called once per frame
    void Update() 
    {
      
    }

    IEnumerator GenSolidStrokes() // solid color brush
    {

        int strokeCounter = 0;

        while (strokeCounter < BrushStrokesSlider.SolidSliderValue) // as long as the current num of strokes is lower than the amount that the player set in the slider
        {
            Vector2 SpawnPos = new Vector3(Random.Range(CanvaBound.bounds.min.x, CanvaBound.bounds.max.x), Random.Range(CanvaBound.bounds.min.y, CanvaBound.bounds.max.y)); 
            // set the spawn position to be withinthe canva boundary
            GameObject newStroke = Instantiate(solidStroke, SpawnPos, Quaternion.Euler(0, 0, Random.Range(-100, 100))); //spawn the brushstroke randomly and set rotation

            newStroke.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f)); //change the color of each generated stroke randomly
            newStroke.transform.localScale *= Random.Range(0.5f, 1.5f); // change the size of each stroke randomly 
            strokeCounter++;

            brushStrokes.Add(newStroke); // add the spawned brushstroke into this list

            yield return null; //wait 1 frame after finishing this coroutine
        }
    }


    IEnumerator GenWavyStrokes() // wavy brush, same method
    {
        int strokeCounter = 0;

        while (strokeCounter < BrushStrokesSlider.WavySliderValue)
        {
            Vector2 SpawnPos = new Vector3(Random.Range(CanvaBound.bounds.min.x, CanvaBound.bounds.max.x), Random.Range(CanvaBound.bounds.min.y, CanvaBound.bounds.max.y));
            //Vector2 SpawnPos = new Vector2(Random.Range(-6f, -1.5f), Random.Range(-3f, 2f));
            GameObject newStroke = Instantiate(wavyStroke, SpawnPos, Quaternion.Euler(0, 0, Random.Range(-100, 100)));

            newStroke.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
            newStroke.transform.localScale *= Random.Range(0.5f, 1.5f);
            strokeCounter++;

            brushStrokes.Add(newStroke);

            yield return null;
        }
    }

    IEnumerator GenStripeStrokes()
    {
        int strokeCounter = 0;

        while (strokeCounter < BrushStrokesSlider.StripeSliderValue)
        {
            Vector2 SpawnPos = new Vector3(Random.Range(CanvaBound.bounds.min.x, CanvaBound.bounds.max.x), Random.Range(CanvaBound.bounds.min.y, CanvaBound.bounds.max.y));
   
            GameObject newStroke = Instantiate(stripeStroke, SpawnPos, Quaternion.Euler(0, 0, Random.Range(-100, 100)));

            newStroke.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
            newStroke.transform.localScale *= Random.Range(0.5f, 1.5f);
            strokeCounter++;

            brushStrokes.Add(newStroke);

            yield return null;
        }
    }

    IEnumerator GenDotStroke()
    {
        int strokeCounter = 0;

        while (strokeCounter < BrushStrokesSlider.DotSliderValue)
        {
            Vector2 SpawnPos = new Vector3(Random.Range(CanvaBound.bounds.min.x, CanvaBound.bounds.max.x), Random.Range(CanvaBound.bounds.min.y, CanvaBound.bounds.max.y));
          
            GameObject newStroke = Instantiate(dotStroke, SpawnPos, Quaternion.Euler(0, 0, Random.Range(-100, 100)));

            newStroke.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
            newStroke.transform.localScale *= Random.Range(0.5f, 1.5f);
            strokeCounter++;

            brushStrokes.Add(newStroke);

            yield return null;
        }
    }

    IEnumerator GenSwirlStroke()
    {
        int strokeCounter = 0;

        while (strokeCounter < BrushStrokesSlider.SwirlSliderValue)
        {
            Vector2 SpawnPos = new Vector3(Random.Range(CanvaBound.bounds.min.x, CanvaBound.bounds.max.x), Random.Range(CanvaBound.bounds.min.y, CanvaBound.bounds.max.y));
         
            GameObject newStroke = Instantiate(swirlStroke, SpawnPos, Quaternion.Euler(0, 0, Random.Range(-100, 100)));

            newStroke.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
            newStroke.transform.localScale *= Random.Range(0.5f, 1.5f);
            strokeCounter++;

            brushStrokes.Add(newStroke);

            yield return null;
        }
    }




    IEnumerator GenStamps() //randomly pick and generate a stamp from the list aka SpecialStampList
    {
        int strokeCounter = 0;

        while (strokeCounter < BrushStrokesSlider.StampSliderValue)
        {
            int randomStampValue = Random.Range(0, specialStampList.Count); //get a random number
            GameObject randomStamp = specialStampList[randomStampValue]; //select an object from the list based on that number (the number is the order of their placement)

            Vector2 SpawnPos = new Vector3(Random.Range(CanvaBound.bounds.min.x, CanvaBound.bounds.max.x), Random.Range(CanvaBound.bounds.min.y, CanvaBound.bounds.max.y));


            GameObject Stamp = Instantiate(randomStamp, SpawnPos, Quaternion.Euler(0, 0, Random.Range(-100, 100))); 
            //spawn that random object/stamp that you just got from the list and store it in a diff gameobject

            //randomStamp.transform.localScale *= Random.Range(1f, 1.5f); // randomly change the size of each stamp // scale of prefab stamp gets changed along with this. Why is that?
            randomStamp.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f)); //randomly change the color of the stamp
            strokeCounter++;

            brushStrokes.Add(Stamp); //add all spawned stamps into the "spawnedStrokes" list 

            yield return null;
        }
    }
}
