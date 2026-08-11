# Security review record — IgniteUI.Blazor.GridLite `<version>`

> Copy this file to `review-<version>.md` for each release under review, fill it in, and
> merge it. It is the second artifact Microsoft requires alongside
> [threat-model.md](threat-model.md).

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
- [ ] Dependency review (`package-lock.json`, `PackageReference`)
- [ ] Static analysis results reviewed (CodeQL `csharp` + `javascript`)
- [ ] Build and release pipeline review (`.github/workflows/`)
- [ ] Package content inspection (contents of the produced `.nupkg`)
- [ ] Consumer-facing security documentation reviewed for accuracy

## Findings register

Every threat carried from the threat model plus anything new found during the review. No
row may be left blank at sign-off.

| ID | Summary | Sev | Disposition | Evidence / justification |
|---|---|---|---|---|
| TM-IX-01 | `DotNetObjectReference` exposed via global `window` map | Medium | <!-- Fixed / Mitigated / Accepted --> | |
| TM-IX-02 | Untrusted sort/filter fields reach consumer handlers | High | | |
| TM-IX-03 | Unbounded callback invocation rate | Low | Accepted | Framework message-size and interop timeouts cap per-call cost |
| TM-IX-04 | `catch { }` swallows tampering with no logging | Medium | | |
| TM-IX-05 | Empty-bodied `[JSInvokable]` methods are reachable dead surface | Low | | |
| TM-IX-06 | `get_igc_grid_lite()` returns `window` | Low | | |
| TM-SER-01 | Full `TItem` object graph serialized to the client | High | | |
| TM-SER-02 | Prerendered state embedded in initial HTML | Low | Accepted | Inherent to Blazor SSR; app-level cache headers |
| TM-DOM-01 | Cell rendering: text vs. markup | TBD | | |
| TM-DOM-02 | `AdditionalAttributes` splatted onto the host element | Low | | |
| TM-DOM-03 | `AdoptRootStyles` pierces shadow-DOM encapsulation | Low | By design | Opt-in, defaults to `false` |
| TM-SC-01 | Bundled third-party JS is not independently patchable | Medium | By design | Covered by the published disclosure SLAs |
| TM-SC-02 | Pre-1.0 `~0.9.0` dependency range | Medium | | |
| TM-SC-03 | No CI workflow; no CodeQL/SCA/dependency-review | High | | |
| TM-BLD-01 | Actions tag-pinned in a job holding `id-token: write` | Medium | | |
| TM-BLD-02 | `npm install` on local builds bypasses the lockfile | Low | Accepted | Release artifacts are built only in CI with `npm ci` |
| TM-BLD-03 | Package is not source-verifiable (no SourceLink/deterministic build) | Low | | |
| TM-PKG-01 | No `SECURITY.md`; no private reporting channel | High | | |

**Disposition values** — `Fixed` (code changed, link the PR) · `Mitigated` (compensating
control, name it) · `Accepted` (residual risk, requires an approver in the table below).

## Accepted risks

Every `Accepted` disposition above needs a named approver here.

| ID | Justification | Approver | Date |
|---|---|---|---|
| <!-- TODO --> | | | |

## Release gate

- [ ] No finding of severity **High** or above is left `Open`
- [ ] Every `Accepted` risk has a named approver
- [ ] TM-DOM-01 has a definitive answer
- [ ] `threat-model.md` has been updated to reflect this review

**Statement:** <!-- e.g. "As of <SHA>, version <x.y.z> has no open Critical or High
findings. Reviewed by <names> on <date>." -->
