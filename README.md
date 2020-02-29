# SajberSim



## Nerladdning

Ifall du på något sätt hittat hit utan att ha spelet kan du ladda ner det här



## Modding intro

När vi gjorde SajberSim behövde vi ett enkelt sätt att lägga in storyn i spelet så vi kan lägga till och redigera våra bakgrunder, karaktärer, dialoger etc smidigt. Detta gjorde vi genom att implementera ett eget mänskligt läsbart filformat som gör det mycket lättare för oss att redigera storyn, tillsammans med verktyg för att effektivt testa storyn. Med detta format så blir det inte bara lättare för oss dock, utan för alla med spelet nerladdat! Läs vidare ifall du är intresserad av att skapa en ny story eller redigera vår!



För att göra egna stories behöver du inte använda källkoden här, du kan redigera spelfilerna direkt. 

Spelet läser in allting från 4 mappar som finns i `CyberSim_Data/Modding`, och det är i "Dialogues" som du skriver storyn. Varje textfil är en del utav storyn, och spelet går vidare till nästa via alternativ i spelet. Det varje textfil gör är att starta färdiga funktioner som anges med en siffra och tar in argument delat med ett vertikalstreck `|`. Detta är funktionerna du kan använda, se vår färdiga story för exempel: 



## Funktioner

- 0 - Skapar en textbox med någons ikon. Person-argumentet är personens ID, som du kan läsa om under "Karaktär-setup". För att använda karaktärens namn eller smeknamn i storyn, använd `{ID.name}` eller `{ID.nick}`

  `0|person|text`

  

- 1 - Byter bakgrund. Ifall argument 3 saknas så rensas ingen bort. Bakgrunds-argumentet ska vara namnet på en bild i `Backgrounds/` utan ".png"

  `1|bakgrund|rensakaraktärer`

  

- 2 - Skapar eller flyttar en karaktär. För att ta bort kan du sätta x till 100. Om argumentet "flip" existerar spegelvänds karaktären. Person-argumentet ska vara personen ID. Våra humör är happy, neutral eller sad, men du kan skapa egna (info under "Karaktär-setup")!

  `2|person|humör|x|y|flip`

  

- 3 - Skapar en fråga & startar ny dialog. Dialog-argumenten ska vara namnet på en textfil i `Dialogues/` utan ".txt".

  `3|fråga|alt1|dialog1|alt2|dialog2`

  

- 4 - Startar ny dialog utan att fråga. Precis som med bakgrundsbytet så rensas bara karaktärer om argument 3 finns. Dialog-argumentet ska vara namnet på en textfil i `Dialogues/` utan ".txt".

  `4|dialog|rensakaraktärer`

  

- 5 - Skapar en textbox utan någon ikon.

  `5|text`

  

- WAIT - Väntar i x sekunder innan spelet går vidare till nästa rad. Surprising huh? Decimaler skrivs med en punkt (6.9, inte 6,9)

  `WAIT|sekunder`

  

- PLAYMUSIC - Spelar en ljudfil. Ljud-argumentet ska vara namnet på en OGG-fil i `Audio/` utan ".ogg".  Ifall du har [FFmpeg](https://www.ffmpeg.org/) installerat så kan du konvertera alla MP3s i mappen med shell-skriptet som ligger i mappen.

  `PLAYMUSIC|ljud`   



- PLAYSFX - Spelar en ljudfil som ovan, fast på ett annat objekt.



- STOPSOUNDS - Tar bort alla ljud.

  

- OPENSCENE - Öppnar ett minigame. För att lägga till egna scener så måste du göra det i Unity med källkoden

  `OPENSCENE|scen`
  
  
  
- FINISHGAME - Markerar spelet som avklarat och startar credits. Om man kör denna så kommer ens sparfil tas bort och nästa gång man startar spelet börjar man från början med nya slumpade karaktärer.



## Karaktär-setup

Alla karaktärer har ett namn & smeknamn som sparas i `Characters/characterconfig.txt`, och 2 bilder var, ett porträtt som används i textrutorna och en pose som används när du skapar karaktären i scenen. För att länka bilder med karaktärer så döper du bara bilden till karaktärens namn i små bokstäver och sedan namnet på posen, till exempel happy/sad/neutral som vi har använt.  

När ett nytt spel skapas så får alla karaktärer ett unikt nummer (börjar på 0), om du har 3 karaktärer till exempel så kan du då använda ID 0, 1 och 2 i dialoger



## Frågor?

Jag hoppas att jag nämnt det mesta här men ifall något saknas eller om du har allmänna frågor kan du antingen mejla (fabian.lindgren@elev.cybergymnasiet.se) eller skriva på Discord (Fabian#1540). 



### To-do

- [ ] implement story
- [ ] main menu sounds
- [x] updated UI buttons
- [x] story list tester
- [x] story debugger
- [x] updated dev mode
- [ ] implement minigame menu
- [x] save characters globally when creating new game only
- [ ] ingame pause menu
- [x] credits
- [ ] let story use variables (regex/)
- [ ] finishgame argument
- [ ] preload stuff