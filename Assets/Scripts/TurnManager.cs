using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public PlayerController player;
    public EnemyAI enemy;

    private enum Turn { Player, Enemy } // enum for turns 
    private Turn currentTurn = Turn.Player;

    void Start()
    {
        player.SetTurnManager(this);
        enemy.SetTurnManager(this);
    }

    public bool IsPlayerTurn()
    {
        return currentTurn == Turn.Player;
    }

    public void EndPlayerTurn()
    {
        currentTurn = Turn.Enemy;
        enemy.TakeTurn();
    }

    public void EndEnemyTurn()
    {
        currentTurn = Turn.Player;
    }
}
