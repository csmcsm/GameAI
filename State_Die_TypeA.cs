using System.Collections.Generic;
public class State_Die_TypeA : IState_Die
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
        nowAnitagHash = troll.ani.GetCurrentAnimatorStateInfo(4).tagHash;
        if (nowAnitagHash == lastAniTagHash) return;
        troll.useSpecialEffect(4);
        nowAnitag = troll.getCurrentAniStateTag(4);
        if (openTheAni.ContainsKey(nowAnitag))
        {
            troll.animatorSwitch(openTheAni[nowAnitag],
                closeTheAni[nowAnitag]);
        }
        for (int i = 0; i < exitAniTag.Length; i++)
        {
            if (nowAnitag == exitAniTag[i]) troll.changeState(
                    StateName, nowAnitag);
        }
        lastAniTagHash = nowAnitagHash;
    }
    public void Enter()
    {
        troll.ChangeWeight(4, 1);
        troll.printState(StateName);
        nowAnitagHash = troll.ani.GetCurrentAnimatorStateInfo(4).tagHash;
        troll.useSpecialEffect(4);
        nowAnitag = troll.getCurrentAniStateTag(1);
        nowAnitagHash = lastAniTagHash;
    }
    public void Exit()
    {

    }
    public void die()
    {
    }
    public void SetTroll(Troll troll)
    {
        this.troll = troll;
    }
    public State_Die_TypeA(Dictionary<string, string> openTheAni,
     Dictionary<string, string> closeTheAni,
     string[] exitAniTag)
    {
        StateName = "State_Die_TypeA";
        this.openTheAni = openTheAni;
        this.closeTheAni = closeTheAni;
        this.exitAniTag = exitAniTag;
    }
}