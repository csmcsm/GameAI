
public interface IState_Attack : IState
{
    void Enter();
    void Excute();
    void Exit();
    void SetTroll(Troll troll);
    void attack();
}