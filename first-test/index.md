dotnet core testサンプル
=======================

donnet coreで、class libraryとして、
テスト対象となるクラスを作成する。

- first-lib
テスト対象のclass library

- first-xunittests


# first-lib
first-libは、以下のコマンドで作成した

```
dotnet new -t lib
```

パッケージング

```
dotnet pack
```

## first-xunittest
以下コマンドで作成
```
dotnet new -t xunittest
```

テスト実行

```
dotnet test
```

