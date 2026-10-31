# Phase 23: Offline-Bereitstellung

## Ziel

Die Anwendung samt Laufzeitressourcen für Installation und Nutzung ohne Internet vorbereiten.

## Umfang

- Veröffentlichungsprofil beziehungsweise reproduzierbares Paket erstellen.
- Erforderliche WebView2-Runtime und lokale Web-/Avatarressourcen berücksichtigen.
- Voraussetzungen, Paketumfang und Offline-Installation dokumentieren.

## Abhängigkeit

Phasen 02, 07, 08 und 21.

## Akzeptanzkriterien

- [x] Ein Self-Contained-win-x64-Publishprofil und Skript erstellen ein Paket inklusive lokaler Assets und geprüfter WebView2 Fixed Version.
- [x] Das Paket enthält ein Manifest mit RID, WebView2-Version und Offline-Merkmal.
- [x] `AvatarWebView` verwendet die mitgelieferte Runtime; eine unvollständige Runtime wird abgewiesen, statt online nachzuladen.
- [x] Ein Paketprüfskript kontrolliert Manifest, Runtime-Dateien, App-Assets und WPF-Start.
- [x] Paketierungsbefehle, Restore-Voraussetzung und offizielle Runtime-Quelle sind in der `README.md` dokumentiert.