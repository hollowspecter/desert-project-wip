using UnityEngine;
using System.Collections;

public class CharacterLogic : MonoBehaviour
{
    #region Variables (private)
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private float directionDampTime = .25f; //delays animatior settings for a natural feel, goodfor pivoting
    private float speed = 0.0f;
    private float h = 0.0f;
    private float v = 0.0f;
    #endregion

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();

        if (anim.layerCount >= 2) {
            anim.SetLayerWeight(1, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (anim) {
            //Pull values from controller/keyboard
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            speed = new Vector2(h, v).sqrMagnitude;

            anim.SetFloat("Speed", speed);
            anim.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        }
    }
}