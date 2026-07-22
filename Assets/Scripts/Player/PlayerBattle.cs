using UnityEngine;
using UnityEngine.UI;
using System.Collections;
[System.Serializable]
public struct AttackRange
{
    public Vector2 offset, size;
    public bool drawGizmos;
}

public class PlayerBattle : MonoBehaviour
{
    public EntityHealth health;
    public PlayerMovement movement;
    [SerializeField] DamageIndicator indicator;
    public EntityStat stat;
    public float atkCool;
    
    public AttackRange defaultAttack;
    [SerializeField] LayerMask enemyMask;
    [SerializeField] float dashPower, dashTime;
    public bool inDash;
    [SerializeField] Slider healthbar;
    void Start()
    {
        health = GetComponent<EntityHealth>();
        stat = GetComponent<EntityStat>();
        movement = GetComponent<PlayerMovement>();
        health.OnDamage(OnHurt);
    }
    void OnHurt(EntityHealth.Context ctx)
    {
        if(inDash)
            ctx.canceled = true;
        if(ctx.canceled)
        {
            return;
        }
        indicator.IndicateDamage(ctx.damage,transform.position+new Vector3(0, 1), Color.red);
    }
    void Update()
    {
        healthbar.value = health.health / health.maxHealth;
        if(atkCool > 0)
        {
            atkCool -= Time.deltaTime * (1 + stat.GetResultValue("atkSpeed") / 100);
        }
    }
    public void Dash(int direction)
    {
        StartCoroutine(dash_(direction));
    }
    IEnumerator dash_(int direction)
    {
        movement.SetVelocity(Vector2.right * direction * dashPower);

        yield return new WaitForSeconds(dashTime);
        movement.SetVelocity(Vector2.zero);
        inDash = false;
    }
    public void Attack()
    {
        if (atkCool > 0)
            return;
        atkCool = 0.5f;

        var col = Physics2D.OverlapBoxAll((Vector2)transform.position + defaultAttack.offset, defaultAttack.size, 0,enemyMask);
        foreach (var target in col)
        {
            EntityHealth hp = target.GetComponent<EntityHealth>();
            if(hp != null)
            {
                hp.GetDamage(stat.GetResultValue("attackDamage"), health);
            }
        }
    }
    public void Skill1()
    {
        StartCoroutine(skill1());
    }
    IEnumerator skill1()
    {
        var atkBuf = new EntityStat.Buf
        {
            Key = "attackDamage",
            mathType = MathType.Increase,
            Value = 60,
        };
        var atkspeedBuf = new EntityStat.Buf
        {
            Key = "atkSpeed",
            mathType = MathType.Increase,
            Value = 50,
        };
        stat.bufs.Add(atkBuf);
        stat.bufs.Add(atkspeedBuf);
        stat.Calc("attackDamage");
        stat.Calc("atkSpeed");

        yield return new WaitForSeconds(5);

        stat.Calc("attackDamage");
        stat.Calc("atkSpeed");
        stat.bufs.Remove(atkBuf);
        stat.bufs.Remove(atkspeedBuf);
    }
    void Draw(AttackRange range)
    {
        if (!range.drawGizmos)
            return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + range.offset,  range.size);
    }
    void OnDrawGizmos()
    {
        Draw(defaultAttack);
    }
}
