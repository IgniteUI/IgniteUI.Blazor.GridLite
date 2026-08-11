# Security review record — IgniteUI.Blazor.GridLite `<version>`

> Copy this file to `review-<version>.md` for each release under review, complete every applicable section, and merge it alongside any resulting fixes or risk acceptances. Use [threat-model.md](threat-model.md) as the baseline.

| | |
|---|---|
| **Package / version** | `IgniteUI.Blazor.GridLite` <!-- TODO --> |
| **Commit reviewed** | <!-- TODO: full SHA --> |
| **Review date** | <!-- TODO --> |
| **Threat model version** | <!-- TODO: commit SHA of threat-model.md at review time --> |
| **Outcome** | <!-- Approved / Approved with conditions / Blocked --> |

## Reviewers

At least one reviewer must not be an author of the code under review.

| Name | Role | Author of reviewed code? |
|---|---|---|
| <!-- TODO --> | | |

## Coverage

Tick what was actually performed; an unticked row is a stated limitation, not an omission.

- [ ] Threat model walkthrough against the current code
- [ ] Manual review of the JS interop surface (`Internal/JSHandler.cs`, `Internal/JSLoader.cs`, `igc-grid-lite-entry.js`)
- [ ] Manual review of the serialization boundary (`IgbGridLite.razor.cs`)
- [ ] Default cell rendering verified against the exact `igniteui-grid-lite` version in `package-lock.json`
- [ ] Dependency review (`package-lock.json`, `PackageReference`)
- [ ] CodeQL default-setup results reviewed (Actions, C#, and JavaScript/TypeScript)
- [ ] Build and release pipeline review (`.github/workflows/`)
- [ ] Package content inspection (contents of the produced `.nupkg`)
- [ ] Consumer-facing security documentation reviewed for accuracy

## Findings register

Every threat carried from the threat model plus anything new found during the review. No row may be left blank at sign-off.

| ID | Summary | Sev | Disposition | Evidence / justification |
|---|---|---|---|---|
| TM-IX-01 | `DotNetObjectReference` exposed via global `window` map | Medium | <!-- Fixed / Mitigated / Accepted --> | |
| TM-IX-02 | Client-supplied sort/filter expressions reach consumer handlers | High | | |
| TM-IX-03 | No component-specific callback rate limiting | Low | <!-- Accepted if approved --> | Hosting limits bound message size and circuit resources |
| TM-IX-04 | Callback exceptions are swallowed without logging | Low | | |
| TM-SER-01 | Serializable `TItem` object graph is sent to the client | High | | |
| TM-SC-01 | Bundled JavaScript is not independently patchable | Medium | <!-- Accepted if approved --> | Upstream fixes require a new GridLite package release |
| TM-BLD-01 | Actions tag-pinned in a job holding `id-token: write` | Medium | | |

**Disposition values** — `Fixed` (code changed, link the PR) · `Mitigated` (compensating control, name it) · `Accepted` (residual risk, requires an approver in the table below).

## Accepted risks

Every `Accepted` disposition above needs a named approver here.

| ID | Justification | Approver | Date |
|---|---|---|---|
| <!-- TODO --> | | | |

## Release gate

- [ ] No finding of severity **High** or above is left `Open`
- [ ] Every `Accepted` risk has a named approver
- [ ] `threat-model.md` has been updated to reflect this review

**Statement:** <!-- e.g. "As of <SHA>, version <x.y.z> has no open Critical or High findings. Reviewed by <names> on <date>." -->
