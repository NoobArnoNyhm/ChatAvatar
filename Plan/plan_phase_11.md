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

- [x] Für eine Testäußerung liefert System.Speech Phonem- und Visemereignisse mit Audiozeitstempel und Dauer.
- [x] Ein rendererunabhängiger Track fasst beide Ereignistypen in Audiozeit-Reihenfolge zusammen.
- [x] Die Trackmodelle sind unabhängig von der Avatar-Darstellung automatisiert getestet.
- [x] Die Synchronisationsquelle, lokale Sprachvoraussetzung und Möglichkeit eines leeren Tracks sind dokumentiert; Online-Fallbacks gibt es nicht.
- [x] Tests prüfen Sortierung, Zeitvalidierung und leere sowie reale lokale Spuren.