using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Troll : MonoBehaviour
{
    public Player player;
    public IState nowState, sleep, attack, move, die, evade;
    public Dictionary<string, int> allAniNameToEffectsNumberDiactionary;
    public Dictionary<string, InitTroll.EnhanceTheFunction> allAniNameToEnhanceTheFunctionDiactionary;
    public Dictionary<string, int> attackAniNameToTriggerDiactionary;
    public Dictionary<string, Dictionary<string, string>> exitAniTag;
    public int[] readAttackPointToAttackAngleDiactionary;
    public Dictionary<int, int> attackPointToAttackAngleDiactionary;
    public string[] aniStateTag;
    public string[] aniStateName_EffectOrAttack;
    public ParticleSystem[] specialEffects;
    public CollisionTrigger[] collisionTriggers;
    public bool IfAttack, IfIdle, Ifdie;
    public int hurt;
    //public BattleManager battleManager;
    public TrollAttribute trollAttribute;
    public Animator ani;
    public DecisionTree decisionTree;
    public DecisionTree.ControlWindows warmWindows, deterWindows;
    public Rigidbody rigidBody;
    event InitTroll.EnhanceTheFunction eventCome;
    float rotateSpeed, moveSpeed, backRotateSpeed, backMoveSpeed, runOrBackPoint;
    //function para
    //runOrBack
    Vector3 dir;
    float angle;
    //runMove
    Quaternion runMoveTempPara;
    //angry
    Vector3 angryTempPara;
    //forceToChangeState
    Vector3 tempV3;
    //decisionState
    System.Random random = new System.Random();
    //escape
    Vector3 escapeTemp;
    //scared
    public string afterScaredTag;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>

    void Start()
    {
        //collisionTrigger.is_trigger;
        initTroll();
    }

    void Update()
    {
        lock (nowState)
        {
            nowState.Excute();
        }
    }
    public void changeState(string stateName, string exitAniName)
    {
        lock (nowState)
        {
            nowState.Exit();
            if (scared() && makeDecisionToEvade())
            {
                switchState("evade");
            }
            else
            {
                switchState(exitAniTag
                [stateName][exitAniName]);
            }
            nowState.Enter();
        }
    }
    public void forceToChangeState(string stateName = "")
    {
        if (stateName == "evade")
        {
            StartCoroutine(forceToChangeStateAsync());
            return;
        }
        lock (nowState)
        {
            nowState.Exit();
            switchState(stateName);
            nowState.Enter();
        }
    }
    IEnumerator forceToChangeStateAsync()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (!ani.GetCurrentAnimatorStateInfo(3).
                IsTag("AfterEvade"))
            {
                lock (nowState)
                {
                    nowState.Exit();
                    switchState("evade");
                    nowState.Enter();
                }
                break;
            }
        }
        yield break;
    }
    bool makeDecisionToEvade()
    {
        dir = player.transform.position - transform.position;
        dir.y = 0f;
        angle = Mathf.Acos(Vector3.Dot(transform.forward.
            normalized, dir.normalized)) * Mathf.Rad2Deg;
        dir = transform.position - player.transform.position;
        dir.y = 0f;
        float enemyAngle = Mathf.Acos(Vector3.Dot(player.transform.forward.
            normalized, dir.normalized)) * Mathf.Rad2Deg;
        //player.Speed
        int ran = random.Next(1, 100);
        print(decisionTree.evadeByDecisionTree(
            angle, enemyAngle, 1, deterWindows.windowsWidth,
            warmWindows.windowsWidth) + " : " + ran);
        if (ran < decisionTree.evadeByDecisionTree(
              angle, enemyAngle, 1, deterWindows.windowsWidth,
              warmWindows.windowsWidth))
        {
            tempV3 = transform.position +
            warmWindows.windowsWidth * transform.forward * (-1);
            return true;
        }
        return false;
    }
    public void animatorSwitch(string setTruePara, string setFalsePara = "")
    {
        if (setTruePara != "") ani.SetBool(setTruePara, true);
        if (setFalsePara != "") ani.SetBool(setFalsePara, false);
    }
    //if( Quaternion.LookatRotation(player.transform.position-transform.position),
    //  rotateSpeed*Time.deltatime);)
    public void attackIt()
    {
        //battleManager.sendInfo(info);
    }
    public void runMove()
    {
        runMoveTempPara = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(player.transform.position - transform.position),
            backRotateSpeed * Time.deltaTime
        );
        runMoveTempPara.x = 0f;
        runMoveTempPara.Normalize();
        rigidBody.MoveRotation(runMoveTempPara);
        rigidBody.MovePosition(transform.position +
            Time.deltaTime * transform.forward * moveSpeed);
    }
    public void backMove_aimAt()
    {
        runMoveTempPara = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(player.transform.position - transform.position),
            backRotateSpeed * Time.deltaTime
        );
        runMoveTempPara.x = 0f;
        runMoveTempPara.Normalize();
        rigidBody.MovePosition(transform.position +
            Time.deltaTime * transform.forward * backMoveSpeed * (-1));
        rigidBody.MoveRotation(runMoveTempPara);

    }
    public float[] escapeAimAt()
    {
        Vector3[] aimAts = new Vector3[7];
        aimAts[0] = transform.forward * (-1);
        aimAts[1] = Vector3.Slerp(transform.forward * (-1), transform.right, 0.5f).normalized;
        aimAts[2] = Vector3.Slerp(transform.forward * (-1), transform.right * (-1), 0.5f).normalized;
        aimAts[3] = transform.right;
        aimAts[4] = transform.right * (-1);
        aimAts[5] = Vector3.Slerp(transform.forward, transform.right, 0.5f).normalized;
        aimAts[6] = Vector3.Slerp(transform.forward, transform.right * (-1), 0.5f).normalized;
        for (int i = 0; i < aimAts.Length; i++)
        {
            if (
            Physics.Raycast(transform.position,
                aimAts[i], warmWindows.windowsWidth))
            {
                return new float[] { aimAts[i].x, aimAts[i].y, aimAts[i].z };
            }
        }
        return new float[] { aimAts[0].x, aimAts[0].y, aimAts[0].z };
    }
    public void escape(float[] aimAt)
    {
        runMoveTempPara = Quaternion.Slerp(
           transform.rotation,
           Quaternion.LookRotation(player.transform.position - transform.position),
           backRotateSpeed * Time.deltaTime
        );
        runMoveTempPara.x = 0f;
        runMoveTempPara.Normalize();
        rigidBody.MoveRotation(runMoveTempPara);
        rigidBody.MovePosition(this.transform.position + Time.deltaTime *
            new Vector3(aimAt[0], aimAt[1], aimAt[2]) *
            backMoveSpeed * (-1));
        //if (escapeTemp.magnitude > warmWindows.windowsWidth)
        //{
        //    return true;
        //}
        //return false;
    }
    public bool aimAtOtherTroll(float aimAtiPoint= 1f,bool runOrBack=true)
    {
        if (runOrBack) aimAtiPoint = runOrBackPoint;
        dir = player.transform.position - transform.position;
        dir.y = 0f;
        angle = Mathf.Acos(Vector3.Dot(transform.forward.
            normalized, dir.normalized)) * Mathf.Rad2Deg;
        //print(angle);
        if (angle > aimAtiPoint)
        {
            return false;
        }
        return true;
    }
    public bool angry()
    {
        angryTempPara = transform.position - player.transform.position;
        angryTempPara.y = 0;
        if (angryTempPara.magnitude < deterWindows.windowsWidth)
        {
            return true;
        }
        return false;
    }
    public bool scared()
    {
        if (!ani.GetCurrentAnimatorStateInfo(3).
            IsTag("AfterEvade")&&
            !ani.GetBool("ifEvade"))
        {
            return true;
        }
        return false;
    }
    void initTroll()
    {
        ani = this.GetComponent<Animator>();
        rigidBody = this.GetComponent<Rigidbody>();
        trollAttribute = this.GetComponent<TrollAttribute>();
        rotateSpeed = trollAttribute.rotateSpeed;
        moveSpeed = trollAttribute.moveSpeed;
        backRotateSpeed = trollAttribute.backRotateSpeed;
        backMoveSpeed = trollAttribute.backMoveSpeed;
        runOrBackPoint = trollAttribute.runOrBackPoint;
        InitTroll.TypeA.initTroll(this);
        decisionTree.deterWindowsHistory = deterWindows.windowsWidth;
        decisionTree.warmWindowsHistory = warmWindows.windowsWidth;
    }
    void switchState(string stateName)
    {
        switch (stateName)
        {
            case "attack":
                nowState = attack;
                break;
            case "sleep":
                nowState = sleep;
                break;
            case "move":
                nowState = move;
                break;
            case "die":
                nowState = die;
                break;
            case "evade":
                nowState = evade;
                break;
            default:
                break;
        }
    }
    public string getCurrentAniStateTag(int layer)
    {
        for (int i = 0; i < aniStateTag.Length; i++)
        {
            if (ani.GetCurrentAnimatorStateInfo(layer).IsTag(
                aniStateTag[i]))
            {
                return aniStateTag[i];
            }
        }
        return "";
    }
    public string getCurrentAniStateName(int layer)
    {
        for (int i = 0; i < aniStateName_EffectOrAttack.Length; i++)
        {
            if (ani.GetCurrentAnimatorStateInfo(layer).IsName(
                aniStateName_EffectOrAttack[i]))
            {
                return aniStateName_EffectOrAttack[i];
            }
        }
        return "";
    }
    public void useSpecialEffect(int layer, string stateName = "")
    {
        ParticleSystem temp;
        if (stateName == "") stateName = getCurrentAniStateName(layer);
        if (allAniNameToEffectsNumberDiactionary.ContainsKey
            (stateName))
        {
            temp = specialEffects[
                allAniNameToEffectsNumberDiactionary[stateName]];
            if (!temp.isPlaying)
            {
                temp.Play();
            }
        }
        if (allAniNameToEnhanceTheFunctionDiactionary.ContainsKey
            (stateName))
        {
            print("Enhance Start");
            eventCome = allAniNameToEnhanceTheFunctionDiactionary[stateName];
            eventCome(this);
        }
    }
    public void printState(string name)
    {
        print(name + " : " + Time.time);
    }
    public void ChangeWeight(int upLayer, int lowLayer)
    {
        if (ani.GetLayerWeight(upLayer) > 0.9f) return;
        StartCoroutine(startChangeWeight(upLayer, lowLayer));
    }
    IEnumerator startChangeWeight(int upLayer, int lowLayer)
    {
        float changeWeight = 0;
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            changeWeight = Mathf.Lerp(changeWeight, 1, 0.25f);
            ani.SetLayerWeight(upLayer, changeWeight);
            ani.SetLayerWeight(lowLayer, 1 - changeWeight);
            if (1 - changeWeight < 0.05f && 1 - changeWeight > -0.05f)
            {
                ani.SetLayerWeight(upLayer, 1);
                ani.SetLayerWeight(lowLayer, 0);
                break;
            }
        }
        yield break;
    }
    public IEnumerator renewDecisionTree()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);
            if (warmWindows.windowsWidth > deterWindows.windowsWidth)
            {
                warmWindows.Convergence();
                deterWindows.Convergence();
                if (false)
                {
                    break;
                }
            }
        }
        yield break;
    }
    public void testBeAttacked()
    {

    }
}
