using UnityEngine;
using System.Collections;

public class PlayerTag : MonoBehaviour {
    private Vector3 startScale;
    private Material left;
    private float leftFill = 1;
    //private ParticleSystem leftParticles;
    private Material right;
    private float rightFill = 1;
    //private ParticleSystem rightParticles;
    private int fillID;

    private IAction attack;
    private IAction shield;

    void Start()
    {
        startScale = transform.localScale;
        fillID = Shader.PropertyToID("_Fill");
        right = transform.FindChild("Right").GetComponent<SpriteRenderer>().material;
        //rightParticles = transform.FindChild("Right/Particles").GetComponent<ParticleSystem>();
        left = transform.FindChild("Left").GetComponent<SpriteRenderer>().material;
        //leftParticles = transform.FindChild("Left/Particles").GetComponent<ParticleSystem>();
        IAction[] attachedActions = transform.parent.GetComponents<IAction>();
        for (int i = 0; i < attachedActions.Length; i++)
        {
            if (attachedActions[i].IsAttack())
                attack = attachedActions[i];
            else
                shield = attachedActions[i];
        }
    }
    void LateUpdate () {
        Vector3 localScale = startScale;
        if (transform.parent.lossyScale.x < 0)
            localScale.x = -startScale.x;
        else
            localScale.x = startScale.x;
        transform.localScale = localScale;
        SetLeftFill(shield.GetPercentage());
        SetRightFill(attack.GetPercentage());
	}

    private void SetLeftFill(float fill)
    {
        //if (fill >= 1 && leftFill < 1)
        //    leftParticles.Play();
        left.SetFloat(fillID, fill);
        leftFill = fill;
    }

    private void SetRightFill(float fill)
    {
        //if (fill >= 1 && rightFill < 1)
        //    rightParticles.Play();
        right.SetFloat(fillID, fill);
        rightFill = fill;
    }
}