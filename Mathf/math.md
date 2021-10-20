# [数学语法](https://www.jianshu.com/p/a0aa94ef8ab2)

$$
\begin{bmatrix}
{a_{11}}&{a_{12}}&{\cdots}&{a_{1n}}\\
{a_{21}}&{a_{22}}&{\cdots}&{a_{2n}}\\
{\vdots}&{\vdots}&{\ddots}&{\vdots}\\
{a_{m1}}&{a_{m2}}&{\cdots}&{a_{mn}}\\
\end{bmatrix}
$$



##### 4.5 方程组

- 需要cases环境：起始、结束处以{cases}声明

举例


$$
\begin{cases}
a_1x+b_1y+c_1z=d_1\\
a_2x+b_2y+c_2z=d_2\\
a_3x+b_3y+c_3z=d_3\\
\end{cases}
$$




# 向量

## 向量内积的几何意义

内积（点乘）的几何意义包括：

1. 表征或计算两个向量之间的夹角
2. b向量在a向量方向上的投影

推导过程如下，首先看一下向量组成：

![img](https://images2017.cnblogs.com/blog/810440/201709/810440-20170926172349104-213873300.png)

定义向量**c**：

$$
c=a-b
$$
根据三角形余弦定理（这里a、b、c均为向量，下同）有：
$$
c^2=a^2+b^2-2|a||b|cos⁡(θ)
$$
根据关系**c**=**a**-**b**有：
$$
(a-b)*(a-b)=a^2+b^2-2ab=a^2+b^2-2abcos(θ)
$$
即：

$$
a∙b=|a||b|cos⁡(θ)
$$


# 线性方程

## 何为线性

Tihis is linear equation  and that word linear got the letters for line in it.

线性就是直线的意思

## 几何解释


$$
x\begin{bmatrix}
{2}\\{-1}\\{0}
\end{bmatrix}+y\begin{bmatrix}
{2}\\{-1}\\{0}
\end{bmatrix}+z\begin{bmatrix}
{2}\\{-1}\\{0}
\end{bmatrix}=\begin{bmatrix}
{2}\\{-1}\\{0}
\end{bmatrix}
$$
x,y,z右边的三个列向量通过求解得出右边的向量"b",解为三条直线交点



如果三个列向量同处一个平面,那么它的组合也在该平面,不管怎么组合,都得不出它们平面以外的向量

因此当b处于平面内,方程组有解

反之不在平面内,这种情形称作奇异,矩阵不可逆



**将A乘以x,看作A各列的线性组合**

**A times x is a combimation of the columns of A.**
$$
Ax=b\\\\
\begin{bmatrix}
{2}&{5}\\{1}&{3}
\end{bmatrix}+\begin{bmatrix}
{1}\\{2}
\end{bmatrix}=1\begin{bmatrix}
{2}\\{1}
\end{bmatrix}+2\begin{bmatrix}
{5}\\{3}
\end{bmatrix}=\begin{bmatrix}
{12}\\{7}
\end{bmatrix}
$$

## 矩阵消元

**矩阵乘法可以用列,行来求,或直接用每一个元素**

**we will have by columns,by rows,by each entry at a time.**

eg:

3*3矩阵A和B相乘得C

A中的第二行与B中第三列点乘相加等于C中第二行第三列的元素
$$
\begin{bmatrix}
{1}&{0}&{1}\\
{-3}&{1}&{0}\\
{0}&{0}&{1}\\
\end{bmatrix}
\begin{bmatrix}
{1}&{2}&{1}\\
{3}&{8}&{1}\\
{0}&{4}&{1}\\
\end{bmatrix}=
\begin{bmatrix}
{1}&{2}&{1}\\
{0}&{2}&{-2}\\
{0}&{4}&{1}\\
\end{bmatrix}
$$
**矩阵乘以[1,0,0]等于取第一行而不取其他行(第一行不变)**

**that's just the right thing that takes one of that row and none of the other rows**
$$
\begin{bmatrix}
{1}&{0}&{0}
\end{bmatrix}*A=B
$$
不断的进行初等变换得到最后的矩阵 
$$
{E_{32}}代表每次变换\\\
{E_{32}}({E_{21}}A){\cdots}=U
$$
**行变换左乘,列变换右乘**
$$
\begin{bmatrix}
{0}&{1}\\
{1}&{0}\\
\end{bmatrix}
\begin{bmatrix}
{a}&{b}\\
{c}&{d}\\
\end{bmatrix}=\begin{bmatrix}{c}&{d}\\
{a}&{b}\\
\end{bmatrix}\\\



\begin{bmatrix}
{a}&{b}\\
{c}&{d}\\
\end{bmatrix}\begin{bmatrix}
{0}&{1}\\
{1}&{0}\\
\end{bmatrix}=\begin{bmatrix}{d}&{c}\\
{b}&{a}\\
\end{bmatrix}
$$

**矩阵乘法可以用列,行,用列乘以行,分块**

# 逆矩阵(Inversses)

左逆=右逆
$$
A^{-1}A=I=AA^{-1}
$$
