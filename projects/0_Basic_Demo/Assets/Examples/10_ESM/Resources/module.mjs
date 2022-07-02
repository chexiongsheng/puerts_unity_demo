import CS from 'csharp';
// dont do this --> import { UnityEngine } from 'csharp' 目前版本中，不能使用该方式引入csharp接口

export async function startup() {
    await new Promise((resolve, reject) => {
        setTimeout(resolve, 500)
    });

    console.log('waited 500ms');
    console.log('created vector3', new CS.UnityEngine.Vector3(1, 2, 3));
}