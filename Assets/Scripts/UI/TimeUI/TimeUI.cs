using UnityEngine;
using UnityEngine.UIElements;

namespace UI.TimeUI
{
    public class TimeUI : MonoBehaviour
    {
        public UIDocument timeUI;
        private ProgressBar progressBar;
        private Label timeLabel;
        private VisualElement fill;

        private void OnEnable()
        {
            var root = timeUI.rootVisualElement;
            timeLabel = root.Q<Label>("timeLabel");
            progressBar = root.Q<ProgressBar>("progressTime");
            fill = progressBar.Q<VisualElement>(className: "unity-progress-bar__progress");
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnTimeChanged += UpdateTime;
                TimeManager.Instance.OnProgressChanged += UpdateProgress;
                TimeManager.Instance.OnNightTriggered += HandleNightTriggered;
                TimeManager.Instance.OnTutorialComplete += ShowUI;
            }
            if(GameState.inTutorial || GameState.isInRoom)
            {
                root.style.display = DisplayStyle.None;
            }
        }

        private void OnDisable()
        {
            if (TimeManager.Instance == null) return;
            TimeManager.Instance.OnTimeChanged -= UpdateTime;
            TimeManager.Instance.OnProgressChanged -= UpdateProgress;
            TimeManager.Instance.OnNightTriggered -= HandleNightTriggered;
            TimeManager.Instance.OnTutorialComplete -= ShowUI;
        }

        public void ShowUI()
        {
            timeUI.rootVisualElement.style.display = DisplayStyle.Flex;
        }

        private void UpdateTime(float currentTime)
        {
            var seconds = Mathf.FloorToInt(currentTime);
            int mins = seconds / 60;
            int secs = seconds % 60;

            timeLabel.text = $"{mins:00}:{secs:00}";
        }

        private void UpdateProgress(float progress)
        {

            progressBar.value = progress;
        }

        private void HandleNightTriggered()
        {
            fill.style.backgroundColor = new Color(0.08f, 0.01f, 0.28f);
        }
    }
}
