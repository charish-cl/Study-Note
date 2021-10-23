# Scene（场景）

Scene模块提供场景管理的功能，可以同时加载多个场景，也可以随时卸载任何一个场景，从而很容易地实现场景的分部加载。

 

1.Scene模块主要由场景管理器，场景事件两部分组成

 

2.场景管理器（SceneManager）分别使用3个列表维护已加载，加载中，已卸载的场景，持有IResourceManager引用，调用其中的加载场景与卸载场景方法，持有一个IResourceManager，通过其进行字场景的加载与卸载（**加载与卸载过程中通过****6****个委托将场景加载与卸载事件通知给SceneComponent****，然后由其派发6****个全局事件**）

 

3.场景事件分为两部分，一部分是SceneManager里的模块局部事件，定义在GF里，负责将场景加载与卸载事件通知到SceneComponent，另一部分是定义在UGF里的全局事件，由SceneComponent接收到加载与卸载事件后进行派发

# Localization（本地化）

Localization模块提供本地化功能，也就是我们平时所说的多语言。Game Framework 在本地化方面，不但支持文本的本地化，还支持任意资源的本地化，

 

1.Localization模块主要由本地化辅助器，本地化管理器，加载字典事件三部分组成

 

2.本地化辅助器（ILocalizationHelper）负责进行字典的实际操作（加载，解析，释放）

 

3.本地化管理器（LocalizationManager）使用一个字典来维护本地化数据，持有一个ILocalizationHelper的引用，通过调用ILocalizationHelper里的方法提供相关操作。持有一个IResourceManager，通过其进行字典的加载（**加载过程中通过****4****个委托将字典加载事件通知给LocalizationComponent****，然后由其派发4****个全局事件**）

 

4.加载字典事件分为两部分，一部分是LocalizationManager里的模块局部事件，定义在GF里，负责将资源加载事件通知到LocalizationComponent，另一部分是定义在UGF里的全局事件，由LocalizationComponent接收到加载字典事件后进行派发，

##  读取配置

```cs
private void LoadDictionary(string dictionaryName)
{
 string dictionaryAssetName = AssetUtility.GetDictionaryAsset(dictionaryName, false);
 m_LoadedFlag.Add(dictionaryAssetName, false);
 GameEntry.Localization.ReadData(dictionaryAssetName, this);
}
```

## 问题

![image-20211023162337230](C:\Users\CodeElk\AppData\Roaming\Typora\typora-user-images\image-20211023162337230.png)

好像只能用本工程的helper，其他通过反射获取到的读取失败

![image-20211023162429530](C:\Users\CodeElk\AppData\Roaming\Typora\typora-user-images\image-20211023162429530.png)

# Config

Config模块与DataTable模块类似，区别在于配置模块无需建立不同的数据行类

 

1.Config模块主要由配置数据，配置辅助器，配置管理器，加载配置事件四个部分组成

 

2.配置数据（ConfigData）保存配置项的数据，有bool，int，float，string四种类型

 

3.配置辅助器（IConfigHelper）负责进行配置的实际操作（加载，解析，释放）

 

4.配置管理器（ConfigManager）使用一个字典来维护所有ConfigData，持有一个IConfigHelper的引用，通过调用IConfigHelper里的方法提供相关操作。持有一个IResourceManager，通过其进行配置的加载（**加载过程中通过****4****个委托将配置加载事件通知给ConfigComponent****，然后由其派发4****个全局事件**）

 

5.加载配置事件分为两部分，一部分是ConfigManager里的模块局部事件，定义在GF里，负责将资源加载事件通知到ConfigComponent，另一部分是定义在UGF里的全局事件，由ConfigComponent接收到配置加载事件后进行派发，

 

## 配置加载流程

外部调用ConfigManager里加载配置的方法，在该方法里调用ResourceManager的异步加载资源方法

ResourceManager在异步加载资源成功后调用ConfigManager的回调方法，回调方法里调用ConfigHelper的加载配置方法

ConfigHelper在加载配置方法里调用ConfigManager的解析配置方法

ConfigManager在解析配置方法里调用ConfigHelper的解析配置方法

ConfigConfigHelper在解析配置方法里解析配置文本，然后调用ConfigManager的添加配置方法，将配置数据添加到字典里，至此加载完毕



ConfigManager.LoadConfig()→ResourceManager.LoadAsset()→ConfigManager.LoadConfigSuccessCallback()→ConfigHelper.LoadConfig()→ConfigManager.ParseConfig()*→ConfigHelper.ParseConfig()→ConfigManager.AddConfig()

# Procedure

## Procedure流程图

![img](https://myfirstblog.oss-cn-hangzhou.aliyuncs.com/2019/04/QQ%E6%88%AA%E5%9B%BE20190429123328.png!webp)

```cs
protected override void OnEnter(ProcedureOwner procedureOwner)
{
    base.OnEnter(procedureOwner);

    m_IsChangeSceneComplete = false;

    GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
    GameEntry.Event.Subscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
    GameEntry.Event.Subscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneUpdate);
    GameEntry.Event.Subscribe(LoadSceneDependencyAssetEventArgs.EventId, OnLoadSceneDependencyAsset);

    // 停止所有声音
    GameEntry.Sound.StopAllLoadingSounds();
    GameEntry.Sound.StopAllLoadedSounds();

    // 隐藏所有实体
    GameEntry.Entity.HideAllLoadingEntities();
    GameEntry.Entity.HideAllLoadedEntities();

    // 卸载所有场景
    string[] loadedSceneAssetNames = GameEntry.Scene.GetLoadedSceneAssetNames();
    for (int i = 0; i < loadedSceneAssetNames.Length; i++)
    {
        GameEntry.Scene.UnloadScene(loadedSceneAssetNames[i]);
    }

    // 还原游戏速度
    GameEntry.Base.ResetNormalGameSpeed();

    int sceneId = procedureOwner.GetData<VarInt32>("NextSceneId");
    m_ChangeToMenu = sceneId == MenuSceneId;
    IDataTable<DRScene> dtScene = GameEntry.DataTable.GetDataTable<DRScene>();
    DRScene drScene = dtScene.GetDataRow(sceneId);
    if (drScene == null)
    {
        Log.Warning("Can not load scene '{0}' from data table.", sceneId.ToString());
        return;
    }

    GameEntry.Scene.LoadScene(AssetUtility.GetSceneAsset(drScene.AssetName), Constant.AssetPriority.SceneAsset, this);
    m_BackgroundMusicId = drScene.BackgroundMusicId;
}
```

## procedureOwner

**procedureOwner为流程持有者**，**为GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>别名**

```csharp
protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
{
    GameEntry.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
    GameEntry.Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
    GameEntry.Event.Unsubscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneUpdate);
    GameEntry.Event.Unsubscribe(LoadSceneDependencyAssetEventArgs.EventId, OnLoadSceneDependencyAsset);

    base.OnLeave(procedureOwner, isShutdown);
}
```

**离开流程，你要搬家了，自然要把订的报纸杂志之类的给取消，如果不取消订阅，会造成回调函数的重复执行(按自己需求来)**

![img](https://myfirstblog.oss-cn-hangzhou.aliyuncs.com/2019/04/QQ%E6%88%AA%E5%9B%BE20190429124945.png!webp)

# Entity

GF将游戏场景中，动态创建的一切物体定义为实体。

Entity模块提供管理实体和实体组的功能，如显示隐藏实体、挂接实体（如挂接武器、坐骑，或者抓起另一个实体）等。实体使用结束后可以不立刻销毁，从而等待下一次重新使用。

 

1.Entity模块主要由实体，实体信息，实体组，辅助器，实体管理器，实体事件五个部分组成

 

2.实体（IEntity）保存了实体相关数据以及方法

 

3.实体信息（EntityInfo）主要保存实体的父子实体引用

 

4.实体组（EntityGroup）负责**使用对象池来管理实体**

 

5.辅助器分为实体辅助器（IEntityHelper）与实体组辅助器（IEntityGroupHelper），IEntityHelper提供了实体的实例化，创建与释放的方法，IEntityGroupHelper主要在UGF中为实例化出来的实体提供默认父实体

 

6.实体管理器（EntityManager）使用字典管理所有EntityInfo与EntityGroup，持有一个IEntityHelper引用，通过其中的方法进行实体相关的操作。持有一个IResourceManager，通过其进行实体资源的加载（**加载过程中通过****4****个委托将实体资源加载事件通知给EntityComponent****，然后由其派发4****个全局事件**）。

 

7.实体事件分为两部分，一部分是EntityManager里的模块局部事件，定义在GF里，负责将实体资源加载事件与隐藏实体事件通知到EntityComponent，另一部分是定义在UGF里的全局事件，由EntityComponent接收到事件后进行派发

 

8.在UGF中，还提供了继承MonoBehaviour的EntityLogic（实体逻辑基类）与Entity（实体），EntityLogic负责处理实体的逻辑（实体的初始化，显示，隐藏，子实体的附加，解除），Entity实现了IEntity接口，持有并代理对应的EntityLogic

 

9.StartForce中，提供了EntityData（实体数据基类），由继承了EntityLogic的类持有，并在OnShow方法中赋值

## EntityLogic

只要让类继承EntityLogic，即可成为逻辑处理类

## 生成实体

```cs
GameEntry.Entity.ShowEntity<Hero>(1,"Assets/Slay/Entities/Hero.prefab","Default");
```



# UI

UI模块提供管理界面和界面组的功能，如显示隐藏界面、激活界面、改变界面层级等。不论是 Unity 内置的 uGUI 还是其它类型的 UI 插件（如 NGUI），只要派生自 UIFormLogic 类并实现自己的界面类即可使用

 

1.UI模块主要由界面，界面信息，界面组，辅助器，界面管理器，界面事件五个部分组成

 

2.界面（IUIForm）保存了界面相关数据以及方法

 

3.界面信息（UIFormInfo）主要保存界面的引用以及设置界面是否暂停或遮挡

 

4.界面组（UIGroup）使用链表维护组内所有UIFormInfo

 

5.辅助器分为界面辅助器（IUIFormHelper）与界面组辅助器（IUIGroupHelper），IUIFormHelper提供了界面的实例化，创建与释放的方法，IUIGroupHelper主要在UGF中为实例化出来的界面提供默认父实体

 

6.界面管理器（UIManager）使用字典管理所有UIGroup，使用链表管理所有IUIForm，并负责使用对象池来管理某界面。持有一个IUIFormHelper引用，通过其中的方法进行界面相关的操作。持有一个IResourceManager，通过其进行界面资源的加载（**加载过程中通过****4****个委托将实体资源加载事件通知给UIComponent****，然后由其派发4****个全局事件**）。

 

7.界面事件分为两部分，一部分是UIManager里的模块局部事件，定义在GF里，负责将界面资源加载事件与关闭界面事件通知到UIComponent，另一部分是定义在UGF里的全局事件，由UIComponent接收到模块局部事件后进行派发

 

8.在UGF中，还提供了继承MonoBehaviour的UIFormLogic（界面逻辑基类）与UIForm（界面），UIFormLogic负责处理界面的逻辑（界面的初始化，打开，关闭，暂停，激活），UIForm实现了IUIForm接口，持有并代理对应的UIFormLogic

## 加载UI

第一个参数为资源路径，第二个为UiGroup

```cs
GameEntry.UI.OpenUIForm("Assets/Slay/UI/Menu.prefab", "Default");
```

![image-20211022212143665](C:\Users\CodeElk\AppData\Roaming\Typora\typora-user-images\image-20211022212143665.png)

## 事件

![img](https://myfirstblog.oss-cn-hangzhou.aliyuncs.com/2019/04/20190105175224517.png!webp)

## Ui事件订阅

```cs
GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId,OpenUISuccess);
GameEntry.UI.OpenUIForm("Assets/Slay/UI/Menu.prefab", "Default",this);
```

```csharp
  private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
    {
        OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
        // 判断userData是否为自己
        if (ne.UserData != this)
        {
            return;
        }
        Log.Debug("UI_Menu：恭喜你，成功地召唤了我。");
    }
```

通过回调函数的e参数，我们可以获取UserData对象，也就是我们在订阅事件时传入的对象。

# Event

## 内置的事件

![image-20211022212438707](C:\Users\CodeElk\AppData\Roaming\Typora\typora-user-images\image-20211022212438707.png)

![image-20211022212513050](C:\Users\CodeElk\AppData\Roaming\Typora\typora-user-images\image-20211022212513050.png)

## 订阅

Event.Subscribe(事件唯一ID, 回调函数)。

# WebRequest（Web请求）

WebRequest模块提供使用短连接的功能，可以用 Get 或者 Post 方法向服务器发送请求并获取响应数据，可指定允许几个 Web 请求器进行同时请求

 

1.WebRequest模块主要由由Web请求任务，Web请求代理辅助器，Web请求任务代理，Web请求管理器，Web请求事件五个部分组成

 

2.Web请求任务（WebRequestTask）实现了ITask接口，保存了Web请求任务的相关数据

 

3.Web请求辅助器（IWebRequestAgentHelper）负责进行**实际的请求**逻辑处理（**在****UGF****中提供了默认的实现DefaultWebRequestAgentHelper****，使用WWW****类进行请求**）（**请求过程中通过****2****个委托向WebRequestAgent****通知请求事件**）

 

4.Web请求任务代理（WebRequestAgent）实现了ITaskAgent接口，负责处理请求任务，并持有一个IWebRequestAgentHelper，调用其中的方法进行实际的请求。（**请求过程中通过****3****个委托向WebRequestManager****通知请求事件**）

 

5.Web请求管理器（WebRequestManager）维护一个WebRequestTask的TaskPool，对外提供WebRequestTask的相关操作（**请求过程中通过****3****个委托将WebRequestAgent****的请求事件通知给UGF****的WebRequestComponent****，然后由其派发3****个全局事件**）

 

6.下载事件分为三部分，一部分是WebRequestManager里的事件，另一部分是WebRequestAgentHelper里的事件，这两部分定义在GF里，是**模块局部事件**，第三部分则是**UGF****里定义的全局事件**,由WebRequestComponent接收到模块局部事件后进行派发
