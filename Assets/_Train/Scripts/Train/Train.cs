using _Train.Scripts.Train.Root;
using UnityEngine;

public class Train : MonoBehaviour
{
    private IBrakingDown[] brakingDowns;

    private void Start()
    {
        brakingDowns = GetComponentsInChildren<IBrakingDown>();
    }

    public void TakeDamage(float damage)
    {
        foreach (var brakingDown in brakingDowns)
        {
            brakingDown.Break(damage / 100);
        }
    }
}
