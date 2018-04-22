using UnityEngine;

[ExecuteInEditMode]
public class UIAutoScaler : MonoBehaviour 
{
	public enum SizeScale {Both, OnlyX, OnlyY}
	public SizeScale sizeScale;
	public enum PositionScale {Both, OnlyX, OnlyY}
	public PositionScale positionScale;

	RectTransform rt;

	public int sizeX = 10;
	public int sizeY = 10;
	public Vector2 position;

    void Awake () { rt = GetComponent <RectTransform> (); }
    void Start () { UpdateConfiguration (); }

    void OnEnable () { UIScalerEvent.UIEvent += UpdateConfiguration; }
    void OnDisable () { UIScalerEvent.UIEvent -= UpdateConfiguration; }

    private Vector2 desiredSize;
	private Vector2 desiredPosition;

    void UpdateConfiguration ()
	{
		//
		// SIZE
		//
		desiredSize.x = sizeX * (Screen.width / 100f);
		desiredSize.y = sizeY * (Screen.height / 100f);
		if (sizeScale == SizeScale.OnlyX)
			desiredSize.y = desiredSize.x;
		if (sizeScale == SizeScale.OnlyY)
			desiredSize.x = desiredSize.y;
		//
		// POSITION
		//
		desiredPosition.x = position.x * (Screen.width / 100f);
		desiredPosition.y = position.y * (Screen.height / 100f);
		if (positionScale == PositionScale.OnlyX)
			desiredPosition.y = position.y * (Screen.width / 100f);
		if (positionScale == PositionScale.OnlyY)
			desiredPosition.x = position.x * (Screen.height / 100f);
		//
		// SET VALUES
		//
		rt.sizeDelta = desiredSize;
		rt.anchoredPosition = desiredPosition;
	}

    public void ChangePosition(Vector2 newPosition)
    {
        position = newPosition;
        rt.anchoredPosition = new Vector2(newPosition.x * (Screen.width / 100f), newPosition.y * (Screen.height / 100f));
    }
}
