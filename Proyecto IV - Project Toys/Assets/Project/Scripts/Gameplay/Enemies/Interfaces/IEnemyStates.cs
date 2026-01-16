using UnityEngine;

public interface IEnemyStates
{
    #region Idle
    public void Idle_Enter();
    public void Idle_Update();
    public void Idle_Exit();
    #endregion
    #region Move
    public void Move_Enter();
    public void Move_Update();
    public void Move_Exit();
    #endregion
    #region Pursuit
    public void Pursuit_Enter();
    public void Pursuit_Update();
    public void Pursuit_Exit();
    #endregion
    #region Attack
    public void Attack_Enter();
    public void Attack_Update();
    public void Attack_Exit();
    #endregion
    #region WaitAttack
    public void WaitAttack_Enter();
    public void WaitAttack_Update();
    public void WaitAttack_Exit();
    #endregion
    #region Dead
    public void Dead_Enter();
    public void Dead_Update();
    public void Dead_Exit();
    #endregion
    #region Flinch
    public void Flinch_Enter();
    public void Flinch_Update();
    public void Flinch_Exit();
    #endregion
    #region Execution
    public void Execution_Enter();
    public void Execution_Update();
    public void Execution_Exit();
    #endregion
}
