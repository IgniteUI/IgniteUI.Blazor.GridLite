import { IgcGridLite } from 'igniteui-grid-lite';

// Register the component
IgcGridLite.register();

export const blazor_igc_grid_lite = {
  grids: new Map(),
  dotNetRefs: new Map(),
  // An AbortController per grid; aborting it removes that grid's event listeners.
  listeners: new Map(),

  renderGrid(dotNetObject, gridElement, options) {
    const config = JSON.parse(options);

    if (!customElements.get('igc-grid-lite')) {
      IgcGridLite.register();
    }

    gridElement.data = config.data;

    if (config.autoGenerate !== undefined) {
      gridElement.autoGenerate = config.autoGenerate;
    }

    if (config.adoptRootStyles !== undefined) {
      gridElement.adoptRootStyles = config.adoptRootStyles;
    }

    if (config.sortingOptions) {
      gridElement.sortingOptions = config.sortingOptions;
    }

    if (config.sortingExpressions) {
      gridElement.sortingExpressions = config.sortingExpressions;
    }

    if (config.filterExpressions) {
      gridElement.filterExpressions = config.filterExpressions;
    }

    this.grids.set(config.id, gridElement);
    this.dotNetRefs.set(config.id, dotNetObject);
    this.attachListeners(config.id, config.events);
  },

  // Listens for the events that have a bound callback. A re-render or a changed binding calls this again
  // for the same grid, so its previous listeners go first.
  attachListeners(id, events) {
    const gridElement = this.grids.get(id);
    const dotNetObject = this.dotNetRefs.get(id);

    this.listeners.get(id)?.abort();
    const controller = new AbortController();
    this.listeners.set(id, controller);

    // The returned promise is left to the browser, so a failed .NET handler shows as an unhandled rejection.
    const listen = (type, method, transformDetail = (detail) => detail) => {
      gridElement.addEventListener(type, (e) => dotNetObject.invokeMethodAsync(method, transformDetail(e.detail)), {
        signal: controller.signal,
      });
    };

    /**
     * Transform conditions as names only to match .NET model. grid-lite 0.11.0 still reports
     * a condition as either that name or operation object `({ name, label, unary, logic })`.
     * Safely support both. TODO: Remove once grid-lite updates condition handling.
     */
    const withConditionsAsNames = (expressions) =>
      expressions.map((expression) => {
        const { condition } = expression;
        return { ...expression, condition: typeof condition === 'string' ? condition : condition?.name };
      });

    // TODO: the sorting and filtering handlers cannot cancel: grid-lite reads dispatchEvent's result synchronously,
    // so a preventDefault after the .NET call would come too late. Cancelling needs a client-side script parameter
    // (the *Script pattern, e.g. IgbCombo.ItemTemplateScript) or a synchronous callback.
    if (events.hasSorting) {
      listen('sorting', 'JSSorting');
    }

    if (events.hasSorted) {
      listen('sorted', 'JSSorted');
    }

    if (events.hasFiltering) {
      listen('filtering', 'JSFiltering', (detail) => ({
        ...detail,
        expressions: withConditionsAsNames(detail.expressions),
      }));
    }

    if (events.hasFiltered) {
      listen('filtered', 'JSFiltered', (detail) => ({ ...detail, state: withConditionsAsNames(detail.state) }));
    }
  },

  updateGrid(id, options) {
    const grid = this.grids.get(id);
    if (!grid) return;

    const config = JSON.parse(options);

    if (config.data !== undefined) {
      grid.data = config.data;
    }

    if (config.autoGenerate !== undefined) {
      grid.autoGenerate = config.autoGenerate;
    }

    if (config.adoptRootStyles !== undefined) {
      grid.adoptRootStyles = config.adoptRootStyles;
    }

    if (config.sortingOptions !== undefined) {
      grid.sortingOptions = config.sortingOptions;
    }

    if (config.sortingExpressions !== undefined) {
      grid.sortingExpressions = config.sortingExpressions;
    }

    if (config.filterExpressions !== undefined) {
      grid.filterExpressions = config.filterExpressions;
    }

    if (config.events !== undefined) {
      this.attachListeners(id, config.events);
    }
  },

  updateData(id, data) {
    const grid = this.grids.get(id);
    if (grid) {
      grid.data = JSON.parse(data);
    }
  },

  sort(id, expressions) {
    const grid = this.grids.get(id);
    if (grid) {
      const sortExpressions = JSON.parse(expressions);
      grid.sort(Array.isArray(sortExpressions) ? sortExpressions : [sortExpressions]);
    }
  },

  clearSort(id, key) {
    const grid = this.grids.get(id);
    if (grid) {
      grid.clearSort(key);
    }
  },

  filter(id, expressions) {
    const grid = this.grids.get(id);
    if (grid) {
      const filterExpressions = JSON.parse(expressions);
      grid.filter(Array.isArray(filterExpressions) ? filterExpressions : [filterExpressions]);
    }
  },

  clearFilter(id, key) {
    const grid = this.grids.get(id);
    if (grid) {
      grid.clearFilter(key);
    }
  },

  getColumns(id) {
    const grid = this.grids.get(id);
    return grid ? grid.columns : [];
  },

  navigateTo(id, row, field, activate) {
    const grid = this.grids.get(id);
    if (grid) {
      grid.navigateTo(row, { column: field, activate });
    }
  },

  destroyGrid(id) {
    const grid = this.grids.get(id);
    if (grid) {
      this.listeners.get(id).abort();
      this.listeners.delete(id);
      this.grids.delete(id);
      this.dotNetRefs.delete(id);
    }
  },

  // TODO: getDataView/getTotalItems read the element's dataView/totalItems getters; not wired to .NET yet.
  getDataView(id) {
    const grid = this.grids.get(id);
    return grid ? grid.dataView : [];
  },

  getTotalItems(id) {
    const grid = this.grids.get(id);
    return grid ? grid.totalItems : 0;
  },
};
