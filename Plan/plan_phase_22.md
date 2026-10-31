# Phase 22: Ende-zu-Ende-Tests

## Ziel

Die wichtigsten Abläufe über die Grenzen von Eingabe, Synthese, Synchronisation und Avatar-Darstellung automatisiert absichern.

## Umfang

- Testbare Adapter für Audioausgabe und WebView-Kommunikation verwenden.
- Erfolgreiche Antwort, Abbruch und Fehlerpfad integrieren.
- Tests ohne Onlinedienste und ohne unnötige echte Audioausgabe ausführbar halten.

## Abhängigkeit

Phasen 03 bis 21.

## Akzeptanzkriterien

- [x] Ende-zu-Ende-Tests verbinden Avatarwahl, Hostsettings, Textsynthese, Audiofortschritt, Timeline-Frames und Antwortabschluss.
- [x] Abbruch mit Frame-Reset und Synthesefehler mit terminalem Failed-Ergebnis sind getestet.
- [x] Die Tests liegen im separaten Testprojekt unter `Feature/Avatar/Integration`.
- [x] Tests verwenden deterministische Adapter statt echter Lautsprecher oder Onlinezugriff und laufen mit `dotnet test ChatAvatar.sln`.