# Phase 18: Avatar-Editor und Vorschau

## Ziel

Die unterstützten Avatar-Eigenschaften bearbeiten und Änderungen vor dem Speichern betrachten können.

## Umfang

- Pro Avatar-Typ nur die dafür gültigen Eigenschaften editierbar machen.
- Änderungen unmittelbar in einer Vorschau anzeigen.
- Ungültige Werte vor dem Speichern abweisen.

## Abhängigkeit

Phasen 13 bis 17.

## Akzeptanzkriterien

- [x] Änderungen an typbezogenen Eigenschaften werden als versionierte Nachrichten an die lokale Live-Vorschau gesendet.
- [x] Gesichts-, Bildstil- und Abstrakteigenschaften haben getrennte Editoren; jeweils nicht passende Felder sind ausgeblendet.
- [x] Ungültige Namen, Farben, Skalierungen und externe/absolute Assetpfade werden sichtbar abgewiesen und nicht gespeichert.
- [x] Modellvalidierung und typbezogene Preview-Payloads sind gezielt automatisiert getestet.