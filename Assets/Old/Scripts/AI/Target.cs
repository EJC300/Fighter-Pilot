using UnityEngine;
using UnityEngine.UI;
public class Target : MonoBehaviour
{
    public Canvas cameraCanvas;
    public Transform player;
    public Image targetDiamondBox;
    public Image targetDiamondBoxInstance;
    public Image targetBox;
    public  Image targetBoxInstance;
    private bool targeted;
    
    private void Start()
    {
       cameraCanvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
       targetBoxInstance = Instantiate(targetBox);
       targetBoxInstance.transform.SetParent(cameraCanvas.transform);
       targetDiamondBoxInstance = Instantiate(targetDiamondBox);
       targetDiamondBoxInstance.transform.SetParent(cameraCanvas.transform);
       
    }

    public void Kill()
    {
        EntityTargeting.instance.RemoveFromTargets(this.transform);
    }
    public void SetTargeted(bool state)
    {
        targeted = state;
        
    }
    void UpdateTargetBoxLocation(RectTransform targetBox)
    {
        player = GameObject.FindWithTag("Player").transform;
        float distanceX = Mathf.Abs((player.position - transform.position).x + (player.position - transform.position).z);
        float distanceY = Mathf.Abs((player.position - transform.position).z + (player.position - transform.position).y);
        distanceY = Mathf.Clamp(distanceY, 50, 100);
        distanceX = Mathf.Clamp(distanceX, 50, 100);
        Vector3 pos = ((Camera.main.WorldToScreenPoint(Quaternion.identity * transform.position)));
        if (pos.x > Screen.width) pos.x = Screen.width - 50;
        if (pos.y > Screen.height) pos.y = Screen.height - 50;
        if (pos.x < 0) pos.x = 50;
        if (pos.y < 0) pos.y = 50;
        pos.z = 500;
        targetBox.position = pos;

        targetBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, distanceY);
        targetBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, distanceX);
    }

    private void Update()
    {
        EntityTargeting.instance.AddToTargets(transform);
        UpdateTargetBoxLocation(targetBoxInstance.rectTransform);
        targetDiamondBoxInstance.gameObject.SetActive(targeted);
        if (targeted)
        {
            UpdateTargetBoxLocation(targetDiamondBoxInstance.rectTransform);
       
           
                
            
        }
        

    }
}
