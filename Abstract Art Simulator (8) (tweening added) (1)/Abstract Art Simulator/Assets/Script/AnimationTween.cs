using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using DG.Tweening;

public class AnimationTween : MonoBehaviour
{
    public float fadeTime = 2f;

    public float bounceIntensity = 500f;

    public RectTransform rectTransform;
    public Vector3 EndPos;
    public Vector3 StartPos;

    //currenly using genbutton as experiementation

    private void Awake()
    {
        EndPos = transform.localPosition; //get the pos of where you want the game object to be
      
    }

    IEnumerator UIPanelAnimation()
    {
        rectTransform.DOLocalMoveY(EndPos.y, fadeTime, false).SetEase(Ease.OutElastic); //test out other animation transition here (change Ease.(transition)) 
        yield return null;
    }

 
   
    void Start()
    {

        StartPos = new Vector3(EndPos.x, EndPos.y + bounceIntensity * -1, EndPos.z); // get the start position (starting point of animation), which is outside of the game screen
        rectTransform.transform.localPosition = StartPos;//place the gameobject at that starting point

        StartCoroutine(UIPanelAnimation()); 

      
     
    }
}
