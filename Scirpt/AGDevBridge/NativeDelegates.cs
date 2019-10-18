using System.Runtime.InteropServices;

namespace AGDev.Native {
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void SignalCallback();
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void FloatCallback(float value);
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void SingleWordCallback(System.IntPtr text, int length);
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void SingleWordBehaviorCallback(System.IntPtr text, int length, System.IntPtr behaviorCheckListener);
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void SinglePtrCallback(System.IntPtr ptr);
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate int SinglePtrReturnIntCallback(System.IntPtr ptr);
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void TwoPtrCallback(System.IntPtr ptr1, System.IntPtr ptr2);
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void ThreePtrCallback(System.IntPtr ptr1, System.IntPtr ptr2, System.IntPtr ptr3);

}
