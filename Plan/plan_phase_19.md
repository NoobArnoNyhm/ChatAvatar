# Phase 19: Einstellungen und Einstellungsfenster

## Ziel

Konfigurationswerte vom Host entgegennehmen und in der Avatar-Oberfläche bearbeiten, ohne sie selbst dauerhaft zu speichern.

## Umfang

- Ein typisiertes Einstellungsmodell mit sinnvollen Standardwerten definieren.
- Einstellungsfenster für unterstützte Optionen bereitstellen.
- Geänderte Werte an das aufrufende Programm zurückgeben.

## Abhängigkeit

Phasen 03 und 07.

## Akzeptanzkriterien

- [ ] Der Host kann Einstellungen beim Initialisieren übergeben.
- [ ] Das Fenster zeigt und validiert die unterstützten Einstellungen.
- [ ] Änderungen werden an den Host zurückgegeben; die Komponente schreibt keine Anwendungseinstellungen auf Datenträger.
- [ ] Tests prüfen Übergabe, Rückgabe und Validierung.
- [ ] Der Vertrag ist in der `README.md` dokumentiert.