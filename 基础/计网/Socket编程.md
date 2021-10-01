# Socket编程

## 2种传输层服务的socket类型：

```
TCP：可靠的、字节流的服务
UDP：不可靠（数据UDP数据报）服务
```

## 数据结构sockaddr_in

```C#
IP地址和port捆绑关系的数据结构（标示进程的端节点）

struct sockaddr_in{
short sin_family；//AF_INET u_short sin_port；//port 
struct in_addr sin_addr；
//IP address，unsigned long 
char sin_zero[8]；//align
}
```

#### **套接字是同一台主机内应用层与运输层之间的接口 由于该套接字是建立网络应用程序的可编程接口，因此套接字也被称为应用程序和网络之间的应用程序编程接口**

