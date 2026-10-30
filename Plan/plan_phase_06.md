# Phase 06: Avatar-Katalog

## Ziel

Gespeicherte Avatar-Dateien auffindbar und für die Wiedergabe auswählbar machen.

## Umfang

- Einen Katalogdienst für Auflisten und Laden aus einem vom Host übergebenen Speicherort erstellen.
- Ungültige Einträge erfassen, ohne den gesamten Katalog unbenutzbar zu machen.
- Keine eigenständige globale Speicherort- oder Anwendungseinstellung einführen.

## Abhängigkeit

Phase 05.

## Akzeptanzkriterien

- [x] Der Katalog listet gültige Avatare mit Identität, Name und Typ.
- [x] Ein Avatar kann anhand seiner Identität aus dem übergebenen Verzeichnis geladen werden.
- [x] Fehlerhafte Dateien verhindern nicht das Auflisten der übrigen gültigen Dateien.
- [x] Speicherort, Dateimuster und Verhalten bei doppelten IDs sind automatisiert getestet und in der `README.md` beschrieben.