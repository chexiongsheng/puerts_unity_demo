# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

you can get the english version change log at [Github Release](https://github.com/Tencent/puerts/releases)

## [1.3.4] - 2022-05-18
1. the module csharp in d.ts will use export = just like what Node.js did #750
2. fix: ignored assemblies which path is with Editor when generating extension method #735
3. add try catch for builtin script running. and will destroy the jsengine when error is thrown.
4. the Debug build of Plugin will now have the global.gc function.

## [1.3.3] - 2022-04-17
1. fix: some event members did not generated as LazyMember Tencent#739
2. fix: some parameters with in modifier would use ref modifier to invoke in the generated code. Tencent#758
3. fix: the op_Implicit method could not be called Tencent#767

## [1.3.2] - 2022-04-06
1. fix: unity would not ignore iOS library when in Android Mode.
2. fix: old filter wrongly return BindingMode.SlowBinding
3. add .asmdef in main repository

## [1.3.0] - 2022-04-05
1. rearrange the directory layout to support UPM
2. Generator refactored. Generator is seperated to many small file now.
3. Rename LibVersion to ApiLevel。
4. JSFunction::Invoke refactored，fix #681 。
5. Deleted some deprecated v8 calls. to make puerts compat with v8 8.4++。
6. New concept `LazyBinding`：
    In 1.3- when you write `return false` in filter for some c# member. That member will still do TypeRegist by reflection could be call during runtime.
    Now, this feature will still work. But that kind of member will do reflection during first invoke but not during TypeRegister.
 8. New form of `filter`
     To compat with the new `LazyBinding` mode. `Filter` now can not only return a boolean but can return a `BindingMode`. Which is a enum inculde `FastBinding` (means will generate static wrapper)、`LazyBinding` (mentioned above)、`DontBinding` (can not be called during runtime)。you can find an example in `U2018Compatible.cs`
