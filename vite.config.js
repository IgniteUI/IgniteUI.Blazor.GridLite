import { defineConfig } from 'vite';
import { resolve } from 'path';
import { copyFileSync, mkdirSync, existsSync, renameSync, rmSync, writeFileSync } from 'fs';

const projectDir = './src/IgniteUI.Blazor.GridLite';
const outDir = `${projectDir}/wwwroot/js`;
const emptyOutDir = true;
const licenseManifest = 'THIRD-PARTY-LICENSES.md';

export default defineConfig(({ mode }) => {
  const isDev = mode === 'development';
  return {
    build: {
      lib: {
        entry: resolve(import.meta.dirname, 'src/IgniteUI.Blazor.GridLite/igc-grid-lite-entry.js'),
        fileName: 'blazor-igc-grid-lite',
        formats: ['es'],
      },
      outDir,
      emptyOutDir,
      // One manifest of every bundled dependency's license, packed with the library (see the csproj).
      license: { fileName: licenseManifest },
      rolldownOptions: {
        external: [],
        output: {
          preserveModules: false,
          codeSplitting: false,
          // Lib mode only honors minify at the output level.
          minify: !isDev,
          // Legal notices (@license, /*! …) stay inline in the bundle as well.
          comments: { legal: true },
        },
      },
      target: 'es2020',
      sourcemap: isDev,
    },
    plugins: [
      {
        name: 'move-license-manifest',
        closeBundle() {
          // Rolldown emits only inside outDir; the manifest is not a web asset, so it moves next to the csproj.
          renameSync(`${outDir}/${licenseManifest}`, `${projectDir}/${licenseManifest}`);
        },
      },
      {
        name: 'copy-igniteui-themes',
        writeBundle() {
          const themesSourceDir = resolve(import.meta.dirname, 'node_modules/igniteui-webcomponents/themes');
          const themesDestDir = resolve(import.meta.dirname, './src/IgniteUI.Blazor.GridLite/wwwroot/themes');
          // The previous theme location: stubs that @import the themes, so links to the deprecated path keep working.
          const legacyThemesDestDir = resolve(import.meta.dirname, './src/IgniteUI.Blazor.GridLite/wwwroot/css/themes');
          if (emptyOutDir) {
            rmSync(themesDestDir, { recursive: true, force: true });
            rmSync(legacyThemesDestDir, { recursive: true, force: true });
          }

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

                // Relative, so it resolves under any <base href> and from the stub's fingerprinted URL alike.
                const legacyDir = resolve(legacyThemesDestDir, variant);
                mkdirSync(legacyDir, { recursive: true });
                writeFileSync(
                  resolve(legacyDir, `${theme}.css`),
                  `/* Deprecated: use _content/IgniteUI.Blazor.GridLite/themes/${variant}/${theme}.css instead. */\n` +
                    `@import url("../../../themes/${variant}/${theme}.css");\n`,
                );
              } else {
                console.warn(`⚠ Theme file not found: ${sourceFile}`);
              }
            });
          });

          console.log('✓ All theme files copied to wwwroot/themes, with deprecated stubs in wwwroot/css/themes');
        },
      },
    ],
  };
});
