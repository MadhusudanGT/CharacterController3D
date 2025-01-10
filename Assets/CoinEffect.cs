using System.Threading.Tasks;
using UnityEngine;

public class CoinEffect : MonoBehaviour
{
    private ParticleSystem particleSystem;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }
    public void PlayEffect()
    {
        if (particleSystem == null)
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        particleSystem?.Play();
        StopEffect();
    }
    async Task StopEffect()
    {
        await Task.Delay(1000);
        particleSystem?.Stop();
        PoolManagerGen poolManager = ManagerRegistry.Get<PoolManagerGen>();
        poolManager.ReturnObject<CoinEffect>(PoolManagerKeys.COIN_EFFECT, this);
    }


}
