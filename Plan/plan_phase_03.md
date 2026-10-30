# Phase 03: Integrationsvertrag

## Ziel

Den minimalen Vertrag zwischen aufrufendem Chatprogramm und Avatar-Komponente definieren und testen.

## Umfang

- Textübergabe und Start einer Avatar-Antwort beschreiben.
- Ergebniszustände wie beendet, abgebrochen und fehlgeschlagen festlegen.
- Konfigurationsübergabe vom aufrufenden Programm vorsehen, ohne dort Einstellungen zu speichern.

## Abhängigkeit

Phase 01.

## Akzeptanzkriterien

- [x] Die öffentliche API ist mit einem kurzen Beispiel dokumentiert.
- [x] Die API kann einen Text entgegennehmen und einen Antwortvorgang identifizieren.
- [x] Fehler und Abbruch sind als eindeutige Zustände abbildbar.
- [x] Tests decken gültige Eingabe sowie leere Eingabe und Abbruch ab.
- [x] Die API ist in der `README.md` dokumentiert.