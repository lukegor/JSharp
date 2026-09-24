# Governance (seed)

Standard-track governance for the recipe standard: how a stranger **claims**
conformance, **proposes** a change, and what **versioning** means at release.
Seed status: badge process +
RFC flow are seeded at first release, not before — this folder is the seed,
not the full process.

## Scope (standard track only)

- Conformance claims: `badge-claim.md`
- Change proposals: `rfc-template.md`, filed under `../rfcs/`
- Versioning and deprecation rules: `versioning.md`
- Trust docs: `keys.md`, `threat-model.md`

Repository hygiene (CODEOWNERS, SECURITY.md, CONTRIBUTING, issue/PR templates,
coverage/license badges) is NOT here — it belongs to
`docs/engineering-infrastructure/infrastructure-backlog.md` (G-items).

## Decision rule

The maintainer accepts or rejects RFCs. Acceptance requires the proposal's
invariant-filter section to pass and its alternatives section to be non-empty.

## Invariant gate

Every proposal must argue determinism, share-safety, and auditability
(recipe standard §2). A proposal weakening any of the three is
closed, not debated.

## Authority

The recipe standard wins on any conflict with these seed docs or any
frozen operational doc.
