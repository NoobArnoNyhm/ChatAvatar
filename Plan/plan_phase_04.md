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

- [x] Jeder Avatar hat eine eindeutige Identität und genau einen unterstützten Typ.
- [x] Ungültige Identitäten, Namen, typbezogene Werte und nicht unterstützte Schema-Versionen werden kontrolliert abgewiesen.
- [x] Das JSON-Dateiformat ist versioniert und mit einem Beispiel dokumentiert.
- [x] Fachregeln liegen im passenden Feature unter `Feature` und sind automatisiert getestet.
- [x] Das Modell und Format sind in der `README.md` dokumentiert.