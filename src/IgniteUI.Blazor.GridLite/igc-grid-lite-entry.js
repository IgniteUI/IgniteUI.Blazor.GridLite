import { IgcGridLite } from 'igniteui-grid-lite';

// Register the component
IgcGridLite.register();

export { IgcGridLite };

export function get_igc_grid_lite() {
    window.IgcGridLite = IgcGridLite;
    return window;
}

window.blazor_igc_grid_lite = {
    grids: new Map(),
    dotNetRefs: new Map(),

    renderGrid(dotNetObject, gridElement, options, events) {
        const config = JSON.parse(options);

        if (!customElements.get('igc-grid-lite')) {
            IgcGridLite.register();
        }

        this.destroyGrid(config.id);

        gridElement.data = config.data;

        if (config.autoGenerate !== undefined) {
            gridElement.autoGenerate = config.autoGenerate;
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

        if (events.hasSorting) {
            gridElement.addEventListener('sorting', async (e) => {
                const cancel = await dotNetObject.invokeMethodAsync('JSSorting', e.detail);
                if (cancel) {
                    e.preventDefault();
                }
            });
        }

        if (events.hasSorted) {
            gridElement.addEventListener('sorted', (e) => {
                dotNetObject.invokeMethodAsync('JSSorted', e.detail);
            });
        }

        if (events.hasFiltering) {
            gridElement.addEventListener('filtering', async (e) => {
                const cancel = await dotNetObject.invokeMethodAsync('JSFiltering', e.detail);
                if (cancel) {
                    e.preventDefault();
                }
            });
        }

        if (events.hasFiltered) {
            gridElement.addEventListener('filtered', (e) => {
                dotNetObject.invokeMethodAsync('JSFiltered', e.detail);
            });
        }

        if (config.debug) {
            console.log('IgcGridLite rendered:', config);
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

        if (config.sortingOptions !== undefined) {
            grid.sortingOptions = config.sortingOptions;
        }

        if (config.sortingExpressions !== undefined) {
            grid.sortingExpressions = config.sortingExpressions;
        }

        if (config.filterExpressions !== undefined) {
            grid.filterExpressions = config.filterExpressions;
        }

        if (config.debug) {
            console.log('IgcGridLite updated:', config);
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
        return grid ? grid.columns : null;
    },

    destroyGrid(id) {
        const grid = this.grids.get(id);
        if (grid) {
            const parent = grid.parentNode;
            if (parent) {
                parent.removeChild(grid);
            }
            this.grids.delete(id);
            this.dotNetRefs.delete(id);
        }
    },

    getDataView(id) {
        const grid = this.grids.get(id);
        return grid ? grid.dataView : [];
    },

    getTotalItems(id) {
        const grid = this.grids.get(id);
        return grid ? grid.totalItems : 0;
    }
};