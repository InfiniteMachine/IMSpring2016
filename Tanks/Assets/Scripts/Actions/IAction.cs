public interface IAction {
    bool CanFire();
    bool IsAttack();
    void StartAction();
}
