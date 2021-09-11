## Tinkingli
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

