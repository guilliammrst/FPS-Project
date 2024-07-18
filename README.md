# FPS-Project

Ce projet s'inscrit comme projet annuel de deuxième année à l'ESGI à Nantes.  
Ce projet était en groupe de 3.  

# Technos utilisées
- Unity (editor version 2022.3.15f1)
- C#

## A savoir
Les fichiers C# sont dans le dossier Assets/Scripts. Ils sont structurés avec quelques principales fonctions intégrées par Unity :
- Awake : appelée lorsque l'instance du script est chargée
- Start : appelée avant la première image après que toutes les méthodes Awake ont été appelées
- Update : appelée à chaque frame du jeu (si 60 FPS alors appelée 60 fois par secondes)
- OnDestroy : appelée juste avant que l'objet ne soit détruit

## Pour tester ce projet sur votre machine
Via UnityHub :
1) Cloner le repos
2) Installer UnityHub et cliquer sur ajouter un projet à partir du disque
3) Ouvrir le projet
4) Une fois le projet ouvert, ouvrir la scène Assets/Scenes/MainMenu
5) Lancer le projet

Via le projet compilé :
1) Cloner le repos
2) Cliquer sur build/FPS-Project.exe
3) Jouez !

## Pour déboguer
Via Visual Studio 2022 :
1) Cloner le repos
2) Installer UnityHub et cliquer sur ajouter un projet à partir du disque
3) Ouvrir le projet sur UnityHub et Visual Studio 2022
4) Dans Visual Studio, cliquez sur "Attacher à Unity"
5) Puis dans Unity lancer le projet
6) Vous pouvez maintenant ajouter des points d'arrêts aux endroits voulus
7) Bon débogage !

### Collaborateurs
- Elouan LE BRAS
- Clément KERVICHE
- Guilliam MORISSET
