import { MyAnotherScript } from "./MyAnotherScript";
import { MyScript } from "./MyScript";

const ms: MyScript = new MyScript();

var ar: MyAnotherScript[] = [];
for( let i = 0; i < 10; i++ ){
    ar[i] = new MyAnotherScript( (i+1).toString() )
}