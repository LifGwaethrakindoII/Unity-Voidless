namespace Voidless
{
/// \TODO Add 'On' Prefix into Interface's Callbacks. For more readability.
public interface IFiniteStateMachine<T>
{
	/// <summary>Current state.</summary>
	T state { get; set; }
	/// <summary>Previous State.</summary>
	T previousState { get; set; }

	/// <summary>Enters T State.</summary>
	/// <param name="_state">T State that will be entered.</param>
	void OnEnterState(T _state);

	/// <summary>Exits T State.</summary>
	/// <param name="_state">T State that will be left.</param>
	void OnExitState(T _state);
}
}