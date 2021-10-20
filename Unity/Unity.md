# 热更

U3D开发中关于脚本方面的限制-有关IOS反射和JIT的支持问题
U3D文档中说明了，反射在IOS是支持的，除了system.reflection.emit空间内的，其它都支持。JIT是不支持的。

本质上来说即是：只要不在运行时动态生成代码的行为都支持，reflection.emit下的功能可以动态的生成代码（生成程序集，生成类，生成函数，生成类型等，是真正的生成代码），JIT也能动态的生成代码（如C#生成的IL在各平台上运行时可以由JIT编译成汇编语言）。

其它的并不会生成代码的反射功能是可以用的，如 type.gettype, getfield, Activator.CreateInstance   

iOS不支持的是给动态分配的内存添加执行权限

## ILRuntime

[ILRuntime](https://link.zhihu.com/?target=https%3A//ourpalm.github.io/ILRuntime/public/v1/guide/index.html) 则是将痛点与不方便降到最低的，它是一个纯C#的热更新方案。借助Mono.Cecil库来读取DLL的PE信息，以及当中类型的所有信息，最终得到方法的IL汇编码，然后通过内置的IL解译执行虚拟机来执行DLL中的代码来实现热更新功能。



# 数据类型

## Single

表示一个单精度浮点数。

```csharp
public struct Single : IComparable, IComparable<float>, IConvertible, IEquatable<float>, IFormattable
```

- 继承

  [Object](https://docs.microsoft.com/zh-cn/dotnet/api/system.object?view=net-5.0)[ValueType](https://docs.microsoft.com/zh-cn/dotnet/api/system.valuetype?view=net-5.0)Single

- 实现

  [IComparable](https://docs.microsoft.com/zh-cn/dotnet/api/system.icomparable?view=net-5.0) [IComparable](https://docs.microsoft.com/zh-cn/dotnet/api/system.icomparable-1?view=net-5.0)<[Single](https://docs.microsoft.com/zh-cn/dotnet/api/system.single?view=net-5.0)> [IConvertible](https://docs.microsoft.com/zh-cn/dotnet/api/system.iconvertible?view=net-5.0) [IEquatable](https://docs.microsoft.com/zh-cn/dotnet/api/system.iequatable-1?view=net-5.0)<[Single](https://docs.microsoft.com/zh-cn/dotnet/api/system.single?view=net-5.0)> [IFormattable](https://docs.microsoft.com/zh-cn/dotnet/api/system.iformattable?view=net-5.0)

## 枚举

### 位运算

```CS
[Flags]//[Flags]表示该枚举可以支持C#位运算, 而枚举的每一项值, 我们用2的n次方来赋值, 这样表示成二进制时刚好是1 = 0001, 2 = 0010, 4 = 0100, 8 = 1000等, 每一位表示一种权限, 1表示有该权限, 0表示没有.
public enum ActionMachineEvent
{
    None = 0b0000_0000,
    FrameChanged = 0b0000_0001,
    StateChanged = 0b0000_0010,
    AnimChanged = 0b0000_0100,
    HoldAnimDuration = 0b0000_1000,
    All = 0b1111_1111
}
public ActionMachineEvent eventTypes { get; protected set; }
```

```cs
//更新操作 加法
eventTypes |= ActionMachineEvent.FrameChanged;
//移除操作 减法
eventTypes ^= ActionMachineEvent.FrameChanged;
 //状态改变
eventTypes |= (ActionMachineEvent.StateChanged | ActionMachineEvent.AnimChanged);
```

# 动画

## 获取Animator动画的时间

```cs
 yield return new WaitForSeconds(0.1f);//稍微延迟一会再读取动画时间

 var sawAnimState = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);//读取当前动画事件的时间

 yield return new WaitForSeconds(sawAnimState.length);//动画执行完成后
```

## 获取当前动画播放的帧



```cs
可以通过 **clip.frameRate**，**clip.length**，及**normalizedTime**来计算出具体的帧。

//当前动画机播放时长

currentTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;

//动画片段长度

float length = anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;

//获取动画片段帧频

float frameRate = anim.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate;

//计算动画片段总帧数

float totalFrame = length / (1 / frameRate);

//计算当前播放的动画片段运行至哪一帧

int currentFrame = (int)(Mathf.Floor(totalFrame * clipTime) % totalFrame);

Debug.Log(" Frame: " + currentFrame + “/” + totalFrame);
```



# Math

## Mathf.Clamp

public static int **Clamp** (int **value**, int **min**, int **max**);

**参数**

| **value** | 要限制在最小到最大范围内的整数点值 |
| --------- | ---------------------------------- |
| **min**   | 要比较的最小整数点值。             |
| **max**   | 要比较的最大整数点值。             |

**返回**

**int** 最小值到最大值之间的整数结果。

**描述**

将给定值钳制在给定最小整数和最大整数值定义的范围之间。如果在最小和最大范围内，则返回给定值。

如果给定值小于最小值，则返回最小值。如果给定值大于最大值，则返回最大值。min 和 max 参数包括在内。例如，Clamp(10, 0, 5) 将返回最大参数为 5，而不是 4。

# 物理

## 移动

### 通过Transform组件移动物体

  Transform 组件用于描述物体在空间中的状态，它包括 位置(position)， 旋转(rotation)和 缩放(scale)。 其实所有的移动都会导致position的改变，这里所说的通过Transform组件来移动物体，指的是直接操作Transform来控制物体的位置(position)。

####  Transform.Translate

​    该方法可以将物体从当前位置，移动到指定位置，并且可以选择参照的坐标系。 当需要进行坐标系转换时，可以考虑使用该方法以省去转换坐标系的步骤。

​    *public function Translate(translation: Vector3, relativeTo: Space = Space.Self): void;*

####  Transform.position

​    有时重新赋值position能更快实现我们的目标。

### Vector

####  Vector3.Lerp, Vector3.Slerp, Vector3.MoveTowards

   Vector3 既可以表示三维空间中的一个点，也可以表示一个向量。这三个方法均为插值方法， Lerp为线性插值，Slerp为球形插值， MoveTowards在Lerp的基础上增加了限制最大速度功能。 当需要从指定A点移动到B点时,可以考虑时候这些方法。

####  Vector3.SmoothDamp

  该方法是可以平滑的从A逐渐移动到B点，并且可以控制速度，最常见的用法是相机跟随目标。

### 通过Rigidbody组件移动物体

  Rigidbody组件用于模拟物体的物理状态，比如物体受重力影响，物体被碰撞后的击飞等等。

  注意：关于Rigidbody的调用均应放在FixedUpdate方法中，该方法会在每一次执行物理模拟前被调用。

#### Rigidbody.velocity

   设置刚体速度可以让物体运动并且忽略静摩擦力，这会让物体快速从静止状态进入运动状态。

#### Rigidbody.AddForce

   给刚体添加一个方向的力，这种方式适合模拟物体在外力的作用下的运动状态。

#### Rigidbody.MovePosition

   刚体受到物理约束的情况下，移动到指定点。

### 通过CharacterController组件移动物体

CharacterController用于控制第一人称或第三人称角色的运动，使用这种方式可以模拟人的一些行为，比如限制角色爬坡的最大斜度,步伐的高度等。

####   CharacterController.SimpleMove

  用于模拟简单运动，并且自动应用重力，返回值表示角色当前是否着地。

####   CharacterController.Move

   模拟更复杂的运动,重力需要通过代码实现，返回值表示角色与周围的碰撞信息。

## 旋转

### 2D 物体旋转指向目标

#### Mathf.Atan2 反正切

static function Atan2 (y : float, x : float) : float

 

Description描述

Returns the angle in radians whose Tan is y/x.

以弧度为单位计算并返回 y/x 的反正切值。返回值表示相对直角三角形对角的角，其中 x 是临边边长，而 y 是对边边长。

Return value is the angle between the x-axis and a 2D vector starting at zero and terminating at (x,y).

返回值为x轴和一个零点起始在(x,y)结束的2D向量的之间夹角。

// Usually you use transform.LookAt for this.

//通常使用transform.lookAt.

// But this can give you more control over the angle

//但这可以给你更多的对角度的控制

#### Mathf.Rad2Deg 弧度转度

绕axis轴旋转angle，创建一个旋转。

```cs
Vector2 direction = target.transform.position - transform.position;

float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

//这里还可以使用 Quaternion.Lerp来实现平滑旋转

transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
```



## 碰撞检测

### OnTriggeredStay2D 问题

碰撞条件：

- IsTrigger 一方需要勾选
- 一方必须要有刚体

发现：

- 多个碰撞体进入范围中，每帧运行次数增多
- 2D 3D一定要分清！！！

### Physics.BoxCastAll

```cs
Physics.BoxCastAll //有发射距离,默认无限
   
public static RaycastHit[] BoxCastAll (Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation= Quaternion.identity, float maxDistance= Mathf.Infinity, int layerMask= DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction= QueryTriggerInteraction.UseGlobal);
参数
center	盒体的中心。
halfExtents	盒体各个维度大小的一半。
direction	投射盒体的方向。
orientation	盒体的旋转。
maxDistance	投射的最大长度。
layermask	层遮罩，用于在投射胶囊体时有选择地忽略碰撞体。
queryTriggerInteraction	指定该查询是否应该命中触发器。
```

# 输入

## 手柄

### 测试输入

```cs
usingUnityEngine;

usingSystem.Collections;

usingSystem;

*///<summary>*

*///**测试**游**戏**手柄**键值*

*///</summary>*

publicclass**Test**:MonoBehaviour

{

privatestringcurrentButton;*//当前按下的按**键*

 

*//Usethisforinitialization*

voidStart()

{

 

}

*//Updateiscalledonceperframe*

void**Update**()

{

varvalues=Enum.GetValues(typeof(KeyCode));*//存**储**所有的按**键*

for(intx=0;x<values.Length;x++)

{

if(Input.GetKeyDown((KeyCode)values.GetValue(x)))

{

currentButton=values.GetValue(x).ToString();*//遍**历**并**获**取当前按下的按**键*

}

}

}

*//Showsomedata*

void**OnGUI**()

{

GUI.TextArea(newRect(0,0,250,40),"CurrentButton:"+currentButton);*//使用GUI在屏幕上面**实时**打印当前按下的按**键*

}

}
```

## 鼠标

### 检测鼠标移入移出

- ```cs
  
  
  **IPointerEnterHandler**
  
  该接口实现方法如下：
  
  public void OnPointerEnter(PointerEventData eventData)
   {
     //当鼠标光标移入该对象时触发
   }
  
  
  **IPointerExitHandler**
  
  该接口实现方法如下：
  
  public void OnPointerExit(PointerEventData eventData)
   {
     //当鼠标光标移出该对象时触发
   }
  ```

  

### 检测鼠标点击到哪些物体

```cs
Physics2D.OverlapPointAll

此函数用于检测2D场景中某个位置（Vector2 point）处所有的碰撞体

**返回值** 

Collider2D[] ：

**参数**  

point 被检测的场景中的点

layerMask 在指定的层中检查对象

minDepth 最小深度（z轴）

maxDepth 最大深度

**案例**

检测鼠标点击到哪些物体：

Collider2D[] col = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition));

 

2D射线需要碰撞体

 if(Input.GetMouseButtonDown(0))

  {

   Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);

   RaycastHit2D hit = Physics2D.Raycast (new Vector2(myRay.origin.x, myRay.origin.y), Vector2.down);

   if(!hit.Equals(null)){

   if (hit.collider.tag.Equals("建造点")) 

   {

    Debug.Log("建造"); 

   }

   }

  }
```

### 获取鼠标位置

```cs
Vector3 screenPosition;*//**将物体从世界坐标转换为屏幕坐标*

Vector3 mousePositionOnScreen;*//**获取到点击屏幕的屏幕坐标*

Vector3 mousePositionInWorld;*//**将点击屏幕的屏幕坐标转换为世界坐标*

void Update()

{

MouseFollow();

}

void MouseFollow()

{

*//**获取鼠标在相机中（世界中）的位置，转换为屏幕坐标；*

screenPosition =     Camera.main.WorldToScreenPoint(transform.position);

*//**获取鼠标在场景中坐标*

mousePositionOnScreen =     Input.mousePosition;

*//**让场景中的**Z=**鼠标坐标的**Z*

mousePositionOnScreen.z =     screenPosition.z;

*//**将相机中的坐标转化为世界坐标*

mousePositionInWorld =     Camera.main.ScreenToWorldPoint(mousePositionOnScreen);

*//**物体跟随鼠标移动*

*//transform.position =     mousePositionInWorld;*

*//**物体跟随鼠标**X**轴移动*

transform.position = new     Vector3(mousePositionInWorld.x,transform.position.y,transform.position.z);

 

#改变鼠标指针 Cursor.SetCursor

SetCursor(

Texture2D texture, //材质 既你想要变更成的图片

Vector2 hotspot, // 响应区域 (vector2.zero)

CursorMode cursorMode//渲染形式，auto为平台自适应显示

）
```



# 资源



# Inspector

## layerMask参数

```cs

Raycast (ray : Ray, out hitInfo : RaycastHit, distance : float = Mathf.Infinity, layerMask : int = kDefaultRaycastLayers)

     RaycastHit hit;
     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
     if (Physics.Raycast(ray, out hit, 1000, 1<<LayerMask.NameToLayer("Ground")))
     {
         Log.Debug(" hit :" + hit.point );
     }
     else
     {
        Log.Debug("ray cast null!");
     }
 int layer = LayerMask.NameToLayer(“Ground”); //假设=10

LayerMask.GetMask((“Ground”); //相当于 1 << 10

其实很简单：

1 << 10 打开第10的层。 等价于【1 << LayerMask.NameToLayer(“Ground”);】 也等价于【 LayerMask.GetMask((“Ground”);】

~(1 << 10) 打开除了第10之外的层。

~(1 << 0) 打开所有的层。

(1 << 10) | (1 << 8) 打开第10和第8的层。等价于【 LayerMask.GetMask((“Ground”, “Wall”);】

在代码中使用时如何开启某个Layers？

LayerMask mask = 1 << 你需要开启的Layers层。

LayerMask mask = 0 << 你需要关闭的Layers层。

举几个个栗子：

LayerMask mask = 1 << 2; 表示开启Layer2。

LayerMask mask = 0 << 5;表示关闭Layer5。

LayerMask mask = 1<<2|1<<8;表示开启Layer2和Layer8。

LayerMask mask = 0<<3|0<<7;表示关闭Layer3和Layer7。

上面也可以写成：

LayerMask mask = ~（1<<3|1<<7）;表示关闭Layer3和Layer7。

LayerMask mask = 1<<2|0<<4;表示开启Layer2并且同时关闭Layer4.
```

# Scene

## 坐标系

一 Unity3D中一共有四种坐标系，分别为：

 

### 1.世界坐标系（WorldSpace）

在场景中，有一个坐标原点（0，0，0） ，所有物体都是根据与它的相对位置来得到自己的世界坐标，在挂载在自身的脚本中通过 transform.position 来获得 世界坐标，在其他游戏物体的脚本中通过引入本游戏物体，例如声明一个游戏物体public GameObject go ，通过go.transform.position来获得此游戏物体的世界坐标。

 

### 2.本地坐标（LocalSpace）

游戏物体除了有世界坐标，还有一个自身的坐标，也称为本地坐标。在游戏物体自身需要旋转或者移动的时候，有时候可能会用到自身坐标，例如

 

transform.Translate（transform.up）朝着物体的上方运动（这里还需要注意一点，transform.up 是朝着物体的上方运动，而不是朝着世界坐标中的上方运动，Vector3.up朝着世界坐标中的上方运动，因为Vector3.up恒等于（0,1,0），而transform.up是有可能随时变化的）

 

### 3.屏幕坐标（ScreenSpace）

顾名思义，是物体在电脑屏幕中的坐标，这里的屏幕是在Game视图中，左下角为原点（0,0）右上角为（Screen.width,Screen.height）,width是Game视图窗口大小的宽度，height是Game窗口大小的高度,Z值是摄像机世界坐标取反。鼠标的位置坐标属于屏幕坐标。

 

### 4.视口坐标(ViewPortSpace)

摄像机的前面有一个长方形的小框子，那个即为视口，左下角为坐标原点（0,0），右上角为（1,1），Z轴和屏幕坐标一样，指向你为Z轴正方向，Z轴的值是摄像机的世界坐标取反。

 

### 相互转换

 

1.世界坐标转换为本地坐标：通常情况下相等。

 

2.世界坐标转换为屏幕坐标：Camera.main.WorldToScreenPoint(),屏幕坐标转换为世界坐标：Camera.main.ScreenToWorldPoint();

 

3.世界坐标转换为视口坐标:：Camera.main.WorldToViewportPoint(),视口坐标转换为世界坐标：Camera.main.ViewportToWorldPoint();

 

4.视口坐标转换为屏幕坐标：Camera.main.ViewportToScreenPoint(),屏幕坐标转换为视口坐标：Camera.main.ScreenToViewportPoint();



# 问题

## LayerMask无效的原因

 指定在physic . raycast中使用的图层,危险的类型转

版权

今天上午修改代码时莫名其妙改出了一个小bug，Raycast射出的射线射中已经被LayerMask屏蔽的碰撞器上依然回返回该碰撞器。经过反复测试，才终于搞明白bug的原因

 

错误代码：

 

bool isCollider = Physics.Raycast(ray, out hit,LayerMask.GetMask("Map"));

看起来一点毛病都没有，实际上，这里犯了误用重载的错误，也就是说，Raycast的重载里，并没有对应的Physics.Raycast(Ray ray, out RayCastHit hitInfo, int layerMask)，那为什么编译器没有报错？那是因为，Raycast有这样的一个重载：

 

bool Physics.Raycast(Ray ray, out RayCastHit hitInfo, float maxDistance)

 

是的……按照错误代码的格式输入的话，layerMask被当做了maxDistance……所以我们需要找到一个合适的格式

 

正确代码：

 

bool isCollider = Physics.Raycast(ray, out hit,1000f,LayerMask.GetMask("Map"));

## 不能在主线程调用

After lots of searching, i came to the solution: Using the Loom Unity Package from: [Unity Gems entry about threading](http://unitygems.com/threads/) and using it like mentioned in [Unity answers entry about threading](http://answers.unity3d.com/questions/305882/how-do-i-invoke-functions-on-the-main-thread.html):

```cs
    void Start()
    {
        var tmp = Loom.Current;
        ...
    }
    //Function called from other Thread
    public void NotifyFinished(int status)
    {
        Debug.Log("Notify");
        try
        {
            if (status == (int)LevelStatusCode.OK)
            {
                Loom.QueueOnMainThread(() =>
                {
                    PresentNameInputController();
                });
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }
```

