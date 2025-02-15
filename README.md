# Scene ID

# Установка

## Через GitHub

1. Открываем вкладку `Package Manager` через `Window/Package Manager`:  
   ![image](https://github.com/user-attachments/assets/7947ad1c-cc4e-4d58-a3a4-bb15917125f2)
2. Нажимаем `+` и выбираем `Install Package from git URL`:  
   ![image](https://github.com/user-attachments/assets/7812fff5-a829-4586-a87e-2923c92bfaeb)

# Использование
С плагином `Scene ID` можно с легкостью подгружать и выгружать сцены через `enum`-класс `SceneId`.
``` c#
SceneId.Next.Load();
SceneId.Previous.Unload();

// если в проекте есть сцена под названием "TestScene":
SceneId.TestScene.Load();
```
