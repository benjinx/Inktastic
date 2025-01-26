using UnityEngine;

public class WinControl : MonoBehaviour
{
    public void EndGame(bool _win)
    {
        GameplayStates.EndGame(_win);
    }
}
