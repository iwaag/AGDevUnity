using AGDev;
using AGDevUnity;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
#if false
public class StdExprBehaver : MonoBNativeBehaver, ImmediateMultiGetter<IntPtr> {
	public override ImmediateMultiGetter<IntPtr> nativeBehaverGetter => this;
	public StdExprBehaverFactory factory {
		get {
			if (_factory == null) {
				_factory = new StdExprBehaverFactory();
			}
			return _factory;
		}
	}
	StdExprBehaverFactory _factory;
	public NativeTimeManager timeManager {
		get {
			if (_timeManager == null) {
				_timeManager = new NativeTimeManager(factory);
			}
			return _timeManager;
		}
	}
	NativeTimeManager _timeManager;
	void ImmediateMultiGetter<IntPtr>.GetElement(Taker<IntPtr> collector) {
		collector.Take(timeManager.implPtr);
	}
	private void Update() {
		timeManager.Tick(Time.deltaTime);
	}
}
public class NativeTimeManager {
	public NativeTimeManager(StdExprBehaverFactory factory) {
		implPtr = NewTimeManager(factory.implPtr);
	}
	public void Tick(float deltaTime) {
		LogDeltaTime(deltaTime);
	}
	readonly public System.IntPtr implPtr = System.IntPtr.Zero;
	[DllImport("AGDevStdBridge")]
	private extern static System.IntPtr NewTimeManager(System.IntPtr factory);
	[DllImport("AGDevStdBridge")]
	private extern static void LogDeltaTime(float deltaTime);
}
public class StdExprBehaverFactory
{
	public System.IntPtr implPtr;
	public StdExprBehaverFactory(){
		implPtr = NewStdExpressionBehaverFactory();
    }
	~StdExprBehaverFactory()
	{
		DeleteStdExpressionBehaverFactory(implPtr);
	}
	[DllImport("AGDevStdBridge")]
	private extern static System.IntPtr NewStdExpressionBehaverFactory();
	[DllImport("AGDevStdBridge")]
	private extern static void DeleteStdExpressionBehaverFactory(System.IntPtr factory);
}
#endif