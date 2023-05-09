using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AimeTime
{
    START,
    MIDDLE,
    END,
    INBETWEEN,
    INFIRSTBETWEEN,
    INSECONDBETWEEN
}
public class AnimationFunction
{
    public AimeTime animtime;
    public AnimatorController.ActionDelegate action;
    public AnimatorController.CallbackDelegate onStart;
    public AnimatorController.CallbackDelegate onComplete;
    public int layer;
    public float time;
    public float endsubtracttime;
    public float endtime;
    public float counter;
}

public class AnimatorController : MonoBehaviour
{
    public string currentAnimationState;
    public Animator animator;
    public float counter = 0;

    public delegate bool ActionDelegate();
    public delegate void CallbackDelegate();

    List<AnimationFunction> applyActions=new List<AnimationFunction>();
    // Start is called before the first frame update
    void Start()
    {
        animator.GetComponent<Animator>();
    }

    private void Update()
    {
        for (int i= applyActions.Count-1; i>=0; i--)
        {
            ApplyAction(applyActions[i]);
        }
    }

    public bool ChangeAnimationState(string statename)
    {
        if (currentAnimationState == statename)
            return false;
        animator.Play(statename);
        currentAnimationState = statename;
        for (int i = 0; i < applyActions.Count; i++)
        {
           applyActions[i].onComplete?.Invoke();
        }
        applyActions.Clear();
        counter = 0;
        AnimationFunction animationaction = new AnimationFunction();
        animationaction.animtime = AimeTime.END;
        animationaction.action = () => { Reset(); return false; };
        AddAction(animationaction);
        return true;
    }

    public bool ChangeAnimationStateWithLock(string statename)
    {
        if (currentAnimationState != "")
            return false;
        animator.Play(statename);
        currentAnimationState = statename;
        for (int i = 0; i < applyActions.Count; i++)
        {
            applyActions[i].onComplete?.Invoke();
        }
        applyActions.Clear();
        counter = 0;
        AnimationFunction animationaction = new AnimationFunction();
        animationaction.animtime = AimeTime.END;
        animationaction.action = () => { Reset(); return true; };
        AddAction(animationaction);
        return true;
    }

    public void AddAction(AnimationFunction animationaction)
    {
        applyActions.Add(animationaction);
    }
    

    public void Reset()
    {
        Debug.Log("cleared");
        currentAnimationState = "";
        applyActions.Clear();
        counter = 0;
    }

    void ApplyAction(AnimationFunction animationaction)
    {
        AnimatorStateInfo stateinfo= animator.GetCurrentAnimatorStateInfo(animationaction.layer);
        if(!stateinfo.IsName(currentAnimationState))
        {
            return;
        }
        animationaction.counter += Time.deltaTime;
        float time=stateinfo.length;
        Debug.Log(currentAnimationState + time);

        if (animationaction.animtime== AimeTime.START)
        {
            time = 0;
            
        }
        else if(animationaction.animtime == AimeTime.MIDDLE)
        {
            time /= 2;
            
        }
        else if(animationaction.animtime == AimeTime.END)
        {
            
        }
        else if(animationaction.animtime == AimeTime.INBETWEEN)
        {
            if(animationaction.counter >= animationaction.time && animationaction.counter < (animationaction.endtime!=0 ? animationaction.endtime:time - animationaction.endsubtracttime))
            {
                if(animationaction.action!=null)
                {
                    if (animationaction.action.Invoke())
                    {
                        animationaction.onComplete?.Invoke();
                        applyActions.Remove(animationaction);
                    }
                }
                
            }
        }
        else if (animationaction.animtime == AimeTime.INFIRSTBETWEEN)
        {
            if (animationaction.counter >= animationaction.time && animationaction.counter < (time / 2 + animationaction.time))
            {
                if (animationaction.action != null)
                {
                    if (animationaction.action.Invoke())
                    {
                        animationaction.onComplete?.Invoke();
                        applyActions.Remove(animationaction);
                    }
                }

            }
        }
        else if (animationaction.animtime == AimeTime.INSECONDBETWEEN)
        {
            if (animationaction.counter >= animationaction.time && animationaction.counter>time/2 && animationaction.counter < time)
            {
                if (animationaction.action != null)
                {
                    if (animationaction.action.Invoke())
                    {
                        animationaction.onComplete?.Invoke();
                        applyActions.Remove(animationaction);
                    }
                }

            }
        }

        if (animationaction.counter >= time)
        {
            animationaction.action?.Invoke();
            animationaction.onComplete?.Invoke();
            applyActions.Remove(animationaction);
        }
    }
    public bool IsCurrentState(string state)
    {
        if (currentAnimationState == state)
            return true;
        else return false;
    }

    private void OnDestroy()
    {
        applyActions.Clear();
    }
}
