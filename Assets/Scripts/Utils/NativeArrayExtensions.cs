using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public static class NativeArrayExtensions
{
    public static NativeArray<T> CreateNativeArrayAlias<T>(this T[] array) where T : unmanaged
    {
        unsafe
        {
            fixed (T* ptr = array)
            {
                var nativeArrayAlias = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(ptr, array.Length, Allocator.None);
#if UNITY_EDITOR
                var safetyHandle = AtomicSafetyHandle.Create();
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref nativeArrayAlias, safetyHandle);
                AtomicSafetyHandleManager.Submit(safetyHandle);
#endif
                return nativeArrayAlias;
            }
        }
    }
}
