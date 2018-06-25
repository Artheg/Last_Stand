using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
	
    [SerializeField]
	private Image background;

	void Update () 
	{
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }

    public void UpdateHealth(float ratio)
	{
		background.fillAmount = ratio;
	}
}
