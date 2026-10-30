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

- [x] Die Paketversionen und die lokale Bereitstellung von Three.js sind festgelegt und dokumentiert.
- [x] Die Testanwendung und Three.js verwenden keine CDN- oder externen Laufzeitressourcen.
- [x] WebView2- und System.Speech-Assemblies sowie das lokale Three.js-Modul sind in automatisierten Tests nachweisbar.
- [x] Ein Skript baut und testet die Solution ohne Restore und prüft, dass der WPF-GUI-Prozess die Message-Loop erreicht; es kann bei deaktiviertem Netzwerk ausgeführt werden.
- [x] Die noch erforderliche Fixed-Version-WebView2-Runtime und lokal installierte SAPI-Stimmen sind als Anforderungen für Offline-Bereitstellung dokumentiert.