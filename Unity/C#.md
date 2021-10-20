# 关键字

## params

使用 params 关键字可以指定采用数目可变的参数的[方法参数](https://docs.microsoft.com/zh-cn/dotnet/csharp/language-reference/keywords/method-parameters)。 参数类型必须是一维数组。

在方法声明中的 params 关键字之后不允许有任何其他参数，并且在方法声明中只允许有一个 params 关键字。

如果 params 参数的声明类型不是一维数组，则会发生编译器错误 [CS0225](https://docs.microsoft.com/zh-cn/dotnet/csharp/misc/cs0225)。

使用 params 参数调用方法时，可以传入：

- 数组元素类型的参数的逗号分隔列表。
- 指定类型的参数的数组。
- 无参数。     如果未发送任何参数，则 params 列表的长度为零。

 

来自 <https://docs.microsoft.com/zh-cn/dotnet/csharp/language-reference/keywords/params

## $ 

- 字符串内插

  ```cs
  string name = "Mark";
  
  var date = DateTime.Now;
  
   
  
  // Composite formatting:
  
  Console.WriteLine("Hello, {0}! Today is {1}, it's {2:HH:mm} now.", name, date.DayOfWeek, date);
  
  // String interpolation:
  
  Console.WriteLine($"Hello, {name}! Today is {date.DayOfWeek}, it's {date:HH:mm} now.");
  ```

## In

in 修饰符记录：

新版C# 新增加的 in 修饰符：保证发送到方法当中的数据不被更改（值类型），当in 修饰符用于引用类型时，可以改变变量的内容，单不能更改变量本身。

个人理解：in 修饰符传递的数据，在方法里就是只读的 ，不能进行任何更改。

## ??

a ?? b 当a为null时则返回b，a不为null时则返回a本身。



# 语法糖

## 类



## 属性

1. 属性可以用=初始化

## 索引器

```cs
publicColorPrototypethis[stringkey]

{

    get{return_colors[key];}

    set{_colors.Add(key,value);}

}
```

## [**枚举**](https://www.cnblogs.com/willick/p/csharp-enum-superior-tactics.html)

文章开头先给大家出一道面试题：

在设计某小型项目的数据库（假设用的是 MySQL）时，如果给用户表（User）添加一个字段（Roles）用来存储用户的角色，你会给这个字段设置什么类型？提示：要考虑到角色在后端开发时需要用枚举表示，且一个用户可能会拥有多个角色。

映入你脑海的第一个答案可能是：varchar 类型，用分隔符的方式来存储多个角色，比如用 1|2|3 或 1,2,3 来表示用户拥有多个角色。当然如果角色数量可能超过个位数，考虑到数据库的查询方便（比如用 INSTR 或 POSITION 来判断用户是否包含某个角色），角色的值至少要从数字 10 开始。方案是可行的，可是不是太简单了，有没有更好的方案？更好的回答应是整型（int、bigint 等），优点是写 SQL 查询条件更方便，性能、空间上都优于 varchar。但整型毕竟只是一个数字，怎么表示多个角色呢？此时想到了二进制位操作的你，心中应该早有了答案。且保留你心中的答案，接着看完本文，或许你会有意外的收获，因为实际应用中可能还会遇到一连串的问题。为了更好的说明后面的问题，我们先来回顾一下枚举的基础知识。

### **枚举基础**

枚举类型的作用是限制其变量只能从有限的选项中取值，这些选项（枚举类型的成员）各自对应于一个数字，数字默认从 0 开始，并以此递增。例如：

```cs
public enum Days

{

Sunday, Monday, Tuesday, *// ...*

*}*

其中 Sunday 的值是 0，Monday 是 1，以此类推。为了一眼能看出每个成员代表的值，一般推荐显示地将成员值写出来，不要省略：

public enum Days

{

Sunday = 0, Monday = 1, Tuesday = 2, *// ...*

*}*

C# 枚举成员的类型默认是 int 类型，通过继承可以声明枚举成员为其它类型，比如：

public enum Days : byte

{

Monday = 1,

Tuesday = 2,

Wednesday = 3,

Thursday = 4,

Friday = 5,

Saturday = 6,

Sunday = 7

}

枚举类型一定是继承自 byte、sbyte、short、ushort、int、uint、long 和 ulong 中的一种，不能是其它类型。下面是几个枚举的常见用法（以上面的 Days 枚举为例）：

*/**/ 枚举转字符串*

string foo = Days.Saturday.ToString(); *// "Saturday"*

string foo = Enum.GetName(typeof(Days), 6); *// "Saturday"*

*// 字符串转枚举*

Enum.TryParse("Tuesday", out Days bar); *// true, bar = Days.Tuesday*

(Days)Enum.Parse(typeof(Days), "Tuesday"); *// Days.Tuesday*

 

*// 枚举转数字*

byte foo = (byte)Days.Monday; *// 1*

*// 数字转枚举*

Days foo = (Days)2; *// Days.Tuesday*

 

*// 获取枚举所属的数字类型*

Type foo = Enum.GetUnderlyingType(typeof(Days))); *// System.Byte*

 

*// 获取所有的枚举成员*

Array foo = Enum.GetValues(typeof(MyEnum);

*// 获取所有枚举成员的字段名*

string[] foo = Enum.GetNames(typeof(Days));

另外，值得注意的是，枚举可能会得到非预期的值（值没有对应的成员）。比如：

Days d = (Days)21; *//* *不会报错*

Enum.IsDefined(typeof(Days), d); *//* *false*

即使枚举没有值为 0 的成员，它的默认值永远都是 0。

var z = default(Days); *// 0*

枚举可以通过 Description、Display 等特性来为成员添加有用的辅助信息，比如：

public enum ApiStatus

{

[Description("成功")]

OK = 0,

[Description("资源未找到")]

NotFound = 2,

[Description("拒绝访问")]

AccessDenied = 3

}

 

static class EnumExtensions

{

public static string GetDescription(this Enum val)

{

var field = val.GetType().GetField(val.ToString());

var customAttribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

if (customAttribute == null) { return val.ToString(); }

else { return ((DescriptionAttribute)customAttribute).Description; }

}

}

 

static void Main(string[] args)

{

Console.WriteLine(ApiStatus.Ok.GetDescription()); *// "成功"*

*}
```

上面这些我认为已经包含了大部分我们日常用到的枚举知识了。下面我们继续回到文章开头说的用户角色存储问题。

### **用户角色存储问题**

我们先定义一个枚举类型来表示两种用户角色：

public enum Roles

{

Admin = 1,

Member = 2

}

这样，如果某个用户同时拥有 Admin 和 Member 两种角色，那么 User 表的 Roles 字段就应该存 3。那问题来了，此时若查询所有拥有 Admin 角色的用户的 SQL 该怎么写呢？对于有基础的程序员来说，这个问题很简单，只要用位操作符逻辑与（‘&’）来查询即可。

SELECT * FROM `User` WHERE `Roles` & 1 = 1;

同理，查询同时拥有这两种角色的用户，SQL 语句应该这么写：

SELECT * FROM `User` WHERE `Roles` & 3 = 3;

对这条 SQL 语句用 C# 来实现查询是这样的（为了简单，这里使用了 Dapper）：

public class User

{

public int Id { get; set; }

public Roles Roles { get; set; }

}

 

connection.Query<User>(

"SELECT * FROM `User` WHERE `Roles` & @roles = @roles;",

new { roles = Roles.Admin | Roles.Member });

对应的，在 C# 中要判断用户是否拥有某个角色，可以这么判断：

```cs
/**/ 方式一*

if ((user.Roles & Roles.Admin) == Roles.Admin)

{

*// 做管理员可以做的事情*

}

*// 方式二*

if (user.Roles.HasFlag(Roles.Admin))

{

*// 做管理员可以做的事情*

}
```

同理，在 C# 中你可以对枚举进行任意位逻辑运算，比如要把角色从某个枚举变量中移除：

var foo = Roles.Admin | Roles.Member;

var bar = foo & ~Roles.Admin;

这就解决了文章前面提到的用整型来存储多角色的问题，不论数据库还是 C# 语言，操作上都是可行的，而且也很方便灵活。

### **枚举的 Flags 特性**

下面我们提供一个通过角色来查询用户的方法，并演示如何调用，如下：

public IEnumerable<User> GetUsersInRoles(Roles roles)

{

_logger.LogDebug(roles.ToString());

_connection.Query<User>(

"SELECT * FROM `User` WHERE `Roles` & @roles = @roles;",

new { roles });

}

 

*// 调用*

*_repository.GetUsersInRoles(Roles.Admin | Roles.Member);*

Roles.Admin | Roles.Member 的值是 3，由于 Roles 枚举类型中并没有定义一个值为 3 的字段，所以在方法内 roles 参数显示的是 3。3 这个信息对于我们调试或打印日志很不友好。在方法内，我们并不知道这个 3 代表的是什么。为了解决这个问题，C# 枚举有个很有用的特性：FlagsAtrribute。

[Flags]

public enum Roles

{

Admin = 1,

Member = 2

}

加上这个 Flags 特性后，我们再来调试 **GetUsersInRoles(Roles roles)** 方法时，roles 参数的值就会显示为 Admin|Member 了。简单来说，加不加 Flags 的区别是：

var roles = Roles.Admin | Roles.Member;

Console.WriteLing(roles.ToString()); *//* *"3"**，没有 Flags 特性*

Console.WriteLing(roles.ToString()); *//* *"Admin, Member"**，有 Flags 特性*

给枚举加上 Flags 特性，我觉得应当视为 C# 编程的一种最佳实践，在定义枚举时尽量加上 Flags 特性。

### **解决枚举值冲突：2 的幂**

到这，枚举类型 Roles 一切看上去没什么问题，但如果现在要增加一个角色：Mananger，会发生什么情况？按照数字值递增的规则，Manager 的值应当设为 3。

[Flags]

public enum Roles

{

Admin = 1,

Member = 2,

Manager = 3

}

能不能把 Manager 的值设为 3？显然不能，因为 Admin 和 Member 进行位的或逻辑运算（即：Admin | Member） 的值也是 3，表示同时拥有这两种角色，这和 Manager 冲突了。那怎样设值才能避免冲突呢？既然是二进制逻辑运算“或”会和成员值产生冲突，那就利用逻辑运算或的规律来解决。我们知道“或”运算的逻辑是两边只要出现一个 1 结果就会 1，比如 1|1、1|0 结果都是 1，只有 0|0 的情况结果才是 0。那么我们就要避免任意两个值在相同的位置上出现 1。根据二进制满 2 进 1 的特点，只要保证枚举的各项值都是 2 的幂即可。比如：

1: 00000001

2: 00000010

4: 00000100

8: 00001000

再往后增加的话就是 16、32、64...，其中各值不论怎么相加都不会和成员的任一值冲突。这样问题就解决了，所以我们要这样定义 Roles 枚举的值：

[Flags]

public enum Roles

{

Admin = 1,

Member = 2,

Manager = 4,

Operator = 8

}

不过在定义值的时候要在心中小小计算一下，如果你想懒一点，可以用下面这种“位移”的方法来定义：

[Flags]

public enum Roles

{

Admin  = 1 << 0,

Member  = 1 << 1,

Manager = 1 << 2,

Operator = 1 << 3

}

一直往下递增编值即可，阅读体验好，也不容易编错。两种方式是等效的，常量位移的计算是在编译的时候进行的，所以相比不会有额外的开销。

### [**总结**](<https://www.cnblogs.com/willick/archive/2020/05/14/csharp-enum-superior-tactics.html> )

本文通过一道小小的面试题引发一连串对枚举的思考。在小型系统中，把用户角色直接存储在用户表是很常见的做法，此时把角色字段设为整型（比如 int）是比较好的设计方案。但与此同时，也要考虑到一些最佳实践，比如使用 Flags 特性来帮助更好的调试和日志输出。也要考虑到实际开发中的各种潜在问题，比如多个枚举值进行或（‘|’）运算与成员值发生冲突的问题。

# 特性

## 自定义特性

```cs
[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)] 

public class AuthorAttribute : System.Attribute 
{ 

  private string name; 

  public double version; 


  public AuthorAttribute(string name) 

  { 

      this.name = name; 

      version = 1.0; 

  } 

}
```

类名 AuthorAttribute 是该特性的名称，即 Author 加上 Attribute 后缀。 由于该类派生自 System.Attribute，因此它是一个自定义特性类。 构造函数的参数是自定义特性的位置参数。 在此示例中，name 是位置参数。 所有公共读写字段或属性都是命名参数。 在本例中，version 是唯一的命名参数。 请注意，使用 AttributeUsage 特性可使 Author 特性仅对类和 struct 声明有效。

可按如下方式使用这一新特性：

```cs
[Author("P. Ackerman", version = 1.1)] 

class SampleClass 

{ 

  // P. Ackerman's code goes here... 

}
```

# 异步

## [隐藏细节](https://www.zhihu.com/question/56651792/answer/149968434)

await和async隐藏了很多复杂的细节，不了解的话你就很难正确的理解await和async。

一般我们需要异步的地方都是在进行比较耗时的操作，比如说磁盘IO、网络IO，当你以同步的方式调用系统API进行磁盘读取或者获取网络数据的时候，你的线程会阻塞在那里等待什么事也干不了，直到操作系统从底层返回IO数据。这就是为什么会有异步模式的存在。异步模式就是说在执行耗时IO API的时候线程不等待结果而是直接返回并注册一个回调函数，当操作系统完成耗时操作的时候，调用回调函数通知你IO结果。

await和async就是为了方便我们调用异步API而生的。当你await一个异步API的时候，你的await语句就是当前函数的最后一条语句，你肯定觉得我在胡说，明明很多时候await语句后面还有代码，这是因为编译器在后面做了很多工作。每个异步API都返回的是一个Task对象，当你await异步API的时候你就能获得这个Task对象。这个Task对象所代表的就是我上面说的那个异步模式的回调函数。Task对象有个函数叫ContinueWith，他接受的参数是一个delegate（delegate代表的就是某一个函数），ContinueWith的意思就是说当前Task对象代表的函数执行完后，继续执行ContinueWith注册进去的delegate。编译器就是将await语句后面的代码抽出来变成了另外一个函数，并用ContinueWith注册进await返回的那个Task对象。

所以总的流程就是，当你await一个异步API的时候，返回一个代表第二段所说的回调函数的Task对象，并将await之后的代码注册进Task对象，当前函数就执行完了立即返回，这个时候底层操作系统还在帮你进行费时的IO操作还没拿到结果，但你的函数已经返回上层调用了。当操作系统完成IO后，他就会回调那个Task对象，于是你的await指令后面的代码也就随之执行了。你仔细观察就会发现你执行await时候的线程ID和await之后代码的线程ID是不一样的，说明是两个不同的线程执行的代码，await之后代码是用一种叫做IO线程来执行的，await之前的线程叫做worker线程。

# 数据类型

## IEnumerator

- IEnumerable是IEnumerator的容器,IEnumebale is a box that contains IEnumerator inside it.
- c#中的IEnumerable是一个接口，它定义了一个方法，GetEnumerator则返回一个IEnumerator接口。这允许对集合进行只读访问，然后可以使用for-each语句来使用实现IEnumerable集合

## [Enum,Int,String的互相转换](https://www.cnblogs.com/pato/archive/2011/08/15/2139705.html)

**Enum为枚举提供基类，其基础类型可以是除 Char 外的任何整型。如果没有显式声明基础类型，则使用 Int32。编程语言通常提供语法来声明由一组已命名的常数和它们的值组成的枚举。**

**注意：枚举类型的基类型是除 Char 外的任何整型，所以枚举类型的值是整型值。**

Enum 提供一些实用的静态方法：

(1)比较枚举类的实例的方法

(2)将实例的值转换为其字符串表示形式的方法

(3)将数字的字符串表示形式转换为此类的实例的方法

(4)创建指定枚举和值的实例的方法。

举例：enum Colors { Red, Green, Blue, Yellow };

 

### **Enum-->String**

(1)利用Object.ToString()方法：如Colors.Green.ToString()的值是"Green"字符串；

(2)利用Enum的静态方法GetName与GetNames：

public static [string](http://www.cnblogs.com/pato/admin/ms-help:/MS.MSDNQTR.v90.chs/fxref_system/html/3e108182-236f-5ccb-b5ee-e91a6d09cea0.htm) GetName([Type](http://www.cnblogs.com/pato/admin/ms-help:/MS.MSDNQTR.v90.chs/fxref_system/html/abee003d-eb9f-f380-7902-6af6cb34a622.htm) enumType,[Object](http://www.cnblogs.com/pato/admin/ms-help:/MS.MSDNQTR.v90.chs/fxref_system/html/ee2c26d9-17cc-ab19-8a9c-6fca33a3c7ad.htm) value)

public static [string](http://www.cnblogs.com/pato/admin/ms-help:/MS.MSDNQTR.v90.chs/fxref_system/html/3e108182-236f-5ccb-b5ee-e91a6d09cea0.htm)[] GetNames([Type](http://www.cnblogs.com/pato/admin/ms-help:/MS.MSDNQTR.v90.chs/fxref_system/html/abee003d-eb9f-f380-7902-6af6cb34a622.htm) enumType)

例如：Enum.GetName(typeof(Colors),3))与Enum.GetName(typeof(Colors), Colors.Blue))的值都是"Blue"

Enum.GetNames(typeof(Colors))将返回枚举字符串数组。

 

### **String-->Enum**

(1)利用Enum的静态方法Parse：

public static [Object](http://www.cnblogs.com/pato/admin/ms-help:/MS.MSDNQTR.v90.chs/fxref_system/html/ee2c26d9-17cc-ab19-8a9c-6fca33a3c7ad.htm) Parse([Type](http://www.cnblogs.com/pato/admin/ms-help:/MS.MSDNQTR.v90.chs/fxref_system/html/abee003d-eb9f-f380-7902-6af6cb34a622.htm) enumType,[string](http://www.cnblogs.com/pato/admin/ms-help:/MS.MSDNQTR.v90.chs/fxref_system/html/3e108182-236f-5ccb-b5ee-e91a6d09cea0.htm) value)

例如：(Colors)Enum.Parse(typeof(Colors), "Red")

 

### **Enum-->Int**

(1)因为枚举的基类型是除 Char 外的整型，所以可以进行强制转换。

例如：(int)Colors.Red, (byte)Colors.Green

int i = Convert.ToInt32(e);

int i = (int)(object)e;

int i = (int)Enum.Parse(e.GetType(), e.ToString());

int i = (int)Enum.ToObject(e.GetType(), e);

 

### **Int-->Enum**

(1)可以强制转换将整型转换成枚举类型。

例如：Colors color = (Colors)2 ，那么color即为Colors.Blue

(2)利用Enum的静态方法ToObject。

public static [Object](http://www.cnblogs.com/pato/admin/ms-help:/MS.MSDNQTR.v90.chs/fxref_system/html/ee2c26d9-17cc-ab19-8a9c-6fca33a3c7ad.htm) ToObject([Type](http://www.cnblogs.com/pato/admin/ms-help:/MS.MSDNQTR.v90.chs/fxref_system/html/abee003d-eb9f-f380-7902-6af6cb34a622.htm) enumType,[int](http://www.cnblogs.com/pato/admin/ms-help:/MS.MSDNQTR.v90.chs/fxref_system/html/ed425922-7a7b-5232-1fbc-5e4ac9680de6.htm) value)

例如：Colors color = (Colors)Enum.ToObject(typeof(Colors), 2)，那么color即为Colors.Blue

 

### **判断某个整型是否定义在枚举中的方法：Enum.IsDefined**

public static [bool](http://www.cnblogs.com/pato/admin/ms-help:/MS.MSDNQTR.v90.chs/fxref_system/html/ff35b1f1-386c-370b-2c36-a48e7dcbc147.htm) IsDefined([Type](http://www.cnblogs.com/pato/admin/ms-help:/MS.MSDNQTR.v90.chs/fxref_system/html/abee003d-eb9f-f380-7902-6af6cb34a622.htm) enumType,[Object](http://www.cnblogs.com/pato/admin/ms-help:/MS.MSDNQTR.v90.chs/fxref_system/html/ee2c26d9-17cc-ab19-8a9c-6fca33a3c7ad.htm) value)

例如：Enum.IsDefined(typeof(Colors), n))