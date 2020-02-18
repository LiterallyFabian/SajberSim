# SajberSim



## Nerladdning

Ifall du på något sätt hittat hit utan att ha spelet kan du ladda ner det här



## Modding intro

När vi gjorde SajberSim behövde vi ett enkelt sätt att lägga in storyn i spelet så vi kan lägga till och redigera våra bakgrunder, karaktärer, dialoger etc smidigt. Detta gjorde vi genom att implementera ett eget mänskligt läsbart filformat som gör det mycket lättare för oss att redigera storyn. Med detta format så blir det inte bara lättare för oss dock, utan för alla med spelet nerladdat! Läs vidare ifall du är intresserad av att skapa en ny story eller redigera vår!



För att göra egna stories behöver du inte använda källkoden här, du kan redigera spelfilerna direkt. 

Spelet läser in allting från 4 mappar som finns i `CyberSim_Data/Modding`, och det är i "Dialogues" som du skriver storyn. Varje textfil är en del utav storyn, och spelet går vidare till nästa via alternativ i spelet. Det varje textfil gör är att starta färdiga funktioner som anges med en siffra och tar in argument delat med ett vertikalstreck `|`. Detta är funktionerna du kan använda, se vår färdiga story för exempel: 



## Funktioner

- 0 - Skapar en textbox med någons ikon

  `0|person|text`

  

- 1 - Byter bakgrund. Ifall argument 3 saknas så rensas ingen bort. Bakgrunds-argumentet ska vara namnet på en bild i `Backgrounds/` utan ".png"

  `1|bakgrund|rensakaraktärer`

  

- 2 - Skapar eller flyttar en karaktär. För att ta bort kan du sätta x till 100. Om argumentet "flip" existerar spegelvänds karaktären. Person-argumentet ska vara namnet på en bild i `Characters/` utan ".png"

  `2|person|humör|x|y|flip`

  

- 3 - Skapar en fråga & startar ny dialog. Dialog-argumenten ska vara namnet på en textfil i `Dialogues/` utan ".txt".

  `3|fråga|alt1|dialog1|alt2|dialog2`

  

- 4 - Startar ny dialog utan att fråga. Precis som med bakgrundsbytet så rensas bara karaktärer om argument 3 finns.

  `4,dialognamn,rensakaraktärer`

  

- 5 - Skapar en textbox utan någon ikon.

  `5|text`

  

- WAIT - Väntar i x sekunder innan spelet går vidare till nästa rad. Surprising huh? Decimaler skrivs med en punkt (6.9, inte 6,9)

  `WAIT|sekunder`

  

- PLAYMUSIC - Spelar en ljudfil. Ljud-argumentet ska vara namnet på en OGG-fil i `Audio/` utan ".ogg".  Ifall du har [FFmpeg](https://www.ffmpeg.org/) installerat så kan du konvertera alla MP3s i mappen med shell-skriptet som ligger i mappen.

  `PLAYMUSIC|ljud`   



- PLAYSFX - Spelar en ljudfil som ovan, fast på ett annat objekt.



- STOPSOUNDS - Tar bort alla ljud.



## Frågor?

Jag hoppas att jag nämnt det mesta här men ifall något saknas eller om du har allmänna frågor kan du antingen mejla (fabian.lindgren@elev.cybergymnasiet.se) eller skriva på Discord (Fabian#1540). 
