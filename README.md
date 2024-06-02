# Popis projektu
Aplikace ziskava informace o produktech z SQL serveru, prepocitava ceny v CZK na zaklade aktualniho smenneho kurzu USD na CZK a uklada je do CSV souboru.
Kurzy z daneho dne zadanym uzivatelem jsou nacitane z databaze [Ceske narodni banky](https://www.cnb.cz/cs/financni-trhy/devizovy-trh/kurzy-devizoveho-trhu/kurzy-devizoveho-trhu/rok.txt?rok=).

# Funkcionalita
Uzivatel zadava datum ve formatu DD.MM.YYYY, jako prvni aplikace vyhleda kurz CZK-USD z daneho dne a vypise ho. Aplikace mezitim v pozadi pracuje i s ostatnimi menami a cenami produktu (ktere cerpa z SQL databaze, na kterou se pripojuje), tudiz nasledne vsechna data vypise do nove vygenerovaneho CSV souboru pojmenovaneho podle daneho dne. Uzivatel muze ziskat smenne kurzy a ceny produktu az do roku 1993, tedy obdobi vzniku CNB kdy se data zacala zaznamenavat. V pripade zadani starsiho nebo neplatneho data, aplikace vypise chybu. Aplikace automaticky upravi zadane datum, pokud je z budoucnosti, nebo pokud spada na vikend (presune ho na nejblizsi pracovni den).

# Priklad vstupu a vystupu
```Zadejte datum pro kurzovní lístek (DD.MM.YYYY) nebo 'exit' pro ukončení:```

```08.04.2022```

```Kurz CZK-USD k datu 08.04.2022 je: 1 USD = 30,935 CZK```

```Data byla úspěšně zapsána do souboru: 08.04.2022_adventureworks.csv```

Priklad vystupu v CSV souboru:
```
Date;ProductName;PriceUSD;PriceCZK
01.06.2023;Produkt 1;100.00;2200.00
01.06.2023;Produkt 2;150.00;3300.00
...
```
