using System.Threading.Tasks;
using AssetProvider.Scripts;
using UnityEngine;

namespace _Project.Scripts.Core_Logic.Common
{
  public abstract class GameObjectFactory
  {
    private readonly IAssetProvider m_assetProvider;

    protected GameObjectFactory(IAssetProvider assetProvider) => 
      m_assetProvider = assetProvider;
    
    protected async Task<GameObject> CreateAsync(string prefabPath, Transform parent = null)
    {
      var prefab = await m_assetProvider.Load<GameObject>(prefabPath);
      var obj = Object.Instantiate(prefab, parent);
      return obj;
    }

    protected void Release(string address) =>
      m_assetProvider.Release(address);

    protected void Clear(GameObject obj)
    {
      if (obj != null)
        Object.Destroy(obj);
    }
  }
}