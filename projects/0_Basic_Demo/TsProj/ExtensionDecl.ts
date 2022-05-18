import {$extension} from 'puerts'
import {PuertsTest} from 'csharp'

//要手动声明一下，否则通过obj.func引用扩展方法会失败
$extension(PuertsTest.BaseClass, PuertsTest.BaseClassExtension);
