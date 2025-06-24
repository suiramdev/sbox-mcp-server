# MCP Tools TODO List

## Core S&box API Elements to Implement

### Components
- [ ] GetComponents - Retrieve all components of a specific type
- [ ] GetComponent - Get a single component of a specific type
- [ ] AddComponent - Add a new component to a GameObject
- [ ] RemoveComponent - Remove a component from a GameObject
- [ ] HasComponent - Check if GameObject has a specific component
- [ ] GetComponentInChildren - Find component in child GameObjects
- [ ] GetComponentInParent - Find component in parent GameObjects

### GameObject Management
- [ ] CreateGameObject - Create new GameObjects in the scene
- [ ] DestroyGameObject - Remove GameObjects from the scene
- [ ] FindGameObject - Find GameObjects by name or tag
- [ ] GetGameObjectHierarchy - Get parent/child relationships
- [ ] SetGameObjectParent - Modify GameObject hierarchy
- [ ] CloneGameObject - Duplicate existing GameObjects

### Transform Operations
- [ ] GetTransform - Get GameObject transform data
- [ ] SetPosition - Set GameObject world/local position
- [ ] SetRotation - Set GameObject rotation
- [ ] SetScale - Set GameObject scale
- [ ] TransformPoint - Transform coordinates between spaces
- [ ] LookAt - Orient GameObject to look at target

### Scene Management
- [ ] GetActiveScene - Get current active scene
- [ ] LoadScene - Load a different scene
- [ ] GetSceneObjects - List all objects in scene
- [ ] SaveScene - Save current scene state
- [ ] CreateScene - Create new scene

### Asset Management
- [ ] LoadAsset - Load assets by path
- [ ] GetAssetInfo - Get asset metadata
- [ ] CreateAsset - Create new assets
- [ ] AssetPreview - Generate asset thumbnails/previews
- [ ] GetAssetDependencies - Find asset dependencies

### Editor Attributes
- [ ] SetTitle - Apply TitleAttribute to properties
- [ ] SetDescription - Apply DescriptionAttribute
- [ ] SetCategory - Apply CategoryAttribute
- [ ] SetGroup - Apply GroupAttribute
- [ ] SetIcon - Apply IconAttribute
- [ ] SetPlaceholder - Apply PlaceholderAttribute

### UI/Editor Integration
- [ ] ShowNotification - Display editor notifications
- [ ] CreateEditorWindow - Create custom editor windows
- [ ] UpdateInspector - Refresh inspector panels
- [ ] SetSelection - Set selected objects in editor
- [ ] GetSelection - Get currently selected objects

### Diagnostics & Debugging
- [ ] LogMessage - Send messages to console
- [ ] GetPerformanceMetrics - Monitor performance
- [ ] ValidateScene - Check scene integrity
- [ ] GetErrorList - Retrieve current errors/warnings

### Advanced Features
- [ ] ExecuteCommand - Run editor commands
- [ ] GetTypeInfo - Retrieve type information
- [ ] SerializeObject - Convert objects to JSON
- [ ] DeserializeObject - Create objects from JSON
- [ ] GetDisplayInfo - Get UI display information

### Shader & Material Tools
- [ ] CreateMaterial - Create new materials
- [ ] SetMaterialProperty - Modify material properties
- [ ] GetShaderInfo - Get shader information
- [ ] CompileShader - Compile shader code

### Physics Integration
- [ ] AddPhysicsComponent - Add physics components
- [ ] SetPhysicsProperties - Configure physics settings
- [ ] RaycastQuery - Perform raycasting
- [ ] GetCollisionInfo - Get collision data

## Implementation Priority
1. **High Priority**: Components, GameObject, Transform operations
2. **Medium Priority**: Scene management, Asset management
3. **Low Priority**: Advanced features, Shader tools, Physics

## Notes
- Focus on most commonly used operations first
- Ensure proper error handling for all operations
- Add validation for all inputs
- Consider batch operations for performance
- Implement proper async/await patterns where needed
