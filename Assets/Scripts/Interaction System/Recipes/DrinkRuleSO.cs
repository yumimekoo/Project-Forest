using UnityEngine;

[CreateAssetMenu(fileName = "DrinkRuleSO", menuName = "Scriptable Objects/DrinkRule")]
public class DrinkRuleSO : ScriptableObject
{
    public int id;
    public string ruleName;
    public RuleType ruleType;
    public ItemDataSO requiredState;
    public ItemDataSO addedIngredient;
    public ItemDataSO resultingState;
    public bool putMachine;
}
