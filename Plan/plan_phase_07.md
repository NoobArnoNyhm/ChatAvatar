# Phase 07: Lokaler WebView2-Host

## Ziel

Die WPF-Oberfläche kann eine lokal gespeicherte WebView2-Ansicht anzeigen.

## Umfang

- WebView2 in der UI integrieren.
- Lokale HTML- und Ressourcendateien laden.
- Ladefehler sichtbar und kontrolliert behandeln.

## Abhängigkeit

Phase 02.

## Akzeptanzkriterien

- [x] Die WPF-Testanwendung bindet das WebView2-Control mit einer lokalen HTML-Seite ein.
- [x] Content Security Policy und Navigation-Guard verhindern externe Ressourcen und Navigationen.
- [x] Fehlende Assets, WebView2-Initialisierungsfehler und Ladefehler werden sichtbar gemeldet.
- [x] Das Asset wird in die Buildausgabe kopiert und seine lokale Verfügbarkeit automatisiert getestet.
- [x] Offline-Voraussetzung und Einbindung sind in der `README.md` dokumentiert.