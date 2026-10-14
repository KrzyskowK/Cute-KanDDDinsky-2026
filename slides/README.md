# Slides

The talk is written in Markdown (`markdown/*.md`) and rendered with [reveal.js](https://revealjs.com) 6.0.2 (`index.html`).

## Start the slideshow



From this `slides/` folder, run a static server:

```bash
python3 -m http.server 8000
```

Then open <http://localhost:8000/slides> in a browser.

Any static server works (`npx serve`, the VS Code "Live Server" extension, etc.).

## Keyboard shortcuts

| Key | Action |
|-----|--------|
| `→` / `←` | Next / previous slide |
| `↓` / `↑` | Next / previous vertical slide (`--` in the Markdown) |
| `S` | Speaker view (notes are written after a `Note:` line) |
| `F` | Fullscreen |
| `Esc` / `O` | Slide overview |

## Editing

- One Markdown file per topic in `markdown/`. Slides are separated by a line containing `---`, vertical slides by a line containing `--`.
- The list of files played, and their transitions, is in the `<section>` elements of `index.html`.
- Images live in `img/`, referenced relatively (`./img/...`).
- reveal.js assets are in `dist/`. Do not edit them by hand; they are the 6.0.2 release.
