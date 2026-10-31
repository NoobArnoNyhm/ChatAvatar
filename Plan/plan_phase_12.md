# Phase 12: Text- und Animationssynchronisation

## Ziel

Den übergebenen Text zusammen mit Audio und Animationsspur synchronisiert ausgeben.

## Umfang

- Aktuelle Wiedergabeposition und zugehörigen Textabschnitt bestimmen.
- Animationstakt anhand der Wiedergabezeit auslösen.
- Pause beziehungsweise Abbruchzustände konsistent behandeln, sofern vom Wiedergabedienst unterstützt.

## Abhängigkeit

Phasen 03, 10 und 11.

## Akzeptanzkriterien

- [x] SAPI-Textmarker wählen den aktuellen Textabschnitt anhand von Audiozeit und Zeichenposition aus.
- [x] Playback-Zeitfortschritt und Phonem-/Visemcues verwenden dieselbe Zeitquelle; überfällige Cues werden vollständig weitergegeben.
- [x] Abschluss, Abbruch und Fehler senden einen leeren Frame und setzen Text- sowie Animationszustand zurück.
- [x] Cursor, monotone Zeitfolge, Fortschrittsweitergabe und Zustandsreset sind automatisiert getestet.
- [x] Pause und Fortsetzen sind als von `SoundPlayer` nicht unterstützte Wiedergabeaktionen dokumentiert.