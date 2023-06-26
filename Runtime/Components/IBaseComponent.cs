namespace Components
{
	public interface IBaseComponent<T>
	{
		T Owner { get; }
		void OnInitialized();
		void OnReady();
		void OnCleanUp();
		void OnUpdate(float deltaTime);
		void Init(T parent);
		void UpdateComponent(float deltaTime);
		void CleanUp();
		void Ready();
	}
}