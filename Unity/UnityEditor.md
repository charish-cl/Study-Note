# EditorWindow

继承自 ScriptableObject
制作继承自ScriptableObject的一个例子资源,保存在一个路径下面,
CreateAssetMenu可以在Project里面右键快速创建

```cs
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Tools/Create ExampleAsset Instance")]
public class ExampleAsset : ScriptableObject
{

    [MenuItem("Tools/Create ExampleAsset")]
    static void CreateExampleAsset()
    {
        var exampleAsset = CreateInstance<ExampleAsset>();

        AssetDatabase.CreateAsset(exampleAsset,"Assets/Editor/ExampleAsset.asset");
        AssetDatabase.Refresh();
    }
}
```

# 数据

## 读取数据

```cs
 var exampleAsset =AssetDatabase.LoadAssetAtPath<ExampleAsset>("Assets/Editor/ExampleAsset.asset");
```

# 特性

## MenuItem

是在Unity editer上侧的菜单栏或上下文菜单上追加项目所需的功能.

在菜单前面有个对号,后面 %#g 表示快捷键

```cs
  [MenuItem("CustomMenu/Example  %#g")]
  static void Example()
  {
      var menuPath = "CustomMenu/Example";
      var _checked = Menu.GetChecked(menuPath);
      Menu.SetChecked(menuPath, !_checked);
  }
```



# 特殊文件夹

- Assembly-CSharp.dll 中不能 使用 UnityEditor.dll

- UnityEditor会被unity编译成Assembly-CSharp-Editor.dll

- 在「Standard Assets」「Pro Standard Assets」「Plugins」生成Editor文件夹,则会被unity编译成 Assembly-CSharp-Editor-firstpass.dll 类库(一般不要使用)

- 如果不在Editor中编写的脚本想使用Editor模块的代码,需要使用unity的宏编译,使用了宏编译,运行时会自动将宏编译以及宏编译内的代码移除

  

  ```cs
      using UnityEngine;
      #if UNITY_EDITOR
      using UnityEditor;
      #endif
  
      public class NewBehaviourScript : MonoBehaviour
      {
          void OnEnable ()
          {
              #if UNITY_EDITOR
              EditorWindow.GetWindow<ExampleWindow> ();
              #endif
          }
  }
  ```

Editor Default Resources 文件夹,存放只有Editor模块可以使用的资源,类似于Resources文件夹,可以使用如下代码快速获取assets资源,这个文件夹下的资源不会被运行时使用(***这里所有的运行时,同一指打包后的app使用的脚本或者资源\***)

```cs
  var tex = EditorGUIUtility.Load ("logo.png") as Texture;
```

- 查看所有内置资源

```cs
[MenuItem("Tools/Test1")]
static void GetBultinAssetNames()
{
    var flags = BindingFlags.Static | BindingFlags.NonPublic;
    var info = typeof(EditorGUIUtility).GetMethod("GetEditorAssetBundle", flags);
    var bundle = info.Invoke(null, new object[0]) as AssetBundle;

    foreach (var n in bundle.GetAllAssetNames())
    {
        Debug.Log(n);
    }

}
```



# 布局

## 水平-垂直

```cs
using(new EditorGUILayout.HorizontalScope())

{

*//左半部分*

using(EditorGUILayout.VerticalScopevScope=newEditorGUILayout.VerticalScope(GUILayout.Width(EditorGUIUtility.currentViewWidth*0.5f)))

{

GUI.backgroundColor=Color.white;

Rectrect=vScope.rect;

rect.height=codeWindow.position.height;

GUI.Box(rect,"");

}

*//右半部分*
using(new EditorGUILayout.VerticalScope(GUILayout.Width(EditorGUIUtility.currentViewWidth*0.5f)))
{

    DrawCodeGenTitle();

    DrawCodeGenToolBar();
}
}
```

