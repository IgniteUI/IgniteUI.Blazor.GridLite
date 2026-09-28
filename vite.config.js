import { defineConfig } from 'vite';
import { resolve } from 'path';
import { copyFileSync, mkdirSync, existsSync } from 'fs';

export default defineConfig({
  build: {
    lib: {
      entry: resolve(__dirname, 'src/IgniteUI.Blazor.GridLite/igc-grid-lite-entry.js'),
      name: 'BlazorIgcGridLite',
      fileName: 'blazor-igc-grid-lite',
      formats: ['es'],
    },
    outDir: './src/IgniteUI.Blazor.GridLite/wwwroot/js',
    emptyOutDir: false,
    rollupOptions: {
      external: [],
      output: {
        preserveModules: false,
        inlineDynamicImports: true,
      },
    },
    target: 'es2020',
    minify: 'terser',
    sourcemap: true,
  },
  plugins: [
    {
      name: 'copy-igniteui-themes',
      writeBundle() {
        const themesSourceDir = resolve(__dirname, 'node_modules/igniteui-webcomponents/themes');
        const themesDestDir = resolve(__dirname, './src/IgniteUI.Blazor.GridLite/wwwroot/css/themes');

        // Create destination directory structure
        const variants = ['light', 'dark'];
        const themes = ['bootstrap', 'material', 'fluent', 'indigo'];

        variants.forEach((variant) => {
          themes.forEach((theme) => {
            const sourceFile = resolve(themesSourceDir, variant, `${theme}.css`);
            const destDir = resolve(themesDestDir, variant);
            const destFile = resolve(destDir, `${theme}.css`);

            // Create directory if it doesn't exist
            if (!existsSync(destDir)) {
              mkdirSync(destDir, { recursive: true });
            }

            // Copy the CSS file
            if (existsSync(sourceFile)) {
              copyFileSync(sourceFile, destFile);
              console.log(`✓ Copied ${variant}/${theme}.css`);
            } else {
              console.warn(`⚠ Theme file not found: ${sourceFile}`);
            }
          });
        });

        console.log('✓ All theme files copied to wwwroot/css/themes');
      },
    },
  ],
});
