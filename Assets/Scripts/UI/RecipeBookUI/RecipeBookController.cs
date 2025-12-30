using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class RecipeBookController : MonoBehaviour
{
    public UIDocument recipeBookUI;
    public VisualTreeAsset recipeFinalTemplate;
    public VisualTreeAsset recipeStepTemplate;
    public InputActionAsset InputActions;
    public InputAction bookInteract;

    private int currentPage = 0;
    const int recipesPerPage = 3;

    // testing only
    List<DrinkRuleSO> bookRecipes = new();

    private bool isBookOpen = false;
    private VisualElement
        root,
        recipesContainer;
    private Button
        closeButton,
        prevPageButton,
        nextPageButton;
    private Label
        pageNumberLabel;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }
    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }
    private void Awake()
    {
        bookInteract = InputSystem.actions.FindAction("BookInteract");
        root = recipeBookUI.rootVisualElement;
        closeButton = root.Q<Button>("exitButton");
        prevPageButton = root.Q<Button>("prevPage");
        nextPageButton = root.Q<Button>("nextPage");
        recipesContainer = root.Q<VisualElement>("recipesContainer");
        pageNumberLabel = root.Q<Label>("pageLabel");

        prevPageButton.clicked += PrevPage;
        nextPageButton.clicked += NextPage;
        closeButton.clicked += Close;

        LoadRecipes();
        RefreshRecipesPage();
        Close();
    }

    private void LoadRecipes()
    {
        bookRecipes = UnlockManager.Instance.runtimeDatabase.GetUnlockedRecipes()
            .FindAll(r => r.ruleType == RuleType.FinalDrink || r.ruleType == RuleType.SpecialUnlock);

        currentPage = Mathf.Clamp(currentPage, 0, Mathf.Max(0, (bookRecipes.Count - 1) / recipesPerPage));
    }

    private void Update()
    {
        if (bookInteract.WasPressedThisFrame() && GameState.playerInteractionAllowed)
        {
            if(isBookOpen)
                Close();
            else
                Open();
        }
    }

    private void RefreshRecipesPage()
    {
        recipesContainer.Clear();

        if (bookRecipes.Count == 0)
            return;

        int startIndex = currentPage * recipesPerPage;
        int end = Mathf.Min(startIndex + recipesPerPage, bookRecipes.Count);

        for (int i = startIndex; i < end; i++)
        {
            BuildRecipeItem(bookRecipes[i]);
        }

        pageNumberLabel.text = $"Page {currentPage + 1}";
    }

    private void BuildRecipeItem(DrinkRuleSO recipe)
    {
        var recipeItem = recipeFinalTemplate.Instantiate();

        var headerLabel = recipeItem.Q<Label>("recipeHeaderTitle");
        var stepContainer = recipeItem.Q<VisualElement>("recipeStepContainer");

        stepContainer.Clear();

        headerLabel.text = recipe.resultingState.itemName;

        BuildStepsRecursive(recipe.resultingState, stepContainer);

        recipesContainer.Add(recipeItem);
    }

    private void BuildStepsRecursive(ItemDataSO result, VisualElement stepContainer)
    {
        var rule = FindRuleFromResult(result);
        if(rule == null) 
            return;

        BuildStepsRecursive(rule.requiredState, stepContainer);
        BuildStepsRecursive(rule.addedIngredient, stepContainer);

        AddStep(rule, stepContainer);
    }

    private void AddStep(DrinkRuleSO rule, VisualElement stepContainer)
    {
        var step = recipeStepTemplate.Instantiate();
        step.Q<Label>("left").text = rule.requiredState.itemName;
        step.Q<Label>("right").text = rule.addedIngredient.itemName;
        step.Q<Label>("result").text = rule.resultingState.itemName;

        stepContainer.Add(step);
    }

    private DrinkRuleSO FindRuleFromResult(ItemDataSO result)
    {
        return UnlockManager.Instance.runtimeDatabase.GetUnlockedRecipes().Find(r => r.resultingState == result);
    }

    private void NextPage()
    {
        if ((currentPage + 1) * recipesPerPage >= bookRecipes.Count)
            return;

        currentPage++;
        RefreshRecipesPage();
    }

    private void PrevPage()
    {
        if (currentPage == 0)
            return;

        currentPage--;
        RefreshRecipesPage();
    }

    private void Open()
    {
        LoadRecipes();
        RefreshRecipesPage();
        isBookOpen = true;
        root.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
        // Pause the game or disable player controls if necessary
    }

    private void Close()
    {
        isBookOpen = false;
        root.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
        // Resume the game or enable player controls if necessary
    }
}
