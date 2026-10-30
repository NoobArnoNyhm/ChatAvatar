# Allgemeine Akzeptanzkriterien

Diese Kriterien gelten für alle Features und deren Umsetzung im Projekt.

## Technologie und Architektur

- [ ] Die Anwendung wird mit **.NET 10** umgesetzt.
- [ ] Die Fachlogik wird nach den Prinzipien des **Domain-Driven Design (DDD)** strukturiert; fachliche Modelle und Regeln sind der jeweiligen Domäne zugeordnet.
- [ ] Features werden unter dem Ordner `Featur` angelegt. Jedes Feature besitzt dort einen eigenen Unterordner, dessen Name dem Feature entspricht.
- [ ] Zum Feature gehörende Services und Modellklassen werden innerhalb des jeweiligen Feature-Ordners in dessen Unterordner `Service` abgelegt.
- [ ] Die Benutzeroberfläche wird getrennt von den Features im eigenen Ordner `UI` implementiert.

## Tests

- [ ] Für jedes Feature werden automatisierte Tests erstellt und ausgeführt.
- [ ] Tests liegen in einem eigenen Testprojekt, getrennt von der Anwendung.
- [ ] Die Ordner- und Dateistruktur der Tests orientiert sich an der Struktur der Anwendung, sodass Tests dem jeweiligen Feature eindeutig zugeordnet werden können.
- [ ] Änderungen an einem Feature werden durch passende Tests für dessen relevantes Verhalten abgesichert.

## Dokumentation und Planung

- [ ] Jedes implementierte Feature ist in der `README.md` im Wurzelverzeichnis dokumentiert.
- [ ] Vor Beginn der Implementierung wird für jedes Feature eine explizite Planung im Ordner `Plan` angelegt.
- [ ] Die Feature-Planung wird iterativ in einer Datei im Format `plan_phase_XX.md` geführt, wobei `XX` die jeweilige Phasennummer bezeichnet.
- [ ] Die Planung beschreibt mindestens Ziel, Umfang und überprüfbare Akzeptanzkriterien des Features.
- [ ] Die Feature-Planung wird während der Umsetzung bei relevanten Änderungen oder neuen Erkenntnissen aktualisiert.