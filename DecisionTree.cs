using System;
public class DecisionTree
{
	public float myDamageRatio1, enemyDamageRatio1,
		myDamageRatio2, enemyDamageRatio2, myNowHp, enemyNowHp,
		myAttackPoint, enemyAttackPoint, attackProbablity,
		evadeProbablity;
	private float attackExpect, evadeExpect, decisionRate;
	private float attackDecisionProbablity;
	public float deterWindowsHistory, warmWindowsHistory;
	//这个值用于
	public float evadeByDecisionTree(float myAngle, float enemyAngle, float speed,
		float deterWindows, float warmWindows)
	{
		deterWindows = deterWindows / deterWindowsHistory;
		warmWindows = warmWindows / warmWindowsHistory;
		float temp1 = (float)Math.Abs(Math.Cos(enemyAngle * Math.PI / 180));
		float temp = (1 - (deterWindows / warmWindows)) * temp1 *
			(2 *
			(Math.Abs(myAngle % 180) / 180) * (Math.Abs(myAngle % 180) / 180
			+ (Math.Abs(myAngle % 180) / 180)
			)) * (-50);
		int ran = new Random().Next(0, 100);
		return temp;//ran < temp;
	}
	public float attackProbablityByDecisionTree(int myHp, int enemyHp)
	{
		return attackExpect > evadeExpect ? attackDecisionProbablity : 1 -
			attackDecisionProbablity;
	}
	public void renewDecisionTree(float myNowHp, float enemyNowHp,
		float myAttackPoint = 1f, float enemyAttackPoint = 0f)
	{
		this.myNowHp = myNowHp;
		this.enemyNowHp = enemyNowHp;
		this.myAttackPoint = myAttackPoint;
		this.enemyAttackPoint = enemyAttackPoint;
	}
	public DecisionTree(float decisionRate, float attackDecisionProbablity = 0.7f)
	{
		this.decisionRate = decisionRate;
		this.attackDecisionProbablity = attackDecisionProbablity;
		attackProbablity = 0.7f;
		evadeProbablity = 1 - attackProbablity;

	}
	public void renewAttackProbablity(int ifSuccess)
	{
		attackProbablity += ifSuccess * decisionRate * attackProbablity;
		evadeProbablity += ifSuccess * decisionRate * evadeProbablity * (-1);
		attackProbablity /= (attackProbablity + evadeProbablity);
		evadeProbablity /= (attackProbablity + evadeProbablity);
	}
	/*
	 *这个类用于动态控制怪物ai行为，分为警惕窗口和威慑窗口
	 *在警惕窗口中，怪物以警惕为主，高概率逃跑，低概率进攻
	 *在威慑窗口中，怪物以威慑为主，高概率进攻，低概率逃跑
	 *威慑窗口持续扩张，直到与警惕窗口重合
	 *警惕窗口持续收缩，直到与威慑窗口重合
	 *当玩家入侵警惕窗口范围，威慑窗口“快扩张”（平常为慢扩张），
	 *表现为怪物表现出强烈的进攻欲望
	 *如果怪物被攻击到，威慑窗口的范围除以2，进入慢扩张，警惕窗口乘以2
	 *进入慢收缩，表现为怪物恐惧，躲闪的可能增强
	 *如果玩家进入威慑窗口范围，警惕窗口乘以2，慢收缩
	 *如果怪物成功攻击到玩家，威慑窗口“快扩张”，警惕窗口慢收缩
	 * 表现为怪物表现出强烈的进攻欲望
	 */
	public class ControlWindows
	{
		/*
		 *这个值用于衡量怪物ai活动中，行为收敛的速度，警惕窗口为负数，
		 *收缩式收敛，威慑窗口为正数，扩张式收敛
		 */
		public float windowsWidth;
		public float convergenceSlowPoint;
		//0.01~0.2
		public float convergenceQuickPoint;
		public bool flag;
		public float temp, dif;
		public void qucickConvergence()
		{
			temp = windowsWidth * (1 + convergenceQuickPoint);
			dif = temp - windowsWidth;
		}
		public void slowConvergence()
		{
			temp = windowsWidth + convergenceSlowPoint;
			dif = temp - windowsWidth;
		}
		public void Convergence()
		{
			if (temp - windowsWidth >= (dif / 10))
			{
				windowsWidth += dif / 4;
				return;
			}
			if (flag)
			{
				qucickConvergence();
			}
			else
			{
				slowConvergence();
			}
		}
		public ControlWindows(float windowsWidth,
			float convergenceSlowPoint,
			float convergenceQuickPoint,
			bool flag)
		{
			this.convergenceQuickPoint = convergenceQuickPoint;
			this.convergenceSlowPoint = convergenceSlowPoint;
			this.windowsWidth = windowsWidth;
			this.flag = flag;
		}
	}
}
