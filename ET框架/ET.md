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

## ECS

##### 入口

    1.	OneThreadSynchronizationContext.Instance.Update();
    2.	Game.EventSystem.Update();




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
