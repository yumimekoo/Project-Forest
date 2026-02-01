using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class RecipeBookController : MonoBehaviour
{
    public UIDocument recipeBookUI;
    public VisualTreeAsset recipeFinalTemplate;
    public VisualTreeAsset recipeStepTemplate;
    public VisualTreeAsset recipeTemplate5;
    public VisualTreeAsset recipeTemplate7;
    public VisualTreeAsset orderTemplate;
    public InputActionAsset InputActions;
    public InputAction bookInteract;
    
    [Header("Sounds")]
    public SoundSO bookOpenSound;
    public SoundSO bookCloseSound;
    public SoundSO buttonHoverSound;
    public SoundSO buttonClickSound;

    private int currentPage = 0;
    const int recipesPerPage = 2;
    private List<List<DrinkRuleSO>> pages = new();
    private bool isSinleRecipeView = false;

    private List<DrinkRuleSO> bookRecipes = new();
    private List<DrinkRuleSO> filteredRecipes = new();

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
    
    private const int ID_HOTWATER = 1000;
    private const int ID_FOAM     = 1001;
    private const int ID_POWDER   = 1002;
    private const int ID_COFFEE   = 114;

    private static readonly Dictionary<int, string> MachineActionTextById = new()
    {
        { ID_HOTWATER, "Put in Kettle" },
        { ID_COFFEE,   "Put in Coffee Machine" },
        { ID_POWDER,   "Put in Grinder" },
        { ID_FOAM,     "Put in Frother" },
    };

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
        if (OrderManager.Instance)
            OrderManager.Instance.OnOrderChanged += RefreshOrders;
        if(!UIManager.Instance) Debug.LogError("UIManager not found!");
        UIManager.Instance.OnUIStateChanged += HandleState;
        UIManager.Instance.OnEscapePressed += CloseESC;
        UIManager.Instance.OnRecipeBookPressed += HandleInput;

    }
    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
        if (OrderManager.Instance)
            OrderManager.Instance.OnOrderChanged -= RefreshOrders;
        if(!UIManager.Instance) Debug.LogError("UIManager not found!");
        UIManager.Instance.OnUIStateChanged -= HandleState;
        UIManager.Instance.OnEscapePressed -= CloseESC;
        UIManager.Instance.OnRecipeBookPressed -= HandleInput;
    }
    private void Awake()
    {
        bookInteract = InputSystem.actions.FindAction("BookInteract");
        root = recipeBookUI.rootVisualElement;
        //closeButton = root.Q<Button>("exitButton");
        prevPageButton = root.Q<Button>("prevPage");
        nextPageButton = root.Q<Button>("nextPage");
        recipesContainer = root.Q<VisualElement>("recipesContainer");
        ordersContainer = root.Q<VisualElement>("ordersContainer");
        pageNumberLabel = root.Q<Label>("pageLabel");
        //searchField = root.Q<TextField>("searchField");

        prevPageButton.clicked += PrevPage;
        nextPageButton.clicked += NextPage;
        
        prevPageButton.RegisterCallback<MouseEnterEvent>(_ =>
        {
            AudioManager.Instance.Play(buttonHoverSound);
        });

        nextPageButton.RegisterCallback<MouseEnterEvent>(_ =>
        {
            AudioManager.Instance.Play(buttonHoverSound);
        });

        LoadRecipes();
        RefreshRecipesPage();
        Close(false);
    }

    private void BuildPages(List<DrinkRuleSO> recipes)
    {
        pages.Clear();

        List<DrinkRuleSO> currentPageRecipes = new();

        foreach (var recipe in recipes)
        {
            int steps = CountSteps(recipe.resultingState);


            if (steps > 4)
            {
                if (currentPageRecipes.Count > 0)
                {
                    pages.Add(new List<DrinkRuleSO>(currentPageRecipes));
                    currentPageRecipes.Clear();
                }

                pages.Add(new List<DrinkRuleSO> { recipe });
                continue;
            }

            currentPageRecipes.Add(recipe);

            if (currentPageRecipes.Count == 2)
            {
                pages.Add(new List<DrinkRuleSO>(currentPageRecipes));
                currentPageRecipes.Clear();
            }
        }

        if (currentPageRecipes.Count > 0)
            pages.Add(currentPageRecipes);
    }

    private int CountSteps(ItemDataSO result)
    {
        int count = 0;
        var rule = FindRuleFromResult(result);
        if (!rule)
            return 0;

        void Recurse(ItemDataSO res)
        {
            var r = FindRuleFromResult(res);
            if (!r)
                return;

            Recurse(r.requiredState);
            Recurse(r.addedIngredient);
            count++;
        }

        Recurse(result);
        return count;
    }

    private void HandleState(UIState state)
    {
        switch (state)
        {
            case UIState.RecipeBook:
                Open();
                break;
            default:
                recipeBookUI.rootVisualElement.style.display = DisplayStyle.None;
                break;
        }
    }

    private void HandleInput()
    {
        if (isBookOpen)
            Close();
        else
            UIManager.Instance.SetUIState(UIState.RecipeBook);
    }

    private VisualTreeAsset GetTemplateForRecipe(DrinkRuleSO recipe)
    {
        int steps = CountSteps(recipe.resultingState);

        if (steps <= 3)
            return recipeFinalTemplate;
        if (steps <= 4)
            return recipeTemplate5;

        return recipeTemplate7;
    }

    private void LoadRecipes()
    {
        bookRecipes = UnlockManager.Instance.runtimeDatabase.GetUnlockedRecipes()
            .FindAll(r => r.ruleType == RuleType.FinalDrink || r.ruleType == RuleType.SpecialUnlock)
            .OrderBy(r => r.ruleType)
            .ThenBy(r => r.resultingState.itemName)
            .ToList();

        BuildPages(bookRecipes);

        currentPage = Mathf.Clamp(currentPage, 0, pages.Count - 1);
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
            string drinkName;
            if (drink != null)
            {
                drinkName = drink.itemName;
            } else
            {
                drinkName = "Surprise Drink!";
            }
                // hier dann template machen
            var orderItem = orderTemplate.Instantiate();
            var orderNPCName = orderItem.Q<Label>("npcName");
            var orderItemName = orderItem.Q<Label>("npcOrder");
            var orderIcon = orderItem.Q<VisualElement>("iconOrder");

            orderNPCName.text = npcName;
            orderItemName.text = drinkName;
            
            if(drink)
                orderIcon.style.backgroundImage = drink.icon ? new StyleBackground(drink.icon) : null;

            orderItem.RegisterCallback<ClickEvent>(_ =>
            {
                if (drink != null)
                {
                    ShowSingleRecipe(drink);
                    isSinleRecipeView = true;
                    AudioManager.Instance.Play(buttonClickSound);
                }

            });
            
            orderItem.RegisterCallback<MouseEnterEvent>(_ =>
            {
                if(drink)
                    AudioManager.Instance.Play(buttonHoverSound);
            });
            
            ordersContainer.Add(orderItem);
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

        foreach (var recipe in pages[currentPage])
        {
            BuildRecipeItem(recipe);
        }

        pageNumberLabel.text = $"{currentPage + 1}";
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
        var template = GetTemplateForRecipe(recipe);
        var recipeItem = template.Instantiate();

        var headerLabel = recipeItem.Q<Label>("recipeHeaderTitle");
        var stepContainer = recipeItem.Q<VisualElement>("recipeStepContainer");
        var recipeIcon = recipeItem.Q<VisualElement>("recipeIcon");
        
        recipeIcon.style.backgroundImage = recipe.resultingState.icon ? new StyleBackground(recipe.resultingState.icon) : null;

        stepContainer.Clear();

        headerLabel.text = recipe.resultingState.itemName;

        int stepIndex = 1;
        BuildStepsRecursive(recipe.resultingState, stepContainer, ref stepIndex);

        recipesContainer.Add(recipeItem);
    }

    private void BuildStepsRecursive(ItemDataSO result, VisualElement stepContainer, ref int stepIndex)
    {
        var rule = FindRuleFromResult(result);
        if(rule == null) 
            return;

        BuildStepsRecursive(rule.requiredState, stepContainer, ref stepIndex);
        BuildStepsRecursive(rule.addedIngredient, stepContainer, ref stepIndex);

        AddStep(rule, stepContainer, ref stepIndex);
    }

    private void AddStep(DrinkRuleSO rule, VisualElement stepContainer, ref int stepIndex)
    {
        var step = recipeStepTemplate.Instantiate();
        step.Q<Label>("left").text = rule.requiredState.itemName;
        
        var rightLabel = step.Q<Label>("right");
        rightLabel.text = rule.addedIngredient.itemName;
        
        rightLabel.RemoveFromClassList("machine-action--kettle");
        rightLabel.RemoveFromClassList("machine-action--coffee");
        rightLabel.RemoveFromClassList("machine-action--grinder");
        rightLabel.RemoveFromClassList("machine-action--frother");
        
        int ingredientId = rule.addedIngredient.id;
        
        if (MachineActionTextById.TryGetValue(ingredientId, out var actionText))
        {
            rightLabel.text = actionText;
            switch (ingredientId)
            {
                case ID_HOTWATER: rightLabel.AddToClassList("machine-action--kettle"); break;
                case ID_COFFEE:   rightLabel.AddToClassList("machine-action--coffee"); break;
                case ID_POWDER:   rightLabel.AddToClassList("machine-action--grinder"); break;
                case ID_FOAM:     rightLabel.AddToClassList("machine-action--frother"); break;
            }
        }
        
        step.Q<Label>("result").text = rule.resultingState.itemName;
        step.Q<Label>("stepNumber").text = $"{stepIndex}.";

        var leftVisual = step.Q<VisualElement>("leftVisual");
        var rightVisual = step.Q<VisualElement>("rightVisual");
        var resultVisual = step.Q<VisualElement>("resultVisual");

        leftVisual.style.backgroundImage = rule.requiredState.icon ? new StyleBackground(rule.requiredState.icon) : null;
        if (!rule.requiredState.icon) leftVisual.style.display = DisplayStyle.None;
        
        rightVisual.style.backgroundImage = rule.addedIngredient.icon ? new StyleBackground(rule.addedIngredient.icon) : null;
        if (!rule.addedIngredient.icon) rightVisual.style.display = DisplayStyle.None;
        
        resultVisual.style.backgroundImage = rule.resultingState.icon ? new StyleBackground(rule.resultingState.icon) : null;
        if (!rule.resultingState.icon) resultVisual.style.display = DisplayStyle.None;
        
        stepContainer.Add(step);
        stepIndex++;
    }

    private DrinkRuleSO FindRuleFromResult(ItemDataSO result)
    {
        return UnlockManager.Instance.runtimeDatabase.GetUnlockedRecipes().Find(r => r.resultingState == result);
    }

    private void NextPage()
    {
        AudioManager.Instance.Play(buttonClickSound);
        
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
        AudioManager.Instance.Play(buttonClickSound);
        
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
        AudioManager.Instance.Play(bookOpenSound);
        
        if(GameState.inTutorial && TutorialManager.Instance != null)
        {
            if (TutorialManager.Instance.currentStep != TutorialStep.OpenRecipeBook)
                return;
            TutorialManager.Instance.OnRecipeBookOpened();
        }

        GameState.isInMenu = true;
        LoadRecipes();
        RefreshRecipesPage();
        RefreshOrders();
        isBookOpen = true;
        root.style.display = DisplayStyle.Flex;
        GameTime.SetPaused(true);
    }

    private void CloseESC()
    {
        if(isBookOpen) Close();
    }

    private void Close(bool withSound = true)
    {
        if(withSound)
            AudioManager.Instance.Play(bookCloseSound);
        
        GameState.isInMenu = false;
        isBookOpen = false;
        UIManager.Instance.ResetState();
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
