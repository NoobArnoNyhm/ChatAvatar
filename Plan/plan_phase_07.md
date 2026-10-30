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

- [ ] Die WPF-Testanwendung zeigt die lokale WebView2-Seite an.
- [ ] Die Seite enthält keine extern geladenen Ressourcen.
- [ ] Ein fehlender oder nicht startbarer WebView2-Runtime wird verständlich gemeldet.
- [ ] Der Start ist offline reproduzierbar und in der `README.md` dokumentiert.