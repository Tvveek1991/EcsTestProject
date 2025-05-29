using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AssetProvider.Scripts
{
  public sealed class AssetProvider : IAssetProvider
  {
    private readonly Dictionary<string, AsyncOperationHandle> _completeCache = new();
    private readonly Dictionary<string, List<AsyncOperationHandle>> _handles = new();
    
    public async UniTask<T> Load<T>(string address) where T : class
    {
      if (_completeCache.TryGetValue(address, out AsyncOperationHandle completedHandle))
        return completedHandle.Result as T;

      return await RunWithCacheOnComplete(
        Addressables.LoadAssetAsync<T>(address),
        address);
    }

    public void Release(string address)
    {
      if (!_handles.ContainsKey(address))
        return;

      foreach (AsyncOperationHandle handle in _handles[address])
        Addressables.Release(handle);

      if (_handles.ContainsKey(address))
        _handles.Remove(address);

      if (_completeCache.ContainsKey(address))
        _completeCache.Remove(address);
    }

    private void AddHandle<T>(string key, AsyncOperationHandle<T> handle) where T : class
    {
      if (!_handles.TryGetValue(key, out List<AsyncOperationHandle> resourceHandles))
      {
        resourceHandles = new List<AsyncOperationHandle>();
        _handles[key] = resourceHandles;
      }

      resourceHandles.Add(handle);
    }
    
    private async UniTask<T> RunWithCacheOnComplete<T>(AsyncOperationHandle<T> handle, string cacheKey) where T : class
    {
      handle.Completed += completeHandle => _completeCache[cacheKey] = completeHandle;

      AddHandle(cacheKey, handle);

      return await handle.Task;
    }
  }
}