# Phase 13: Gesichtsavatar

## Ziel

Ein erstes dreidimensionales Gesicht mit Mundbewegungen passend zur Animationsspur darstellen.

## Umfang

- Ein lokal gespeichertes, einfaches 3D-Gesichtsmodell einbinden.
- Mindestens grundlegende Mundformen beziehungsweise Viseme umsetzen.
- Animationsereignisse aus Phase 11 auf das Modell abbilden.

## Abhängigkeit

Phasen 08 und 11.

## Akzeptanzkriterien

- [x] Kopf, Augen, Ohren, Nase und Mund werden als lokale Three.js-3D-Meshes ohne 2D-Sprites aufgebaut.
- [x] SAPI-Visemgruppen steuern unterscheidbare Mundöffnungen über den typisierten WebView2-Kanal.
- [x] `ResetSpeechAnimationAsync` setzt den Mund in den neutralen Zustand zurück.
- [x] Protocol payload und lokales Face-Asset sind automatisiert getestet.
- [ ] Manuelle Sichtprüfung der gerenderten 3D-Mundformen in der WPF-Testanwendung.
- [x] Der Gesichtsavatar und seine Steuer-API sind in der `README.md` beschrieben.