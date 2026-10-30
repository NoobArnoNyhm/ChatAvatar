# Phase 08: Three.js-Grundszene

## Ziel

Eine lokale Three.js-Szene im WebView2 anzeigen und aus WPF initialisieren können.

## Umfang

- Three.js lokal einbinden.
- Kamera, Beleuchtung, Renderer und Größenanpassung implementieren.
- Einen kleinen typisierten Nachrichtenkanal zwischen WPF und Webansicht festlegen.

## Abhängigkeit

Phase 07.

## Akzeptanzkriterien

- [x] Die lokale Three.js-Szene rendert ein animiertes 3D-Testobjekt.
- [x] `ResizeObserver` passt Renderer und Kamera an Größenänderungen des WebView an.
- [x] Ein versionierter JSON-Handshake sendet `Ready` und eine typisierte `Initialize`-Nachricht zwischen WebView2 und WPF.
- [x] Three.js, HTML und JavaScript werden lokal geladen; externe Navigationen und Netzwerkverbindungen sind blockiert.
- [x] Aufbau, lokaler virtueller Ursprung und Nachrichtenformat sind in der `README.md` dokumentiert.