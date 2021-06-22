using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviourPunCallbacks
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;
    private bool isGrounded = false;
    private GameObject lifebar;

    [Header("Damage")]
    public int enemyDamage;

    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider2d;
    [SerializeField] private LayerMask platformLayerMask;
    
    [Header("Physics")] 
    public float linearDrag;
    public float gravity;
    public float fallMultiplier;


    
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        }
        catch (Exception)
        {
        }
        
        boxCollider2d = gameObject.GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
        // lifebar = GameObject.Find("Canvas").transform.Find(transform.parent.name + "LifeBar").gameObject;
    }

    private void Update()
    {
        isGrounded = IsGrounded();
        
        // so that the enemy does slide when moving
        try
        {
            rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y);
            modifyPhysics();
        }
        catch (Exception)
        {
        }
        
        
        if (transform.parent.name != "RoiBlob" && !transform.parent.name.StartsWith("Bomber"))
        {
            try
            {
                lifebar = GameObject.Find("LifeBars").transform.Find(transform.parent.name + "LifeBar").gameObject;
                lifebar.transform.position = new Vector3(transform.position.x - 1, transform.position.y + 1, 0);
            }
            catch (Exception e)
            {
                Debug.Log("NOT FOUND");
                Destroy(gameObject);
            }
            
        }
        else if (transform.parent.name == "RoiBlob")
        {
            lifebar = GameObject.Find("Canvas").transform.Find(transform.parent.name + "LifeBar").gameObject;
        }

        
        
    }
    
    private bool IsGrounded()
    {
        // distance to the ground from which the player can jump again
        float extraHeightText = 0.1f;
        RaycastHit2D boxCastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.down,
            extraHeightText, platformLayerMask);
        return boxCastHit.collider != null;
    }
    
    private void modifyPhysics()
    {
        rigidbody2d.gravityScale = gravity;
        
        // Drag can be used to slow down an object. The higher the drag the more the object slows down.
        rigidbody2d.drag = linearDrag;
        rigidbody2d.gravityScale = gravity * fallMultiplier;
    }
    
    
    [PunRPC]
    private void ChangeBodyVisibility(float r, float g, float b, float a)
    {
        GetComponent<SpriteRenderer>().color = new Color(r, g, b, a);
    }
    public IEnumerator InvincibilityFlash(string playerName)
    {
        while(GetComponent<BossAI>().isInvincible)
        {
            photonView.RPC("ChangeBodyVisibility", RpcTarget.All, 1f, 0f, 0f, 0f);
            yield return new WaitForSeconds(0.15f);
            photonView.RPC("ChangeBodyVisibility", RpcTarget.All, 1f, 0f, 0f, 1f);
            yield return new WaitForSeconds(0.15f);
        }
    }

    public IEnumerator HandleInvincibilityDelay()
    {
        yield return new WaitForSeconds(2f);
        GetComponent<BossAI>().isInvincible = false;
    }
    
    [PunRPC]
    public void SetHealthBar(int value)
    {
        GameObject bar = GameObject.Find("Canvas").transform.Find("RoiBlobLifeBar").gameObject;
        if (value < bar.GetComponent<HPBar>().slider.value)
        {
            bar.GetComponent<HPBar>().SetHealth(value);
        }
        
    }
    public int TakeDamage(int damage)
    {
        
        if (transform.parent.name.StartsWith("RoiBlob"))
        {
            if(!GetComponent<BossAI>().isInvincible)
            {
                currentHealth -= damage;
                photonView.RPC("SetHealthBar", RpcTarget.All, currentHealth);
                GetComponent<BossAI>().isInvincible = true;
                StartCoroutine(InvincibilityFlash(gameObject.name));
                StartCoroutine(HandleInvincibilityDelay());
            }
        }
        else
        {
            currentHealth -= damage;
            lifebar.GetComponent<HPBar>().SetHealth(currentHealth);
        }
        
        
        
        if (transform.parent.name.StartsWith("Harpie"))
        {
            GetComponent<HarpieAI>().PushBack();
        }

        
        if (currentHealth <= 0)
        {
            if (transform.parent.name.StartsWith("RoiBlob"))
            {
                photonView.RPC("Victory", RpcTarget.All);
            }
            Die();
        }

        return currentHealth;
    }

    [PunRPC]
    private void Victory()
    {
        // Confetti
        Instantiate(GetComponent<BossAI>().confetti, transform.position, Quaternion.identity);
        // Display Congrats
        GameObject gameOverPanel = GameObject.Find("Canvas").transform.Find("GameOverPanel").gameObject;
        gameOverPanel.transform.Find("gameover Label").GetComponent<Text>().text = "Congratulations !";
        gameOverPanel.transform.Find("gameover Reason").GetComponent<Text>().text = PhotonNetwork.PlayerList[0].NickName + " and " + PhotonNetwork.PlayerList[1].NickName + " won";
        gameOverPanel.SetActive(true);
        GameObject.Find("PlayerTop(Clone)").GetComponent<PlayerMovement>().NowDead();
        GameObject.Find("PlayerBot(Clone)").GetComponent<PlayerMovement>().NowDead();
    }
    
    
    void Die()
    {
        Debug.Log("Enemy Died " + gameObject.name);
        // Destroy(gameObject.transform.parent.gameObject);
        Destroy(gameObject);
        // PhotonNetwork.Destroy(gameObject);
    }
    
}
