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

- [x] Avatar-Dateien aller drei Darstellungsarten können aus einem hostseitig übergebenen Verzeichnis geladen werden.
- [x] Der Renderer wird mit dem ausgewählten Typ aktiviert; die WebView blendet die vorherige Darstellung aus.
- [x] Fehlende, mehrdeutige oder durch den Renderer nicht aktivierbare Avatare erzeugen kontrollierte Fehler, ohne die aktuelle Auswahl zu ersetzen.
- [x] Auswahl aller drei Typen, Fehlersituationen und Zustandsbeibehaltung sind automatisiert getestet.