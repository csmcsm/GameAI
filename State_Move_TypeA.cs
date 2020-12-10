using System.Collections.Generic;
public class State_Move_TypeA : IState_Move
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
        //
        aimAt();
        //
        nowAnitagHash = troll.ani.GetCurrentAnimatorStateInfo(1).tagHash;
        if (nowAnitagHash == lastAniTagHash) return;
        troll.useSpecialEffect(1);
        nowAnitag = troll.getCurrentAniStateTag(1);
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
        aimAt();
        troll.printState(StateName);
        troll.ChangeWeight(1, 2);
        nowAnitagHash = troll.ani.GetCurrentAnimatorStateInfo(1).tagHash;
        troll.useSpecialEffect(1);
        nowAnitag = troll.getCurrentAniStateTag(1);
        nowAnitagHash = lastAniTagHash;
    }
    public void Exit()
    {
        nowAnitag = "";
        troll.ani.SetBool("ifRun", false);
        troll.ani.SetBool("ifBackMove", false);
    }
    public void SetTroll(Troll troll)
    {
        this.troll = troll;
    }
    public void run()
    {
        if (nowAnitag == "Run")
        {
            troll.runMove();
        }
        else if (nowAnitag == "BackMove")
        {
            troll.backMove_aimAt();
        }
    }
    public void aimAt()
    {
        if (!troll.angry())
        {
            if (troll.aimAtOtherTroll())
            {
                if (nowAnitag != "Run")
                {
                    troll.animatorSwitch("ifRun", "ifBackMove");
                }
            }
            else
            {
                if (nowAnitag != "BackMove")
                {
                    troll.animatorSwitch("ifBackMove", "ifRun");
                }
            }
        }
        else
        {
            troll.ani.SetBool("ifRun", false);
            troll.ani.SetBool("ifIdle", false);
            return;
        }
        run();
    }
    public State_Move_TypeA(Dictionary<string, string> openTheAni,
     Dictionary<string, string> closeTheAni,
     string[] exitAniTag)
    {
        StateName = "State_Move_TypeA";
        this.openTheAni = openTheAni;
        this.closeTheAni = closeTheAni;
        this.exitAniTag = exitAniTag;
    }
}