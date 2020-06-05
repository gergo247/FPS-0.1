using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    RectTransform thrusterFuelFill;

    [SerializeField]
    GameObject scoreBoard;

    private PlayerController controller;
    public void SetController(PlayerController _controller)
    {
        controller = _controller;
    }
    void Update()
    {
        SetFuelAmount(controller.GetThrusterAmount());

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreBoard.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreBoard.SetActive(false);
        }
    }

    void SetFuelAmount(float _amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, _amount, 1f);
    }
}
