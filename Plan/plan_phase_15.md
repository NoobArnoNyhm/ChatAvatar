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

- [ ] Das Licht pulsiert sichtbar während der Wiedergabe.
- [ ] Ohne aktive Wiedergabe bleibt die Darstellung im definierten Ruhezustand.
- [ ] Pegelverarbeitung ist unabhängig von der konkreten Darstellung getestet.
- [ ] Der Avatar wird als Beispiel in der `README.md` dokumentiert.