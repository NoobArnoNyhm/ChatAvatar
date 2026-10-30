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

- [ ] Ein Ende-zu-Ende-Test prüft Textübergabe bis zum Abschluss der Avatar-Sitzung.
- [ ] Abbruch und mindestens ein Fehlerpfad sind getestet.
- [ ] Tests für Features bleiben im separaten Testprojekt und folgen der Featurestruktur.
- [ ] Die Testbefehle funktionieren reproduzierbar offline.