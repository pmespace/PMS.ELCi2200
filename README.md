# ELCi2200 interface

Ce développement comporte tous les outils pour travailler avec un ELC i2200 d'INGENICO, lecteur de chèques français.
La documenation du dialogue avec l'ELC est attachée à ce projet (fichier PDF).

Un ELC se connecte en RS232 au PC.
La câble fourni avec le matériel n'a jamais fonctionné sous WIndows 10. La connexion est donc assurée au moyen d'un adaptateur USB<->RS232 (RJ45) et d'un câble RJ45<->RJ12.
L'adaptateur utilisé dans mon cas est un ATEN UC232 (https://www.aten.com/fr/fr/products/usb-&-thunderbolt/convertisseurs-usb/uc232b/) mais probablement les autres doivent marcher. Ce qu'il faut regarder est le câblage de la prise RJ45.

Le câblage avec un ATEN UC232B est détaillé dans le PDF joint à ce projet. Il est probable que le branchement d'un autre adaptateur que l'ATEN US232B sera très similaire, le schéma est néanmoins attaché pour vérification.

Les fichiers:
- ELCi2200.dll
Le driver de l'ELC (en C) qui permet d'interagir avec lui.
Le fonctionnement permet de dialoguer de façon synchrone pour toutes les fonctions (status, abort, read, write) ou asynchrone quand c'est nécessaire (read, write).

- TestELC.exe
Une application en C permettant de tester toutes les fonctions de l'ELC.
Elle est particulièrement utile pour tester avec un autre adaptateur que l'ATEN UC232B.

- ELCDotNet.dll
Librairie dotnet permettant d'utiliser l'ELC depuis une application dotnet.
Elle supporte toutes les possibilités proposées par le driver.

- ELCManager.exe
Application graphique dotnet permettant de démontrer l'utilisation de l'ELCi2200 depuis une application dotnet.
