public interface IState
{
    void Enter(); // execute when it enters the machine

    void Execute(); // execute when it is in the machine

    void Exit(); // execute when it exits the machine
}
