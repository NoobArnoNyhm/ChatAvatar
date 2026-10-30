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

- [ ] Während einer Wiedergabe wird der übergebene Text angezeigt.
- [ ] Animationsereignisse werden anhand der Audiozeit ausgelöst, nicht anhand einer unabhängigen Schätzung der UI-Zeit.
- [ ] Ende und Abbruch setzen Text- und Animationszustand zurück.
- [ ] Zeitsteuerung und Zustandswechsel sind automatisiert getestet.