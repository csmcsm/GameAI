using System.Collections.Generic;
public class State_Sleep_TypeA : IState_Sleep
{
    Troll troll;
    public Dictionary<string, string> openTheAni { get; set; }
    public Dictionary<string, string> closeTheAni { get; set; }
    public string[] exitAniTag;
    string nowAnitag;
    int lastAniTagHash;
    int nowAnitagHash;
    string StateName;
    public void Excute()
    {
        nowAnitagHash = troll.ani.GetCurrentAnimatorStateInfo(1).tagHash;
        if (nowAnitagHash == lastAniTagHash) return;
        troll.useSpecialEffect(1);
        lastAniTagHash = nowAnitagHash;
        nowAnitag = troll.getCurrentAniStateTag(1);
        if (openTheAni.ContainsKey(nowAnitag))
        {
            troll.animatorSwitch(openTheAni[nowAnitag],
                closeTheAni[nowAnitag]);
        }
        for (int i = 0; i < exitAniTag.Length; i++)
        {
            if (nowAnitag == exitAniTag[i])
            {
                troll.changeState(
                    StateName, nowAnitag);
                return;
            }
        }

    }
    public void Enter()
    {
        troll.ChangeWeight(1, 2);
        troll.ani.SetBool("ifIdle", true);
        troll.printState(StateName);
        nowAnitagHash = troll.ani.GetCurrentAnimatorStateInfo(1).tagHash;
        troll.useSpecialEffect(1);
        nowAnitag = troll.getCurrentAniStateTag(1);
        nowAnitagHash = lastAniTagHash;
    }
    public void Exit()
    {
        nowAnitag = "";
        wakeUp();
    }
    public void SetTroll(Troll troll)
    {
        this.troll = troll;
    }
    public void wakeUp()
    {
        troll.StartCoroutine(troll.renewDecisionTree());
        troll.ani.SetBool("ifIdle", false);
    }
    public State_Sleep_TypeA(Dictionary<string, string> openTheAni,
     Dictionary<string, string> closeTheAni,
     string[] exitAniTag)
    {
        StateName = "State_Sleep_TypeA";
        this.openTheAni = openTheAni;
        this.closeTheAni = closeTheAni;
        this.exitAniTag = exitAniTag;
    }
}