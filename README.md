# dllRemoteInjector
a remote native dll injector; 
一个远程的本地dll注入器

###  测试平台
WINXP_SP3,WIN7_32,WIN7_64

### 注意事项
* 注入64位进程请编译成64位程序后再使用;
* 注入32位进程请先编译成32位后使用；
* 请注意杀毒软件对此操作可能造成的影响

### 开发工具
Visual Studio 2013

### 参考资料
* [32位进程注入64位进程](https://www.cnblogs.com/lanrenxinxin/p/4821152.html)
* [win7 64 DLL的远程注入技术 及注入dll函数调用](https://blog.csdn.net/xuplus/article/details/36051337)
* [Windows 反调试技术——OpenProcess 权限过滤](https://www.jianshu.com/p/68da53bb0cf2)
* [利用Hook API函数OpenProcess与TerminateProcess来防止任务管理器结束进程【转】](https://www.cnblogs.com/delphi7456/archive/2010/10/31/1865729.html)