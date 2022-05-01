using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public GameObject player_prefab;
    public GameObject rope_prefab;
    public GameObject hook_prefab;

    public Player player;
    public Information info;

    void Start() {
        info = new Information(this.transform.localPosition, player_prefab, rope_prefab, hook_prefab);
        player = new Player(info);
    }

    void Update() {
        
    }

    void FixedUpdate() {
        // player.move();
    }

    public class Player {
        public Vector2 prev_pt;
        public Vector2 curr_pt;

        public GameObject player_obj;
        public Rigidbody2D player_rb;
        public GameObject hook_obj;
        public Information info;
        

        public Player(Information info){
            this.player_obj = Instantiate(info.player_prefab, info.start_pt, Quaternion.identity);
            this.player_rb = this.player_obj.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
            this.info = info;

            shoot_hook(Vector2.up);

            // add_force(new Vector2(0.04f, 0.03f));
        }

        public void shoot_hook(Vector2 direction){
            int mask = LayerMask.GetMask("Collision");
            RaycastHit2D hit = Physics2D.Raycast(curr_pt, direction, 20, mask);
            if (hit.collider != null){
                print(hit.point);
                hook_obj = Instantiate(info.hook_prefab, hit.point, Quaternion.identity);
                float new_rope_dist = info.rope_dist;
                float hit_dist = (hit.point - curr_pt).magnitude;
                int i = 0;
                while (new_rope_dist < hit_dist) {
                    info.rope_joints_trans[i].localPosition = curr_pt + (new_rope_dist * direction);
                    info.rope_joints_obj[i].SetActive(true);
                    new_rope_dist += info.rope_dist;
                    i += 1;
                }
            }
        }

        public void add_force(Vector2 force) {
            prev_pt -= force;
        }

        public void move(){
            Vector2 diff = curr_pt - prev_pt;
            prev_pt = curr_pt;
            curr_pt += diff;
            curr_pt += info.gravity;
            player_rb.MovePosition(curr_pt);
        }
    }

    public class Information {
        public Vector2 start_pt;
        public GameObject player_prefab;
        public GameObject rope_prefab;
        public GameObject hook_prefab;
        public Vector2 gravity = new Vector2(0, -0.001f);
        public float rope_dist;
        public List<Transform> rope_joints_trans = new List<Transform>();
        public List<Rigidbody2D> rope_joints_rb = new List<Rigidbody2D>();
        public List<GameObject> rope_joints_obj = new List<GameObject>();

        public Information(Vector2 loc, GameObject pp, GameObject rp, GameObject hp){
            this.start_pt = loc;
            this.player_prefab = pp;
            this.rope_prefab = rp;
            this.hook_prefab = hp;
        }

        public void create_rope_joints(int num_to_make){
            for (int i = 0; i < num_to_make; i++){
                GameObject joint = Instantiate(rope_prefab, Vector2.zero, Quaternion.identity);
                joint.SetActive(false);
                rope_joints_trans.Add(joint.transform);
                rope_joints_rb.Add(joint.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D);
                rope_joints_obj.Add(joint);
            }
        }
    }
}
