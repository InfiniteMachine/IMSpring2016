public interface IAction {
    bool CanFire();
    bool IsAttack();
    void StartAction();
    void AllowFire();
    void ForceDeactivate();
    void ResetCounters();
    float GetPercentage();
}
