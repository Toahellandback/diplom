using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIShopManager : MonoBehaviour
{
    public static UIShopManager Instance;

    [Header("Shop Panel")]
    [SerializeField] private GameObject shopPanel;

    [Header("Damage Upgrade")]
    [SerializeField] private Button damageUpgradeButton;
    [SerializeField] private TextMeshProUGUI damageButtonText;
    [SerializeField] private TextMeshProUGUI damageLevelText;

    [Header("Health Upgrade")]
    [SerializeField] private Button healthUpgradeButton;
    [SerializeField] private TextMeshProUGUI healthButtonText;
    [SerializeField] private TextMeshProUGUI healthLevelText;

    [Header("Info")]
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private Button closeButton;

    private UpgradeShop currentShop;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (shopPanel != null) shopPanel.SetActive(false);
    }

    public void ShowShop(UpgradeShop shop)
    {
        currentShop = shop;
        shopPanel.SetActive(true);
        RefreshUI();

        damageUpgradeButton.onClick.RemoveAllListeners();
        damageUpgradeButton.onClick.AddListener(OnDamageUpgrade);

        healthUpgradeButton.onClick.RemoveAllListeners();
        healthUpgradeButton.onClick.AddListener(OnHealthUpgrade);

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(OnClose);
    }

    public void HideShop()
    {
        shopPanel.SetActive(false);
        currentShop = null;
    }

    private void RefreshUI()
    {
        if (currentShop == null) return;

        int coins = CoinManager.Instance.GetCoins();
        coinsText.text = "Coins: " + coins;

        // Damage
        int dLevel = currentShop.GetDamageLevel();
        int dMax = currentShop.GetMaxLevel();
        int dCost = currentShop.GetDamageCost();
        damageLevelText.text = "Level: " + dLevel + "/" + dMax;
        damageButtonText.text = dLevel >= dMax ? "MAX" : "+" + " (" + dCost + " coins)";
        damageUpgradeButton.interactable = dLevel < dMax && coins >= dCost;

        // Health
        int hLevel = currentShop.GetHealthLevel();
        int hCost = currentShop.GetHealthCost();
        healthLevelText.text = "Level: " + hLevel + "/" + dMax;
        healthButtonText.text = hLevel >= dMax ? "MAX" : "+" + " (" + hCost + " coins)";
        healthUpgradeButton.interactable = hLevel < dMax && coins >= hCost;
    }

    private void OnDamageUpgrade()
    {
        if (currentShop != null && currentShop.UpgradeDamage())
            RefreshUI();
    }

    private void OnHealthUpgrade()
    {
        if (currentShop != null && currentShop.UpgradeHealth())
            RefreshUI();
    }

    private void OnClose()
    {
        if (currentShop != null)
            currentShop.CloseShop();
    }
}