# Neon Shooter

Jeux Monogame réalisé avec le tutoriel : [https://gamedevelopment.tutsplus.com/tutorials/search/Neon+Vector+xna](https://gamedevelopment.tutsplus.com/tutorials/search/Neon+Vector+xna)

Tuto par entierment fini.

[lien du .exe](https://drive.google.com/file/d/1NQT5emJuAyQhiJlCGv_FYFPB3HVW5Lgg/view?usp=sharing)

l'objectif étant de pouvoir le refaire sans tuto

## ToDo
Systeme de score  
Saving systeme  
Particule acceleration player  
Wrapping grid  

## Compile :

https://docs.monogame.net/articles/packaging_games.html

windows :
```bash
dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained
```

Linux :
```bash
dotnet publish -c Release -r linux-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained
```

