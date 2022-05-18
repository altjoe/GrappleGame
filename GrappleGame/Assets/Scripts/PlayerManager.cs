using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public GameObject ball_prefab;
    public GameObject rope_prefab;
    public GameObject hook_prefab;
    
    Player player;
    Information info;
    List<GameObject> rope_pool = new List<GameObject>();
    void Start() {
        fill_rope_pool(rope_prefab);

        info = new Information(ball_prefab, rope_prefab, hook_prefab);
        player = new Player(this.transform.localPosition, info);
    }

    private void FixedUpdate() {
        player.move();
    }

    class Player {
        public Verlet player_movement;
        public Verlet hook_movement;
        public Rigidbody2D player_rb;
        public Rigidbody2D hook_rb;

        public Information info;    
        public bool shot = false;
        public bool hook_hit = false;
        Rope rope;  

        public Player(Vector2 start_pos, Information info){
            this.info = info;
            player_movement = new Verlet(start_pos);
            player_rb = Instantiate(info.ball_prefab, player_movement.curr_pos, Quaternion.identity).GetComponent<Rigidbody2D>() as Rigidbody2D;
            player_movement.add_force(new Vector2(0.03f, 0.03f));
            shoot_hook(new Vector2(1f, 1f));
        }

        public void move(){
            player_rb.MovePosition(player_movement.move_exponential(info.gravity));
            if (shot) {
                if (!hook_hit){
                    hook_hit = hook_movement.move_collision(info.gravity);
                    hook_rb.MovePosition(hook_movement.curr_pos);
                }
            }
        }

        public void shoot_hook(Vector2 direction){
            hook_movement = new Verlet(player_movement.curr_pos, direction, info.hook_speed);
            hook_rb = Instantiate(info.hook_prefab, hook_movement.curr_pos, Quaternion.identity).GetComponent<Rigidbody2D>() as Rigidbody2D;
            shot = true;
        }
    }
    
    class Rope {
        public float rope_segment_length = 0.2f;
        public List<Verlet> rope_movement = new List<Verlet>();

        public Rope(ref List<GameObject> rope_pool, GameObject rope_prefab) {
            int i = 0;
            while (rope_pool[i].activeSelf) {
                rope_pool[i].SetActive(false);
                i += 1;
            }
        }

        
    }

    class Verlet {
        public Vector2 prev_pos;
        public Vector2 curr_pos;

        public Verlet(Vector2 start_pos) {
            curr_pos = start_pos;
            prev_pos = start_pos;
        }

        public Verlet(Vector2 start_pos, Vector2 movement_dir, float force){
            curr_pos = start_pos;
            prev_pos = curr_pos - (movement_dir.normalized * force);
        }

        public Vector2 move_linear() {
            Vector2 diff = curr_pos - prev_pos;
            prev_pos = curr_pos;
            curr_pos += diff;
            return curr_pos;
        }

        public bool move_collision(Vector2 force) {
            Vector2 diff = curr_pos - prev_pos;
            RaycastHit2D hit = Physics2D.Raycast(curr_pos + diff + force, curr_pos, LayerMask.NameToLayer("Collision"));
            if (hit) {
                prev_pos = curr_pos;
                curr_pos = hit.point;
                return true;
            } else {
                prev_pos = curr_pos;
                curr_pos += diff + force;
                return false;
            }
        }

        public Vector2 move_exponential(Vector2 force){
            Vector2 diff = curr_pos - prev_pos;
            prev_pos = curr_pos;
            curr_pos += diff + force;
            return curr_pos;
        }

        public void add_force(Vector2 force) {
            prev_pos -= force;
        }
    }

    class Information {
        public GameObject ball_prefab;
        public GameObject rope_prefab;
        public GameObject hook_prefab;
        public Vector2 gravity = new Vector2(0f, -0.001f);
        public List<GameObject> rope_pool = new List<GameObject>();
        public float hook_speed = 0.5f;

        public Information(GameObject ball_prefab, GameObject rope_prefab, GameObject hook_prefab) {
            this.ball_prefab = ball_prefab;
            this.rope_prefab = rope_prefab;
            this.hook_prefab = hook_prefab;
        }
    }

    void fill_rope_pool(GameObject rope_prefab){
        for (int i = 0; i < 200; i++) {
            GameObject rope = Instantiate(rope_prefab, Vector2.zero, Quaternion.identity);
            rope.SetActive(false);
            rope_pool.Add(rope);
        }
    }


}
