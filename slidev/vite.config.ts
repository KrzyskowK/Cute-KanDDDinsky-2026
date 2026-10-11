import { defineConfig } from 'vite'

// Slidev 53 ships a UnoCSS rule that lightningcss (vite's CSS minifier) cannot parse.
// Skip CSS minification at build time until the upstream rule is fixed.
export default defineConfig({
  build: {
    cssMinify: false,
  },
})
