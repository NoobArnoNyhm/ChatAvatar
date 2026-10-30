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

- [ ] Ein Avatar kann gespeichert, erneut geladen und fachlich gleichwertig verglichen werden.
- [ ] Beschädigte, unbekannte und nicht unterstützte Dateiversionen erzeugen kontrollierte Fehler.
- [ ] Tests decken Rundlauf und mindestens einen Fehlerfall ab.
- [ ] Der Dateizugriff bleibt auf explizit übergebene Pfade beschränkt.
- [ ] Das Verhalten ist in der `README.md` dokumentiert.