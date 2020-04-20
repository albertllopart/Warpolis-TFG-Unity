# Warpolis Alpha

Warpolis és un prototip de videojoc d’estratègia per torns inspirat per la franquícia ”Advance
Wars”, de Nintendo, desenvolupada per Intelligent Systems i abandonada
des de l’any 2008.
En aquest prototip de videojoc s’hi enfronten dos exèrcits, els canis i els hipsters
i lluiten pel control de la ciutat. L’objectiu del jugador és administrar els
seus recursos i seguir una estratègia per tal de complir la condició de victòria, que
és o bé destruir per complet l’exèrcit enemic, conquerir-ne la base central o bé
tenir més control que l’enemic sobre el mapa en haver esgotat el nombre de torns
màxim de la partida.
Warpolis ha estat desenvolupat amb Unity, és d’estètica pixel art i tot el què
conté és feina original meva, excepte la banda sonora que a dia d'avui 20/04/2020 és un "placeholder".

- Repository del projecte (https://github.com/albertllopart/Warpolis-TFG-Unity)
- Github del creador, Albert Llopart Navarra (https://github.com/albertllopart)

L'scripting del projecte està fet íntegrament amb C#.

## Informació Important

- Actualment el menú principal del joc és una imatge estàtica sense funcionalitat. Prement el botó 'O' del teclat o 'X' del comandament iniciarà una nova partida.

- L'únic mode de joc que hi ha disponible a dia d'avui 20/04/2020 és multijugador local u contra u.

## Controls

### Teclat

- WASD: moure el cursor pel mapa
- O: confirmar acció / interactuar amb la casella actual
- K: cancel·lar acció / comprovar rang d'atac de la unitat

### Comandament

- Pad direccional: moure el cursor pel mapa
- Botó Sud (X): confirmar acció / interactuar amb la casella actual
- Botó Oest ([]): cancel·lar acció / comprovar rang d'atac de la unitat

## Gameplay

### Flux de la partida

- A l'inici de cada torn l'exèrcit rebrà 1.000 (mil) unitats monetàries per cada edifici que estigui sota el seu control.

- Seguidament totes aquelles unitats que hagin rebut mal i estiguin ubicades damunt d'un edifici aliat recuperaran un 20% de la salut màxima a canvi d'un 20% del seu valor d'unitat en unitats monetàries.

- Després d'això el jugador és lliure de realitzar totes les accions que cregui convenients.

### Accions

- Interactuar amb una casella: Interactuar amb una casella buida d'elements interactuables obrirà el menú d'opcions, que de moment conté la opció de passar torn o sortir del joc. Alerta, però, ja que encara no es pot guardar ni carregar partida, així que un cop surtis del programa no hi ha cap manera de recuperar la partida anterior.

- Si la casella conté una botiga aliada s'obrirà el menú de botiga per comprar unitats pel seu cost en unitats monetàries.

- Si la casella conté una unitat aliada se seleccionarà aquella unitat i mostrarà el seu rang de moviment. Si es torna a interactuar amb una casella que es troba disponible dins del rang de moviment la unitat es mourà fins la mateixa i obrirà el menú d'unitat, on el jugador haurà de decidir entre les diferents opcions (veure més endavant).

- Si la casella conté una unitat enemiga el jugdor podrà veure'n el rang de moviment. També hi ha la opció d'apretar el botó de 'Cancel·lar acció' per veure el rang d'atac de la unitat, especialment útil per controlar les caselles que es troben amenaçades per l'enemic.

### Menú d'unitat

- Wait: Aquest botó confirma l'acció de moure sense més.

- Attack: Aquest botó inicia la funcionalitat per atacar unitats enemigues.

- Capture: Aquest botó inicia o continua la captura de l'edifici subjacent. Només les unitats d'infanteria ho poden fer.

- Load: Aquest botó carrega una unitat d'infanteria damunt d'una unitat de transport. La unitat d'infanteria previament ha d'haver mogut a la mateixa casella que la unitat de transport aliada.

- Drop: Aquest botó inicia la funcionalitat de descarregar una unitat d'infanteria carregada a una unitat de transport.

## Tipus d'unitat

*Les unitats llistades a continuació estan ordenades igual que a la botiga. Sóc molt conscient que, ara mateix, hi falta molta informació, així que ho explicaré tot amb detall aquí.

### Infanteria

- Cost: 1000

És la unitat més dèbil i lenta i s'encarrega de capturar edificis.

### Transport

- Cost 5000

En el cas dels Canis és una moto, i en el cas dels Hipsters, un patinet. Aquesta unitat no pot atacar però serveix per transportar infanteries de forma més ràpida pel mapa. També són útils per bloquejar camins o parar ofensives fent de barrera a altres unitats més dèbils però amb poder ofensiu.

### Tanc

- Cost 7000

És la unitat terrestre més robusta i útil contra tota la resta d'unitats terrestres. En el cas dels Canis és un cotxe i en el dels Hipsters, una Furgoneta.

### Aeria

- Cost 9000

En el cas dels Canis és una cadernera i en els dels Hipsters, un dron. És una unitat molt ràpida ja que no té impediments de moviment per tipus de terreny. Útil contra totes les unitats terrestres excepte contra l'artiller, que la destrueix amb facilitat.

### Artiller

- Cost 8.000

És una unitat molt útil contra infanteria i contra aeria. En el cas dels Canis és un cani amb un foc artificial i en el cas dels Hipsters, un hipster amb una ampolla de refresc gasós.

### A distància

- Cost 6.000

En el cas dels canis és un Cani amb un smartphone i en el cas dels Hipsters, un fotògraf. És una unitat que només pot atacar a distància a un rang d'entre 2 i 4 caselles. Només pot atacar si ho fa des de la mateixa casella on ha començat el torn. No pot moure i atacar al mateix torn.

## License

MIT License

Copyright (c) 2018 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
