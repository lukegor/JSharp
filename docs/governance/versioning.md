# Versioning and deprecation (seed)

## Pre-release (now — restated, not changed)

v1 evolves in place. Nothing is released, so nothing owes compatibility and
no v2 line is created for pre-release redesigns (recipe standard
§3). This section changes nothing; it records the starting point.

## At first release, the following take effect

- **Three versions, three meanings.** `appVersion`: the implementation
  release. `catalogVersion`: the op catalog the build understands.
  `corpusVersion`: the conformance corpus revision, echoed in every report;
  any expected-hash change bumps it.
- **Additive evolution only.** New keys are ignored by old readers (the
  conformance report's additive-keys rule generalizes to all versioned
  artifacts); field removal or semantic narrowing is a breaking change.
- **Breaking =** any change that alters a pinned digest for identical
  inputs, removes a versioned field, narrows accepted inputs, or changes a
  frozen `recipe.*` code's meaning. Breaking changes require an accepted RFC
  first and a major version bump (mapping in the next bullet).
- **Bump mapping (fixed at first release).** `appVersion` is SemVer
  (MinVer-derived): a breaking change ships as a major bump, additive evolution
  (new ops, new ignored-by-old keys) as minor, digest-identical fixes as patch.
  `catalogVersion` bumps on any catalog change; a removal or semantic narrowing
  additionally counts as breaking (accepted RFC + `appVersion` major).
  `corpusVersion` bumps on any expected-hash change.
- **Removal ordering.** Announced in release N, removed no earlier than
  release N+1. No silent removals, ever.
- **Deprecation needs no code in the seed.** Warnings, compat shims, and
  migration tooling are release-slice work behind an accepted RFC; the seed
  only pins the ordering guarantee above.

## Deferred (with triggers — not promised)

| Deferred item | Revisit trigger |
|---|---|
| SVG / shields badge image | First external claimant requests one |
| Claim signing + published key story + threat model | First-release slice |
| Machine verifier / CI badge gate | First external implementation claims conformance |
| CODEOWNERS, SECURITY.md, CONTRIBUTING, issue/PR templates | Infra-backlog G-items (repo hygiene track) |
| Coverage / license status badges | Infra-backlog C2/G5 items |
| Deprecation warnings / compat shims / migration tooling | Release slice, behind an accepted RFC |
| Corpus scale-up patterns, 16-bit/float lanes | Corpus-growth proposals per the conformance spec §11 |
