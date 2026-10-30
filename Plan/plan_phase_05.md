# Phase 05: Avatar-Dateien

## Ziel

Avatare einzeln und zuverlässig laden und speichern.

## Umfang

- Serialisierung des Domänenmodells in das festgelegte Dateiformat implementieren.
- Lesen, Schreiben und Validieren einzelner Avatar-Dateien ermöglichen.
- Fehler bei beschädigten oder inkompatiblen Dateien verständlich melden.

## Abhängigkeit

Phase 04.

## Akzeptanzkriterien

- [x] Avatare aller drei Typen können gespeichert und erneut geladen werden.
- [x] Beschädigtes JSON, unbekannte Darstellungstypen und nicht unterstützte Dateiversionen erzeugen kontrollierte Fehler.
- [x] Tests decken Rundlauf, Fehlerfälle und fehlende Dateien ab.
- [x] Der Dateizugriff erfolgt ausschließlich über explizit übergebene Pfade.
- [x] Verhalten und atomarer Schreibablauf sind in der `README.md` dokumentiert.