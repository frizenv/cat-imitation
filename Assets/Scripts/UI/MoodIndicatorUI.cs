using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CatImitation.UI
{
    public class MoodIndicatorUI : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _moodText;

        private CatGraph _catGraph;
        private const float k_duration = 2f;

        public void Initialize(CatGraph graph, Mood mood)
        {
            _catGraph = graph;
            float progress = DetermineProgress(mood);
            _slider.value = progress;
            _fillImage.color = Color.red * (1 - progress) + Color.green * progress;
            _moodText.SetText(mood.Name);
        }

        public void SetMood(Mood mood)
        {
            float progress = DetermineProgress(mood);
            SetProgress(progress, mood);
        }

        private float DetermineProgress(Mood mood)
        {
            int max = _catGraph.Moods.Count - 1;
            int current = _catGraph.Moods.Select(wrapper => wrapper.Mood).ToList().IndexOf(mood);
            float progress = (float)current / max;
            return progress;
        }

        private void SetProgress(float progress, Mood mood)
        {
            progress = Mathf.Clamp01(progress);
            _slider.DOValue(progress, k_duration).SetEase(Ease.InOutCirc).onComplete += () => _moodText.SetText(mood.Name);
            _fillImage.DOColor(Color.red * (1 - progress) + Color.green * progress, k_duration).SetEase(Ease.OutElastic);
        }
    }
}