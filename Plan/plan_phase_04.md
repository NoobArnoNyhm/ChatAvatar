# Phase 04: Avatar-Domänenmodell

## Ziel

Ein minimales fachliches Modell für gespeicherte Avatare und ihre drei Darstellungsarten bereitstellen.

## Umfang

- Avatar-Identität, Anzeigename, Typ und typbezogene Konfiguration modellieren.
- Die Typen Gesicht, stilisierte Bilddarstellung und abstrakte Form abbilden.
- Ein versionierbares Dateiformat festlegen.

## Abhängigkeit

Phase 01.

## Akzeptanzkriterien

- [ ] Jeder Avatar hat eine eindeutige Identität und genau einen unterstützten Typ.
- [ ] Ungültige oder unbekannte Typen werden kontrolliert abgewiesen.
- [ ] Das Dateiformat ist versioniert und mit einem Beispiel dokumentiert.
- [ ] Fachregeln liegen im passenden Feature unter `Featur` und sind automatisiert getestet.
- [ ] Das Modell und Format sind in der `README.md` dokumentiert.