using System.Collections.Generic;
using System;
public class State_Attack_TypeA : IState_Attack
{
    Troll troll;
    public Dictionary<string, string> openTheAni { get; set; }
    public Dictionary<string, string> closeTheAni { get; set; }
    public string[] exitAniTag;
    string nowAnitag;
    int lastAniTagHash;
    int nowAnitagHash;
    string StateName;
    int attackPoint, maxAttackPoint;
    CollisionTrigger collideTrigger;
	bool startExit;
    public void Excute()
    {
        nowAnitagHash = troll.ani.GetCurrentAnimatorStateInfo(2).tagHash;
        if (nowAnitagHash == lastAniTagHash) return;
        troll.useSpecialEffect(2);
        nowAnitag = troll.getCurrentAniStateTag(2);
        if (nowAnitag == "Attack") { attack(); }
        if (openTheAni.ContainsKey(nowAnitag))
        {
            troll.animatorSwitch(openTheAni[nowAnitag],
                closeTheAni[nowAnitag]);
        }
		startChangeState();
		for (int i = 0; i < exitAniTag.Length; i++)
        {
            if (nowAnitag == exitAniTag[i]
                )
            {
				startExit = true;
            }
        }
		lastAniTagHash = nowAnitagHash;
    }
    void startChangeState()
	{
		if (startExit && !troll.angry())
		{
            troll.changeState(
		  StateName, nowAnitag);
		}
		else if (startExit)
		{
			troll.changeState(
		  StateName, "this");
		}
	}
    public void Enter()
    {
        startExit = false;
		troll.printState(StateName);
        attackPoint = new Random().Next(2, maxAttackPoint);
        if (!checkForAngle(attackPoint)) return;
        troll.ani.SetInteger("attackTypePoint", attackPoint);
        troll.ChangeWeight(2, 1);
        nowAnitagHash = troll.ani.GetCurrentAnimatorStateInfo(2).tagHash;
        nowAnitag = troll.getCurrentAniStateTag(2);
        troll.useSpecialEffect(2);
        nowAnitagHash = lastAniTagHash;
    }
    public void Exit()
    {
        nowAnitag = "";
        troll.ani.SetInteger("attackTypePoint", 0);
        if (collideTrigger != null)
        {
            collideTrigger.gameObject.SetActive(false);
        }
    }
    bool checkForAngle(int point)
    {
        for (int i = 0; i < troll.
            readAttackPointToAttackAngleDiactionary.Length; i++)
        {
            if (point <=
                troll.readAttackPointToAttackAngleDiactionary[i])
            {
                int temp = troll.attackPointToAttackAngleDiactionary[
                    troll.readAttackPointToAttackAngleDiactionary[i - 1]];
                if (!troll.aimAtOtherTroll(temp, false))
                {
                    troll.printState("JJJJJJJJ");
                    troll.forceToChangeState("evade");
                    nowAnitagHash = troll.ani.GetCurrentAnimatorStateInfo(2).tagHash;
                    nowAnitagHash = lastAniTagHash;
                    return false;
                }
            }
        }
        return true;
    }
    public void SetTroll(Troll troll)
    {
        this.troll = troll;
    }
    public void attack()
    {
        string stateName = troll.getCurrentAniStateName(2);
        if (troll.attackAniNameToTriggerDiactionary.
            ContainsKey(stateName))
        {
            collideTrigger = troll.collisionTriggers[
                troll.attackAniNameToTriggerDiactionary[stateName]];
            collideTrigger.gameObject.SetActive(true);
        }
        lock (troll.decisionTree)
        {
            troll.decisionTree.renewAttackProbablity(2);
        }
    }
    public State_Attack_TypeA(Dictionary<string, string> openTheAni,
     Dictionary<string, string> closeTheAni,
     string[] exitAniTag, int maxAttackPoint = 30)
    {
        StateName = "State_Attack_TypeA";
        this.openTheAni = openTheAni;
        this.closeTheAni = closeTheAni;
        this.exitAniTag = exitAniTag;
        this.maxAttackPoint = maxAttackPoint;
    }
}