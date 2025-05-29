using Cysharp.Threading.Tasks;

namespace AssetProvider.Scripts
{  
  public interface IAssetProvider
  { 
    UniTask<T> Load<T>(string address) where T : class;
    void Release(string address);
  }
}