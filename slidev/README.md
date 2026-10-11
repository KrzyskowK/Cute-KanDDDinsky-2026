# Slides (Slidev)

The same talk as `../slides`, written in Markdown and rendered with [Slidev](https://sli.dev) 53.

The deck is `slides.md`. It pulls each topic from `slides/*.md` with `src:` entries, in the same order as the reveal.js `index.html`. Images are served from `public/img/`.

## Start the slideshow

Requires Node.js 22.12 or newer.

```bash
cd slidev
npm install      # first time only
npm run dev      # opens http://localhost:3030
```

Presenter view and speaker notes are at <http://localhost:3030/presenter>.

## Other commands

| Command | Result |
|---------|--------|
| `npm run build` | Static site in `dist/`, serve it with any static server |
| `npm run export` | PDF export (needs Playwright; first run downloads Chromium) |

## Editing

- Slides are separated by a line containing `---`. There are no vertical slides in Slidev, so the reveal `--` sub-slides became regular slides.
- Speaker notes go in an HTML comment at the end of a slide: `<!-- your notes -->`.
- Images go in `public/img/` and are referenced as `/img/...`.

## Differences from the reveal.js deck

- Vertical slides (`--`) are now plain horizontal slides.
- The reveal transitions per section (`concave`, `convex`) are replaced by one global transition (`slide-left`, in `slides.md`).
- The broken placeholder `<img src="img/">` at the top of `slides/solid.md` was removed, since it pointed to no file and stops the Slidev build.
- `vite.config.ts` disables CSS minification. Slidev 53's bundled UnoCSS output does not pass lightningcss, so `npm run build` fails without it.
