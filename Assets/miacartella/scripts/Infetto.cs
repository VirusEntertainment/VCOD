﻿using UnityEngine;
using System.Collections;
//using UnityEditor;

public class Infetto : MonoBehaviour {

    Animation anim;
	int health;
	int maxHealth;
	GameObject player;
	bool isInCombat;
	bool isAttacking;
	bool backHome;
	bool dead;
	float attackTime;
	float attackanimDuration;
	float attackSpeed;
	bool attackRec;
	string damageRec;
	Vector3 initialPos;
	GameObject sphere;
	public Text_Manager TM;
	int level;
	int ID = 0;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animation>();
		attackanimDuration = 0.79F;
		attackSpeed = 1.5F;
		maxHealth = 20;
		health = maxHealth;
		player = GameObject.FindGameObjectsWithTag("Player")[0];
		initialPos = transform.position;
		damageRec = "";
		attackTime = attackanimDuration;
		isInCombat = false;
		isAttacking = false;
		backHome = false;
		dead = false;
		attackRec = false;
        level = 15;
	}
	public void setId(int id){
		ID = id;
	}
	public void setLevel(int lev){
		level = lev;
	}
	public int getLevel(){
		return level;
	}
	public int getId(){
		return ID;
	}
	public int getHealth(){
		return health;
	}
	public int getMaxHealth(){
		return maxHealth;
	}
	// Update is called once per frame
	void Update () {

        if (!isDead())
        {
            Vector3 posToFace;

            float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
            float distanceFromHome = Vector3.Distance(transform.position, initialPos);
            if (distanceFromPlayer <= 8 && distanceFromPlayer >= 2 && !isInCombat && !backHome)
            {
                isInCombat = true;
                anim.Play("zombieRun");
            }

            //------ posiziona il mob verso la direzione del player
            if (isInCombat)
            {
                posToFace = player.transform.position;
            }
            else
            {
                posToFace = initialPos;
            }
            facePosition(posToFace);

            // se il mob è in combat e la distanza è maggiore o uguale a 2 si muove verso il player e non attacca
            if (isInCombat && distanceFromPlayer >= 2)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 0.13F);
                isAttacking = false;
                attackTime -= Time.deltaTime;
            }
            //------------------------------------------
            //Se il mob è in combat e la distanza è minore di 2 e non sta attaccando allora il mob comincia a attaccare
            if (isInCombat && distanceFromPlayer < 2 && !isAttacking)
            {
                isAttacking = true;
            }
            //-----------------------------------------------
            //questa non l'ho capita anche perchè non ho mai visto 2 booleani confrontati (sono 2 bool no?)
            //isAttacking è un booleano dell'infetto che viene messo a true se sta attaccando
            //anim.getBool o anim.setBool sono i metodi che prendono o settano un booleano all'animazione
            //in sostanza qui viene attivata o disattivata l'animazione in base al valore di isAttacking
            if (!anim.IsPlaying("attack1") && isAttacking /*anim.GetBool("attack") != isAttacking*/)
            {
                anim.Stop("zombieRun");
                anim.Play("attack1");
                if (isAttacking)
                {
                    float duration = 0.79F;
                    float curSpeed = anim["attack1"].speed;
                    anim["attack1"].speed = duration / attackSpeed;

                    //Debug.Log("La velocità dell'animazione è " + anim["attack1"].speed);
                }
            }

            //Se è in combat ma la distanza dal punto di spawn è maggiore o uguale a 25 torna al punto di spawn e resetto l'aggro
            if (isInCombat && distanceFromHome >= 25)
            {
                isInCombat = false;
                backHome = true;
                isAttacking = false;
                anim.Stop("attack1");
                
            }

            //backHome è true quando l'infetto ha disingaggiato il player, questo controllo serve per far
            //tornare alla posizione iniziale l'infetto.
            if (backHome && distanceFromHome > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, initialPos, 0.13F);
                anim.Play("zombieRun");
                if (distanceFromHome <= 1)
                {
                    backHome = false;
                    anim.Stop("zombieRun");
                    health = maxHealth;
                }

            }

            //se il bool è a true attacco il player
            if (isAttacking)
            {
                attackPlayer();
            }

        }
        else
        {
            Destroy(this.gameObject, 2.0f);
        }

	}

	void facePosition(Vector3 pos){
			if (isInCombat || backHome) {
			Vector3 target = pos - transform.position;
			float angle = Vector3.Angle(target, transform.forward);
			Vector3 targetDir = pos - transform.position;
			Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, 30, 1.0F);
			transform.rotation = Quaternion.LookRotation(newDir);
		}
	}
	public bool isDead(){
        
        if (health <= 0)
        {
            dead = true;
            //player.SendMessage("setExp", level);
        }
		return dead;
	}
	void attackPlayer(){
		attackTime -= Time.deltaTime;
		if (attackTime  < 0.0F) {
			int dam = Mathf.RoundToInt(Random.Range(5, 15));
			player.SendMessage("applyDamage", dam);
			attackTime = attackSpeed;
		} 
	}

	void applyDamage(int damage){
		health -= damage;
		damageRec = "<b>" + damage + "</b>";
	}

	//collisione del proiettile con l'infetto
	void OnCollisionEnter (Collision collision)
	{
        Collider ammo = collision.collider;
        if (ammo.gameObject.tag == "Proiettile")
        {
            applyDamage(10);
            Destroy(ammo.gameObject);
        }
	}
}
