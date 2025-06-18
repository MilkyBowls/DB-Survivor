using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerXP : MonoBehaviour
{
    public int currentXP = 0;
    public int level = 1;
    public int xpToNextLevel = 100;

    [Header("UI")]
    public Slider xpSlider;
    public TextMeshProUGUI xpText;

    void Start()
    {
        UpdateXPUI();
    }

    public void GainXP(int amount)
    {
        currentXP += amount;

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            level++;
            xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.25f);
        }

        UpdateXPUI();
    }

    private void UpdateXPUI()
    {
        if (xpSlider != null)
        {
            xpSlider.maxValue = xpToNextLevel;
            xpSlider.value = currentXP;
        }

        if (xpText != null)
        {
            xpText.text = $"Level {level} | XP: {currentXP} / {xpToNextLevel}";
        }
    }
}
