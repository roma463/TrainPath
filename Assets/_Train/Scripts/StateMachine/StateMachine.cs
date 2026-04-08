using _Train.Scripts.Root;
using UnityEngine;

public abstract class StateMachine<T> : MonoBehaviour, IFixedUpdater where T : Context
{
    [field: SerializeField] public T Context { get; private set; }

    private IState _currentState;
    protected bool _isInitializeble;
    
    public IState CurrentState => _currentState;

    public virtual void Initialize()
    {
        _isInitializeble = true;
    }

    public void ChangeState(IState nextState)
    {
        _currentState?.OnExitState();
        _currentState = nextState;
        _currentState.OnEnterState();
    }

    private void Update()
    {
        if (_isInitializeble)
            ExecuteState();
    }
    
    public void FixedExecuteState()
    {
        _currentState.FixedExecute();
    }

    public void ExecuteState()
    {
        _currentState.Execute();
    }

    private void OnDestroy()
    {
        _currentState?.OnExitState();
    }

    public void FUpdate()
    {
        if (_isInitializeble)
            FixedExecuteState();
    }
}
