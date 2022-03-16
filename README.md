# unity demo for puerts

本仓库master分支用到了unity package的能力，因此仅支持Unity2018+。如您在使用5.5和2017，请查看1.2.x或1.0.x分支

## 怎么跑这demo？

* git clone https://github.com/chexiongsheng/puerts_unity_demo.git 

* package就是puerts主仓库的代码，但是将其改造成unity package的建议格式了

* projects/Puerts_Demo就是个unity工程，Examples是示例

* 示例01到示例04都是js编写，可以直接修改对应的js代码看效果

* 实例05是typescript工程生成的代码，用vscode打开根目录的TsProj工程，修改ts代码，然后在vscode里打开一个TERMINAL，敲入“npm run build”，这个命令编译并拷贝到相应的目录。
    - 需要安装vscode，node，npm，typescript
    - 用window命令行工具到TsProj下执行“npm run build”也可以
