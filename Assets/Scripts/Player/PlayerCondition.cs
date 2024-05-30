using System;
using System.Collections;
using UnityEngine;

public interface IDamagable
{
    void TakePysicalDamage(int damage);
}

public class PlayerCondition : MonoBehaviour, IDamagable
{
    [HideInInspector] public UICondition uiCondition;

    private Condition Health => uiCondition.health;
    private Condition Hunger => uiCondition.hunger;
    private Condition Stamina => uiCondition.stamina;

    public float noHungerHealthDelay;

    public event Action onTakeDamage;

    // Update is called once per frame
    void Update()
    {
        //Hunger는 감소하고 Stamina는 증가하는 패시브임
        Hunger.Subtract(Hunger.passiveValue * Time.deltaTime); 
        Stamina.Add(Stamina.passiveValue * Time.deltaTime);

        //Hunger가 0보다 작아지면 Health를 깎을 것임
        if(Hunger.curValue == 0f)
        {
            Health.Subtract(noHungerHealthDelay * Time.deltaTime);
        }
        //Health가 0이면 죽음
        if(Health.curValue == 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        Health.Add(amount);
    }

    public void Eat(float amount)
    {
        Hunger.Add(amount);
    }

    public void Boost(float amount)
    {
        StartCoroutine(BoostCoroutine(amount));
    }

    private IEnumerator BoostCoroutine(float amount)
    {
        float orginSpeed = CharacterManager.Instance.Player.controller.moveSpeed;
        CharacterManager.Instance.Player.controller.moveSpeed = amount * orginSpeed;
        yield return new WaitForSeconds(10f);
        CharacterManager.Instance.Player.controller.moveSpeed = orginSpeed;
    }

    private void Die()
    {
        
    }

    //인터페이스를 상속받아 구현
    public void TakePysicalDamage(int damage)
    {
        Health.Subtract(damage);
        onTakeDamage?.Invoke();
    }

    public bool UseStamina(float amount)
    {
        if(Stamina.curValue - amount < 0f)
        {
            return false;
        }

        Stamina.Subtract(amount);
        return true;
        //이 bool 값을 어디서 받냐? 장비 사용하는 쪽에서 UseStamina를 불러쓰겠지 당연히, EquipTool로 간다.
    }
}
