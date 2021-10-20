## 服务器

### 服务器名称

        Manager : 对服务器进程进行管理
        Realm : 登录服务器 ( 验证账号密码 相当于LoginServer 祖传叫法,你想叫什么随你)
        Gate : 网关服务器
        DB : 数据库服务器
        Location : 位置服务器
        Map : 地图服务器
        Client : 客户端
        All Server: 所有服务器集合
        消息命名( 端到端的命名方式 )
        C2R_Ping = Client to Realm
        R2G_GetLoginKey = Realm to Gate
        M一般情况下代表map, 特殊代表Manager ,具体代表什么看消息协议内容
    
    G2M_CreateUnit = Gate to Map
    C2M_TestRequest = Client to Map
    M2C_CreateUnits = Map to Client
    M2A_Reload = Manager to AllServer 通知全部服务器热更新

### 各服务器的作用(摘录自文档ET框架笔记):

    Manager：连接客户端的外网和连接内部服务器的内网，对服务器进程进行管理，自动检测和启动服务器进程。加载有内网组件NetInnerComponent，外网组件NetOuterComponent，服务器进程管理组件。自动启动突然停止运行的服务器，保证此服务器管理的其它服务器崩溃后能及时自动启动运行。
    
    Realm：对Actor消息进行管理（添加、移除、分发等），连接内网和外网，对内网服务器进程进行操作，随机分配Gate服务器地址。内网组件NetInnerComponent，外网组件NetOuterComponent，Gate服务器随机分发组件。客户端登录时连接的第一个服务器，也可称为登录服务器。
    
    Gate：对玩家进行管理，对Actor消息进行管理（添加、移除、分发等），连接内网和外网，对内网服务器进程进行操作，随机分配Gate服务器地址，对Actor消息进程进行管理，对玩家ID登录后的Key进行管理。加载有玩家管理组件PlayerComponent，管理登陆时联网的Key组件GateSessionKeyComponent。
    
    Location：连接内网，服务器进程状态集中管理（Actor消息IP管理服务器）。加载有内网组件NetInnerComponent，服务器消息处理状态存储组件LocationComponent。对客户端的登录信息进行验证和客户端登录后连接的服务器，登录后通过此服务器进行消息互动，也可称为验证服务器。
    
    Map：连接内网，对ActorMessage消息进行管理（添加、移除、分发等），对场景内现在活动物体存储管理，对内网服务器进程进行操作，对Actor消息进程进行管理，对Actor消息进行管理（添加、移除、分发等），服务器帧率管理。服务器帧率管理组件ServerFrameComponent。
    
    AllServer：将以上服务器功能集中合并成一个服务器。另外增加DB连接组件DBComponent
    
    Benchmark：连接内网和测试服务器承受力。加载有内网组件NetInnerComponent，服务器承受力测试组件BenchmarkComponent。

### 服务器启动

```csharp
Program类:

1. 设置线程同步上下文，用于将异步方法全都能通过Post方法回到主线程调用：SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);


```













## 热更

### 入口

```cs
Game.Hotfix.LoadHotfixAssembly();//加载热更代码入口,每次更新HotFix下的代码需要build一下,5.0 Editor自动检测更新
HotFix不能存数据,热重载造成数据丢失,存放热更代码逻辑
```



### Model层和Hotfix层交互

1. 关于跨域交互：Model2Hotfix 使用事件，Hotfix2Moedel使用引用。
2. 关于跨层交互：View2Data使用引用，Data2View使用引用。（因为是在同一组件内，所以互相都有对方的引用。 ）
3. 服务端ETHotfix可以无限制地使用ETModel的类或函数，只需在头部位置“使用ETModel”即可。
   ETModel不能使用ETHotfix的类或函数，所以您需要使用[Event(EventIdType.xxxx)]来告诉ETHotfix该做什么。

### 热更相关

1. 控制台输入reload,这样就可以热更
2. 热更后update走新的逻辑,协程没结束的会走旧的,结束重新开协程会走新的
3. 服务器端与客户端不同。服务器端的ETHotfix只有逻辑，没有数据。数据被放在ETModel中，所以服务器端可以在运行时重新加载修复DLL。客户端热更新是因为ios不允许Unity加载DLL。它需要使用一个ILRuntime库来执行ETHotfix，所以客户端热更新需要用数据和逻辑重新启动。

## ECS

##### 入口

    1.	OneThreadSynchronizationContext.Instance.Update();
    2.	Game.EventSystem.Update();

##### 理解

ecs有一个演进过程的

**第一种，可以这样：**

**entity:** 纯id属性，无数据状态无逻辑，他只是一个具有生命周期的对象，添加给他的组件都跟随他的生命周期

**componet:** 有数据状态，无逻辑

**System:** 有逻辑无状态

这就是你熟悉的，也是unity中的ecs的模型。

**第二种，也可以这样：**

**entity:** 有重要的数据状态，也可以无数据状态，无逻辑，也是一个具有生命周期的对象，添加给他的组件都跟随他的生命周期

**componet:** 有数据状态，有标准的逻辑，也可以无逻辑

**System:** 有逻辑无状态，而且对同样的组件，在不同系统可以有不同的逻辑

第一种适合前端开发，第二种适合后端开发。

————————————————————————————

第一种这么做，对于前端来说意义是明显的，因为都与场景，游戏物体资源，更复杂的模块（模型网格，物理，网络，材质，渲染，灯光）密切相关，且运行在一个客户程序中。

而在服务端，是不存在前端游戏中的场景与游戏物体资源的，不同的功能模块又可以在不同的物理主机各自独立的程序中运行，而且只是其中部分需要在服务端计算和存放的纯数据状态操作。

这样就能更灵活一些，你可以像第一种一样开发。也可以在必要的时候：

- 实体中可以有重要的数据状态，
- 组件中可以有标准的逻辑，
- 服务端分布式开发的情况下，不同的系统如realm,gate,map可以分别在不同的物理主机系统下运行，这样还能面向同样的组件的system，可以是不同的逻辑。

————————————————————————————

这样看来，不论第一种还是第二种，其核心思想是一样的：

entity是生命周期对象

组件添加给实体，跟随实体生命周期

逻辑在System中面向组件，是无状态的

**区别只是：在服务端实体中可以有一些重要的数据状态，组件中可以有一些标准逻辑。**





而为什么要有ECS这种设计，我觉得风云这篇文章讲得很好：https://blog.codingnow.com/2017/06/overwatch_ecs.html

我相信很多做过游戏开发的程序都会有这种体会。因为游戏对象其实是由很多部分聚合而成，引擎的功能模块很多，不同的模块关注的部分往往互不相关。比如渲染模块并不关心网络连接、游戏业务处理不关心玩家的名字、用的什么模型。从自然意义上说，把游戏对象的属性聚合在一起成为一个对象是很自然的事情，对于这个对象的生命期管理也是最合理的方式。但对于不同的业务模块来说，针对聚合在一起的对象做处理，把处理方法绑定在对象身上就不那么自然了。这会导致模块的内聚性很差、模块间也会出现不必要的耦合。

我觉得守望先锋之所以要设计一个新的框架来解决这个问题，是因为他们面对的问题复杂度可能到了一个更高的程度：比如如何用预测技术做更准确的网络同步。网络同步只关心很少的对象属性，没必要在设计同步模块时牵扯过多不必要的东西。为了准确，需要让客户端和服务器跑同一套代码，而服务器并不需要做显示，所以要比较容易的去掉显示系统；客户端和服务器也不完全是同样的逻辑，需要共享一部分系统，而在另一部分上根据分别实现……

总的来说、需要想一个办法拆分复杂问题，把问题聚焦到一个较小的集合，提高每个子任务的内聚性。

**ECS 的 E ，也就是 Entity ，可以说就是传统引擎中的 Game Object 。但在这个系统下，它仅仅是 C/Component 的组合。它的意义在于生命期管理，这里是用 32bit ID 而不是指针来表示的，另外附着了渲染用到的资源 ID 。因为仅负责生命期管理，而不设计调用其上的方法，用整数 ID 更健壮。整数 ID 更容易指代一个无效的对象，而指针就很难做到。**

**C 和 S 是这个框架的核心。System 系统，也就是我上面提到的模块。对于游戏来说，每个模块应该专注于干好一件事，而每件事要么是作用于游戏世界里同类的一组对象的每单个个体的，要么是关心这类对象的某种特定的交互行为。比如碰撞系统，就只关心对象的体积和位置，不关心对象的名字，连接状态，音效、敌对关系等。它也不一定关心游戏世界中的所有对象，比如关心那些不参与碰撞的装饰物。所以对每个子系统来说，筛选出系统关心的对象子集以及只给它展示它所关心的数据就是框架的责任了。**



### 组件

  #### 更新组件继承自IUpdateSystem 

    EventSystem添加到更新字典中->
    在Game.EventSystem.Update()执行;

### ECS结构

    组件创建: ComponentFactory.Create<Car>()这样才会具有实体生命周期//调用Awake,Start(游戏循环后的那一帧)
    
    实体Dispose或者设置为null都会调用Dispose方法,实体身上的每一个组件随实体更新
        car.Dispose();
    	car = null;

https://github.com

##### 实体生命周期与实体分离开

~~~ C#
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class CarAwake: AwakeSystem<Car>
    {
        public override void Awake(Car self)
        {
            Debug.Log("生命周期---初始");
        }
    }

    [ObjectSystem]
    public class CarStart: StartSystem<Car>
    {
        public override void Start(Car self)
        {
            Debug.Log("生命周期---开始");
        }
    }

    [ObjectSystem]
    public class CarUpdate: UpdateSystem<Car>
    {
        public override void Update(Car self)
        {
            Debug.Log("生命周期---更新");
        }
    }

    [ObjectSystem]
    public class CarDispos: DestroySystem<Car>
    {
        public override void Destroy(Car self)
        {
            Debug.Log("生命周期---销毁");
        }
    }

    public class Car: Entity
    {
    }

    [ObjectSystem]
    public class CarWheelAwake: AwakeSystem<CarWheel>
    {
        public override void Awake(CarWheel self)
        {
            Debug.Log("轮子生命周期--awake");
        }
    }

    public class CarWheel: ComponentWithId
    {
    }
}
~~~

## 消息

### RPC

RPC消息和普通消息相比。就是RPC消息在普通消息头中加入了客户端用于标识挂起消息的唯一ID即RpcID，用于在服务器收到消息后返回透传回客户端重新激活客户端挂起的异步方法。

我们以ET为例，ET中大致分为两种协议：第一种非RPC协议 和 第二种RPC协议

我们以登陆为例讲两种方式：

~~~ C#
Login_C2G = 10001
Login_G2C = 10002
 
[Message(10001)]
[ProtoContract]
public partial class Message_Login_C2G
{
[ProtoMember(1, IsRequired = true)]
public string UserName;
[ProtoMember(91, IsRequired = true)]
public string PWD{ get; set; }
}
[Message(10002)]
[ProtoContract]
public partial class Message_Login_G2C
{
[ProtoMember(1, IsRequired = true)]
public string Result;
}
~~~

第一种非RPC协议：

    协议格式：【（10001）协议号】【字节数组（Message_Login_C2G 序列化）】
    
    客户端把以上包发给服务器，服务器解出来，在以相同的格式发给客户端。这就是普通的协议。

第二种RPC协议

    协议格式：【（10001）协议号、RPCID】【字节数组（Message_Login_C2G 序列化）】
    
    RPC 协议格式上就是在发给服务器的时候包头加了RPCID。这个ID是客户端发起一个RPC请求时唯一标识这个请求的一个ID.

服务器收到协议后，解出包内容后带上客户端发送上来的RpcID返回给客户端。客户端收到服务器返回的消息找到对应挂起的RPC消息重新激活，这就完成了一次RPC的调用

* 普通消息：如果只是发送一条消息，不需要对方作响应（返回消息），那就是一条普通消息。用Session对象的Send方法。

* RPC消息：如果发送一条消息，还需要对方响应，这叫RPC消息，我们把RPC叫远程调用，对方肯定接收你的消息后，要调用一系列方法进行处理，再返回你一个结果。用Session对象的Call方法。

* ET是通过Channel （通信通道）实时同步消息的，一个信道是一个用户的实时连接，搭配一个session。Channel实时在同步，拿到消息后由session分发消息（有时候是直接返回给客户端，有时要转分发给Map、Gate），当然session也负责发起消息与请求。



### Session

```
用来收发热更层的消息
```

属于服务端与客户端之间的消息类型，皆属于**OuterMessage**。（外部消息）

3. 服务器与其他服务器对话的消息（属于内部消息**InnerMessage**，且是Actor消息）
4. 需要返回结果（Actor RPC消息）
5. 不需要返回结果（普通的Actor消息）



### Actor消息

```csharp
public static void Broadcast(IActorMessage message)
		{
			Unit[] units = Game.Scene.GetComponent<UnitComponent>().GetAll();
			ActorMessageSenderComponent actorLocationSenderComponent = Game.Scene.GetComponent<ActorMessageSenderComponent>();
			foreach (Unit unit in units)
			{
				UnitGateComponent unitGateComponent = unit.GetComponent<UnitGateComponent>();
				
				if (unitGateComponent.IsDisconnect)
				{
					continue;
				}
				//根据指定的actorid发送actor消息
				ActorMessageSender actorMessageSender = actorLocationSenderComponent.Get(unitGateComponent.GateSessionActorId);
				actorMessageSender.Send(message);
			}
		}
```

## 组件

### Global(场景中的Global组件)

    public static GameObject Global { get; } = GameObject.Find("/Global");

Component的Parent若为空,则会设置Global为父物体

~~~ C#
//来源Hotfix/Base/Object/Componemt 72行
#if !SERVER
if (this.parent == null)
{
    this.GameObject.transform.SetParent(Global.transform, false);
    return;
}

if (this.GameObject != null && this.parent.GameObject != null)
{
    this.GameObject.transform.SetParent(this.parent.GameObject.transform, false);
}
#endif
~~~

## 事件

### UnOrderMultiMap

要想完全理解EventSystem，先要弄清楚ET里面的UnOrderMultiMap，这是个数据结构辅助类。专门用于管理某个类型对应的List。当然这个类，还带有重用功能，用于提升性能。

## 资源加载

1. ab包名字要以unity3d结尾
2. 代码中名字对应prefab名字

