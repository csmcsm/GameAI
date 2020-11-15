public interface IState_Move : IState
{
    void Enter();
    void Excute();
    void Exit();
    void SetTroll(Troll troll);
    void run();
    void aimAt();
}