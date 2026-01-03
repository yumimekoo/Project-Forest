using System.Collections.Generic;
using System.Linq;
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
    private bool isSinleRecipeView = false;

    List<DrinkRuleSO> bookRecipes = new();
    List<DrinkRuleSO> filteredRecipes = new();

    private bool isBookOpen = false;
    private VisualElement
        root,
        recipesContainer,
        ordersContainer;
    private Button
        closeButton,
        prevPageButton,
        nextPageButton;
    private Label
        pageNumberLabel;
    private TextField
        searchField;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
        OrderManager.Instance.OnOrderChanged += RefreshOrders;
    }
    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
        if (OrderManager.Instance != null)
            OrderManager.Instance.OnOrderChanged -= RefreshOrders;
    }
    private void Awake()
    {
        bookInteract = InputSystem.actions.FindAction("BookInteract");
        root = recipeBookUI.rootVisualElement;
        closeButton = root.Q<Button>("exitButton");
        prevPageButton = root.Q<Button>("prevPage");
        nextPageButton = root.Q<Button>("nextPage");
        recipesContainer = root.Q<VisualElement>("recipesContainer");
        ordersContainer = root.Q<VisualElement>("ordersContainer");
        pageNumberLabel = root.Q<Label>("pageLabel");
        searchField = root.Q<TextField>("searchField");

        prevPageButton.clicked += PrevPage;
        nextPageButton.clicked += NextPage;
        closeButton.clicked += Close;
        searchField.RegisterValueChangedCallback(evt =>
        {
            ApplySearch(evt.newValue);
        });

        LoadRecipes();
        RefreshRecipesPage();
        Close();
    }

    private void LoadRecipes()
    {
        bookRecipes = UnlockManager.Instance.runtimeDatabase.GetUnlockedRecipes()
            .FindAll(r => r.ruleType == RuleType.FinalDrink || r.ruleType == RuleType.SpecialUnlock)
            .OrderBy(r => r.ruleType)
            .ThenBy(r => r.resultingState.itemName)
            .ToList();

        currentPage = Mathf.Clamp(currentPage, 0, Mathf.Max(0, (bookRecipes.Count - 1) / recipesPerPage));
    }

    private void Update()
    {
        if (bookInteract.WasPressedThisFrame() && !GameState.isInConversation)
        {
            if(isBookOpen)
                Close();
            else
                Open();
        }
    }

    private void ApplySearch(string search)
    {
        if(string.IsNullOrWhiteSpace(search))
        {
            filteredRecipes = new List<DrinkRuleSO>(bookRecipes);
        }
        else
        {
            filteredRecipes = bookRecipes.FindAll(r =>
                r.resultingState.itemName.ToLower().Contains(search.ToLower())
            );
        }   

        currentPage = 0;
        RefreshRecipesPageFiltered();
    }

    void RefreshOrders()
    {
        ordersContainer.Clear();

        foreach (var pair in OrderManager.Instance.ActiveOrders)
        {
            NPCIdentitySO npc = pair.Key;
            string npcName = npc.npcName;
            ItemDataSO drink = pair.Value;
            string drinkName = drink.itemName;
            // hier dann template machen
            var label = new Label(
                npcName + " -> " + drinkName // {npc.npcName} -> {drink.itemName}
            );

            label.RegisterCallback<ClickEvent>(_ =>
            {
                ShowSingleRecipe(drink);
                isSinleRecipeView = true;
            });

            ordersContainer.Add(label);
        }
    }

    void ShowSingleRecipe(ItemDataSO drink)
    {
        recipesContainer.Clear();
        currentPage = 0;

        var rule = FindRuleFromResult(drink);
        if(rule == null)
            return;
        BuildRecipeItem(rule);
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

    private void RefreshRecipesPageFiltered()
    {
        recipesContainer.Clear();
        
        int startIndex = currentPage * recipesPerPage;
        int end = Mathf.Min(startIndex + recipesPerPage, filteredRecipes.Count);

        for (int i = startIndex; i < end; i++)
        {
            BuildRecipeItem(filteredRecipes[i]);
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
        if(isSinleRecipeView)
        {
            isSinleRecipeView = false;
            RefreshRecipesPage();
            return;
        }

        if ((currentPage + 1) * recipesPerPage >= bookRecipes.Count)
            return;

        currentPage++;
        RefreshRecipesPage();
    }

    private void PrevPage()
    {
        if(isSinleRecipeView)
        {
            isSinleRecipeView = false;
            RefreshRecipesPage();
            return;
        }

        if (currentPage == 0)
            return;

        currentPage--;
        RefreshRecipesPage();
    }

    private void Open()
    {
        if(GameState.inTutorial && TutorialManager.Instance != null)
        {
            if (TutorialManager.Instance.currentStep != TutorialStep.OpenRecipeBook)
                return;
            TutorialManager.Instance.OnRecipeBookOpened();
        }

        LoadRecipes();
        RefreshRecipesPage();
        RefreshOrders();
        isBookOpen = true;
        root.style.display = DisplayStyle.Flex;
        GameTime.SetPaused(true);
        // Pause the game or disable player controls if necessary
    }

    private void Close()
    {
        isBookOpen = false;
        root.style.display = DisplayStyle.None;
        GameTime.SetPaused(false);

        if (GameState.inTutorial && TutorialManager.Instance != null)
        {
            if (TutorialManager.Instance.currentStep != TutorialStep.CloseRecipeBook)
                return;
            TutorialManager.Instance.OnRecipeBookClosed();
        }
    }
}
