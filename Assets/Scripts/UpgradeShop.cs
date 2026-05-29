using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeShop : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private GameObject interactHint; // "Press F" подсказка

    [Header("Upgrades")]
    [SerializeField] private int damageUpgradeCost = 10;
    [SerializeField] private int damageUpgradeAmount = 1;
    [SerializeField] private int healthUpgradeCost = 15;
    [SerializeField] private int healthUpgradeAmount = 2;
    [SerializeField] private int maxUpgradeLevel = 5;

    private Transform player;
    private HeroKnight heroKnight;

    private bool isOpen = false;
    private bool playerNearby = false;

    private int damageLevel = 0;
    private int healthLevel = 0;

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            heroKnight = playerObj.GetComponent<HeroKnight>();
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }

        if (interactHint != null)
            interactHint.SetActive(false);
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        playerNearby = dist <= interactDistance;

        if (interactHint != null)
            interactHint.SetActive(playerNearby && !isOpen);

        if (playerNearby && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (!isOpen) OpenShop();
            else CloseShop();
        }

        if (isOpen && Keyboard.current.escapeKey.wasPressedThisFrame)
            CloseShop();
    }

    private void OpenShop()
    {
        isOpen = true;
        Time.timeScale = 0f;
        if (UIShopManager.Instance != null)
            UIShopManager.Instance.ShowShop(this);
    }

    public void CloseShop()
    {
        isOpen = false;
        Time.timeScale = 1f;
        if (UIShopManager.Instance != null)
            UIShopManager.Instance.HideShop();
    }

    public bool UpgradeDamage()
    {
        if (damageLevel >= maxUpgradeLevel)
        {
            Debug.Log("Max damage level!");
            return false;
        }
        if (CoinManager.Instance.GetCoins() < damageUpgradeCost)
        {
            Debug.Log("Not enough coins!");
            return false;
        }

        CoinManager.Instance.SpendCoins(damageUpgradeCost);
        damageLevel++;
        heroKnight.UpgradeDamage(damageUpgradeAmount);
        return true;
    }

    public bool UpgradeHealth()
    {
        if (healthLevel >= maxUpgradeLevel)
        {
            Debug.Log("Max health level!");
            return false;
        }
        if (CoinManager.Instance.GetCoins() < healthUpgradeCost)
        {
            Debug.Log("Not enough coins!");
            return false;
        }

        CoinManager.Instance.SpendCoins(healthUpgradeCost);
        healthLevel++;
        playerHealth.UpgradeMaxHealth(healthUpgradeAmount);
        return true;
    }

    public int GetDamageLevel() => damageLevel;
    public int GetHealthLevel() => healthLevel;
    public int GetMaxLevel() => maxUpgradeLevel;
    public int GetDamageCost() => damageUpgradeCost;
    public int GetHealthCost() => healthUpgradeCost;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}