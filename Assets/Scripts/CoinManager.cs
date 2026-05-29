using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    private int coins = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddCoin(int amount = 1)
    {
        coins += amount;

        if (UIManager.Instance != null)
            UIManager.Instance.SetCoins(coins);
    }

    public bool SpendCoins(int amount)
    {
        if (coins < amount)
            return false;

        coins -= amount;

        if (UIManager.Instance != null)
            UIManager.Instance.SetCoins(coins);

        return true;
    }

    public int GetCoins()
    {
        return coins;
    }

    // днаюбэ щрн
    public void SetCoins(int amount)
    {
        coins = amount;

        if (UIManager.Instance != null)
            UIManager.Instance.SetCoins(coins);
    }
}