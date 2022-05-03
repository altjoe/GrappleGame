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
        player.move();
    }

    void FixedUpdate() {
        // player.move();
    }

    public class Player {
        public Vector2 prev_pt;
        public Vector2 curr_pt;

        public GameObject player_obj;
        public Rigidbody2D player_rb;

        public Hook hook;
        public Vector2 prev_rope_point; 

        public Information info;
        int current_rope_length = 0;
        float last_seg_dist;

        public Player(Information info){
            this.player_obj = Instantiate(info.player_prefab, info.start_pt, Quaternion.identity);
            this.player_rb = this.player_obj.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
            this.info = info;
            curr_pt = info.start_pt;

            shoot_hook(Vector2.up);
        }

        public void shoot_hook(Vector2 direction){
            hook = new Hook(curr_pt, direction, info);
        }

        public void rope_creation(Vector2 new_hook_loc, Vector2 prev_rope_loc){
            if ((new_hook_loc - prev_rope_loc).magnitude > info.rope_dist) {
                
            }
        }

        public void remove_rope() {

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

            hook.move();
            prev_rope_point = rope_creation(curr_pt, prev_rope_point)
        }

        public class Hook {
            Vector2 curr_pt;
            Vector2 prev_pt;
            float speed = 0.1f;
            GameObject hook_obj;
            Rigidbody2D hook_rb;
            Information info;
            bool made_contact = false;

            public Hook(Vector2 start_pt, Vector2 direction, Information info) {
                this.info = info;
                hook_obj = Instantiate(info.hook_prefab, start_pt, Quaternion.identity);
                hook_rb = hook_obj.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
                curr_pt = start_pt;
                prev_pt = start_pt - (direction * speed);
            }

            public void move(){
                if (!made_contact) {
                    Vector2 diff = curr_pt - prev_pt;
                    prev_pt = curr_pt;
                    curr_pt += diff;
                    int mask = LayerMask.GetMask("Collision");
                    RaycastHit2D hit = Physics2D.Raycast(prev_pt, diff, diff.magnitude, mask);
                    if (hit.collider != null){
                        curr_pt = hit.point;
                        made_contact = true;
                    }
                    // curr_pt += info.gravity;
                    hook_rb.MovePosition(curr_pt);
                }
            }
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
            rope_dist = 0.2f;

            create_rope_joints(100);
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
