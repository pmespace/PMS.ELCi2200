# ELCi2200 interface

Ce d�veloppement comporte tous les outils pour travailler avec un ELC i2200 d'INGENICO, lecteur de ch�ques fran�ais.
La documenation du dialogue avec l'ELC est attach�e � ce projet (fichier PDF).

Un ELC se connecte en RS232 au PC.
La c�ble fourni avec le mat�riel n'a jamais fonctionn� sous WIndows 10. La connexion est donc assur�e au moyen d'un adaptateur USB<->RS232 (RJ45) et d'un c�ble RJ45<->RJ12.
L'adaptateur utilis� dans mon cas est un ATEN UC232 (https://www.aten.com/fr/fr/products/usb-&-thunderbolt/convertisseurs-usb/uc232b/) mais probablement les autres doivent marcher. Ce qu'il faut regarder est le c�blage de la prise RJ45.

Le c�blage avec un ATEN UC232B est d�taill� dans le PDF joint � ce projet. Il est probable que le branchement d'un autre adaptateur que l'ATEN US232B sera tr�s similaire, le sch�ma est n�anmoins attach� pour v�rification.

Les fichiers:
- ELCi2200.dll
Le driver de l'ELC (en C) qui permet d'interagir avec lui.
Le fonctionnement permet de dialoguer de fa�on synchrone pour toutes les fonctions (status, abort, read, write) ou asynchrone quand c'est n�cessaire (read, write).

- TestELC.exe
Une application en C permettant de tester toutes les fonctions de l'ELC.
Elle est particuli�rement utile pour tester avec un autre adaptateur que l'ATEN UC232B.

- ELCDotNet.dll
Librairie dotnet permettant d'utiliser l'ELC depuis une application dotnet.
Elle supporte toutes les possibilit�s propos�es par le driver.

- ELCManager.exe
Application graphique dotnet permettant de d�montrer l'utilisation de l'ELCi2200 depuis une application dotnet.
