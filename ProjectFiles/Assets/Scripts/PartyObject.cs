using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyObject : MonoBehaviour
{
    private bool allowMovement = true;    
    [SerializeField] private OverworldMovement overworldMovement;
    [SerializeField] private GameObject partyMemberPrefab;
    public Vector2[] storedDir = new Vector2[3];
    public OverworldMovement[] partyOverworldMovement = new OverworldMovement[3];
    private Collider2D standingOver;
    private bool sprinting;
    [SerializeField] private bool allowZInput = true;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip enemyEncounterClip;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        allowZInput = true;
        CreateParty();
    }

    private void Update()
    {
        if (allowMovement)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                sprinting = true;
                foreach (OverworldMovement overworldMovement in partyOverworldMovement)
                    overworldMovement.SetSprint(sprinting);
            }
            else if(Input.GetKeyUp(KeyCode.LeftShift))
            {
                sprinting = false;
                foreach (OverworldMovement overworldMovement in partyOverworldMovement)
                    overworldMovement.SetSprint(sprinting);
            }

            Vector2 movement = new Vector2(0f, 0f);
            
            if (Input.GetKey(KeyCode.UpArrow))
            {
                movement = new Vector2(0, 1);
                if (!partyOverworldMovement[0].MovePos(movement, sprinting))
                {
                    StoreDir(movement);
                    MoveParty();
                }
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                movement = new Vector2(0, -1);
                if (!partyOverworldMovement[0].MovePos(movement, sprinting))
                {
                    StoreDir(movement);
                    MoveParty();
                }
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                movement = new Vector2(-1, 0);
                if (!partyOverworldMovement[0].MovePos(movement, sprinting))
                {
                    StoreDir(movement);
                    MoveParty();
                }
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                movement = new Vector2(1, 0);
                if (!partyOverworldMovement[0].MovePos(movement, sprinting))
                {
                    StoreDir(movement);
                    MoveParty();
                }
            }
            if (Input.GetKey(KeyCode.Z))
            {
                if (allowZInput)
                {
                    if (standingOver != null)
                    {
                        standingOver.GetComponent<Interactable>().TryDialogue(this);
                    }
                    else
                    {
                        RaycastHit2D hit = partyOverworldMovement[0].Interact();
                        if (hit)
                        {
                            if (hit.collider.CompareTag("Interactable"))
                            {
                                hit.collider.GetComponent<Interactable>().BeginDialogue(this);
                            }
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                allowMovement = false;
                GameManager.instance.OpenOverworldMenu();
            }
        }
    }

    public void DelayZInput()
    {
        allowZInput = false;
        StartCoroutine(ZDelay());
    }

    private IEnumerator ZDelay()
    {
        yield return new WaitForSeconds(0.25f);
        allowZInput = true;
    }

    public void CreateParty()
    {
        partyOverworldMovement[0] = this.GetComponent<OverworldMovement>();
        partyOverworldMovement[0].SetUnit(GameManager.instance.party[0].unit);
        for (int i = 1; i < GameManager.instance.party.Count; i++)
        {
            partyOverworldMovement[i] = Instantiate(partyMemberPrefab, transform.position, Quaternion.identity, GameObject.Find("PlayerObjects").transform).GetComponent<OverworldMovement>();
            partyOverworldMovement[i].SetUnit(GameManager.instance.party[i].unit);
        }
        this.transform.GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = 100;
    }

    private void MoveParty()
    {
        for (int i = 1; i < partyOverworldMovement.Length; i++)
        {
            if (partyOverworldMovement[i] != null)
                partyOverworldMovement[i].MovePos(storedDir[i], sprinting);
        }
    }

    private void StoreDir(Vector2 dir)
    {
        Vector2 a = dir;
        Vector2 b;
        for (int i = 0; i < storedDir.Length; i++)
        {
            b = storedDir[i];
            storedDir[i] = a;
            a = b;
        }
    }

    public void PauseMovement()
    {
        allowMovement = false;
    }
    public void AllowMovement()
    {
        DelayZInput();
        allowMovement = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && collision.gameObject.GetComponent<OverworldEnemyObject>())
        {
            audioSource.clip = enemyEncounterClip;
            audioSource.Play();
            allowMovement = false;
            GameManager.instance.partyPosition = transform.position;
            GameManager.instance.LoadBattle(collision.gameObject.GetComponent<OverworldEnemyObject>().GetStartingLoc());
            GameManager.instance.SaveGame();
        }
        if (collision.gameObject.CompareTag("Interactable"))
        {
            standingOver = collision.gameObject.GetComponent<Collider2D>();
        }

        if (collision.CompareTag("Staircase"))
        {
            if (allowMovement)
            {
                
                Staircase c = collision.gameObject.GetComponent<Staircase>();
                if (c.GetChangeScene())
                {
                    PauseMovement();
                    GameManager.instance.ChangeGameScene(c.GetSceneChangeIndex());
                }
                else
                {
                    GameManager.instance.DoTransition();
                    transform.position = c.GetSceneChangePos();
                    overworldMovement.moving = false;
                }
                    
                GameManager.instance.gameData.playerPos = c.GetSceneChangePos();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        standingOver = null;
    }
}
