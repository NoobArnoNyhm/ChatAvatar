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

- [x] Ein nichtleerer Text erzeugt über System.Speech einen WAV-Audiostream; ein vorhandener SAPI-Test prüft den RIFF-Header.
- [x] Leere Texte, Abbruch, nicht verfügbare Stimmen und Synthesefehler werden kontrolliert behandelt.
- [x] Ein isolierbarer Engine-Vertrag deckt Stimmennamen, Erfolg, Abbruch und Fehler automatisiert ab.
- [x] System.Speech, lokale Stimmvoraussetzung und Stream-Verantwortung sind in der `README.md` dokumentiert.