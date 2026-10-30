# Phase 02: Offline-Abhängigkeiten

## Ziel

Die für die Laufzeit notwendigen Abhängigkeiten so festlegen und erproben, dass die Anwendung ohne Onlinedienste starten kann.

## Umfang

- Lokale Bereitstellung von WebView2, Three.js und den benötigten Assets festlegen.
- Verfügbarkeit von System.Speech unter der vorgesehenen .NET-10-WPF-Umgebung prüfen.
- Einen kurzen Offline-Starttest für die Lösung einrichten.

## Abhängigkeit

Phase 01.

## Akzeptanzkriterien

- [ ] Abhängigkeiten und Bezugs-/Paketierungsstrategie sind dokumentiert.
- [ ] Es gibt keine Laufzeitreferenzen auf CDN, externe URLs oder andere Onlinedienste.
- [ ] Die Testanwendung startet mit deaktiviertem Netzwerk und lädt ihre Laufzeitressourcen lokal.
- [ ] Ein automatisierter oder reproduzierbarer Prüfschritt weist den Offline-Start nach.