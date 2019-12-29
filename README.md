# Feature Services

## Feature
A feature is a logical grouping of code within an application. It may be a section of the application that may be used once, or it can be a partial section of the application that can be created multiple times.

### Creation
A feature requires both its own declaration and that of its request. A request is a class that provides parameters from the caller when routing to that feature.

```csharp
public class MiniGameFeatureRequest
{
    public Difficulty Difficulty = Difficulty.Easy;
    public string LevelId;
}

public class MiniGameFeature : Feature<MiniGameFeatureRequest>
{
    public override Task Load(MiniGameFeatureRequest request, IRouter router)
    {
        // Instantiate any scenes, UI, resources and event listeners that this feature needs...
    }

    public override Task Unload()
    {
        // Unloads any resources or assets when navigating away from this feature...
    }
}
```

### Binding
In order to use the newly created Feature, you'll need to not only bind in the MicroDI container, but also register the Request => Feature pair.

```csharp
// Bind in the MicroDI container...
container.Bind<MiniGameFeature>();

// Register with the router...
_router.AddFeature<MiniGameFeatureRequest, MiniGameFeature>();
```

## Router
The router controls the loading and unloading of the features. It also preserves a stack of history for features; this allows you to navigate back without knowing the predecessor. 

### Navigation
Invoke the Navigate method with the request object. This request will be matched with the paired feature.

```csharp
await _router.Navigate(new MiniGameFeatureRequest());
```

### Navigating Backward
You can unload the current feature and navigate to the previous:

```csharp
await _router.Back();
```

### Replace Current Feature
You can unload the current feature and replace with a new request without affecting the stack history.

```csharp
_router.Navigate(new HomeScreenFeatureRequest());
// User clicks play, stack looks like: HomeScreenFeatureRequest -> MiniGameFeatureRequest
_router.Navigate(new MiniGameFeatureRequest());

// User hits replay, stack remains with: HomeScreenFeatureRequest -> MiniGameFeatureRequest
_router.ReplaceCurrent(new MiniGameFeatureRequest());

// Goes back with HomeScreenFeatureRequest...
_router.Back();
```

### Feature State Save & Restore
The router can maintain a single state per feature type. It's up to the feature to invoke these as necessary.

```csharp
_router.SaveState<MiniGameFeature, MiniGameFeatureState>(state);
var state = _router.GetState<MiniGameFeature, MiniGameFeatureState>();
```

#### Serialization
TBD