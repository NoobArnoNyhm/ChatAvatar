# Phase 16: Avatar-Laufzeit und Typwechsel

## Ziel

Die ausgewählte Avatar-Datei in der Laufzeit laden und ihre Darstellungsart aktivieren.

## Umfang

- Avatar-Auswahl aus dem Katalog mit dem Renderer verbinden.
- Die drei unterstützten Typen an ihre jeweilige Darstellung weiterleiten.
- Wechsel und Ladefehler kontrolliert behandeln.

## Abhängigkeit

Phasen 06, 13, 14 und 15.

## Akzeptanzkriterien

- [ ] Jeder der drei Beispieltypen kann geladen und angezeigt werden.
- [ ] Ein Typwechsel aktiviert die passende Darstellung und räumt den vorherigen Zustand auf.
- [ ] Unbekannte oder nicht ladbare Avatare erzeugen einen verständlichen Fehler.
- [ ] Auswahl und Wechsel sind automatisiert getestet.