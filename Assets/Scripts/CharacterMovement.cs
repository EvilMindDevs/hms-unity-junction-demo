using UnityEngine;
using UnityEngine.UI;
using HmsPlugin;

public class CharacterMovement : MonoBehaviour
{
    public int Speed = 1;
    public Text coin;
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * Speed);
    }

    public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D character");
        if (collision.tag == "Coin") 
        {
            gameManager.Coin += 1;
            collision.gameObject.SetActive(false);
            coin.text = "1";
            HMSAchievementsManager.Instance.Reach(HMSAchievementConstants.FirstCoin); //DO not use Unclock method for this logic. Getting 7203 error Achiement already unclocked.
        }
        else if(collision.tag == "Finish") 
        {
            Speed = 0;
            gameManager.Finish();
        }
        
    }
}
