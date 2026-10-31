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

- [x] Der Host kann Stimme, Avatar-ID, Lautstärke und Sprechtempo typisiert über `AvatarResponseSettings` übergeben.
- [x] Das Einstellungsfenster zeigt hostgelieferte Stimmen und validiert die Wertebereiche.
- [x] Übernehmen gibt Einstellungen zurück, Abbrechen liefert kein Ergebnis; die Komponente speichert sie nicht.
- [x] Tests prüfen Standardwerte, Übergabe und Wertebereichsvalidierung.
- [x] Host-Verantwortung und API-Vertrag sind in der `README.md` dokumentiert.