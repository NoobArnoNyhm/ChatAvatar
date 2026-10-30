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

- [ ] Der Katalog listet gültige Avatare mit Identität, Name und Typ.
- [ ] Ein Avatar kann anhand seiner Identität geladen werden.
- [ ] Fehlerhafte Dateien verhindern nicht das Auflisten der übrigen gültigen Dateien.
- [ ] Die Fälle sind automatisiert getestet und in der `README.md` beschrieben.