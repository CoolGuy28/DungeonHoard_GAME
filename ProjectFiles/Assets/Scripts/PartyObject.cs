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
    [SerializeField] private float keyPressCooldown = 0.05f;
    private bool cantMove;

    private void Start()
    {
        CreateParty();
    }

    private void Update()
    {
        if (cantMove)
            return;
        if (allowMovement)
        {
            Vector2 movement = new Vector2(0f, 0f);
            
            if (Input.GetKey(KeyCode.UpArrow))
            {
                movement = new Vector2(0, 1);
                if (!partyOverworldMovement[0].MovePos(movement))
                {
                    StoreDir(movement);
                    MoveParty();
                }
                StartCoroutine(KeyCooldown());
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                movement = new Vector2(0, -1);
                if (!partyOverworldMovement[0].MovePos(movement))
                {
                    StoreDir(movement);
                    MoveParty();
                }
                StartCoroutine(KeyCooldown());
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                movement = new Vector2(-1, 0);
                if (!partyOverworldMovement[0].MovePos(movement))
                {
                    StoreDir(movement);
                    MoveParty();
                }
                StartCoroutine(KeyCooldown());
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                movement = new Vector2(1, 0);
                if (!partyOverworldMovement[0].MovePos(movement))
                {
                    StoreDir(movement);
                    MoveParty();
                }
                StartCoroutine(KeyCooldown());
            }
            if (Input.GetKey(KeyCode.Z))
            {

                partyOverworldMovement[0].Interact(movement);
            }
        }
    }

    public void CreateParty()
    {
        partyOverworldMovement[0] = this.GetComponent<OverworldMovement>();
        partyOverworldMovement[0].SetUnit(GameManager.instance.party[0].unit);
        partyOverworldMovement[0].SetSpriteLayer(5);
        for (int i = 1; i < GameManager.instance.party.Count; i++)
        {
            partyOverworldMovement[i] = Instantiate(partyMemberPrefab, transform.position, Quaternion.identity, GameObject.Find("PlayerObjects").transform).GetComponent<OverworldMovement>();
            partyOverworldMovement[i].SetUnit(GameManager.instance.party[i].unit);
            partyOverworldMovement[i].SetSpriteLayer(5-i);
        }
    }

    private void MoveParty()
    {
        for (int i = 1; i < partyOverworldMovement.Length; i++)
        {
            if (partyOverworldMovement[i] != null)
                partyOverworldMovement[i].MovePos(storedDir[i]);
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

    private IEnumerator KeyCooldown()
    {
        cantMove = true;
        yield return new WaitForSeconds(keyPressCooldown);
        cantMove = false;
    }

    public void BeginFishing()
    {
        allowMovement = false;
    }
    public void EndFishing()
    {
        allowMovement = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && collision.gameObject.GetComponent<OverworldEnemyObject>())
        {
            allowMovement = false;
            GameManager.instance.partyPosition = transform.position;
            GameManager.instance.LoadBattle(collision.gameObject.GetComponent<OverworldEnemyObject>().index);
        }
    }
}
