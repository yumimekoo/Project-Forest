using UnityEngine;

[CreateAssetMenu(fileName = "DrinkRuleSO", menuName = "Recipe/DrinkRule")]
public class DrinkRuleSO : ScriptableObject
{
    public string ruleName;
    public ItemDataSO requiredState;
    public ItemDataSO addedIngredient;
    public ItemDataSO resultingState;
}
