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

