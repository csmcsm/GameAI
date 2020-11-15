public interface IState_Die : IState
{
    void Enter();
    void Excute();
    void Exit();
    void SetTroll(Troll troll);
    void die();
}