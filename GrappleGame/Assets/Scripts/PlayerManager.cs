using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public GameObject ball_prefab;
    public GameObject rope_prefab;
    public GameObject hook_prefab;

    Information info;
    Player player;
    
    void Start() {
        info = new Information(ball_prefab, rope_prefab, hook_prefab, this.transform.localPosition);
        player = new Player(info);
    }

    void Update() {
        if (player.active_hook == false) {
            Vector2 shoot_dir = new Vector2(1, 1);
            player.shoot_hook(shoot_dir.normalized);
        }
    }

    void FixedUpdate() {
        player.move();
    }

    public class Player {
        Information info;
        List<Rigidbody2D> ball_rope_hook_rb = new List<Rigidbody2D>();
        List<Vector2> ball_rope_hook_prev_pos = new List<Vector2>(); 
        Rigidbody2D hook_obj;
        Vector2 prev_hook_pos;
        float shoot_speed = 1;
        public bool active_hook = false;
        public bool hook_hit = false;
        Vector2 shoot_drection;

        public Player(Information i) {
            info = i;
            GameObject ball_obj = Instantiate(info.ball_prefab, i.start_loc, Quaternion.identity);
            Rigidbody2D ball_rb = ball_obj.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
            ball_rope_hook_rb.Add(ball_rb);
            ball_rope_hook_prev_pos.Add(ball_rb.position);

            ball_add_force(new Vector2(0.025f, 0.025f));
        }

        public void move() {
            for (int i = 0; i < ball_rope_hook_rb.Count; i++) {
                Vector2 diff = ball_rope_hook_rb[i].position - ball_rope_hook_prev_pos[i];
                ball_rope_hook_prev_pos[i] = ball_rope_hook_rb[i].position;
                if (i == ball_rope_hook_rb.Count - 1) {
                    RaycastHit2D hit = Physics2D.Raycast(ball_rope_hook_rb[i].position, ball_rope_hook_prev_pos[i], diff.magnitude, LayerMask.GetMask("Collision"));
                    if (hit.collider != null) {
                        ball_rope_hook_rb[i].MovePosition(hit.point);
                        create_rope();
                        hook_hit = true;
                    } 
                    else {
                        ball_rope_hook_rb[i].MovePosition(ball_rope_hook_prev_pos[i] + diff + info.gravity);
                        create_rope();
                    }
                    
                } else {
                    ball_rope_hook_rb[i].MovePosition(ball_rope_hook_prev_pos[i] + diff + info.gravity);
                }
                
            }
            if (hook_hit) {
                constraints();
            }
        }

        public void create_rope() {
            if (!hook_hit){
                Vector2 hook_pos = ball_rope_hook_rb[ball_rope_hook_rb.Count - 1].position;
                Vector2 newest_rope_pos = ball_rope_hook_rb[ball_rope_hook_rb.Count - 2].position;
                Vector2 diff = hook_pos - newest_rope_pos;
                while (diff.magnitude > info.rope_length){
                    newest_rope_pos = newest_rope_pos + (diff.normalized * info.rope_length);
                    info.rope_parts_obj[info.active_rope_segments].transform.localPosition = newest_rope_pos;
                    info.rope_parts_obj[info.active_rope_segments].SetActive(true); 
                    ball_rope_hook_rb.Insert(ball_rope_hook_rb.Count - 1, info.rope_parts_rb[info.active_rope_segments]);

                    info.active_rope_segments += 1;
                    diff = hook_pos - newest_rope_pos;
                    
                }
            }
        }

        public void constraints() {

        }

        public void ball_add_force(Vector2 force){
            if (ball_rope_hook_prev_pos.Count > 0){
                ball_rope_hook_prev_pos[0] -= force;
            } 
        }

        public void shoot_hook(Vector2 direction){
            hook_obj = Instantiate(info.hook_prefab, ball_rope_hook_rb[0].position, Quaternion.identity);
            Rigidbody2D hook_rb = hook_obj.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
            ball_rope_hook_prev_pos.Add(hook_rb.position - (direction * shoot_speed));
            ball_rope_hook_rb.Add(hook_rb);
            active_hook = true;
            shoot_drection = direction.normalized;
        }
    }

    

    public class Information {
        public GameObject ball_prefab;
        public GameObject rope_prefab;
        public GameObject hook_prefab;
        public Vector2 start_loc;
        public List<GameObject> rope_parts_obj = new List<GameObject>();
        public List<Rigidbody2D> rope_parts_rb = new List<Rigidbody2D>();
        public Vector2 gravity = new Vector2(0f, -0.001f);
        public float rope_length = 0.3f;

        public int active_rope_segments = 0;

        public Information(GameObject b, GameObject r, GameObject h, Vector2 start_loc){
            ball_prefab = b;
            rope_prefab = r;
            hook_prefab = h;
            this.start_loc = start_loc;
            generate_rope();
        }

        public void generate_rope(){
            for (int i = 0; i < 100; i++){
                GameObject rope_part = Instantiate(rope_prefab, Vector2.zero, Quaternion.identity);
                rope_part.SetActive(false);
                Rigidbody2D rb = rope_part.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
                rope_parts_obj.Add(rope_part);
                rope_parts_rb.Add(rb);
            }
        }
    }
}
