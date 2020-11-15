public interface IState_Sleep : IState
{
    void Enter();
    void Excute();
    void Exit();
    void SetTroll(Troll troll);
    void wakeUp();
}