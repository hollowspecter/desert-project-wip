using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class DoorRiddle : MonoBehaviour 
{
    [SerializeField]
    private int resolutionOfTextures = 256;

    private CompareTextures[] comparer;
    private Animator _animator;
    private bool[] doorSolutions;
    private ParticleSystem doorsolvedParticles;

    void Awake()
    {
        comparer = GetComponents<CompareTextures>();
        Assert.IsNotNull<CompareTextures[]>(comparer);

        _animator = GetComponent<Animator>();
        Assert.IsNotNull<Animator>(_animator);

        doorSolutions = new bool[3];
        for (int i = 0; i < 3; ++i) {
            doorSolutions[i] = false;
        }

        doorsolvedParticles = GetComponentInChildren<ParticleSystem>();
        Assert.IsNotNull<ParticleSystem>(doorsolvedParticles);
    }

    // This function is called, when the pen is lifted in one of the doors
    public void CheckOneDoor(Texture2D textureToCheck, DoorDrawingScript drawingScript)
    {
        for (int i = 0; i < 3; ++i) {
            if (doorSolutions[i] == false) {
                bool isRight = comparer[i].CompareTextureToPerfect(textureToCheck, resolutionOfTextures);
                if (isRight) {
                    doorSolutions[i] = true;
                    /* INSERT HERE: Code to deactivate the
                     * Drawing Script at this door and notify the player
                     * that this is right, also deactivate the sphere collider
                     * or do something else to prevent the player to interact with it again*/ //DONE
                    drawingScript.Deactivate();
                    Transform drawingTransform = drawingScript.transform;
                    DoorInteractable doorInteractable = drawingTransform.parent.GetComponent<DoorInteractable>();
                    doorInteractable.Deactivate();

                    //Start the particle system on the right position
                    doorsolvedParticles.transform.position = drawingTransform.position + Vector3.up * 0.02f;
                    doorsolvedParticles.Play();

                    Debug.Log("RIDDLE SOLVED! " + i);
                    break;
                }
            }
        }
        CheckConditions();
    }

    // checks if all the doors are solved
    void CheckConditions()
    {
        bool solved = true;
        for (int i = 0; i < 3; ++i) {
            if (!doorSolutions[i])
                solved = false;
        }
        if (solved) {
            RiddleSolved();
        }
    }

    void RiddleSolved()
    {
        Debug.Log("THE RIDDLE IS SOLVED");
        _animator.SetTrigger("Open");
    }
}
