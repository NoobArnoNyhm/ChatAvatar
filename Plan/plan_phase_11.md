# Phase 11: Phonem- und Visemspur

## Ziel

Zeitbezogene Sprechereignisse bereitstellen, mit denen die Avatar-Animation während der Wiedergabe synchronisiert werden kann.

## Umfang

- Verfügbare Ereignisse von System.Speech für die Zielumgebung prüfen.
- Eine Spur mit Zeitstempeln und Phonem-/Visemzuordnung modellieren.
- Falls System.Speech keine ausreichenden Ereignisse liefert, eine lokale, offlinefähige Ergänzung abgrenzen und integrieren.

## Abhängigkeit

Phasen 09 und 10.

## Akzeptanzkriterien

- [ ] Für eine Testäußerung steht eine geordnete, zeitbezogene Animationsspur zur Verfügung.
- [ ] Die Spur kann unabhängig von einer konkreten Avatar-Darstellung getestet werden.
- [ ] Die Synchronisationsquelle und ihre Grenzen sind dokumentiert; es gibt keinen Online-Fallback.
- [ ] Tests prüfen Reihenfolge, Zeitbezug und leere beziehungsweise nicht unterstützte Spuren.