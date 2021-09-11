[Nlog使用](https://www.cnblogs.com/zhangchengye/p/6297685.html)
# Nlog
    NLog是什么

    NLog是一个基于.NET平台编写的类库，我们可以使用NLog在应用程序中添加极为完善的跟踪调试代码。
    NLog是一个简单灵活的.NET日志记录类库。通过使用NLog，我们可以在任何一种.NET语言中输出带有上下文的（contextual information）调试诊断信息，根据喜好配置其表现样式之后发送到一个或多个输出目标（target）中。
    NLog的API非常类似于log4net，且配置方式非常简单。NLog使用路由表（routing table）进行配置，这样就让NLog的配置文件非常容易阅读，并便于今后维护。
    NLog遵从BSD license，即允许商业应用且完全开放源代码。任何人都可以免费使用并对其进行测试，然后通过邮件列表反馈问题以及建议。
    NLog支持.NET、C/C++以及COM interop API，因此我们的程序、组件、包括用C++/COM 编写的遗留模块都可以通过同一个路由引擎将信息发送至NLog中。
    简单来说Nlog就是用来记录项目日志的组件

# NLog日志输出目标

    文件 比如TXT、Excel
    文本控制台
    Email
    数据库
    网络中的其它计算机（通过TCP或UDP）
    基于MSMQ的消息队列
    Windows系统日志
# 简单Demo
~~~ xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      xsi:schemaLocation="http://www.nlog-project.org/schemas/NLog.xsd NLog.xsd"
      autoReload="true"
      throwExceptions="false"
      internalLogLevel="Off" 
      internalLogFile="c:\temp\nlog-internal.log">
  <variable name="myvar" value="myvalue"/>
  <targets>
    <target xsi:type="File" name="SimpleDemoFile" fileName="../../../Logs/SimpleDemo.txt" layout="${message}" encoding="UTF-8"/>
  </targets>
  <rules>
    <logger name="SimpleDemo" level="Error" writeTo="SimpleDemoFile"/>
  </rules>
</nlog>
~~~ 