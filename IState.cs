public interface IState
{
    void Enter();
    void Excute();
    void Exit();
    void SetTroll(Troll troll);
}