import { MyAnotherScript } from "./MyAnotherScript";
import { MyScript } from "./MyScript";

const ms: MyScript = new MyScript();

var mArr: MyAnotherScript[] = [];
for( let i = 0; i < 10; i++ ){
    mArr[i] = new MyAnotherScript( (i+1).toString() )
}