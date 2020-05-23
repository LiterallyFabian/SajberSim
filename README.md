# SajberSim



## Nerladdning

Färdiga builds finns [här](https://github.com/LiterallyFabian/SajberSim/releases) eller på huvudsidan [Sajber.me](http://sajber.me), notera att dessa i nuläget inte direkt har något intressant att se.

För källkod kan du klona projektet med Git, och sedan öppna mappen i Unity.

```
git clone https://github.com/LiterallyFabian/SajberSim
cd SajberSim
```





## Modding intro

*I nuläget används detta spel för CyberGymnasiet och modding kommer färdigställas i framtiden. Du får gärna testa göra något eget, men verktygen arbetas fortfarande på*

~~När vi gjorde SajberSim behövde vi ett enkelt sätt att lägga in storyn i spelet så vi kan lägga till och redigera våra bakgrunder, karaktärer, dialoger etc smidigt. Detta gjorde vi genom att implementera ett eget mänskligt läsbart filformat som gör det mycket lättare för oss att redigera storyn, tillsammans med verktyg för att effektivt testa storyn. Med detta format så blir det inte bara lättare för oss dock, utan för alla med spelet nerladdat! Läs vidare ifall du är intresserad av att skapa en ny story eller redigera vår!~~

För att göra egna stories behöver du inte använda källkoden här, du kan redigera spelfilerna direkt. 

Alla kommande filvägar utgår ifrån `C:\Program Files (x86)\SajberSim\SajberSim_Data\Modding\`, ifall du har ändrat installationsmappen så vet du förmodligen själv vart filerna finns. I spelet kan du komma åt filerna från Huvudmenyn > Modding.



Spelet läser in allting från 4 mappar som finns i `SajberSim/SajberSim_Data/Modding`, och det är i "Dialogues" som du skriver storyn. Varje textfil är en del utav storyn, och spelet går vidare till nästa via alternativ i spelet. Det varje textfil gör är att starta färdiga funktioner som anges med en siffra och tar in argument delat med ett vertikalstreck `|`. Det finns en mängd olika funktioner som du kan anropa från din story, och du kan se vår färdiga story för exempel. 

Ett nytt spel börjar alltid på "start" som är en tutorial och går sedan vidare till "intro", båda dessa är hårdkodade för tillfället men går enkelt att komma undan med LOADSTORY funktionen.



## Karaktär-setup

I SajberSim finns det två olika typer av karaktärer, slumpmässiga och bestämda. Alla slumpmässiga karaktärer har ett namn & smeknamn som sparas i `Characters/characterconfig.txt`, och dessa använder du via ett ID.  När ett nytt spel skapas så får alla karaktärer ett unikt nummer (börjar på 0), dvs om du har 3 karaktärer till exempel kan du då använda ID 0, 1 och 2 i dialoger. Detta kan vara användbart ifall du har flera karaktärer med liknande pose och vill ha mer variation enkelt.

Ifall du inte vill använda ID-systemet så går det bra att använda namnet direkt när du skapar karaktären, men ett tips isåfall kan vara att inte lägga in karaktärer du skapar med namn i characterconfig, då de kan förekomma dubbelt.

I `Characters/` sparas alla bilder, och dem ska ha formatet `(namn)(mood).png`, till exempel `fabinahappy.png`.  Det är viktigt att alla slumpmässiga karaktärer har samma humör och att alla har ett porträtt som används när personen talar `(namn)port.png`.



## Funktioner

- T - Skapar en textbox med någons ikon. Person-argumentet är antingen personens ID eller namn, beroende på hur du väljer att göra.

  `T|person|text`

  

- ALERT - Skapar en textbox utan någon ikon.

  `ALERT|text`

  

- BG - Byter bakgrund. Ifall argument 3 saknas så rensas ingen bort. Bakgrunds-argumentet ska vara namnet på en bild i `Backgrounds/` utan ".png"

  `BG|bakgrund|rensakaraktärer`

  

- CHAR - Skapar eller flyttar en karaktär. Flip är antingen 1, eller -1 för att spegelvända karaktären. Våra humör är happy, neutral, angry eller blush, men du kan skapa egna! Precis som med textboxar så är person-argumentet namn eller ID.

  För att få fram positioner enklare kan du använda vårat verktyg du hittar via Main > Modding > Kraktär-Setup.

  `CHAR|person|humör|x|y|flip`

  

- DEL - Ta bort en karaktär med ID eller namn

  `DEL|person`

  

- QUESTION - Skapar en fråga & startar ny dialog. `Text` är vad som ska stå på knappen eller i listan, och `dialog` är namnet på en dialog som kommer öppnas när man väljer alternativet. Du kan ha 2 eller fler alternativ, där 3+ blir en dropdown-lista istället för knappar 

  `QUESTION|fråga|text1|dialog1|text2|dialog2` //Knappar 

  `QUESTION|fråga|text1|dialog1|text2|dialog2|text3|dialog3|text4|dialog4` //Lista

  

- LOADSTORY - Startar ny dialog utan att fråga. Precis som med bakgrundsbytet så rensas bara karaktärer om argument 3 finns. Dialog-argumentet ska vara namnet på en textfil i `Dialogues/` utan ".txt".

  `LOADSTORY|dialog|rensakaraktärer`

  

- WAIT - Väntar i x sekunder innan spelet går vidare till nästa rad. Surprising huh? Decimaler skrivs med en punkt (6.9, inte 6,9)

  `WAIT|sekunder`

  

- PLAYMUSIC - Spelar en ljudfil. Ljud-argumentet ska vara namnet på en OGG-fil i `Audio/` utan ".ogg".  Ifall du har [FFmpeg](https://www.ffmpeg.org/) installerat så kan du konvertera alla MP3s i mappen med shell-skriptet som ligger i mappen. 

  `PLAYMUSIC|ljud`   



- PLAYSFX - Spelar en ljudfil som ovan, fast på ett annat objekt. Till skillnad från musiken som loopar så kommer ljudeffekter bara spelas en gång.



- STOPSOUNDS - Tar bort alla ljud.

  

- OPENSCENE - Öppnar ett minigame. För att lägga till egna scener så måste du göra det i Unity med källkoden

  `OPENSCENE|scen`
  
  
  
- FINISHGAME - Markerar spelet som avklarat och startar credits. Om man kör denna så kommer ens sparfil tas bort och nästa gång man startar spelet börjar man från början med nya slumpade karaktärer.



## Debugging

För att hitta problem i din story så kan du köra vår story-debugger som du hittar via huvudmenyn > Modding > Felsök. Den läser igenom alla textfiler i rätt mapp och om returnerar en textfil med alla potentiella fel den hittade. Du kan även gå in i devmode som låter dig hoppa runt mellan stories och visa lite extra information.

Ett allmännt tips kan vara att avsluta alla stories med en fråga eller funktion 4 som går vidare direkt till ett skript. Om storyn är spelets sista skriver du FINISHGAME.



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
- [x] ingame pause menu
- [x] credits
- [x] let story use variables (regex/)
- [x] finishgame argument
- [x] ~~preload stuff~~
- [x] fix the audio channel mess
- [x] info when game saved
- [x] working pause
- [x] bug: double space fucks up the game
- [ ] fix installer
- [x] bug: esc works in main
- [ ] stats
- [x] rich presance
- [x] character preview mode
- [x] fix wait func
- [ ] redo button framework
- [ ] fix storysetup menu
- [ ] redo skip function
- [ ] more fluent UI
- [ ] easter eggs :3
- [ ] better utility classes