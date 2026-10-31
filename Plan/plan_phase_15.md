# Phase 15: Abstrakter Avatar

## Ziel

Eine abstrakte 3D-Darstellung implementieren, deren Licht auf den Audiostream reagiert.

## Umfang

- Ein einfaches Licht-/Formobjekt in Three.js erstellen.
- Helligkeit oder Puls anhand des Audiowiedergabepegels steuern.
- Bei fehlenden Audiodaten einen neutralen Zustand verwenden.

## Abhängigkeit

Phasen 08 und 10.

## Akzeptanzkriterien

- [x] Ein dreidimensionales Lichtobjekt reagiert auf den RMS-Pegel des tatsächlich abgespielten WAV-Fensters.
- [x] Stille und Reset setzen den Audiopegel auf den neutralen Ruhezustand zurück.
- [x] WAV-Pegelberechnung und Pegelweitergabe sind unabhängig von der Darstellung automatisiert getestet.
- [x] Geometrie, Pegelquelle und WebView-Anbindung sind in der `README.md` dokumentiert.