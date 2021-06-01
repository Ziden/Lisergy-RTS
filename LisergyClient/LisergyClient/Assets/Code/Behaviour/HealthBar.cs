
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float speed = 1f;

    private Slider _slider;
    private int targetPct = 100;

    // Start is called before the first frame update
    void Start()
    {
        _slider = GetComponent<Slider>();
    }

    public void SetPct(int pct)
    {
        targetPct = pct;
    }

    // Update is called once per frame
    void Update()
    {
        var pct = _slider.value;
        var diff = pct - targetPct;
        if (Mathf.Abs(diff) < 0.1)
            return;

        if (pct > targetPct)
            _slider.value -= speed * Time.deltaTime;
        else if (pct < targetPct)
            _slider.value += speed * Time.deltaTime;
    }
}
