# Phase 09: Sprachsynthese

## Ziel

Aus einem Text mit System.Speech einen für die Wiedergabe nutzbaren Audiostream erzeugen.

## Umfang

- Einen Synthesedienst kapseln.
- Stimme und erforderliche Optionen übergeben können, ohne Anwendungseinstellungen selbst zu speichern.
- Abbruch und Synthesefehler behandeln.

## Abhängigkeit

Phase 03.

## Akzeptanzkriterien

- [ ] Ein nichtleerer Text erzeugt einen lesbaren Audiostream.
- [ ] Leere Eingabe und Synthesefehler werden kontrolliert behandelt.
- [ ] Tests verwenden eine isolierbare Abstraktion und prüfen Erfolg sowie Fehlerpfad.
- [ ] System.Speech und seine Einschränkungen sind in der `README.md` dokumentiert.