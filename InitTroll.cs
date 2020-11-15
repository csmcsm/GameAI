using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/*
 * 请注意，请按照附送图片对应的动画变量和标签名进行修改
 * 在动画的attack层中，有多条动画路径，请修改其使用条件
 */

public class InitTroll : MonoBehaviour
{
    public delegate void EnhanceTheFunction(Troll troll);
    public event EnhanceTheFunction eventHappened;
    public class TypeA
    {
        public static void initTroll(Troll troll)
        {
            /*
             *在这里，我们将动画中添加到的所有标签写入(state中的tag属性)
             * 还有，需要添加Trigger的攻击动画的名称呼也要写入
             */
            troll.aniStateTag = new string[] { "Sleep", "Idle", "Run" , "BackMove"
            ,"BeforeAttack","Attack","AfterAttack","Die","AfterDie","Evade",
                "AfterRun" ,"AfterEvade"};
            troll.aniStateName_EffectOrAttack = new string[] { "Sleep" };
            /*
             *在这里，我们初始化所有的状态机，并添加好退出状态机所对应的的动画（当播放到这几个动画时，我们退出状态机）
             *dic1,dic2,list只是临时变量，无须在意
             *在dic1中，我们写入需要打开的变量，第一个string是需要修改动画变量的当前state的tag，第二个是需要修改为true
             *的动画变量名,写完后，我们必须在dic2中同样添加一个相同的需要修改动画变量的当前state的tag，但第二个是需要修
             *改为false的动画变量名
             *list中为退出状态机所对应的的动画的tag名（当播放到这几个动画时，我们退出状态机）
             */
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            string[] list = new string[5];
            //攻击状态机初始化
            list = new string[] { "AfterAttack" };
            troll.attack = new State_Attack_TypeA(dic1, dic2, list, 100);
            troll.attack.SetTroll(troll);
            //睡眠状态机初始化
            list = new string[] { "Idle" };
            troll.sleep = new State_Sleep_TypeA(dic1, dic2, list);
            troll.sleep.SetTroll(troll);
            //移动状态机初始化（包括追击和后移调整方向）
            list = new string[] { "AfterRun" };
            troll.move = new State_Move_TypeA(dic1, dic2, new string[] { "AfterRun" });
            troll.move.SetTroll(troll);
            //紧急退避状态机初始化
            list = new string[] { "AfterEvade" };
            troll.evade = new State_Evade_TypeA(dic1, dic2, list);
            troll.evade.SetTroll(troll);
            //死亡状态机初始化
            troll.die = new State_Die_TypeA(dic1, dic2, list);
            troll.die.SetTroll(troll);
            //ai实体当前状态机初始化
            troll.nowState = troll.sleep;
            /*
             * 在这里，我们对ai体的决策树进行初始化
             *第一个值控制决策树的适应速度，值越大，速度越快，怪物越聪明，但不能大于1
             *第二个值反映决策完成后，决策的执行力度，0是做出相反的决定，1是彻底的按照决策执行，最好控制在0.6到0.8之间
             */
            troll.decisionTree = new DecisionTree(0.1f, 0.7f);
            troll.decisionTree.renewDecisionTree(1,
                troll.trollAttribute.Hp);
            troll.warmWindows = new DecisionTree.ControlWindows(27f, 0.2f, 0.2f, false);
            troll.deterWindows = new DecisionTree.ControlWindows(12f, 0.2f, 0.2f, true);
            /*
             *下面这个用于控制ai体退出当前状态，“new Dictionary<string, Dictionary<string, string>>()”中，第一个string
             *指当前的状态，在前面我们已经设置过了，如：State_Attack_TypeA，第二个值是退出时，动画播放到的state的tag属性，
             *第三个指需要转移的状态，分别有：sleep,attack,move,die,evade
             *temp是临时变量无须在意
             */
            troll.exitAniTag = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> temp = new Dictionary<string, string>();
            //睡眠状态机状态转移初始化
            temp.Add("Idle", "move");
            troll.exitAniTag.Add("State_Sleep_TypeA", temp);
            //移动状态机状态转移初始化（包括追击和后移调整方向）
            Dictionary<string, string> temp1 = new Dictionary<string, string>();
            temp1.Add("AfterRun", "attack");
            troll.exitAniTag.Add("State_Move_TypeA", temp1);
            //攻击状态机状态转移初始化
            temp = new Dictionary<string, string>();
            temp.Add("AfterAttack", "move");
            temp.Add("this", "attack");
            troll.exitAniTag.Add("State_Attack_TypeA", temp);
            //紧急闪避状态机初始化
            temp = new Dictionary<string, string>();
            temp.Add("AfterAttack", "move");
            troll.exitAniTag.Add("State_Evade_TypeA", temp);
            //这里，我们开始组装各种特效和攻击的Trigger
            //
            //allAniNameToEffectsNumberDiactionary记录动画的名字和效果的索引号(在Unity中)
            //attackAniNameToTriggerDiactionary记录攻击动画的名称和它们对应的Trigger
            Dictionary<string, int> tempTrigger = new Dictionary<string, int>();
            //tempTrigger.Add("AttackGT1", 0);
            Dictionary<string, int> tempEffect = new Dictionary<string, int>();
            //tempEffect.Add("AttackGT1", 0);
            //tempEffect.Add("Run", 1);
            troll.allAniNameToEffectsNumberDiactionary = tempEffect;
            troll.attackAniNameToTriggerDiactionary = tempTrigger;
            //在这里，我们规定攻击能够触及的角度
            //readAttackPointToAttackAngleDiactionary
            //用于声明Animator中的attackPoint范围
            //attackPointToAttackAngleDiactionary用于规定相对应的角度
            troll.readAttackPointToAttackAngleDiactionary = new int[] { 0,30,60,9999 };
            Dictionary<int, int> tempAttack = new Dictionary<int, int>();
            tempAttack.Add(0,35);
            tempAttack.Add(30, 60);
            tempAttack.Add(60, 25);
            troll.attackPointToAttackAngleDiactionary = tempAttack;
            //在这里，我们增强方法
            EnhanceTheFunction enhanceTheFunction = new EnhanceTheFunction(test);
            Dictionary<string, EnhanceTheFunction> tempEnhance = new Dictionary<string, EnhanceTheFunction>();
            tempEnhance.Add("Sleep", enhanceTheFunction);
            troll.allAniNameToEnhanceTheFunctionDiactionary = tempEnhance;

            //完成所有工作后，打开状态机
            troll.nowState.Enter();
        }
        public static void test(Troll troll)
        {
            troll.rigidBody.AddForce(troll.transform.up*1000);
            troll.printState("staeteujkkdasf");
        }

    }
}
