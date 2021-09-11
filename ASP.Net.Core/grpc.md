# ProtocoBuff

    标量类型·数值型
    数值型有很多种形式：double，float，int32，int64，Uint32，uint64，sint32，sint64，fixed32，fixed64，sfixed32，sfixed64。根据需要选择对应的数值类型。
  
    ·布尔型
    -bool型可以有True和False两个值。

    ·字符串
    -string表示任意长度的文本，但是它必须包含的是UTF-8编码或，位ASC的文本，长度不可超过23z。

    ·字节型
    -bytes可表示任意的byte数组序列，但是长度也不可以超过232，最后是由你来决定如何解释这些bytes。例如你可以使用这个类型来表示一个图片。

~~~ protobuf
    syntax = "proto3";
    message First{
        int32 id=1;
        string name=2;
        repeated phone_number=3;
    }
~~~