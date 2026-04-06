using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressOverlayManager : Singleton<ProgressOverlayManager>
{
    [SerializeField] private GameObject overlayPanel;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private Slider progressBar;

    private int totalSteps = 0;
    private int currentStep = 0;

    public void ShowProgress(string title, int total)
    {
        totalSteps = total;
        currentStep = 0;

        overlayPanel.SetActive(true);
        UpdateUI(title);
    }

    public void IncrementProgress(string title)
    {
        currentStep++;
        UpdateUI(title);
    }

    private void UpdateUI(string title)
    {
        if (totalSteps > 0)
        {
            progressText.text = $"{title}\n({currentStep} / {totalSteps})";
            progressBar.value = (float)currentStep / totalSteps;
        }
        else
        {
            progressText.text = $"{title}";
            progressBar.value = 0f;
        }
    }

    public void HideProgress()
    {
        overlayPanel.SetActive(false);
    }
}
