using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    RecipeMixer recipeMixer;
    Animator animator;
    private void OnEnable()
    {
        CheckRecipeMixer();
        CheckAnimator();

        animator.Play("Open");

        recipeMixer.OnRecipeComplete += AnimationHandler_OnRecipeComplete;
    }

    private void AnimationHandler_OnRecipeComplete()
    {

        CheckAnimator();
        animator.Play("Close");
    }

    private void OnDisable()
    {
        CheckRecipeMixer();

        recipeMixer.OnRecipeComplete += AnimationHandler_OnRecipeComplete;
    }

    void CheckRecipeMixer()
    {
        if (recipeMixer == null) recipeMixer = this.GetComponent<RecipeMixer>();
    }
    void CheckAnimator()
    {
        if (animator == null)
            animator = this.GetComponent<Animator>();
    }
}
