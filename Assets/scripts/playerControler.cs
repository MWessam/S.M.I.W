
using UnityEngine;

using static events;

using DG.Tweening;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using System;
using static inventory;




//Responsibility: Player input handling
public class playerControler : MonoBehaviour
{
    [SerializeField] GameObject inventoryObject;

    public static playerControler pc; //not needed currently 
    bool grounded = false;
    bool forwardHead = false;
    bool forwardLeg = false;
    Animator an;
    Rigidbody2D rb;

    [SerializeField] Inventory inventory;
    private void Awake()
    {
        if (pc == null)
        {
            pc = this;
        }
        else
        {
            Debug.LogWarning("multiple player controller ditected");
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        an = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();


        //subscribe to events
        onTileBreakEnd += OnTileBreakEnd;
        onGroundCheck += OnGroundCheck;
        onForwardLegCheck += OnForwardLegCheck;
        onForwardHeadCheck += OnForwardHeadCheck;
        onItemDataRetrived += OnItemDataRetrived;
    }

    [SerializeField] float speed = 0.1f;
    [SerializeField] float jump_Force = 10f;
    // Update is called once per frame
    float x;
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEditor.EditorWindow.focusedWindow.maximized = !UnityEditor.EditorWindow.focusedWindow.maximized;
        }
#endif
        if (roundTo1(rb.velocity.x) != 0)
        {

            transform.localScale = new Vector3(Mathf.Sign(roundTo1(rb.velocity.x)) * 1, 1, 1);
        }
        x = Input.GetAxis("Horizontal");
        if (Input.GetKey(KeyCode.LeftShift) && (x == 1 || x == -1) && forwardLeg && !forwardHead)
        {
            forwardLeg = false;
            jump();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            // bool toggleTo = !inventoryObject.activeSelf;
            // inventoryObject.SetActive(toggleTo);
            // TriggerInventoryToggle(toggleTo);
            //Inventory.ToggleInventory();
        }

        if (Input.GetMosueButtonDown(0))
        {
            // TileManager.StartBreakingBlock(postion);
            
        }

        if (Input.GetMouseButtonup(0))
        {
            tileManager.StopBreakingBlock();
        }

    }


[Serializable]
public class PowerUpEntry
{
    public ItemData itemData;
    public float multiplier;
}

  
    void FixedUpdate()
    {
 

        an.SetFloat("yVelocity", roundTo1(rb.velocity.y));

        //control the animation --------------------------------------
        if (roundTo1(rb.velocity.x) != 0 && grounded)
        {
            an.SetBool("walk", true);
        }
        else
        {
            an.SetBool("walk", false);
        }
        //Debug.Log(1 / Time.deltaTime);
        rb.AddForce(new Vector3(x * speed * Time.deltaTime, 0, 0));

    }
    float roundTo1(float value)
    {
        return Mathf.Round(value * 10f) / 10f;
    }

    [HideInInspector] float timeSinceLastGround = 0;
    float timesinceTheLastJump = 0;
    void jump()
    {

        if (Time.time - timesinceTheLastJump > Time.deltaTime * 15)
        {

            if (grounded || Time.time - timeSinceLastGround < Time.deltaTime * 5)
            {
                an.SetTrigger("jump");
                timesinceTheLastJump = Time.time;
                rb.AddForce(new Vector3(0, jump_Force * Time.fixedDeltaTime, 0));
            }

        }



    }

    void OnGroundCheck(bool state)
    {
        grounded = state;
        if (state)
        {


            if (!an.GetBool("Grounded_bool"))
            {
                //first time to be true(enter)
                rb.velocity = new Vector3(0, 0, 0);
                an.SetBool("Grounded_bool", true);
                an.SetTrigger("grounded");
                an.ResetTrigger("jump");
            }
            else
            {
                //it was true before(stay)
                an.SetBool("Grounded_bool", true);
            }
        }
        else
        {
            timeSinceLastGround = Time.time;
            an.SetBool("Grounded_bool", false);
        }

    }
    void OnForwardLegCheck(bool state)
    {
        forwardLeg = state;
    }
    void OnForwardHeadCheck(bool state)
    {
        forwardHead = state;
    }

    // void OnTileBreakEnd((ItemData, TileBase) data, Vector3Int cellPos)
    // {
    //     // inventory.addItem(data.Item1);
    //     Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //     pos.z = 0;
    //     Instantiate(data.Item1.worldPrefabe).transform.position = pos;
    //
    // }

    void OnItemDataRetrived(List<(ItemData, TileBase)> datas, Vector3Int cellpos)
    {

        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            foreach ((ItemData, TileBase) data in datas)
            {
                if (data.Item1.breakable)
                {


                    TriggerTileBreakStart(data, cellpos);
                }
            }
        }

    }

}
