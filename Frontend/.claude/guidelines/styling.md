# Frontend Styling Guidelines

Styling is Tailwind CSS v4 on top of a custom CSS-variable theme with light and dark modes. Colors are never hardcoded in components — they come from theme tokens.

## File layout

```
src/
├── style.css                 # Global entry point (linked from index.html). Imports tailwindcss, then the files below, in this order.
└── styles/
    ├── palette/              # Raw color scales (light, light P3, dark, dark P3)
    │   ├── blue.css          # Accent — #1375A0 is step 9
    │   └── gray.css          # Neutral — #8B8D98 is step 9
    ├── tokens/               # Named roles built on the palette
    │   ├── surface.css       # Page background (light #FFFFFF, dark #222222) + color-scheme
    │   ├── semantic.css      # --primary, --secondary, --muted, --accent, --border, --input, --ring, --radius, ...
    │   └── feedback.css      # --destructive, --warning, --success (+ -foreground, -subtle, -border)
    └── tailwind-theme.css    # @theme inline mapping to Tailwind utilities + base body styles. No values live here.
```

Where to edit:

- Change the brand/accent or gray color → `palette/`.
- Change which scale step a role uses → `tokens/semantic.css`.
- Change error/warning/success colors → `tokens/feedback.css`.
- Add a token → define it in `tokens/`, then expose it in `tailwind-theme.css` (`--color-<name>: var(--<name>)`).

Component-scoped styles stay in `<ComponentName>.module.css` (see [Architecture Guidelines](./architecture.md)). Do not add global styles to component files, and do not recreate `App.css`.

## Theming

- Light is the default (`:root`, `.light`, `.light-theme`). Dark is enabled by a `.dark` (or `.dark-theme`) class — put it on `<html>`.
- Each token is declared on every theme selector so nested `.light`/`.dark` scopes re-resolve correctly.
- `color-scheme` is set per theme so native controls and scrollbars follow it.
- Palette files give sRGB values first, then a wide-gamut (P3/oklch) override inside `@supports` + `@media (color-gamut: p3)`. Keep that order.
- There is no theme toggle or flash-prevention script yet; add both with the theme provider.

## Using colors in components

Use Tailwind utilities backed by the tokens — never hex values or raw `blue-9` unless no semantic role fits.

| Need | Classes |
|---|---|
| Page / card / popover | `bg-background text-foreground`, `bg-card text-card-foreground`, `bg-popover` |
| Primary button | `bg-primary text-primary-foreground` |
| Close / cancel button | `bg-secondary text-secondary-foreground`, or an outline with `border-border` |
| Muted text / surfaces | `text-muted-foreground`, `bg-muted` |
| Inputs and focus | `border-input`, `ring-ring` |
| Delete button | `bg-destructive text-destructive-foreground` |
| Error message / toast | `text-destructive bg-destructive-subtle border-destructive-border` |
| Warning message / toast | `text-warning bg-warning-subtle border-warning-border` |
| Success message / toast | `text-success bg-success-subtle border-success-border` |

Feedback color rules: the base color (`--destructive`, `--warning`, `--success`) is used for both solid button backgrounds and message text; `-subtle` is the light tinted background; `-border` is the message border; `-foreground` is text on a solid background.

Direct scale access (`bg-blue-3`, `text-gray-11`, 1–12) is available for cases the semantic roles don't cover. Only `blue` and `gray` scales exist — do not add other color palettes without agreement.

## Rules

- No hardcoded colors (hex, rgb, Tailwind default palette like `bg-red-500`) in components.
- Don't edit palette values by hand to tweak one component; adjust the token or add a new one.
- Prefer Tailwind utilities in JSX; use CSS modules only when utilities are impractical.
- Keep `tailwind-theme.css` value-free so palette/tokens remain the single source of truth.
